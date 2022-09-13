using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class SerializationForObjectGenerator
{
    private readonly PropertyFilter _propertyFilter;
    private readonly LoquiNameRetriever _nameRetriever;
    private readonly SerializationFieldGenerator _forFieldGenerator;
    private readonly WhereClauseGenerator _whereClauseGenerator;
    private readonly LoquiSerializationNaming _loquiSerializationNaming;

    public SerializationForObjectGenerator(
        PropertyFilter propertyFilter,
        LoquiNameRetriever nameRetriever,
        SerializationFieldGenerator forFieldGenerator,
        WhereClauseGenerator whereClauseGenerator,
        LoquiSerializationNaming loquiSerializationNaming)
    {
        _propertyFilter = propertyFilter;
        _nameRetriever = nameRetriever;
        _forFieldGenerator = forFieldGenerator;
        _whereClauseGenerator = whereClauseGenerator;
        _loquiSerializationNaming = loquiSerializationNaming;
    }
    
    public void Generate(
        CompilationUnit compilation, 
        SourceProductionContext context, 
        ITypeSymbol obj)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        
        if (!compilation.Mapping.TryGetTypeSet(obj, out var typeSet)) return;
        var baseType = compilation.Mapping.TryGetBaseClass(obj);
        var inheriting = compilation.Mapping.TryGetInheritingClasses(typeSet.Getter);
        
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine($"using Mutagen.Bethesda.Serialization;");
        sb.AppendLine($"using {obj.ContainingNamespace};");
        
        using (sb.Namespace(obj.ContainingNamespace.ToString()))
        {
        }
        
        string writeObjectGenerics = "<TWriteObject>";
        string readObjectGenerics = "<TReadObject>";
        IEnumerable<string> readerWheres = Enumerable.Empty<string>();
        IEnumerable<string> writerWheres = Enumerable.Empty<string>();
        if (typeSet.Getter is INamedTypeSymbol writerTypeSymbol
            && writerTypeSymbol.TypeArguments.Length > 0)
        {
            var generics = string.Join(", ", writerTypeSymbol.TypeArguments);
            writeObjectGenerics = $"<{string.Join(", ", "TWriteObject", generics)}>";
            writerWheres = _whereClauseGenerator.GetWheres(writerTypeSymbol);
        }
        if (typeSet.Setter is INamedTypeSymbol readerTypeSymbol
            && readerTypeSymbol.TypeArguments.Length > 0)
        {
            var generics = string.Join(", ", readerTypeSymbol.TypeArguments);
            readObjectGenerics = $"<{string.Join(", ", "TReadObject", generics)}>";
            readerWheres = _whereClauseGenerator.GetWheres(readerTypeSymbol);
        }

        if (!_loquiSerializationNaming.TryGetSerializationItems(obj, out var objSerializationItems)) return;
        
        using (var c = sb.Class(objSerializationItems.SerializationHousingClassName))
        {
            c.AccessModifier = AccessModifier.Internal;
            c.Static = true;
        }
        using (sb.CurlyBrace())
        {
            if (inheriting.Count > 0)
            {
                using (var args = sb.Function($"public static void SerializeWithCheck{writeObjectGenerics}"))
                {
                    args.Add($"TWriteObject writer");
                    args.Add($"{typeSet.Getter} item");
                    args.Add($"ISerializationWriterKernel<TWriteObject> kernel");
                    args.Wheres.AddRange(writerWheres);
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("switch (item)");
                    using (sb.CurlyBrace())
                    {
                        foreach (var inherit in inheriting)
                        {
                            context.CancellationToken.ThrowIfCancellationRequested();
                            var names = _nameRetriever.GetNames(inherit);
                            if (!_loquiSerializationNaming.TryGetSerializationItems(inherit, out var inheritSerializeItems)) continue;
                            if (!compilation.Mapping.TryGetTypeSet(inherit, out var inheritTypes)) continue;
                            if (inheritTypes.Direct?.IsAbstract ?? true) continue;
                            sb.AppendLine($"case {inherit.ContainingNamespace}.{names.Getter} {names.Direct}Getter:");
                            using (sb.IncreaseDepth())
                            {
                                sb.AppendLine($"{inheritSerializeItems.SerializationCall(serialize: true)}(writer, {names.Direct}Getter, kernel);");
                                sb.AppendLine("break;");
                            }
                        }

                        if (_loquiSerializationNaming.TryGetSerializationItems(obj, out var curSerializationItems)
                            && (!typeSet.Direct?.IsAbstract ?? false))
                        {
                            sb.AppendLine($"case {typeSet.Getter} {typeSet.Getter.Name}:");
                            using (sb.IncreaseDepth())
                            {
                                sb.AppendLine($"{curSerializationItems.SerializationCall(serialize: true)}(writer, {typeSet.Getter.Name}, kernel);");
                                sb.AppendLine("break;");
                            }
                        }
                        sb.AppendLine("default:");
                        using (sb.IncreaseDepth())
                        {
                            sb.AppendLine($"throw new NotImplementedException();");
                        }
                    }
                }
                sb.AppendLine();
            }
            
            using (var args = sb.Function($"public static void Serialize{writeObjectGenerics}"))
            {
                args.Add($"TWriteObject writer");
                args.Add($"{typeSet.Getter} item");
                args.Add($"ISerializationWriterKernel<TWriteObject> kernel");
                args.Wheres.AddRange(writerWheres);
            }
            using (sb.CurlyBrace())
            {
                if (baseType != null
                    && _loquiSerializationNaming.TryGetSerializationItems(baseType, out var baseSerializationItems))
                {
                    sb.AppendLine($"{baseSerializationItems.SerializationCall(serialize: true)}<TWriteObject>(item, writer, kernel);");
                }
                foreach (var prop in obj.GetMembers().WhereCastable<ISymbol, IPropertySymbol>())
                {
                    GenerateForProperty(compilation, obj, prop, sb, context.CancellationToken);
                }
            }
            sb.AppendLine();
            
            using (var args = sb.Function($"public static {typeSet.Setter} Deserialize{readObjectGenerics}"))
            {
                args.Add($"TReadObject reader");
                args.Add($"ISerializationReaderKernel<TReadObject> kernel");
                args.Wheres.AddRange(readerWheres);
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("throw new NotImplementedException();");
            }
            sb.AppendLine();
        }
        sb.AppendLine();
        
        context.AddSource(objSerializationItems.SerializationHousingFileName, SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private void GenerateForProperty(CompilationUnit compilation, ITypeSymbol obj, IPropertySymbol prop, StructuredStringBuilder sb, CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (_propertyFilter.Skip(prop)) return;
        _forFieldGenerator.GenerateForField(compilation, obj, prop.Type, "writer", prop.Name, $"item.{prop.Name}", sb, cancel);
    }
}
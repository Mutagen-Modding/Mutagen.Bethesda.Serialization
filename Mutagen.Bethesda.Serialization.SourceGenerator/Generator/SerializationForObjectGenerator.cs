using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class SerializationForObjectGenerator
{
    private readonly PropertyFilter _propertyFilter;
    private readonly LoquiNameRetriever _nameRetriever;
    private readonly SerializationFieldGenerator _forFieldGenerator;
    private readonly LoquiSerializationNaming _loquiSerializationNaming;

    public SerializationForObjectGenerator(
        PropertyFilter propertyFilter,
        LoquiNameRetriever nameRetriever,
        SerializationFieldGenerator forFieldGenerator,
        LoquiSerializationNaming loquiSerializationNaming)
    {
        _propertyFilter = propertyFilter;
        _nameRetriever = nameRetriever;
        _forFieldGenerator = forFieldGenerator;
        _loquiSerializationNaming = loquiSerializationNaming;
    }
    
    public void Generate(
        CompilationUnit compilation, 
        SourceProductionContext context, 
        ITypeSymbol obj)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        var baseType = compilation.Mapping.TryGetBaseClass(obj);
        var inheriting = compilation.Mapping.TryGetInheritingClasses(obj);
        
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine($"using Mutagen.Bethesda.Serialization;");
        sb.AppendLine($"using {obj.ContainingNamespace};");
        
        using (sb.Namespace(obj.ContainingNamespace.ToString()))
        {
        }
        
        string writeObjectGenerics = "<TWriteObject>";
        string readObjectGenerics = "<TReadObject>";
        IEnumerable<string> wheres = Enumerable.Empty<string>();
        if (obj is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.TypeArguments.Length > 0)
        {
            var generics = string.Join(", ", namedTypeSymbol.TypeArguments);
            writeObjectGenerics = $"<{string.Join(", ", "TWriteObject", generics)}>";
            readObjectGenerics = $"<{string.Join(", ", "TReadObject", generics)}>";
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
                    args.Add($"{obj} item");
                    args.Add($"TWriteObject writer");
                    args.Add($"ISerializationWriterKernel<TWriteObject> kernel");
                    args.Wheres.AddRange(wheres);
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
                            sb.AppendLine($"case {inherit.ContainingNamespace}.{names.Getter} {names.Direct}Getter:");
                            using (sb.IncreaseDepth())
                            {
                                sb.AppendLine($"{inheritSerializeItems.SerializationCall(serialize: true)}({names.Direct}Getter, writer, kernel);");
                                sb.AppendLine("break;");
                            }
                        }

                        if (_loquiSerializationNaming.TryGetSerializationItems(obj, out var curSerializationItems)
                            && compilation.Mapping.TryGetDirectClass(obj, out var objDirect)
                            && !objDirect.IsAbstract)
                        {
                            sb.AppendLine($"case {obj} {obj}Getter:");
                            using (sb.IncreaseDepth())
                            {
                                sb.AppendLine($"{curSerializationItems.SerializationCall(serialize: true)}({obj}Getter, writer, kernel);");
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
                args.Add($"{obj} item");
                args.Add($"TWriteObject writer");
                args.Add($"ISerializationWriterKernel<TWriteObject> kernel");
                args.Wheres.AddRange(wheres);
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
            
            using (var args = sb.Function($"public static {obj} Deserialize{readObjectGenerics}"))
            {
                args.Add($"TReadObject reader");
                args.Add($"ISerializationReaderKernel<TReadObject> kernel");
                args.Wheres.AddRange(wheres);
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
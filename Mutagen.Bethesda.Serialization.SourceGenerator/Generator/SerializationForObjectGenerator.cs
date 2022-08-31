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
    private readonly SerializationFieldGenerator _forFieldGenerator;
    private readonly LoquiSerializationNaming _loquiSerializationNaming;

    public SerializationForObjectGenerator(
        PropertyFilter propertyFilter,
        SerializationFieldGenerator forFieldGenerator,
        LoquiSerializationNaming loquiSerializationNaming)
    {
        _propertyFilter = propertyFilter;
        _forFieldGenerator = forFieldGenerator;
        _loquiSerializationNaming = loquiSerializationNaming;
    }
    
    public void Generate(
        LoquiMapping loquiMapping,
        Compilation compilation, 
        SourceProductionContext context, 
        ITypeSymbol obj)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        var baseType = loquiMapping.TryGetBaseClass(obj, context.CancellationToken);
        var inheriting = loquiMapping.TryGetInheritingClasses(obj, context.CancellationToken);
        
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
                            if (!_loquiSerializationNaming.TryGetSerializationItems(inherit.GetterType, out var inheritSerializeItems)) continue;
                            sb.AppendLine($"case {inherit.GetterType} {inherit.ClassType.Name}Getter:");
                            using (sb.IncreaseDepth())
                            {
                                sb.AppendLine($"{inheritSerializeItems.SerializationCall(serialize: true)}({inherit.ClassType.Name}Getter, writer, kernel);");
                                sb.AppendLine("break;");
                            }
                        }

                        if (_loquiSerializationNaming.TryGetSerializationItems(obj, out var curSerializationItems)
                            && !curSerializationItems.LoquiRegistration.ClassType.IsAbstract)
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

    private void GenerateForProperty(Compilation compilation, ITypeSymbol obj, IPropertySymbol prop, StructuredStringBuilder sb, CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();
        if (_propertyFilter.Skip(prop)) return;
        _forFieldGenerator.GenerateForField(compilation, obj, prop.Type, "writer", prop.Name, $"item.{prop.Name}", sb, cancel);
    }
}
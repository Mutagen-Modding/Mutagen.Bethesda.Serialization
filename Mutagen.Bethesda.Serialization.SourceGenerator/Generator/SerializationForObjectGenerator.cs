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
    private readonly LoquiMapping _loquiMapping;

    public SerializationForObjectGenerator(
        PropertyFilter propertyFilter,
        SerializationFieldGenerator forFieldGenerator,
        LoquiMapping loquiMapping)
    {
        _propertyFilter = propertyFilter;
        _forFieldGenerator = forFieldGenerator;
        _loquiMapping = loquiMapping;
    }
    
    public void Generate(SourceProductionContext context, ITypeSymbol obj)
    {
        var baseType = _loquiMapping.TryGetBaseClass(obj);
        
        var sb = new StructuredStringBuilder();
        
        sb.AppendLine($"using Mutagen.Bethesda.Serialization;");
        sb.AppendLine($"using {obj.ContainingNamespace};");
        
        using (sb.Namespace(obj.ContainingNamespace.ToString()))
        {
        }
        
        using (var c = sb.Class($"{obj.Name}_Serialization"))
        {
            c.AccessModifier = AccessModifier.Internal;
            c.Static = true;
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Function($"public static void Serialize<TWriteObject>"))
            {
                args.Add($"{obj} item");
                args.Add($"TWriteObject writer");
                args.Add($"ISerializationWriterKernel<TWriteObject> kernel");
            }
            using (sb.CurlyBrace())
            {
                if (baseType != null)
                {
                    sb.AppendLine($"{baseType.Name}_Serialization.Serialize<TWriteObject>(item, writer, kernel);");
                }
                foreach (var prop in obj.GetMembers().WhereCastable<ISymbol, IPropertySymbol>())
                {
                    if (_propertyFilter.Skip(prop)) continue;
                    _forFieldGenerator.GenerateForField(obj, prop.Type, "writer", prop.Name, $"item.{prop.Name}", sb);
                }
            }
            sb.AppendLine();
            
            using (var args = sb.Function($"public static {obj} Deserialize<TReadObject>"))
            {
                args.Add($"TReadObject reader");
                args.Add($"ISerializationReaderKernel<TReadObject> kernel");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("throw new NotImplementedException();");
            }
            sb.AppendLine();
        }
        sb.AppendLine();
        
        var sanitizedName = obj.MetadataName;
        var genericIndex = sanitizedName.IndexOf('`');
        if (genericIndex != -1)
        {
            sanitizedName = sanitizedName.Substring(0, genericIndex);
        }
        
        context.AddSource($"{sanitizedName}_Serializations.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}
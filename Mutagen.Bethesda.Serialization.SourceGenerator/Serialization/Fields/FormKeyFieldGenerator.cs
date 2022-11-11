using Microsoft.CodeAnalysis;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;

public class FormKeyFieldGenerator : PrimitiveFieldGenerator
{
    public new static readonly string[] AssociatedTypes = new string[]
    {
        "FormKey",
        "Mutagen.Bethesda.Plugins.FormKey",
    };

    public override bool ShouldGenerate(IPropertySymbol propertySymbol) => true;

    public override IEnumerable<string> RequiredNamespaces(ITypeSymbol typeSymbol, CancellationToken cancel)
    {
        yield return "Mutagen.Bethesda.Plugins";
    }

    public FormKeyFieldGenerator()
        : base("FormKey", AssociatedTypes)
    {
    }

    public override void GenerateForDeserialize(CompilationUnit compilation, ITypeSymbol obj, ITypeSymbol field, string? fieldName,
        string fieldAccessor, string readerAccessor, string kernelAccessor, string metaAccessor, bool insideCollection,
        bool canSet, StructuredStringBuilder sb, CancellationToken cancel)
    {
    }
}
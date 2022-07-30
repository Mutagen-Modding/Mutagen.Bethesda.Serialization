using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Utility;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator
{
    [Generator]
    public class SerializationSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            new SourceGeneratorContainer()
                .Resolve().Value
                .Initialize(context);
        }
    }
}

using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization
{
    [Generator]
    public class SerializationSourceGenerator : IIncrementalGenerator
    {
        private readonly bool _generateMixIns;

        public SerializationSourceGenerator(bool generateMixIns)
        {
            _generateMixIns = generateMixIns;
        }

        public SerializationSourceGenerator()
        {
            _generateMixIns = true;
        }
        
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            new SerializationSourceGeneratorContainer()
                .Resolve().Value
                .Initialize(context, _generateMixIns);
        }
    }
}

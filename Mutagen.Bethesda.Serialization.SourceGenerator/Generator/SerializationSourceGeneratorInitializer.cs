using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class SerializationSourceGeneratorInitializer
{
    private readonly BootstrapInvocationDetector _invocationDetector;
    private readonly MixinForModGenerator _mixinForModGenerator;
    private readonly ModInvocationDetector _modInvocationDetector;
    private readonly SerializationForObjectsGenerator _serializationForObjectsGenerator;
    private readonly StubGenerator _stubGenerator;

    public SerializationSourceGeneratorInitializer(
        BootstrapInvocationDetector invocationDetector,
        MixinForModGenerator mixinForModGenerator,
        ModInvocationDetector modInvocationDetector,
        SerializationForObjectsGenerator serializationForObjectsGenerator,
        StubGenerator stubGenerator)
    {
        _invocationDetector = invocationDetector;
        _mixinForModGenerator = mixinForModGenerator;
        _modInvocationDetector = modInvocationDetector;
        _serializationForObjectsGenerator = serializationForObjectsGenerator;
        _stubGenerator = stubGenerator;
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var bootstrapSymbols = _invocationDetector.GetBootstrapInvocations(context);
        _stubGenerator.Initialize(context, bootstrapSymbols);
        var modBootstraps = _modInvocationDetector.Detect(bootstrapSymbols);
        _mixinForModGenerator.Initialize(context, modBootstraps);
        _serializationForObjectsGenerator.Initialize(context, modBootstraps);
    }
}
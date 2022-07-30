using Microsoft.CodeAnalysis;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Generator;

public class SerializationSourceGeneratorInitializer
{
    private readonly BootstrapInvocationDetector _invocationDetector;
    private readonly MixinForModGenerator _mixinForModGenerator;
    private readonly ModInvocationDetector _modInvocationDetector;
    private readonly SerializationForLoquiGenerator _serializationForLoquiGenerator;
    private readonly StubGenerator _stubGenerator;

    public SerializationSourceGeneratorInitializer(
        BootstrapInvocationDetector invocationDetector,
        MixinForModGenerator mixinForModGenerator,
        ModInvocationDetector modInvocationDetector,
        SerializationForLoquiGenerator serializationForLoquiGenerator,
        StubGenerator stubGenerator)
    {
        _invocationDetector = invocationDetector;
        _mixinForModGenerator = mixinForModGenerator;
        _modInvocationDetector = modInvocationDetector;
        _serializationForLoquiGenerator = serializationForLoquiGenerator;
        _stubGenerator = stubGenerator;
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var bootstrapSymbols = _invocationDetector.GetBootstrapInvocations(context);
        _stubGenerator.Initialize(context, bootstrapSymbols);
        var modBootstraps = _modInvocationDetector.Detect(bootstrapSymbols);
        _mixinForModGenerator.Initialize(context, modBootstraps);
        _serializationForLoquiGenerator.Initialize(context, modBootstraps);
    }
}
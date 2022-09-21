using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

namespace Mutagen.Bethesda.Serialization.SourceGenerator;

public class SerializationSourceGeneratorInitializer
{
    private readonly BootstrapInvocationDetector _invocationDetector;
    private readonly MixinGenerator _mixinGenerator;
    private readonly ModInvocationDetector _modInvocationDetector;
    private readonly SerializationForObjectsGenerator _serializationForObjectsGenerator;
    private readonly StubGenerator _stubGenerator;

    public SerializationSourceGeneratorInitializer(
        BootstrapInvocationDetector invocationDetector,
        MixinGenerator mixinGenerator,
        ModInvocationDetector modInvocationDetector,
        SerializationForObjectsGenerator serializationForObjectsGenerator,
        StubGenerator stubGenerator)
    {
        _invocationDetector = invocationDetector;
        _mixinGenerator = mixinGenerator;
        _modInvocationDetector = modInvocationDetector;
        _serializationForObjectsGenerator = serializationForObjectsGenerator;
        _stubGenerator = stubGenerator;
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var bootstrapSymbols = _invocationDetector.GetBootstrapInvocations(context);
        _stubGenerator.Initialize(context, bootstrapSymbols);
        var modBootstraps = _modInvocationDetector.Detect(bootstrapSymbols);
        _mixinGenerator.Initialize(context, modBootstraps);
        _serializationForObjectsGenerator.Initialize(context, modBootstraps);
    }
}
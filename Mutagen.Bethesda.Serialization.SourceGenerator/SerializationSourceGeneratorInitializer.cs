using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

namespace Mutagen.Bethesda.Serialization.SourceGenerator;

public class SerializationSourceGeneratorInitializer
{
    private readonly BootstrapInvocationDetector _invocationDetector;
    private readonly MixinGenerator _mixinGenerator;
    private readonly CustomizationProvider _customizationProvider;
    private readonly ModInvocationDetector _modInvocationDetector;
    private readonly SerializationForObjectsGenerator _serializationForObjectsGenerator;
    private readonly StubGenerator _stubGenerator;

    public SerializationSourceGeneratorInitializer(
        BootstrapInvocationDetector invocationDetector,
        MixinGenerator mixinGenerator,
        CustomizationProvider customizationProvider,
        ModInvocationDetector modInvocationDetector,
        SerializationForObjectsGenerator serializationForObjectsGenerator,
        StubGenerator stubGenerator)
    {
        _invocationDetector = invocationDetector;
        _mixinGenerator = mixinGenerator;
        _customizationProvider = customizationProvider;
        _modInvocationDetector = modInvocationDetector;
        _serializationForObjectsGenerator = serializationForObjectsGenerator;
        _stubGenerator = stubGenerator;
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var bootstrapSymbols = _invocationDetector.GetBootstrapInvocations(context);
        var customizations = _customizationProvider.Get(context);
        _stubGenerator.Initialize(context, bootstrapSymbols);
        var modBootstraps = _modInvocationDetector.Detect(bootstrapSymbols);
        _mixinGenerator.Initialize(context, modBootstraps);
        _serializationForObjectsGenerator.Initialize(context, modBootstraps, customizations);
    }
}
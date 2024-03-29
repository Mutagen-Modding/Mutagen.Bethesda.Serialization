using Microsoft.CodeAnalysis;
using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Bootstrapping;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

public class SerializationSourceGeneratorInitializer
{
    private readonly BootstrapInvocationDetector _invocationDetector;
    private readonly MixinGenerator _mixinGenerator;
    private readonly CustomizationProvider _customizationProvider;
    private readonly RecordCustomizationProvider _recordCustomizationProvider;
    private readonly ModInvocationDetector _modInvocationDetector;
    private readonly SerializationForObjectsGenerator _serializationForObjectsGenerator;
    private readonly StubGenerator _stubGenerator;
    private readonly LoquiMapper _loquiMapper;

    public SerializationSourceGeneratorInitializer(
        BootstrapInvocationDetector invocationDetector,
        MixinGenerator mixinGenerator,
        CustomizationProvider customizationProvider,
        RecordCustomizationProvider recordCustomizationProvider,
        ModInvocationDetector modInvocationDetector,
        SerializationForObjectsGenerator serializationForObjectsGenerator,
        StubGenerator stubGenerator,
        LoquiMapper loquiMapper)
    {
        _invocationDetector = invocationDetector;
        _mixinGenerator = mixinGenerator;
        _customizationProvider = customizationProvider;
        _recordCustomizationProvider = recordCustomizationProvider;
        _modInvocationDetector = modInvocationDetector;
        _serializationForObjectsGenerator = serializationForObjectsGenerator;
        _stubGenerator = stubGenerator;
        _loquiMapper = loquiMapper;
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context, bool generateMixIns)
    {
        var bootstrapSymbols = _invocationDetector.GetBootstrapInvocations(context);
        var mappings = context.CompilationProvider
            .Select((x, c) => _loquiMapper.GetMappings(x, c));
        var customizations = _customizationProvider.Get(context);
        var recordCustomizations = _recordCustomizationProvider.Get(context, mappings);
        _stubGenerator.Initialize(context, bootstrapSymbols);
        var modBootstraps = _modInvocationDetector.Detect(bootstrapSymbols);
        if (generateMixIns)
        {
            _mixinGenerator.Initialize(context, modBootstraps, customizations, recordCustomizations);
        }
        _serializationForObjectsGenerator.Initialize(context, modBootstraps, mappings, customizations, recordCustomizations);
    }
}
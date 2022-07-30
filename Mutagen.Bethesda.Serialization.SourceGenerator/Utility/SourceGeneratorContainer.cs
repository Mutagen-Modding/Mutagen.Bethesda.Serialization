using Mutagen.Bethesda.Serialization.SourceGenerator.Generator;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Utility;

[Register<SerializationSourceGeneratorInitializer>]
[Register<BootstrapInvocationDetector>]
[Register<StubGenerator>]
[Register<ModInvocationDetector>]
[Register<MixinForModGenerator>]
[Register<SerializationForLoquiGenerator>]
partial class SourceGeneratorContainer : IContainer<SerializationSourceGeneratorInitializer>
{
}
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Utility;

[Register<SerializationSourceGeneratorInitializer>]
[Register<BootstrapInvocationDetector>]
[Register<StubGenerator>]
[Register<ModInvocationDetector>]
[Register<MixinForModGenerator>]
[Register<SerializationForLoquiGenerator>]
[Register<RelatedObjectAccumulator>]
[Register<IsLoquiObjectTester>]
[Register<SerializationForObjectGenerator>]
[Register<LoquiFieldGenerator>]
[Register(typeof(StringFieldGenerator), typeof(ISerializationForFieldGenerator))]
partial class SourceGeneratorContainer : IContainer<SerializationSourceGeneratorInitializer>
{
}
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator;
using Mutagen.Bethesda.Serialization.SourceGenerator.Generator.Fields;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Utility;

[Register<SerializationSourceGeneratorInitializer>]
[Register<BootstrapInvocationDetector>]
[Register<StubGenerator>]
[Register<ModInvocationDetector>]
[Register<MixinForModGenerator>]
[Register<SerializationForObjectsGenerator>]
[Register<RelatedObjectAccumulator>]
[Register<IsLoquiObjectTester>]
[Register<SerializationForObjectGenerator>]
[Register<SerializationFieldGenerator>]
[Register<PropertyFilter>]
[Register(typeof(TranslatedStringFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(FloatFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(BoolFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(LoquiFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(EnumFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(StringFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(Int8FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(Int16FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(Int32FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(Int64FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(UInt8FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(UInt16FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(UInt32FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(UInt64FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(ListFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(FormLinkFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(ColorFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(ByteArrayFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(RecordTypeFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(GenderedTypeFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(AssetLinkFieldGenerator), typeof(ISerializationForFieldGenerator))]
partial class SourceGeneratorContainer : IContainer<SerializationSourceGeneratorInitializer>
{
}
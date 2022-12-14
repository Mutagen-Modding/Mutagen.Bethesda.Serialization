using Mutagen.Bethesda.Serialization.SourceGenerator.Customizations;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;
using Mutagen.Bethesda.Serialization.SourceGenerator.Serialization.Fields;
using StrongInject;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Serialization;

[Register<SerializationSourceGeneratorInitializer>]
[Register<BootstrapInvocationDetector>]
[Register<StubGenerator>]
[Register<ModInvocationDetector>]
[Register<MixinGenerator>]
[Register<SerializationForObjectsGenerator>]
[Register<RelatedObjectAccumulator>]
[Register<IsLoquiObjectTester>]
[Register<SerializationForObjectGenerator>]
[Register<SerializationFieldGenerator>]
[Register<PropertyFilter>]
[Register<LoquiMapper>]
[Register<LoquiSerializationNaming>]
[Register<LoquiNameRetriever>]
[Register<WhereClauseGenerator>]
[Register<IsGroupTester>]
[Register<PropertyCollectionRetriever>]
[Register<ObjectTypeTester>]
[Register<CustomizationDetector>]
[Register<CustomizationInterpreter>]
[Register<CustomizationProvider>]
[Register<CustomizationDriver>]
[Register<ReleaseRetriever>]
[Register(typeof(TranslatedStringFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(FloatFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(BoolFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(LoquiFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(EnumFieldGenerator), typeof(ISerializationForFieldGenerator), typeof(EnumFieldGenerator))]
[Register(typeof(StringFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(Int8FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(Int16FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(Int32FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(Int64FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(UInt8FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(UInt16FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(UInt32FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(UInt64FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(P2FloatFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(P3FloatFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(P3UInt8FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(P3Int16FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(P3UInt16FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(P2IntFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(P2Int16FieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(PercentFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(ListFieldGenerator), typeof(ISerializationForFieldGenerator), typeof(ListFieldGenerator))]
[Register(typeof(ModKeyFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(FormKeyFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(FormLinkFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(ColorFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(ByteArrayFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(RecordTypeFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(GenderedTypeFieldGenerator), typeof(ISerializationForFieldGenerator), typeof(GenderedTypeFieldGenerator))]
[Register(typeof(AssetLinkFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(DictFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(ArrayFieldGenerator), typeof(ISerializationForFieldGenerator), typeof(ArrayFieldGenerator))]
[Register(typeof(GroupFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(CharFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(Array2dFieldGenerator), typeof(ISerializationForFieldGenerator))]
[Register(typeof(CacheFieldGenerator), typeof(ISerializationForFieldGenerator))]
partial class SerializationSourceGeneratorContainer : IContainer<SerializationSourceGeneratorInitializer>
{
}
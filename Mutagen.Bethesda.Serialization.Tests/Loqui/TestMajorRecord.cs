using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public interface ITestMajorRecord : ITestMajorRecordGetter, IMajorRecordInternal
{
    new string String { get; set; }
}

public interface ITestMajorRecordGetter : ILoquiObjectGetter, IMajorRecordGetter
{
    string String { get; }
}

public class TestMajorRecord : ITestMajorRecord, IEquatable<TestMajorRecord>
{
    public string String { get; set; }
    public FormKey FormKey { get; set; }

    public string? EditorID { get; set; }

    public TestMajorRecord(FormKey i, string s)
    {
        String = s;
        FormKey = i;
    }
    
    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        throw new NotImplementedException();
    }

    public void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories, IAssetLinkCache? linkCache = null,
        Type? assetType = null)
    {
        throw new NotImplementedException();
    }

    public void RemapAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping, AssetLinkQuery query, IAssetLinkCache? linkCache)
    {
        throw new NotImplementedException();
    }

    public void RemapListedAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IAssetLink> EnumerateListedAssetLinks()
    {
        throw new NotImplementedException();
    }

    public ILoquiRegistration Registration => throw new NotImplementedException();
    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords()
    {
        return EnumerateMajorRecords();
    }

    IEnumerable<TMajor> IMajorRecordEnumerable.EnumerateMajorRecords<TMajor>(bool throwIfUnknown)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IMajorRecord> EnumerateMajorRecords(Type? t, bool throwIfUnknown = true)
    {
        throw new NotImplementedException();
    }

    public void Remove(FormKey formKey)
    {
        throw new NotImplementedException();
    }

    public void Remove(IEnumerable<FormKey> formKeys)
    {
        throw new NotImplementedException();
    }

    public void Remove(HashSet<FormKey> formKeys)
    {
        throw new NotImplementedException();
    }

    public void Remove(FormKey formKey, Type type, bool throwIfUnknown = true)
    {
        throw new NotImplementedException();
    }

    public void Remove(IEnumerable<FormKey> formKeys, Type type, bool throwIfUnknown = true)
    {
        throw new NotImplementedException();
    }

    public void Remove(HashSet<FormKey> formKeys, Type type, bool throwIfUnknown = true)
    {
        throw new NotImplementedException();
    }

    public void Remove<TMajor>(FormKey formKey, bool throwIfUnknown = true) where TMajor : IMajorRecordGetter
    {
        throw new NotImplementedException();
    }

    public void Remove<TMajor>(HashSet<FormKey> formKeys, bool throwIfUnknown = true) where TMajor : IMajorRecordGetter
    {
        throw new NotImplementedException();
    }

    public void Remove<TMajor>(IEnumerable<FormKey> formKeys, bool throwIfUnknown = true) where TMajor : IMajorRecordGetter
    {
        throw new NotImplementedException();
    }

    public void Remove<TMajor>(TMajor record, bool throwIfUnknown = true) where TMajor : IMajorRecordGetter
    {
        throw new NotImplementedException();
    }

    public void Remove<TMajor>(IEnumerable<TMajor> records, bool throwIfUnknown = true) where TMajor : IMajorRecordGetter
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IMajorRecord> EnumerateMajorRecords()
    {
        throw new NotImplementedException();
    }

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
    {
        throw new NotImplementedException();
    }

    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords(Type type, bool throwIfUnknown)
    {
        return EnumerateMajorRecords(type, throwIfUnknown);
    }

    public object CommonInstance()
    {
        throw new NotImplementedException();
    }

    public object? CommonSetterInstance()
    {
        throw new NotImplementedException();
    }

    public object CommonSetterTranslationInstance()
    {
        throw new NotImplementedException();
    }

    bool IMajorRecord.IsCompressed
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    bool IMajorRecord.IsDeleted
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    bool IMajorRecordGetter.IsCompressed => throw new NotImplementedException();

    bool IMajorRecordGetter.IsDeleted => throw new NotImplementedException();

    ushort? IMajorRecordGetter.FormVersion => throw new NotImplementedException();

    int IMajorRecordGetter.MajorRecordFlagsRaw => throw new NotImplementedException();

    uint IMajorRecord.VersionControl
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    int IMajorRecord.MajorRecordFlagsRaw
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    uint IMajorRecordGetter.VersionControl => throw new NotImplementedException();

    ushort? IFormVersionGetter.FormVersion => throw new NotImplementedException();

    public bool Disable()
    {
        throw new NotImplementedException();
    }

    FormKey IMajorRecordInternal.FormKey
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    FormKey IMajorRecord.FormKey => FormKey;

    FormKey IFormKeyGetter.FormKey => FormKey;

    string? IMajorRecordIdentifier.EditorID => throw new NotImplementedException();

    public Type Type => throw new NotImplementedException();
    public bool Equals(IFormLinkGetter? other)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams = new TypedWriteParams())
    {
        throw new NotImplementedException();
    }

    public object BinaryWriteTranslator => throw new NotImplementedException();

    public bool Equals(TestMajorRecord? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return String == other.String && FormKey == other.FormKey && EditorID == other.EditorID;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TestMajorRecord)obj);
    }
}

internal class TestMajorRecord_Registration : ARegistration
{
    public override Type ClassType => typeof(TestMajorRecord);
    public override Type GetterType => typeof(ITestMajorRecord);
    public override Type SetterType => typeof(ITestMajorRecordGetter);
    public override string Name => nameof(TestMajorRecord);
}
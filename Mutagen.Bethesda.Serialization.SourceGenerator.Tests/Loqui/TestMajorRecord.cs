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
}

public interface ITestMajorRecordGetter : ILoquiObjectGetter, IMajorRecordGetter
{
}

public class TestMajorRecord : ITestMajorRecord
{
    private ushort? _formVersion;
    private FormKey _formKey;
    private string? _editorId;
    private bool _isCompressed;
    private bool _isDeleted;
    private ushort? _formVersion1;
    private int _majorRecordFlagsRaw;
    private uint _versionControl;
    private string? _editorId1;
    private FormKey _formKey1;
    private bool _isCompressed1;
    private bool _isDeleted1;
    private int _majorRecordFlagsRaw1;
    private uint _versionControl1;
    private string? _editorId2;
    private FormKey _formKey2;

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

    public void RemapListedAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IAssetLink> EnumerateListedAssetLinks()
    {
        throw new NotImplementedException();
    }

    public ILoquiRegistration Registration { get; }
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
        get => _isCompressed1;
        set => _isCompressed1 = value;
    }

    bool IMajorRecord.IsDeleted
    {
        get => _isDeleted1;
        set => _isDeleted1 = value;
    }

    bool IMajorRecordGetter.IsCompressed => _isCompressed;

    bool IMajorRecordGetter.IsDeleted => _isDeleted;

    ushort? IMajorRecordGetter.FormVersion => _formVersion1;

    int IMajorRecordGetter.MajorRecordFlagsRaw => _majorRecordFlagsRaw;

    uint IMajorRecord.VersionControl
    {
        get => _versionControl1;
        set => _versionControl1 = value;
    }

    string? IMajorRecord.EditorID
    {
        get => _editorId2;
        set => _editorId2 = value;
    }

    int IMajorRecord.MajorRecordFlagsRaw
    {
        get => _majorRecordFlagsRaw1;
        set => _majorRecordFlagsRaw1 = value;
    }

    uint IMajorRecordGetter.VersionControl => _versionControl;

    string? IMajorRecordGetter.EditorID => _editorId1;

    ushort? IFormVersionGetter.FormVersion => _formVersion;

    public bool Disable()
    {
        throw new NotImplementedException();
    }

    FormKey IMajorRecordInternal.FormKey
    {
        get => _formKey2;
        set => _formKey2 = value;
    }

    FormKey IMajorRecord.FormKey => _formKey1;

    FormKey IFormKeyGetter.FormKey => _formKey;

    string? IMajorRecordIdentifier.EditorID => _editorId;

    public Type Type { get; }
    public bool Equals(IFormLinkGetter? other)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams = new TypedWriteParams())
    {
        throw new NotImplementedException();
    }

    public object BinaryWriteTranslator { get; }
}

internal class TestMajorRecord_Registration : ARegistration
{
    public override ObjectKey ObjectKey { get; } = new(StaticProtocolKey, 7, 0);
    public override Type ClassType => typeof(TestMajorRecord);
    public override Type GetterType => typeof(ITestMajorRecord);
    public override Type SetterType => typeof(ITestMajorRecordGetter);
    public override string Name => nameof(TestMajorRecord);
}
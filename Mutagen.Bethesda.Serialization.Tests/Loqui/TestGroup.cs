using System.Collections;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public class TestGroup : IGroup<TestMajorRecord>, IEquatable<TestGroup>
{
    public bool SomeGroupField { get; set; }
    public Cache<TestMajorRecord, FormKey> Records { get; init; } = new(x => x.FormKey);

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories, IAssetLinkCache? linkCache = null,
        Type? assetType = null)
    {
        throw new NotImplementedException();
    }

    public bool ContainsKey(FormKey key)
    {
        throw new NotImplementedException();
    }

    public IMod SourceMod => throw new NotImplementedException();
    public IEnumerable<FormKey> FormKeys => throw new NotImplementedException();
    public void SetUntyped(IMajorRecord record)
    {
        throw new NotImplementedException();
    }

    public void SetUntyped(IEnumerable<IMajorRecord> records)
    {
        throw new NotImplementedException();
    }

    IEnumerable<IMajorRecord> IGroup<TestMajorRecord>.Records => throw new NotImplementedException();

    ICache<TestMajorRecord, FormKey> IGroup<TestMajorRecord>.RecordCache => Records;

    public void Add(TestMajorRecord record)
    {
        Records.Add(record);
    }

    public void Set(TestMajorRecord record)
    {
        throw new NotImplementedException();
    }

    public void Set(IEnumerable<TestMajorRecord> records)
    {
        throw new NotImplementedException();
    }

    public bool Remove(FormKey key)
    {
        throw new NotImplementedException();
    }

    public void Remove(IEnumerable<FormKey> keys)
    {
        throw new NotImplementedException();
    }

    IEnumerable<IMajorRecord> IGroup.Records => throw new NotImplementedException();

    IEnumerable<TestMajorRecord> IGroupGetter<TestMajorRecord>.Records => throw new NotImplementedException();

    IReadOnlyCache<TestMajorRecord, FormKey> IGroupGetter<TestMajorRecord>.RecordCache => throw new NotImplementedException();

    public TestMajorRecord this[FormKey key] => throw new NotImplementedException();

    IEnumerable<IMajorRecordGetter> IGroupGetter.Records => Records.Items;

    IReadOnlyCache<IMajorRecordGetter, FormKey> IGroupGetter.RecordCache => Records;

    IMajorRecordGetter IGroupGetter.this[FormKey key] => this[key];

    IEnumerable<TestMajorRecord> IGroupCommonGetter<TestMajorRecord>.Records => throw new NotImplementedException();

    IEnumerable<TestMajorRecord> IGroupCommon<TestMajorRecord>.Records => throw new NotImplementedException();

    int IGroupCommonGetter<TestMajorRecord>.Count => throw new NotImplementedException();

    int IGroupCommonGetter.Count => throw new NotImplementedException();

    IEnumerable<ILoquiObject> IGroupCommonGetter.Records => Records.Items;

    public ILoquiRegistration ContainedRecordRegistration => throw new NotImplementedException();
    public Type ContainedRecordType => throw new NotImplementedException();
    public IEnumerator<TestMajorRecord> GetEnumerator() => Records.Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    int IReadOnlyCollection<TestMajorRecord>.Count => Records.Count;

    public void RemapListedAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IAssetLink> EnumerateListedAssetLinks()
    {
        throw new NotImplementedException();
    }

    public void AddUntyped(IMajorRecord record)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        SomeGroupField = default;
        Records.Clear();
    }

    public bool Equals(TestGroup? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SomeGroupField == other.SomeGroupField && Records.SequenceEqual(other.Records);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TestGroup)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (SomeGroupField.GetHashCode() * 397) ^ Records.GetHashCode();
        }
    }
}
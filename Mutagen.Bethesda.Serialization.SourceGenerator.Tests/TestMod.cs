using System.IO.Abstractions;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public partial class TestMod : AMod, ILoquiObject, IModGetter
{
    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true)
        where T : class, IMajorRecordQueryableGetter
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> EnumerateMajorRecordSimpleContexts()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IModContext<TMajor>> EnumerateMajorRecordSimpleContexts<TMajor>(bool throwIfUnknown = true)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> EnumerateMajorRecordSimpleContexts(Type t,
        bool throwIfUnknown = true)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        throw new NotImplementedException();
    }

    public ModKey ModKey { get; }

    public IMask<bool> GetEqualsMask(object rhs,
        EqualsMaskHelper.Include include = EqualsMaskHelper.Include.OnlyFailures)
    {
        throw new NotImplementedException();
    }

    public override void SyncRecordCount()
    {
        throw new NotImplementedException();
    }

    public IGroupGetter<TMajor> GetTopLevelGroup<TMajor>() where TMajor : IMajorRecordGetter
    {
        throw new NotImplementedException();
    }

    public IGroupGetter GetTopLevelGroup(Type type)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinary(FilePath path, BinaryWriteParameters? param = null, IFileSystem? fileSystem = null)
    {
        throw new NotImplementedException();
    }

    public void WriteToBinaryParallel(FilePath path, BinaryWriteParameters? param = null,
        IFileSystem? fileSystem = null,
        ParallelWriteParameters? parallelWriteParameters = null)
    {
        throw new NotImplementedException();
    }

    public override GameRelease GameRelease { get; }
    public override bool CanUseLocalization { get; }
    public override bool UsingLocalization { get; set; }
    public ILoquiRegistration Registration { get; }
}
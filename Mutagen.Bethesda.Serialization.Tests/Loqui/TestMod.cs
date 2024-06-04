using System.IO.Abstractions;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Serialization.SourceGenerator.Tests;

public partial interface ITestMod : ITestModGetter, ILoquiObject, IMod
{
}

public partial interface ITestModGetter : ILoquiObjectGetter, IModGetter
{
}

internal class TestMod_Registration : ARegistration
{
    public override Type ClassType => typeof(TestMod);
    public override Type GetterType => typeof(ITestModGetter);
    public override Type SetterType => typeof(ITestMod);
    public override string Name => nameof(TestMod);
}

public partial class TestMod : AMod, ILoquiObject, IModGetter, IMajorRecordEnumerable, ITestMod
{
    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        throw new NotImplementedException();
    }

    IEnumerable<TMajor> IMajorRecordEnumerable.EnumerateMajorRecords<TMajor>(bool throwIfUnknown)
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

    public override uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges)
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
    public override bool CanBeLightMaster { get; }
    public override bool IsLightMaster { get; set; }
    public override bool CanBeHalfMaster { get; }
    public override bool IsHalfMaster { get; set; }
    public ILoquiRegistration Registration => throw new NotImplementedException();
    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        throw new NotImplementedException();
    }
}
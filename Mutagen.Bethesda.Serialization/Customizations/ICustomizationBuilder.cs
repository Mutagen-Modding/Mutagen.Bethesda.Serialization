using System.Linq.Expressions;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Serialization.Customizations;

public interface ICustomizationBuilder
{
    IFilePerRecordCustomizationBuilder FilePerRecord();
    ICustomizationBuilder OmitLastModifiedData();
    ICustomizationBuilder OmitTimestampData();
    ICustomizationBuilder OmitUnknownGroupData();
    ICustomizationBuilder SkipRecordsWithErrors();
}

public interface IFilePerRecordCustomizationBuilder : ICustomizationBuilder
{
    IFilePerRecordCustomizationBuilder EnforceRecordOrder();
}

public interface ICustomizationBuilder<TObject>
{
    ICustomizationBuilder<TObject> Omit<TField>(Expression<Func<TObject, TField>> field);
    ICustomizationBuilder<TObject> Omit<TField>(Expression<Func<TObject, TField>> field, Func<TObject, TField, bool> predicate);
    ICustomizationBuilder<TObject> EmbedRecordsInSameFile(Expression<Func<TObject, IMajorRecordGetter?>> field);
    ICustomizationBuilder<TObject> EmbedRecordsInSameFile(Expression<Func<TObject, IReadOnlyList<IMajorRecordGetter>?>> field);
}
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
    IListSortBuilder<TObject, TItem> SortList<TItem>(Expression<Func<TObject, IEnumerable<TItem>?>> listField);
}

public interface IListSortBuilder<TObject, TItem>
{
    IListSortFieldChainBuilder<TObject, TItem> ByField<TField>(Expression<Func<TItem, TField>> itemField);
}

public interface IListSortFieldChainBuilder<TObject, TItem> : ICustomizationBuilder<TObject>
{
    IListSortFieldChainBuilder<TObject, TItem> ThenByField<TField>(Expression<Func<TItem, TField>> itemField);
}
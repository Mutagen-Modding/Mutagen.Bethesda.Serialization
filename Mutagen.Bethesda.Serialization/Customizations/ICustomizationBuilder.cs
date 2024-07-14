using System.Linq.Expressions;

namespace Mutagen.Bethesda.Serialization.Customizations;

public interface ICustomizationBuilder
{
    IFilePerRecordCustomizationBuilder FilePerRecord();
    ICustomizationBuilder OmitLastModifiedData();
    ICustomizationBuilder OmitTimestampData();
}

public interface IFilePerRecordCustomizationBuilder : ICustomizationBuilder
{
    IFilePerRecordCustomizationBuilder EnforceRecordOrder();
}

public interface ICustomizationBuilder<TObject>
{
    ICustomizationBuilder<TObject> Omit<TField>(Expression<Func<TObject, TField>> field);
    ICustomizationBuilder<TObject> Omit<TField>(Expression<Func<TObject, TField>> field, Func<TObject, TField, bool> predicate);
}
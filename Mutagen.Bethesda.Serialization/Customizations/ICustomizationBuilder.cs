using System.Linq.Expressions;

namespace Mutagen.Bethesda.Serialization.Customizations;

public interface ICustomizationBuilder
{
    ICustomizationBuilder FolderPerRecord();
}

public interface ICustomizationBuilder<TObject>
{
    ICustomizationBuilder<TObject> Omit<TField>(Expression<Func<TObject, TField>> field);
    ICustomizationBuilder<TObject> Omit<TField>(Expression<Func<TObject, TField>> field, Func<TObject, TField, bool> predicate);
}
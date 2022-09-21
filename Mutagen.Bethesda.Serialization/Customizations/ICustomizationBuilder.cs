using System.Linq.Expressions;

namespace Mutagen.Bethesda.Serialization.Customizations;

public interface ICustomizationBuilder<TObject>
{
    void Omit<TField>(Expression<Func<TObject, TField>> field);
    void Omit<TField>(Expression<Func<TObject, TField>> field, Func<TObject, TField, bool> predicate);
}
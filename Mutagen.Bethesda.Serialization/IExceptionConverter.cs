namespace Mutagen.Bethesda.Serialization;

public interface IExceptionConverter
{
    Exception ConvertException(Exception ex);
}
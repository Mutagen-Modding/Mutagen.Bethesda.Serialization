using System.Text;
using YamlDotNet.Core;

namespace Mutagen.Bethesda.Serialization.Yaml;

public class SpriggitYamlException : Exception
{
    public SpriggitYamlException(YamlException exception)
        : base("YAML had a problem parsing", exception)
    {
    }

    public override string ToString()
    {
        return FlattenException(this);
    }

    static string FlattenException(Exception? exception)
    {
        var stringBuilder = new StringBuilder();

        while (exception != null)
        {
            stringBuilder.AppendLine(exception.Message);
            stringBuilder.AppendLine(exception.StackTrace);

            exception = exception.InnerException;
        }

        return stringBuilder.ToString();
    }
}
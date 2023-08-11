namespace Mutagen.Bethesda.Serialization.Utility;

public class TimeOnlyHelper
{
    public static string TimeOnlyPrinter(TimeOnly to)
    {
        return to.ToString("o");
    }
}
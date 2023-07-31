namespace Mutagen.Bethesda.Serialization.Utility;

public class TimeOnlyHelper
{
    public static string TimeOnlyPrinter(TimeOnly to)
    {
        if (to.Millisecond == 0 && to.Microsecond == 0)
        {
            return to.ToLongTimeString();
        }

        return to.ToString("O");
    }
}
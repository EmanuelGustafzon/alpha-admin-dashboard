
namespace Business.Services;

public static class DateCalculator
{
    public static string GetTimeDiffFromToday(DateTime startDate, DateTime endDate)
    {
        DateTime today = DateTime.Today;

        if (today < startDate)
        {
            TimeSpan diff = startDate - today;
            return FormatTimeSpan(diff, "left");
        }
        if (today > endDate)
        {
            TimeSpan diff = today - endDate;
            return FormatTimeSpan(diff, "overdue");
        }
            
        return "in process";  
    }

    private static string FormatTimeSpan(TimeSpan diff, string suffix)
    {
        int totalDays = (int)diff.TotalDays;

        if (totalDays < 7)
        {
            return $"{totalDays} day{(totalDays != 1 ? "s" : "")} {suffix}";
        }
        if (totalDays < 30)
        {
            int weeks = totalDays / 7;
            return $"{weeks} week{(weeks != 1 ? "s" : "")} {suffix}";
        }

        int months = totalDays / 30; 
        return $"{months} month{(months != 1 ? "s" : "")} {suffix}";

    }
}

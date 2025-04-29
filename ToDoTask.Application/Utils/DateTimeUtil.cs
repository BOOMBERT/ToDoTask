using ToDoTask.Application.Interfaces;
using ToDoTask.Domain.Constants;
using ToDoTask.Domain.Exceptions;

namespace ToDoTask.Application.Utils;

public static class DateTimeUtil
{
    public static (DateTime UtcStart, DateTime UtcEnd) GetUtcDateRange(DateTimeRange range, TimeZoneInfo timeZone, IClock clock)
    {
        DateTime localToday = TimeZoneInfo.ConvertTime(clock.UtcNow, timeZone).Date;
        DateTime localStart, localEnd;

        switch (range)
        {
            case DateTimeRange.Today:
                localStart = localToday;
                localEnd = localToday.AddDays(1);
                break;
            case DateTimeRange.Tomorrow:
                localStart = localToday.AddDays(1);
                localEnd = localStart.AddDays(1);
                break;
            case DateTimeRange.ThisWeek:
                int diff = (7 + (localToday.DayOfWeek - DayOfWeek.Monday)) % 7;
                localStart = localToday.AddDays(-diff);
                localEnd = localStart.AddDays(7);
                break;
            default:
                throw new BadRequestException($"Unsupported DateTimeRange value: '{range}'.");
        }

        return (
            TimeZoneInfo.ConvertTimeToUtc(localStart, timeZone),
            TimeZoneInfo.ConvertTimeToUtc(localEnd, timeZone)
        );
    }
}

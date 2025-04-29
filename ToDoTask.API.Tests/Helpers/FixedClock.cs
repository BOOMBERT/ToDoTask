using ToDoTask.Application.Interfaces;

namespace ToDoTask.API.Tests.Helpers;

public class FixedClock : IClock
{
    private readonly DateTime _utcNow;

    public FixedClock(DateTime utcNow)
    {
        _utcNow = utcNow;
    }

    public DateTime UtcNow => _utcNow;
}

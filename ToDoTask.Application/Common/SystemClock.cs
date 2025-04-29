using ToDoTask.Application.Interfaces;

namespace ToDoTask.Application.Common;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}

﻿namespace ToDoTask.Application.Interfaces;

public interface IClock
{
    DateTime UtcNow { get; }
}

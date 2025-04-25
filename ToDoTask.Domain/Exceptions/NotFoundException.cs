using System.Net;

namespace ToDoTask.Domain.Exceptions;

public class NotFoundException : CustomException
{
    public NotFoundException(string resourceType, string resourceIdentifier)
        : base(
            "Resource not found", 
            HttpStatusCode.NotFound, 
            $"{resourceType} with identifier '{resourceIdentifier}' was not found"
            ) { }
}

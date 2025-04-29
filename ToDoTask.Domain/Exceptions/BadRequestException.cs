using System.Net;

namespace ToDoTask.Domain.Exceptions;

public class BadRequestException : CustomException
{
    public BadRequestException(object details) 
        : base(
            "Bad Request", 
            HttpStatusCode.BadRequest, 
            details
            ) { }
}

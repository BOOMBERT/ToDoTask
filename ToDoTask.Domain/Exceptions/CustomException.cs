using System.Net;

namespace ToDoTask.Domain.Exceptions;

public class CustomException : Exception
{
    public string Title { get; }
    public HttpStatusCode StatusCode { get; }

    public CustomException(string title, HttpStatusCode statusCode, object details) : base(details.ToString())
    {
        Title = title;
        StatusCode = statusCode;
    }
}

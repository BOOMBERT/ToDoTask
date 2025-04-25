using Microsoft.AspNetCore.Mvc;
using System.Net;
using ToDoTask.Domain.Exceptions;

namespace ToDoTask.API.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            var (title, statusCode, message) = ex switch
            {
                CustomException customException => (customException.Title, customException.StatusCode, customException.Message),
                _ => ("An Unexpected Error Occurred", HttpStatusCode.InternalServerError, ex.Message)
            };

            await ReturnErrorResponse(context, title, statusCode, message);
        }
    }

    private async Task ReturnErrorResponse(HttpContext context, string title, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{(int)statusCode}",
            Title = title,
            Status = (int)statusCode,
            Detail = message,
            Instance = context.Request.Path,
            Extensions = { ["traceId"] = context.TraceIdentifier }
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}

using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace DonutReceiptService.API.Common.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred.";

        if (exception is FluentResults.IResultBase resultBase && resultBase.IsFailed)
        {
            var firstError = resultBase.Errors.FirstOrDefault();
            if (firstError != null)
            {
                message = firstError.Message;
                if (firstError.Metadata.TryGetValue("StatusCode", out var sc) && sc is int sCode)
                {
                    statusCode = sCode;
                }
            }
        }

        var problemDetails = new ProblemDetails
        {
            Title = "Operation Failed",
            Detail = message,
            Status = statusCode
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}

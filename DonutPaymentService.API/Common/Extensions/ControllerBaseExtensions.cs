using FluentResults;
using Microsoft.AspNetCore.Mvc;
using DonutPaymentService.API.Common.Errors;

namespace DonutPaymentService.API.Common.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static ActionResult<T> Success<T>(this ControllerBase controller, T value)
        {
            return controller.Ok(value);
        }

        public static ActionResult Success(this ControllerBase controller)
        {
            return controller.Ok();
        }

        public static ActionResult Created<T>(this ControllerBase controller, string uri, T value)
        {
            return controller.Created(uri, value);
        }

        public static ActionResult Created(this ControllerBase controller, string uri)
        {
            return controller.Created(uri, null);
        }

        public static ActionResult NoContent(this ControllerBase controller)
        {
            return controller.NoContent();
        }

        public static ActionResult Error(this ControllerBase controller, string errorCode, int statusCode, string? message = null)
        {
            var error = new ApplicationError(errorCode, message ?? ErrorMessages.GetMessage(errorCode), statusCode);
            return new ObjectResult(new ProblemDetails { Status = statusCode, Title = message, Detail = message, Extensions = { { "errorCode", errorCode } } }) { StatusCode = statusCode };
        }

        public static ActionResult ValidationError(this ControllerBase controller, string propertyName, string errorMessage)
        {
            var error = new ValidationError(propertyName, errorMessage);
            return Result.Fail(error).ToActionResult(controller.HttpContext);
        }

        public static ActionResult ValidationError(this ControllerBase controller, Dictionary<string, string[]> validationErrors)
        {
            var error = new ValidationError(validationErrors);
            return Result.Fail(error).ToActionResult(controller.HttpContext);
        }
    }
}



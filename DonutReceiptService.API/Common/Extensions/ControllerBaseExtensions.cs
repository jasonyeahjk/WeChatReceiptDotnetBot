using FluentResults;
using Microsoft.AspNetCore.Mvc;
using DonutReceiptService.API.Common.Errors;
using DonutReceiptService.API.Common.Extensions;

namespace DonutReceiptService.API.Common.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static ActionResult Success(this ControllerBase controller)
        {
            return new OkResult();
        }

        public static ActionResult<T> Success<T>(this ControllerBase controller, T value)
        {
            return new OkObjectResult(value);
        }

        public static ActionResult Error(this ControllerBase controller, string errorCode, int statusCode = 500, string? message = null)
        {
            var error = new ApplicationError(errorCode, message ?? ErrorMessages.GetMessage(errorCode), statusCode);
            return Result.Fail(error).ToActionResult(controller.HttpContext);
        }

        public static ActionResult<T> Error<T>(this ControllerBase controller, string errorCode, int statusCode = 500, string? message = null)
        {
            var error = new ApplicationError(errorCode, message ?? ErrorMessages.GetMessage(errorCode), statusCode);
            return Result.Fail<T>(error).ToActionResult(controller.HttpContext);
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


using FluentResults;
using Microsoft.AspNetCore.Mvc;

using DonutReceiptService.API.Common.Errors;

namespace DonutReceiptService.API.Common.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToActionResult(this Result result, HttpContext httpContext)
        {
            if (result.IsSuccess)
            {
                return new OkResult();
            }

            return CreateProblemResult(result, httpContext);
        }

        public static ActionResult<T> ToActionResult<T>(this Result<T> result, HttpContext httpContext)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Value);
            }

            return CreateProblemResult(result, httpContext);
        }

        public static ActionResult ToCreatedResult(this Result result, string uri, HttpContext httpContext)
        {
            if (result.IsSuccess)
            {
                return new CreatedResult(uri, null);
            }

            return CreateProblemResult(result, httpContext);
        }

        public static ActionResult<T> ToCreatedResult<T>(this Result<T> result, string uri, HttpContext httpContext)
        {
            if (result.IsSuccess)
            {
                return new CreatedResult(uri, result.Value);
            }

            return CreateProblemResult(result, httpContext);
        }

        public static ActionResult ToNoContentResult(this Result result, HttpContext httpContext)
        {
            if (result.IsSuccess)
            {
                return new NoContentResult();
            }

            return CreateProblemResult(result, httpContext);
        }

        private static ActionResult CreateProblemResult(ResultBase result, HttpContext httpContext)
        {
            var problemDetails = new ProblemDetails
            {
                Instance = httpContext?.Request.Path
            };

            var firstError = result.Errors.FirstOrDefault();

            // If the result is a success, but we are somehow in CreateProblemResult, return a generic error
            if (result.IsSuccess)
            {
                return new ObjectResult(new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = 500,
                    Detail = "A successful result was passed to the error handler.",
                    Type = "https://httpstatuses.com/500",
                    Instance = httpContext?.Request.Path
                }) { StatusCode = 500 };
            }

            if (firstError is ValidationError validationError)
            {
                var validationProblemDetails = new ValidationProblemDetails(validationError.ValidationErrors)
                {
                    Title = "Validation Error",
                    Status = 422,
                    Type = "https://httpstatuses.com/422",
                    Detail = validationError.Message,
                    Instance = httpContext?.Request.Path
                };
                return new ObjectResult(validationProblemDetails) { StatusCode = 422 };
            }
            else if (firstError is ApplicationError applicationError)
            {
                problemDetails.Title = GetTitleForStatusCode(applicationError.StatusCode);
                problemDetails.Status = applicationError.StatusCode;
                problemDetails.Detail = applicationError.Message;
                problemDetails.Type = $"https://httpstatuses.com/{applicationError.StatusCode}";
                problemDetails.Extensions["errorCode"] = applicationError.ErrorCode;

                return new ObjectResult(problemDetails) { StatusCode = applicationError.StatusCode };
            }
            else
            {
                problemDetails.Title = "An error occurred";
                problemDetails.Status = 500;
                problemDetails.Detail = firstError?.Message ?? "An unexpected error occurred.";
                problemDetails.Type = "https://httpstatuses.com/500";
                problemDetails.Extensions["errorCode"] = ErrorCodes.SYSTEM_INTERNAL_ERROR;

                return new ObjectResult(problemDetails) { StatusCode = 500 };
            }
        }

        private static string GetTitleForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                409 => "Conflict",
                422 => "Unprocessable Entity",
                500 => "Internal Server Error",
                503 => "Service Unavailable",
                _ => "An error occurred"
            };
        }
    }
}


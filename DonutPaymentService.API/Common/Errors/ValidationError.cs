using FluentResults;
using System.Collections.Generic;

namespace DonutPaymentService.API.Common.Errors
{
    public class ValidationError : Error
    {
        public Dictionary<string, string[]> ValidationErrors { get; }

        public ValidationError(string message) : base(message)
        {
            ValidationErrors = new Dictionary<string, string[]>();
            Metadata.Add("errorCode", ErrorCodes.SYSTEM_VALIDATION_FAILED);
        }

        public ValidationError(string propertyName, string errorMessage) : base($"{propertyName}: {errorMessage}")
        {
            ValidationErrors = new Dictionary<string, string[]>
            {
                { propertyName, new[] { errorMessage } }
            };
            Metadata.Add("errorCode", ErrorCodes.SYSTEM_VALIDATION_FAILED);
        }

        public ValidationError(Dictionary<string, string[]> validationErrors) : base("One or more validation errors occurred.")
        {
            ValidationErrors = validationErrors;
            Metadata.Add("errorCode", ErrorCodes.SYSTEM_VALIDATION_FAILED);
        }
    }
}


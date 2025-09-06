using FluentResults;

namespace DonutReceiptService.API.Common.Errors
{
    public class ApplicationError : Error
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }

        public ApplicationError(string errorCode, string message, int statusCode = 500)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            Metadata.Add("errorCode", errorCode);
        }

        public ApplicationError(string errorCode, string message, string propertyName, int statusCode = 500)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            Metadata.Add("errorCode", errorCode);
            Metadata.Add("propertyName", propertyName);
        }
    }
}


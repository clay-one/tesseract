using System.Collections.Generic;
using Tesseract.Common.Extensions;

namespace Tesseract.Common.Results
{
    public class ApiValidationError
    {
        public ApiValidationError()
        {
        }

        public ApiValidationError(string errorKey)
        {
            ErrorKey = errorKey;
        }

        public ApiValidationError(string propertyPath, string errorKey)
        {
            PropertyPath = propertyPath;
            ErrorKey = errorKey;
        }

        public ApiValidationError(string errorKey, IEnumerable<string> errorParameters)
        {
            ErrorKey = errorKey;
            ErrorParameters = errorParameters.SafeToList(null);
        }

        public ApiValidationError(string propertyPath, string errorKey, IEnumerable<string> errorParameters)
        {
            PropertyPath = propertyPath;
            ErrorKey = errorKey;
            ErrorParameters = errorParameters.SafeToList(null);
        }

        public string PropertyPath { get; set; }

        public string ErrorKey { get; set; }

        public List<string> ErrorParameters { get; set; }

        public string LocalizedMessage { get; set; }
    }
}
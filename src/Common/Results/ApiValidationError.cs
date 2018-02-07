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
            this.ErrorKey = errorKey;
        }

        public ApiValidationError(string propertyPath, string errorKey)
        {
            this.PropertyPath = propertyPath;
            this.ErrorKey = errorKey;
        }

        public ApiValidationError(string errorKey, IEnumerable<string> errorParameters)
        {
            this.ErrorKey = errorKey;
            this.ErrorParameters = errorParameters.SafeToList<string>((List<string>) null);
        }

        public ApiValidationError(string propertyPath, string errorKey, IEnumerable<string> errorParameters)
        {
            this.PropertyPath = propertyPath;
            this.ErrorKey = errorKey;
            this.ErrorParameters = errorParameters.SafeToList<string>((List<string>) null);
        }

        public string PropertyPath { get; set; }

        public string ErrorKey { get; set; }

        public List<string> ErrorParameters { get; set; }

        public string LocalizedMessage { get; set; }
    }
}
using System;

namespace Tesseract.Common.Results
{
    public class ApiValidatedResult<T>
    {
        public static ApiValidatedResult<T> Ok(T response)
        {
            throw new NotImplementedException();
        }

        public static ApiValidatedResult<T> Failure(ApiValidationError error)
        {
            throw new NotImplementedException();
        }
    }
}
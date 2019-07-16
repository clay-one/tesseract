using System;
using System.Collections.Generic;
using Tesseract.Common.Extensions;

namespace Tesseract.Common.Results
{
    public class ApiValidatedResult<T> : ApiValidationResult
    {
        public ApiValidatedResult()
        {
        }

        public ApiValidatedResult(T result)
        {
            Result = result;
        }

        public ApiValidatedResult(ApiValidationError error)
            : base(error)
        {
        }

        public ApiValidatedResult(IEnumerable<ApiValidationError> errors)
            : base(errors)
        {
        }

        public T Result { get; set; }

        public static  new ApiValidatedResult<T> Ok()
        {
            return new ApiValidatedResult<T>();
        }

        public static ApiValidatedResult<T> Ok(T result)
        {
            return new ApiValidatedResult<T>(result);
        }

        public static new ApiValidatedResult<T> Failure()
        {
            var apiValidatedResult = new ApiValidatedResult<T>();
            apiValidatedResult.Success = false;
            return apiValidatedResult;
        }

        public static new ApiValidatedResult<T> Failure(ApiValidationError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            return new ApiValidatedResult<T>(error);
        }

        public static new ApiValidatedResult<T> Failure(IEnumerable<ApiValidationError> errors)
        {
            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            return new ApiValidatedResult<T>(errors);
        }

        public static new ApiValidatedResult<T> Failure(string errorKey)
        {
            if (errorKey.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(errorKey));
            }

            return Failure(new ApiValidationError(errorKey));
        }

        public static new ApiValidatedResult<T> Failure(string errorKey, IEnumerable<string> errorParameters)
        {
            if (errorKey.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(errorKey));
            }

            return Failure(new ApiValidationError(errorKey, errorParameters));
        }

        public static new ApiValidatedResult<T> Failure(string propertyPath, string errorKey)
        {
            if (errorKey.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(errorKey));
            }

            return Failure(new ApiValidationError(propertyPath, errorKey));
        }

        public static new ApiValidatedResult<T> Failure(string propertyPath, string errorKey,
            IEnumerable<string> errorParameters)
        {
            if (errorKey.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(errorKey));
            }

            return Failure(new ApiValidationError(propertyPath, errorKey, errorParameters));
        }

        public static new ApiValidatedResult<T> Aggregate(params ApiValidationError[] errors)
        {
            if (errors == null)
            {
                return Ok();
            }

            var result = new ApiValidatedResult<T>();
            errors.SafeForEach(e => result.Append(e));
            return result;
        }

        public static new ApiValidatedResult<T> Aggregate(params ApiValidationResult[] results)
        {
            if (results == null)
            {
                return Ok();
            }

            var result = new ApiValidatedResult<T>();
            results.SafeForEach(r => result.Append(r));
            return result;
        }
    }
}
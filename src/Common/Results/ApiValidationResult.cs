using System;
using System.Collections.Generic;
using System.Linq;
using Tesseract.Common.Extensions;

namespace Tesseract.Common.Results
{
    public class ApiValidationResult
    {
        public ApiValidationResult()
        {
            Success = true;
            Errors = null;
        }

        public ApiValidationResult(ApiValidationError error)
        {
            if (error == null)
            {
                Success = true;
                Errors = null;
            }
            else
            {
                Success = false;
                Errors = new List<ApiValidationError>
                {
                    error
                };
            }
        }

        public ApiValidationResult(IEnumerable<ApiValidationError> errors)
        {
            if (errors == null)
            {
                Success = true;
                Errors = null;
            }
            else
            {
                var source = errors;
                // ISSUE: variable of the null type
                IEnumerable<ApiValidationError> local = null;
                Errors = source.SafeWhere(e => e != null, local).SafeToList(null);
                Success = !Errors.SafeAny(false);
            }
        }

        public bool Success { get; set; }

        public List<ApiValidationError> Errors { get; set; }

        public string Message { get; set; }

        public static ApiValidationResult Ok()
        {
            return new ApiValidationResult();
        }

        public static ApiValidationResult Failure()
        {
            return new ApiValidationResult {Success = false};
        }

        public static ApiValidationResult Failure(ApiValidationError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            return new ApiValidationResult(error);
        }

        public static ApiValidationResult Failure(IEnumerable<ApiValidationError> errors)
        {
            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            return new ApiValidationResult(errors)
            {
                Success = false
            };
        }

        public static ApiValidationResult Failure(string errorKey)
        {
            if (errorKey.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(errorKey));
            }

            return new ApiValidationResult(new ApiValidationError(errorKey));
        }

        public static ApiValidationResult Failure(string errorKey, IEnumerable<string> errorParameters)
        {
            if (errorKey.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(errorKey));
            }

            return new ApiValidationResult(new ApiValidationError(errorKey, errorParameters));
        }

        public static ApiValidationResult Failure(string propertyPath, string errorKey)
        {
            if (errorKey.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(errorKey));
            }

            return new ApiValidationResult(new ApiValidationError(propertyPath, errorKey));
        }

        public static ApiValidationResult Failure(string propertyPath, string errorKey,
            IEnumerable<string> errorParameters)
        {
            if (errorKey.IsNullOrWhitespace())
            {
                throw new ArgumentNullException(nameof(errorKey));
            }

            return new ApiValidationResult(new ApiValidationError(propertyPath, errorKey, errorParameters));
        }

        public static ApiValidationResult Aggregate(params ApiValidationError[] errors)
        {
            if (errors == null)
            {
                return Ok();
            }

            var result = new ApiValidationResult();
            errors.ForEach(e => result.Append(e));
            return result;
        }

        public static ApiValidationResult Aggregate(params ApiValidationResult[] results)
        {
            if (results == null)
            {
                return Ok();
            }

            var result = new ApiValidationResult();
            results.SafeForEach(r => result.Append(r));
            return result;
        }

        public ApiValidationResult Append(ApiValidationResult other)
        {
            if (other == null)
            {
                return this;
            }

            Success &= other.Success;
            if (other.Errors != null)
            {
                EnsureErrorsPropertyIsNotNull();
                Errors.AddRange(other.Errors.Where(e => e != null));
            }

            return this;
        }

        public ApiValidationResult Append(ApiValidationError error)
        {
            if (error != null)
            {
                EnsureErrorsPropertyIsNotNull();
                Errors.Add(error);
                Success = false;
            }

            return this;
        }

        public ApiValidationResult Append(string errorKey)
        {
            return Append(new ApiValidationError(errorKey));
        }

        public ApiValidationResult Append(string errorKey, IEnumerable<string> errorParameters)
        {
            return Append(new ApiValidationError(errorKey, errorParameters));
        }

        public ApiValidationResult Append(string propertyPath, string errorKey)
        {
            return Append(new ApiValidationError(propertyPath, errorKey));
        }

        public ApiValidationResult Append(string propertyPath, string errorKey, IEnumerable<string> errorParameters)
        {
            return Append(new ApiValidationError(propertyPath, errorKey, errorParameters));
        }

        public ApiValidatedResult<T> ToFailedValidatedResult<T>()
        {
            if (Success)
            {
                throw new InvalidOperationException(
                    "This method should only be called on a ValidationResult instance that contains errors. Consider checking the state using IsValid property before calling this method.");
            }

            return ApiValidatedResult<T>.Failure(Errors);
        }

        protected void EnsureErrorsPropertyIsNotNull()
        {
            if (Errors != null)
            {
                return;
            }

            Errors = new List<ApiValidationError>();
        }
    }
}
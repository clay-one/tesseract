using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Tesseract.Common.Results;

namespace Appson.Tesseract.Web.Base
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult NotImplemented()
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }

        protected IActionResult ValidationResult(ApiValidationResult result)
        {
            if (result.Success)
                return Ok();


            //return new ValidationErrorResult(result, this);
            return BadRequest(result);
        }

        protected IActionResult ValidatedResult<T>(ApiValidatedResult<T> result)
        {
            if (result.Success)
                return Ok(result.Result);

            //return new ValidationErrorResult(result, this);
            return BadRequest(result);
        }

        protected IActionResult ValidationError(ApiValidationError error)
        {
            //return new ValidationErrorResult(ApiValidationResult.Failure(error), this);
            return BadRequest(ApiValidationResult.Failure(error));
        }

        protected IActionResult ValidationError(IEnumerable<ApiValidationError> errors)
        {
            //return new ValidationErrorResult(ApiValidationResult.Failure(errors), this);
            return BadRequest(ApiValidationResult.Failure(errors));
        }

        protected IActionResult ValidationError(string errorKey, IEnumerable<string> errorParams = null)
        {
            //return new ValidationErrorResult(errorParams == null
            //    ? ApiValidationResult.Failure(errorKey)
            //    : ApiValidationResult.Failure(errorKey, errorParams), this);
            return BadRequest(errorParams == null
                ? ApiValidationResult.Failure(errorKey)
                : ApiValidationResult.Failure(errorKey, errorParams));
        }

        protected IActionResult ValidationError(string propertyPath, string errorKey)
        {
            //return new ValidationErrorResult(ApiValidationResult.Failure(propertyPath, errorKey), this);
            return BadRequest(ApiValidationResult.Failure(propertyPath, errorKey));
        }
    }
}

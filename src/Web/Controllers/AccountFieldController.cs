using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;

namespace Appson.Tesseract.Web.Controllers
{
    public class AccountFieldController : ApiControllerBase
    {
        // GET  	/accounts/a/:a/fields	-B, SAFE Get list of fields and their corresponding values for a specific account
        // GET  	/accounts/a/:a/fields/f/:f	-B, SAFE	Get the value of a specific field for an account
        // PUT  	/accounts/a/:a/fields/f/:f/value/:v	-B, ASYNC, IDMP Set the value of the specified field for the account
        // PATCH	/accounts/a/:a/fields/f/:f/value-delta/:d	-B, A/SYNC Change the value of the specified field for the account by a delta

        /// <summary>
        /// GET
        /// /accounts/a/:a/fields
        /// -B, SAFE
        /// Get list of fields and their corresponding values for a specific account
        /// </summary>
        [HttpGet("accounts/a/{accountId}/fields")]
        public IActionResult GetAccountFields(string accountId)
        {
            return NotImplemented();
        }

        /// <summary>
        /// GET
        /// /accounts/a/:a/fields/f/:f
        /// -B, SAFE
        /// Get the value of a specific field for an account
        /// </summary>
        [HttpGet("accounts/a/{accountId}/fields/f/{fieldId}")]
        public IActionResult GetAccountFieldValue(string accountId, string fieldId)
        {
            return NotImplemented();
        }

        /// <summary>
        /// PUT
        /// /accounts/a/:a/fields/f/:f/value/:v
        /// -B, ASYNC, IDMP
        /// Set the value of the specified field for the account
        /// </summary>
        [HttpPut("accounts/a/{accountId}/fields/f/{fieldId}/value/{fieldValue}")]
        public IActionResult SetFieldOfAccount(string accountId, string fieldId, double fieldValue)
        {
            return NotImplemented();
        }

        /// <summary>
        /// PATCH
        /// /accounts/a/:a/fields/f/:f/value-delta/:d
        /// -B, A/SYNC
        /// Change the value of the specified field for the account by a delta
        /// </summary>
        [HttpPatch("accounts/a/{accountId}/fields/f/{fieldId}/value-delta/{fieldValueDelta}")]
        public IActionResult PatchFieldOfAccount(string accountId, string fieldId, double fieldValueDelta)
        {
            return NotImplemented();
        }
    }
}
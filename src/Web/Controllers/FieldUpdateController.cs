using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;

namespace Appson.Tesseract.Web.Controllers
{
    public class FieldUpdateController : ApiControllerBase
    {
        // PUT  	/fields/f/:f/accounts	+B, ASYNC, IDMP	Set different values for a field on multiple accounts.
        // PATCH	/fields/f/:f/accounts	+B, A/SYNC Change values for a field on multiple accounts, by different delta. Sync variation returns the changed value for each account.

        /// <summary>
        /// PUT
        /// /fields/f/:f/accounts
        /// +B, ASYNC, IDMP
        /// Set different values for a field on multiple accounts.
        /// </summary>
        [HttpPut("fields/f/{fieldId}/accounts")]
        public IActionResult SeteFieldOnAccounts(string fieldId, string input)
        {
            return NotImplemented();
        }

        /// <summary>
        /// PATCH
        /// /fields/f/:f/accounts
        /// +B, A/SYNC
        /// Change values for a field on multiple accounts, by different delta. Sync variation returns the changed value for each account.
        /// </summary>
        [HttpPatch("fields/f/{fieldId}/accounts")]
        public IActionResult PatchFieldOnAccounts(string fieldId, string input)
        {
            return NotImplemented();
        }


    }
}

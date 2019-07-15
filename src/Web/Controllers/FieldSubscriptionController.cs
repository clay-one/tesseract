using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;

namespace Appson.Tesseract.Web.Controllers
{
    public class FieldSubscriptionController : ApiControllerBase
    {
        // PUT    /fields/f/:f/subscriptions	+B, IDMP	Subscribe a numeric field to an external source like a topic in Abaci (anything else?)
        // DELETE /fields/f/:f/subscriptions	-B, IDMP Remove subscription
        // GET    /fields/f/:f/subscriptions	-B, SAFE Get settings and status of subscription

        /// <summary>
        /// PUT
        /// /fields/f/:f/subscriptions
        /// +B, IDMP
        /// Subscribe a numeric field to an external source like a topic in Abaci (anything else?)
        /// </summary>
        [HttpPut("fields/f/{fieldId}/subscriptions")]
        public IActionResult SetFieldSubscription(string fieldId, string input)
        {
            return NotImplemented();
        }

        /// <summary>
        /// DELETE
        /// /fields/f/:f/subscriptions
        /// -B, IDMP
        /// Remove subscription
        /// </summary>
        [HttpDelete("fields/f/{fieldId}/subscriptions")]
        public IActionResult RemoveFieldSubscription(string fieldId)
        {
            return NotImplemented();
        }

        /// <summary>
        /// GET
        /// /fields/f/:f/subscriptions
        /// -B, SAFE
        /// Get settings and status of subscription
        /// </summary>
        [HttpGet("fields/f/{fieldId}/subscriptions")]
        public IActionResult GetFieldSubscription(string fieldId)
        {
            return NotImplemented();
        }


    }
}

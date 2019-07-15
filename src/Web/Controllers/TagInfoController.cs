using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;

namespace Appson.Tesseract.Web.Controllers
{
    public class TagInfoController : ApiControllerBase
    {
        // GET	/tags/ns/:ns/list	-B, SAFE	Get list of tags in a namespace
        // GET	/tags/ns/:ns/t/:t	-B, SAFE	Get info on a single tag, its cardinality, etc.

        /// <summary>
        /// GET
        /// /tags/ns/:ns/list
        /// -B, SAFE
        /// Get list of tags in a namespace
        /// </summary>
        [HttpGet("tags/ns/{ns}/list")]
        public IActionResult GetTagListInNamespace(string ns)
        {
            return NotImplemented();
        }

        /// <summary>
        /// GET
        /// /tags/ns/:ns/t/:t
        /// -B, SAFE
        /// Get info on a single tag, its cardinality, etc.
        /// </summary>
        [HttpGet("tags/ns/{ns}/t/{tag}")]
        public IActionResult GetTagInfo(string ns, string tag)
        {
            return NotImplemented();
        }


    }
}
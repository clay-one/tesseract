using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;

namespace Appson.Tesseract.Web.Controllers
{
    public class TagSynonymController : ApiControllerBase
    {
        // PUT  	/tags/ns/:ns/t/:t/synonyms/list	+B, IDMP	Set (replace) the list of synonyms for a specific tag
        // PATCH	/tags/ns/:ns/t/:t/synonyms/list	+B Add / remove a set of synonyms to / from a specific tag
        // GET  	/tags/ns/:ns/t/:t/synonyms/list	-B, SAFE Get list of synonyms for a specific tag
        // DELETE	/tags/ns/:ns/t/:t/synonyms/s/:s	-B, IDMP Remove a synonym from list of synonyms of a tag

        /// <summary>
        /// PUT
        /// /tags/ns/:ns/t/:t/synonyms/list
        /// +B, IDMP
        /// Set (replace) the list of synonyms for a specific tag
        /// </summary>
        [HttpPut("tags/ns/{ns}/t/{tag}/synonyms/list")]
        public IActionResult SetListOfTagSynonyms(string ns, string tag, string input)
        {
            return NotImplemented();
        }

        /// <summary>
        /// PATCH
        /// /tags/ns/:ns/t/:t/synonyms/list
        /// +B
        /// Add / remove a set of synonyms to / from a specific tag
        /// </summary>
        [HttpPatch("tags/ns/{ns}/t/{tag}/synonyms/list")]
        public IActionResult PatchListOfTagSynonyms(string ns, string tag, string input)
        {
            return NotImplemented();
        }

        /// <summary>
        /// GET
        /// /tags/ns/:ns/t/:t/synonyms/list
        /// -B, SAFE
        /// Get list of synonyms for a specific tag
        /// </summary>
        [HttpGet("tags/ns/{ns}/t/{tag}/synonyms/list")]
        public IActionResult SetListOfTagSynonyms(string ns, string tag)
        {
            return NotImplemented();
        }

        /// <summary>
        /// DELETE
        /// /tags/ns/:ns/t/:t/synonyms/s/:s
        /// -B, IDMP
        /// Remove a synonym from list of synonyms of a tag
        /// </summary>
        [HttpDelete("tags/ns/{ns}/t/{tag}/synonyms/s/{synonym}")]
        public IActionResult DeleteTagSynonym(string ns, string tag, string synonym)
        {
            return NotImplemented();
        }


    }
}

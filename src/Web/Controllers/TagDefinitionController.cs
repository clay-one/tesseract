using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.Tags;
using Tesseract.Core;
using Tesseract.Core.Logic;

namespace Appson.Tesseract.Web.Controllers
{
    public class TagDefinitionController : ApiControllerBase
    {
        // GET  	/tags/ns/:ns/settings	-B, SAFE	    Get settings and options specific to a namespace
        // PUT  	/tags/ns/:ns/settings	+B, IDMP        Set settings and options specific to a namespace
        // GET	    /tags/ns-list	        -B, SAFE        Get list of defined tag namespaces
        // GET	    /tags/ns/:ns	        -B, SAFE        Get information about a tag namespace definition
        // PUT	    /tags/ns/:ns	        +B, IDMP        Define a new tag namespace, or update its parameters and settings
        // DELETE	/tags/ns/:ns	        -B, TASK, IDMP	Remote namespace settings and all tags in it from all accounts
        // DELETE	/tags/ns/:ns/t/:t	    -B, TASK, IDMP  Remove a single tag from all accounts
        // POST 	/tags/ns/:ns/t/:t/copies	+B, TASK	Duplicate tag (along with all accounts and weights)


        private readonly IDefinitionLogic _definitionLogic;
        private readonly IInputValidationLogic _validation;

        public TagDefinitionController(IDefinitionLogic definitionLogic, IInputValidationLogic inputValidationLogic)
        {
            _definitionLogic = definitionLogic;
            _validation = inputValidationLogic;
        }

        /// <summary>
        /// GET
        /// /tags/ns/:ns/settings
        /// -B, SAFE
        /// Get settings and options specific to a namespace
        /// </summary>
        [HttpGet("tags/ns/{ns}/settings")]
        public IActionResult GetTagNamespaceSettings(string ns)
        {
            return NotImplemented();
        }

        /// <summary>
        /// PUT
        /// /tags/ns/:ns/settings
        /// +B, IDMP
        /// Set settings and options specific to a namespace
        /// </summary>
        [HttpPut("tags/ns/{ns}/settings")]
        public IActionResult SetTagNamespaceSettings(string ns, string input)
        {
            return NotImplemented();
        }

        /// <summary>
        /// GET	    
        /// /tags/ns-list	
        /// -B, SAFE    
        /// Get list of defined tag namespaces
        /// </summary>
        /// <returns></returns>
        [HttpGet("tags/ns-list")]
        public async Task<IActionResult> GetTagNsList()
        {
            var allNamespaces = await _definitionLogic.LoadAllNsDefinitions();

            return Ok(new GetTagNsListResponse
            {
                TagNamespaces = allNamespaces.Select(ns => new GetTagNsListItem
                {
                    TagNamespace = ns.Namespace,
                    CreationTime = ns.CreationTime,
                    LastModificationTime = ns.LastModificationTime,
                    KeepHistory = ns.KeepHistory
                }).ToList()
            });
        }

        /// <summary>
        /// GET	    
        /// /tags/ns/:ns	
        /// -B, SAFE    
        /// Get information about a tag namespace definition
        /// </summary>
        [HttpGet("tags/ns/{ns}")]
        public async Task<IActionResult> GetTagNsDefinition(string ns)
        {
            var validationError = _validation.ValidateTagNs(ns);
            if (validationError != null)
                return ValidationError(validationError);

            var definition = await _definitionLogic.LoadNsDefinition(ns);
            if (definition == null)
                return NotFound();

            return Ok(new GetTagNsDefinitionResponse
            {
                TagNamespace = definition.Namespace,
                CreationTime = definition.CreationTime,
                LastModificationTime = definition.LastModificationTime,
                KeepHistory = definition.KeepHistory
            });
        }

        /// <summary>
        /// PUT	    
        /// /tags/ns/:ns	
        /// +B, IDMP    
        /// Define a new tag namespace, or update its parameters and settings
        /// </summary>
        [HttpPut("tags/ns/{ns}")]
        public async Task<IActionResult> PutTagNsDefinition(string ns, PutTagNsDefinitionRequest request)
        {
            if (request == null)
                return ValidationError(ErrorKeys.InputCanNotBeEmpty);

            var validationError = _validation.ValidateTagNs(ns);
            if (validationError != null)
                return ValidationError(validationError);

            await _definitionLogic.AddOrUpdateTagNsDefinition(ns, request);
            return Ok();
        }

        /// <summary>
        /// DELETE
        /// /tags/ns/:ns
        /// -B, TASK, IDMP
        /// Remote namespace settings and all tags in it from all accounts
        /// </summary>
        [HttpDelete("tags/ns/{ns}")]
        public IActionResult DeleteTagNsDefinition(string ns)
        {
            return NotImplemented();
        }

        /// <summary>
        /// DELETE
        /// /tags/ns/:ns/t/:t
        /// -B, TASK, IDMP
        /// Remove a single tag from all accounts
        /// </summary>
        [HttpDelete("tags/ns/{ns}/t/{tag}")]
        public IActionResult DeleteTag(string ns, string tag)
        {
            return NotImplemented();
        }

        /// <summary>
        /// POST
        /// /tags/ns/:ns/t/:t/copies
        /// +B, TASK
        /// Duplicate tag (along with all accounts and weights)
        /// </summary>
        [HttpPost("tags/ns/{ns}/t/{tag}/copies")]
        public IActionResult CopyTag(string ns, string tag, string input)
        {
            return NotImplemented();
        }


    }
}

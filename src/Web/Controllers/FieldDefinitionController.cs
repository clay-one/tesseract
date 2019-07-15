using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.Fields;
using Tesseract.Core;
using Tesseract.Core.Logic;

namespace Appson.Tesseract.Web.Controllers
{
    public class FieldDefinitionController : ApiControllerBase
    {
        // GET		/fields/list	-B, SAFE	Get list of defined fields
        // GET		/fields/f/:f	-B, SAFE Get information about a field definition
        // PUT		/fields/f/:f	+B, IDMP Set or update field parameters(if any!)
        // DELETE	/fields/f/:f	-B, IDMP Remove a field definition and all related data


        private readonly IDefinitionLogic _definitionLogic;
        private readonly IInputValidationLogic _validation;

        public FieldDefinitionController(IDefinitionLogic definitionLogic, IInputValidationLogic inputValidationLogic)
        {
            _definitionLogic = definitionLogic;
            _validation = inputValidationLogic;
        }

        /// <summary>
        /// GET	
        /// /fields/list
        /// -B, SAFE
        /// Get list of defined fields
        /// </summary>
        [HttpGet("fields/list")]
        public async Task<IActionResult> GetFieldDefinitionList()
        {
            var allFields = await _definitionLogic.LoadAllFieldDefinitions();

            return Ok(new GetFieldDefinitionListResponse
            {
                Fields = allFields.Select(f => new GetFieldDefinitionListItem
                {
                    FieldName = f.FieldName,
                    CreationTime = f.CreationTime,
                    LastModificationTime = f.LastModificationTime,
                    KeepHistory = f.KeepHistory
                }).ToList()
            });
        }

        /// <summary>
        /// GET	
        /// /fields/f/:f
        /// -B, SAFE
        /// Get information about a field definition
        /// </summary>
        [HttpGet("fields/f/{fieldName}")]
        public async Task<IActionResult> GetFieldDefinition(string fieldName)
        {
            var validationError = _validation.ValidateFieldName(fieldName);
            if (validationError != null)
                return ValidationError(validationError);

            var definition = await _definitionLogic.LoadFieldDefinition(fieldName);
            if (definition == null)
                return NotFound();

            return Ok(new GetFieldDefinitionResponse
            {
                FieldName = definition.FieldName,
                CreationTime = definition.CreationTime,
                LastModificationTime = definition.LastModificationTime,
                KeepHistory = definition.KeepHistory
            });
        }

        /// <summary>
        /// PUT
        /// /fields/f/:f
        /// +B, IDMP
        /// Set or update field parameters(if any!)
        /// </summary>
        [HttpPut("fields/f/{fieldName}")]
        public async Task<IActionResult> PutFieldDefinition(string fieldName, PutFieldDefinitionRequest request)
        {
            if (request == null)
                return ValidationError(ErrorKeys.InputCanNotBeEmpty);

            var validationError = _validation.ValidateFieldName(fieldName);
            if (validationError != null)
                return ValidationError(validationError);

            await _definitionLogic.AddOrUpdateFieldDefinition(fieldName, request);
            return Ok();
        }

        /// <summary>
        /// DELETE
        /// /fields/f/:f
        /// -B, IDMP
        /// Remove a field definition and all related data
        /// </summary>
        [HttpDelete("fields/f/{fieldId}")]
        public IActionResult DeleteFieldDefinition(string fieldId)
        {
            return NotImplemented();
        }

    }
}

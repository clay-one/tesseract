using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.Accounts;
using Tesseract.Common.Results;
using Tesseract.Core;
using Tesseract.Core.Logic;
using Tesseract.Core.Storage;

namespace Appson.Tesseract.Web.Controllers
{
    public class AccountTagNamespaceController : ApiControllerBase
    {
        // GET  	/accounts/a/:a/tags/ns/:ns	-B, SAFE Get list of tags under a certain namespace and their weights for a specific account
        // PUT  	/accounts/a/:a/tags/ns/:ns	+B, ASYNC, IDMP	Replace all tags within a given namespace with the provided tags and weights
        // DELETE	/accounts/a/:a/tags/ns/:ns	-B, ASYNC, IDMP Remove all tags from within a namespace from the account


        private readonly IAccountStore _accountStore;
        private readonly ITaggingLogic _taggingLogic;
        private readonly IInputValidationLogic _validation;

        public AccountTagNamespaceController(IAccountStore accountStore,
            ITaggingLogic taggingLogic,
            IInputValidationLogic inputValidationLogic)
        {
            _accountStore = accountStore;
            _taggingLogic = taggingLogic;
            _validation = inputValidationLogic;
        }

        /// <summary>
        /// GET
        /// /accounts/a/:a/tags/ns/:ns
        /// -B, SAFE
        /// Get list of tags under a certain namespace and their weights for a specific account
        /// </summary>
        [HttpGet("accounts/a/{accountId}/tags/ns/{ns}")]
        public async Task<IActionResult> GetTagsOfAccountInNs(string accountId, string ns)
        {
            var validationResult = ApiValidationResult.Aggregate(
                _validation.ValidateAccountId(accountId),
                _validation.ValidateExistingTagNs(ns)
            );

            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var account = await _accountStore.LoadAccount(null, accountId);
            if (account == null)
                return NotFound();

            var response = new GetTagsOfAccountInNsResponse
            {
                AccountId = account.AccountId,
                Namespace = ns
            };

            if (account.TagNamespaces != null && account.TagNamespaces.ContainsKey(ns))
            {
                response.Tags = account.TagNamespaces[ns]
                    .Select(t => new TagWeightOnAccount
                    {
                        Tag = t.Key,
                        Weight = t.Value
                    })
                    .ToList();
            }

            response.Tags = response.Tags ?? new List<TagWeightOnAccount>();
            return Ok(response);
        }

        /// <summary>
        /// PUT
        /// /accounts/a/:a/tags/ns/:ns
        /// +B, ASYNC, IDMP	
        /// Replace all tags within a given namespace with the provided tags and weights
        /// </summary>
        [HttpPut("accounts/a/{accountId}/tags/ns/{ns}")]
        public async Task<IActionResult> PutAndReplaceAccountTagsInNs(string accountId, string ns, PutAndReplaceAccountTagsInNsRequest input)
        {
            if (input == null)
                return ValidationError(ErrorKeys.InputCanNotBeEmpty);

            var validationResult = ApiValidationResult.Aggregate(
                _validation.ValidateAccountId(accountId),
                _validation.ValidateExistingTagNs(ns)
            );

            input.Tags?.ForEach(tw =>
            {
                validationResult.Append(_validation.ValidateAbsoluteTagWeight(tw.Weight));
                validationResult.Append(_validation.ValidateTag(tw.Tag));
            });

            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await _taggingLogic.ReplaceTagNs(accountId, ns, input.Tags?.ToDictionary(t => t.Tag, t => t.Weight));
            return Ok();
        }

        /// <summary>
        /// DELETE
        /// /accounts/a/:a/tags/ns/:ns
        /// -B, ASYNC, IDMP
        /// Remove all tags from within a namespace from the account
        /// </summary>
        [HttpDelete("accounts/a/{accountId}/tags/ns/{ns}")]
        public async Task<IActionResult> DeleteTagNsFromAccount(string accountId, string ns)
        {
            var validationResult = ApiValidationResult.Aggregate(
                _validation.ValidateAccountId(accountId),
                _validation.ValidateExistingTagNs(ns)
            );

            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await _taggingLogic.ReplaceTagNs(accountId, ns, null);
            return Ok();
        }

    }
}
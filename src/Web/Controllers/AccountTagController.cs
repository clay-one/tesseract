using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.Accounts;
using Tesseract.Common.Results;
using Tesseract.Core.Logic;
using Tesseract.Core.Storage;
using Tesseract.Core.Utility;

namespace Appson.Tesseract.Web.Controllers
{
    public class AccountTagController : ApiControllerBase
    {
        // GET  	/accounts/a/:a/tags	-B, SAFE	Get list of tags and their weights for a specific account
        // GET  	/accounts/a/:a/tags/ns/:ns/t/:t	-B, SAFE	Determine the weight associated with a specific tag in an account
        // PUT  	/accounts/a/:a/tags/ns/:ns/t/:t	-B, ASYNC, IDMP Add the specified tag to the account if not already included
        // DELETE	/accounts/a/:a/tags/ns/:ns/t/:t	-B, ASYNC, IDMP Remove a specific tag from the account
        // PUT  	/accounts/a/:a/tags/ns/:ns/t/:t/weight/:w	-B, ASYNC, IDMP Set the weight of the specified tag for the account
        // PATCH	/accounts/a/:a/tags/ns/:ns/t/:t/weight-delta/:d	-B, A/SYNC Change the weight of the specified tag for the account by a delta


        private readonly ITaggingLogic _taggingLogic;
        private readonly IAccountLogic _accountLogic;
        private readonly IAccountStore _accountStore;
        private readonly IInputValidationLogic _validation;

        public AccountTagController(ITaggingLogic taggingLogic,
            IAccountLogic accountLogic,
            IAccountStore accountStore,
            IInputValidationLogic inputValidationLogic)
        {
            _taggingLogic = taggingLogic;
            _accountLogic = accountLogic;
            _accountStore = accountStore;
            _validation = inputValidationLogic;
        }

        /// <summary>
        /// GET
        /// /accounts/a/:a/tags
        /// -B, SAFE
        /// Get list of tags and their weights for a specific account
        /// </summary>
        [HttpGet("accounts/a/{accountId}/tags")]
        public async Task<IActionResult> GetAllTagsOfAccount(string accountId)
        {
            var validationError = _validation.ValidateAccountId(accountId);

            if (validationError != null)
                return ValidationError(validationError);

            var account = await _accountStore.LoadAccount(null, accountId);
            if (account == null)
                return NotFound();

            var response = new GetAllTagsOfAccountResponse
            {
                AccountId = account.AccountId,
                Namespaces = account.TagNamespaces?
                                 .Where(ns => ns.Value != null && ns.Value.Count > 0)
                                 .Select(ns => new TagNamespaceWeightsOnAccount
                                 {
                                     Namespace = ns.Key,
                                     Tags = ns.Value?.Select(t => new TagWeightOnAccount
                                     {
                                         Tag = t.Key,
                                         Weight = t.Value
                                     })
                                         .ToList()
                                 })
                                 .ToList() ?? new List<TagNamespaceWeightsOnAccount>()
            };

            return Ok(response);
        }

        /// <summary>
        /// GET
        /// /accounts/a/:a/tags/ns/:ns/t/:t
        /// -B, SAFE
        /// Determine the weight associated with a specific tag in an account
        /// </summary>
        [HttpGet("accounts/a/{accountId}/tags/ns/{ns}/t/{tag}")]
        public async Task<IActionResult> GetTagWeightOnAccount(string accountId, string ns, string tag)
        {
            var validationResult = ApiValidationResult.Aggregate(
                _validation.ValidateAccountId(accountId),
                _validation.ValidateExistingTagNs(ns),
                _validation.ValidateTag(tag)
            );

            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var accountInfo = await _accountStore.LoadAccount(null, accountId);
            var weight = 0d;

            if (accountInfo?.TagNamespaces != null &&
                accountInfo.TagNamespaces.ContainsKey(ns) &&
                accountInfo.TagNamespaces[ns] != null &&
                accountInfo.TagNamespaces[ns].ContainsKey(tag))
            {
                weight = accountInfo.TagNamespaces[ns][tag];
            }

            return Ok(new GetTagWeightOnAccountResponse
            {
                AccountId = accountId,
                TagNs = ns,
                Tag = tag,
                Weight = weight
            });
        }

        /// <summary>
        /// PUT
        /// /accounts/a/:a/tags/ns/:ns/t/:t
        /// -B, ASYNC, IDMP
        /// Add the specified tag to the account if not already included
        /// </summary>
        [HttpPut("accounts/a/{accountId}/tags/ns/{ns}/t/{tag}")]
        public async Task<IActionResult> PutTagOnAccount(string accountId, string ns, string tag)
        {
            var validationResult = ApiValidationResult.Aggregate(
                _validation.ValidateAccountId(accountId),
                _validation.ValidateExistingTagNs(ns),
                _validation.ValidateTag(tag)
            );

            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await _taggingLogic.AddTag(accountId, ns, tag);
            return Ok();
        }

        /// <summary>
        /// DELETE
        /// /accounts/a/:a/tags/ns/:ns/t/:t
        /// -B, ASYNC, IDMP
        /// Remove a specific tag from the account
        /// </summary>
        [HttpDelete("accounts/a/{accountId}/tags/ns/{ns}/t/{tag}")]
        public async Task<IActionResult> DeleteTagFromAccount(string accountId, string ns, string tag)
        {
            var validationResult = ApiValidationResult.Aggregate(
                _validation.ValidateAccountId(accountId),
                _validation.ValidateExistingTagNs(ns),
                _validation.ValidateTag(tag)
            );

            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await _taggingLogic.SetTagWeight(accountId, ns, tag, 0d);
            return Ok();
        }

        /// <summary>
        /// PUT
        /// /accounts/a/:a/tags/ns/:ns/t/:t/weight/:w
        /// -B, ASYNC, IDMP
        /// Set the weight of the specified tag for the account
        /// </summary>
        [HttpPut("accounts/a/{accountId}/tags/ns/{ns}/t/{tag}/weight/{weight}")]
        public async Task<IActionResult> PutTagWeightOnAccount(string accountId, string ns, string tag,
            double weight)
        {
            var validationResult = ApiValidationResult.Aggregate(
                _validation.ValidateAccountId(accountId),
                _validation.ValidateExistingTagNs(ns),
                _validation.ValidateAbsoluteTagWeight(weight),
                _validation.ValidateTag(tag)
            );

            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await _taggingLogic.SetTagWeight(accountId, ns, tag, weight);
            return Ok();
        }

        /// <summary>
        /// PATCH
        /// /accounts/a/:a/tags/ns/:ns/t/:t/weight-delta/:d
        /// -B, A/SYNC (ASYNC variation)
        /// Change the weight of the specified tag for the account by a delta
        /// </summary>
        [HttpPatch("accounts/a/{accountId}/tags/ns/{ns}/t/{tag}/weight-delta/{weightDelta}")]
        public async Task<IActionResult> PatchTagWeightOnAccount(string accountId, string ns, string tag, double weightDelta)
        {
            var validationResult = ValidateForPatchTagWeightOnAccount(accountId, ns, tag);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await _accountLogic.PatchAccount(accountId, new PatchAccountRequest
            {
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction
                    {
                        TagNs = ns,
                        Tag = tag,
                        WeightDelta = weightDelta
                    }
                }
            });

            return Ok();
        }

        /// <summary>
        /// PATCH
        /// /accounts/a/:a/tags/ns/:ns/t/:t/weight-delta/:d/sync
        /// -B, A/SYNC (SYNC variation)
        /// Change the weight of the specified tag for the account by a delta
        /// </summary>
        [HttpPatch("accounts/a/{accountId}/tags/ns/{ns}/t/{tag}/weight-delta/{weightDelta}/sync")]
        public async Task<IActionResult> PatchTagWeightOnAccountAndWait(string accountId, string ns, string tag, double weightDelta)
        {
            var validationResult = ValidateForPatchTagWeightOnAccount(accountId, ns, tag);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var account = await _accountLogic.PatchAccount(accountId, new PatchAccountRequest
            {
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction
                    {
                        TagNs = ns,
                        Tag = tag,
                        WeightDelta = weightDelta
                    }
                }
            });

            return Ok(new PatchTagWeightOnAccountResponse
            {
                AccountId = accountId,
                TagNs = ns,
                Tag = tag,
                Weight = account.GetTagWeight(ns, tag)
            });
        }

        #region Validation methods

        private ApiValidationResult ValidateForPatchTagWeightOnAccount(string accountId, string ns, string tag)
        {
            return ApiValidationResult.Aggregate(
                _validation.ValidateAccountId(accountId),
                _validation.ValidateExistingTagNs(ns),
                _validation.ValidateTag(tag)
            );
        }

        #endregion
    }
}
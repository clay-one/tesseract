using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Hydrogen.General.Collections;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.Tags;
using Tesseract.Common.Results;
using Tesseract.Core;
using Tesseract.Core.Logic;
using Tesseract.Core.Utility;

namespace Appson.Tesseract.Web.Controllers
{
    public class TagAccountController : ApiControllerBase
    {
        // PUT  	/tags/ns/:ns/t/:t/accounts 	+B, ASYNC, IDMP	Set different weights for a tag on multiple accounts. Remove tag if weight is zero.
        // PATCH	/tags/ns/:ns/t/:t/accounts 	+B, A/SYNC Change weights for a tag on multiple accounts, by different delta. Sync variation returns the changed weight for each account.


        private readonly ITaggingLogic _taggingLogic;
        private readonly IAccountLogic _accountLogic;
        private readonly IInputValidationLogic _validation;

        public TagAccountController(ITaggingLogic taggingLogic,
            IAccountLogic accountLogic,
            IInputValidationLogic inputValidationLogic)
        {
            _taggingLogic = taggingLogic;
            _accountLogic = accountLogic;
            _validation = inputValidationLogic;
        }

        /// <summary>
        /// PUT
        /// /tags/ns/:ns/t/:t/accounts
        /// +B, ASYNC, IDMP
        /// Set different weights for a tag on multiple accounts. Remove tag if weight is zero.
        /// </summary>
        [HttpPut("tags/ns/{ns}/t/{tag}/accounts")]
        public async Task<IActionResult> PutAccountWeightsOnTag(string ns, string tag, PutAccountWeightsOnTagRequest input)
        {
            if (input == null)
                return ValidationError(ErrorKeys.InputCanNotBeEmpty);

            var validationResult = ApiValidationResult.Aggregate(
                input.Accounts.EmptyIfNull()
                    .Select(a => _validation.ValidateAccountId(a.AccountId))
                    .Union(input.Accounts.EmptyIfNull().Select(a => _validation.ValidateAbsoluteTagWeight(a.Weight)))
                    .ToArray());
            validationResult.Append(_validation.ValidateExistingTagNs(ns));
            validationResult.Append(_validation.ValidateTag(tag));
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await Task.WhenAll(input.Accounts.EmptyIfNull().Select(a => _taggingLogic.SetTagWeight(a.AccountId, ns, tag, a.Weight)));
            return Ok();
        }

        /// <summary>
        /// PATCH
        /// /tags/ns/:ns/t/:t/accounts 
        /// +B, A/SYNC (ASYNC variant)
        /// Change weights for a tag on multiple accounts, by different delta. Sync variation returns the changed weight for each account.
        /// </summary>
        [HttpPatch("tags/ns/{ns}/t/{tag}/accounts")]
        public async Task<IActionResult> PatchAccountWeightsOnTag(string ns, string tag, PatchAccountWeightsOnTagRequest input)
        {
            var validationResult = ValidateForPatchAccountWeightsOnTag(ns, tag, input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await Task.WhenAll(input.AccountPatches.EmptyIfNull()
                .Select(a => _accountLogic.PatchAccount(a.AccountId, new PatchAccountRequest
                {
                    TagPatches = new List<AccountTagPatchInstruction>
                    {
                        new AccountTagPatchInstruction {TagNs = ns, Tag = tag, WeightDelta = a.WeightDelta}
                    }
                })));

            return Ok();
        }

        /// <summary>
        /// PATCH
        /// /tags/ns/:ns/t/:t/accounts/sync
        /// +B, A/SYNC (SYNC variant)
        /// Change weights for a tag on multiple accounts, by different delta. Sync variation returns the changed weight for each account.
        /// </summary>
        [HttpPatch("tags/ns/{ns}/t/{tag}/accounts/sync")]
        public async Task<IActionResult> PatchAccountWeightsOnTagAndWait(string ns, string tag, PatchAccountWeightsOnTagRequest input)
        {
            var validationResult = ValidateForPatchAccountWeightsOnTag(ns, tag, input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var accounts = await Task.WhenAll(input.AccountPatches.EmptyIfNull()
                .Select(a => _accountLogic.PatchAccount(a.AccountId, new PatchAccountRequest
                {
                    TagPatches = new List<AccountTagPatchInstruction>
                    {
                        new AccountTagPatchInstruction {TagNs = ns, Tag = tag, WeightDelta = a.WeightDelta}
                    }
                })));



            return Ok(new PatchAccountWeightsOnTagResponse
            {
                TagNs = ns,
                Tag = tag,
                Accounts = accounts
                    .Select(a => new AccountWeightOnTag { AccountId = a.AccountId, Weight = a.GetTagWeight(ns, tag) })
                    .ToList()
            });
        }

        #region Validation methods

        private ApiValidationResult ValidateForPatchAccountWeightsOnTag(string ns, string tag,
            PatchAccountWeightsOnTagRequest input)
        {
            if (input == null)
                return ApiValidationResult.Failure(ErrorKeys.InputCanNotBeEmpty);

            var result = ApiValidationResult.Aggregate(
                input.AccountPatches.EmptyIfNull().Select(a => _validation.ValidateAccountId(a.AccountId)).ToArray());

            result.Append(_validation.ValidateExistingTagNs(ns));
            result.Append(_validation.ValidateTag(tag));
            return result;
        }

        #endregion
    }
}

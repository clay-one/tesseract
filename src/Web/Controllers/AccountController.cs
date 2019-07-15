using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Hydrogen.General.Collections;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.Accounts;
using Tesseract.Common.Results;
using Tesseract.Core;
using Tesseract.Core.Logic;
using Tesseract.Core.Storage;
using Tesseract.Core.Utility;

namespace Appson.Tesseract.Web.Controllers
{
    public class AccountController : ApiControllerBase
    {
        // GET  	/accounts/a/:a	-B, SAFE	Get all information, tags and values for a single account
        // PATCH	/accounts/a/:a	+B, A/SYNC Perform multiple changes to a single account at once(tag, remove tag, set weight, change weight by delta, set value, change value by delta, …). Sync variation returns all account information.
        // PUT  	/accounts/a/:a/changes	+B, ASYNC, IDMP	Perform multiple "set" changes to a single account at once (no delta changes, hence idempotent API)
        // DELETE	/accounts/a/:a/index	-B, ASYNC, IDMP	Marks an account for re-indexing

        private readonly IAccountStore AccountStore;
        private readonly ITaggingLogic _taggingLogic;
        private readonly IAccountLogic _accountLogic;
        private readonly IInputValidationLogic _validation;

        public AccountController(IAccountStore accountStore,
            ITaggingLogic taggingLogic,
            IAccountLogic accountLogic,
            IInputValidationLogic inputValidationLogic)
        {
            AccountStore = accountStore;
            _taggingLogic = taggingLogic;
            _accountLogic = accountLogic;
            _validation = inputValidationLogic;
        }

        /// <summary>
        /// GET
        /// /accounts/a/:a
        /// -B, SAFE
        /// Get all information, tags and values for a single account
        /// </summary>
        [HttpGet("accounts/a/{accountId}")]
        public async Task<IActionResult> GetAccountInfo(string accountId)
        {
            var validationError = _validation.ValidateAccountId(accountId);

            if (validationError != null)
                return ValidationError(validationError);

            var account = await AccountStore.LoadAccount(null, accountId);
            if (account == null)
                return NotFound();

            var response = new GetAccountInfoResponse
            {
                AccountId = account.AccountId,
                Fields = account.Fields?
                             .Select(f => new FieldValueOnAccount
                             {
                                 FieldName = f.Key,
                                 FieldValue = f.Value
                             })
                             .ToList() ?? new List<FieldValueOnAccount>(),
                TagNamespaces = account.TagNamespaces?
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
        /// PATCH
        /// /accounts/a/:a
        /// +B, A/SYNC (ASYNC variation)
        /// Perform multiple changes to a single account at once(tag, remove tag, set weight, change weight by delta, set value, change value by delta, …). Sync variation returns all account information.
        /// </summary>
        [HttpPatch("accounts/a/{accountId}")]
        public async Task<IActionResult> PatchAccount(string accountId, PatchAccountRequest input)
        {
            var validationResult = ValidateForPatchAccount(accountId, input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await _accountLogic.PatchAccount(accountId, input);
            return Ok();
        }

        /// <summary>
        /// PATCH
        /// /accounts/a/:a/sync
        /// +B, A/SYNC (SYNC variation)
        /// Perform multiple changes to a single account at once(tag, remove tag, set weight, change weight by delta, set value, change value by delta, …). Sync variation returns all account information.
        /// </summary>
        [HttpPatch("accounts/a/{accountId}/sync")]
        public async Task<IActionResult> PatchAccountAndWait(string accountId, PatchAccountRequest input)
        {
            var validationResult = ValidateForPatchAccount(accountId, input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var account = await _accountLogic.PatchAccount(accountId, input);

            var response = new PatchAccountResponse
            {
                AccountId = account.AccountId,
                Fields = account.Fields?
                             .Select(f => new FieldValueOnAccount
                             {
                                 FieldName = f.Key,
                                 FieldValue = f.Value
                             })
                             .ToList() ?? new List<FieldValueOnAccount>(),
                TagNamespaces = account.TagNamespaces?
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
        /// PUT
        /// /accounts/a/:a/changes
        /// +B, ASYNC, IDMP
        /// Perform multiple "set" changes to a single account at once (no delta changes, hence idempotent API)
        /// </summary>
        [HttpPut("accounts/a/{accountId}/changes")]
        public async Task<IActionResult> PutAccountChange(string accountId, PutAccountChangeRequest input)
        {
            var validationResult = ValidateForPutAccountChange(accountId, input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await _accountLogic.PatchAccount(accountId, new PatchAccountRequest
            {
                TagChanges = input.TagChanges,
                FieldChanges = input.FieldChanges
            });

            return Ok();
        }

        /// <summary>
        /// DELETE
        /// /accounts/a/:a/index
        /// -B, ASYNC, IDMP
        /// Marks an account for re-indexing
        /// </summary>
        [HttpDelete("accounts/a/{accountId}/index")]
        public async Task<IActionResult> MarkAccountForReindexing(string accountId)
        {
            var validationResult = ValidateForMarkAccountForReindexing(accountId);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await _accountLogic.QueueForReindex(accountId);

            return Ok();
        }

        #region Validation methods

        private ApiValidationResult ValidateForPatchAccount(string accountId, PatchAccountRequest input)
        {
            if (input == null)
                return ApiValidationResult.Failure(ErrorKeys.InputCanNotBeEmpty);

            var result = ApiValidationResult.Aggregate(_validation.ValidateAccountId(accountId));

            input.TagChanges?.ForEach(tc =>
            {
                result.Append(_validation.ValidateExistingTagNs(tc.TagNs));
                result.Append(_validation.ValidateTag(tc.Tag));
                result.Append(_validation.ValidateAbsoluteTagWeight(tc.Weight));
            });

            input.TagPatches?.ForEach(tp =>
            {
                result.Append(_validation.ValidateExistingTagNs(tp.TagNs));
                result.Append(_validation.ValidateTag(tp.Tag));
            });

            input.FieldChanges?.ForEach(fc =>
            {
                result.Append(_validation.ValidateExistingFieldName(fc.FieldName));
                result.Append(_validation.ValidateFieldValue(fc.FieldValue));
            });

            input.FieldPatches?.ForEach(fp => { result.Append(_validation.ValidateExistingFieldName(fp.FieldName)); });

            if (!result.Success)
                return result;

            if (_validation.HasDuplicates(
                input.TagChanges.EmptyIfNull()
                    .Select(tc => TagUtils.FqTag(tc.TagNs, tc.Tag))
                    .Concat(input.TagPatches.EmptyIfNull().Select(tc => TagUtils.FqTag(tc.TagNs, tc.Tag)))))
            {
                return ApiValidationResult.Failure(ErrorKeys.DuplicateTagInstruction);
            }

            if (_validation.HasDuplicates(
                input.FieldChanges.EmptyIfNull()
                    .Select(fc => fc.FieldName)
                    .Concat(input.FieldPatches.EmptyIfNull().Select(fp => fp.FieldName))))
            {
                return ApiValidationResult.Failure(ErrorKeys.DuplicateFieldInstruction);
            }

            return ApiValidationResult.Ok();
        }

        private ApiValidationResult ValidateForPutAccountChange(string accountId, PutAccountChangeRequest input)
        {
            if (input == null)
                return ApiValidationResult.Failure(ErrorKeys.InputCanNotBeEmpty);

            var result = ApiValidationResult.Aggregate(_validation.ValidateAccountId(accountId));

            input.TagChanges?.ForEach(tc =>
            {
                result.Append(_validation.ValidateExistingTagNs(tc.TagNs));
                result.Append(_validation.ValidateTag(tc.Tag));
                result.Append(_validation.ValidateAbsoluteTagWeight(tc.Weight));
            });

            input.FieldChanges?.ForEach(fc =>
            {
                result.Append(_validation.ValidateExistingFieldName(fc.FieldName));
                result.Append(_validation.ValidateFieldValue(fc.FieldValue));
            });

            if (!result.Success)
                return result;

            if (_validation.HasDuplicates(input.TagChanges?.Select(tc => TagUtils.FqTag(tc.TagNs, tc.Tag))))
                return ApiValidationResult.Failure(ErrorKeys.DuplicateTagInstruction);

            if (_validation.HasDuplicates(input.FieldChanges?.Select(fc => fc.FieldName)))
                return ApiValidationResult.Failure(ErrorKeys.DuplicateFieldInstruction);

            return ApiValidationResult.Ok();
        }

        private ApiValidationResult ValidateForMarkAccountForReindexing(string accountId)
        {
            return new ApiValidationResult(_validation.ValidateAccountId(accountId));
        }

        #endregion
    }
}
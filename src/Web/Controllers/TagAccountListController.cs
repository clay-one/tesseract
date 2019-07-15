using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Hydrogen.General.Collections;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.General;
using Tesseract.ApiModel.Tags;
using Tesseract.Common.Results;
using Tesseract.Core;
using Tesseract.Core.Logic;

namespace Appson.Tesseract.Web.Controllers
{
    public class TagAccountListController : ApiControllerBase
    {
        // GET  	/tags/ns/:ns/t/:t/accounts/list	-B, SAFE	Get list of accounts tagged with a specific tag (paginated, limited number)
        // PUT  	/tags/ns/:ns/t/:t/accounts/list	+B, ASYNC, IDMP Add a specific tag to multiple accounts if not already present.If adding, sets the weight to 1
        // DELETE	/tags/ns/:ns/t/:t/accounts/list	+B, ASYNC, IDMP Remove a specific tag from multiple accounts


        public ITaggingLogic _taggingLogic;
        public IIndexLogic _indexLogic;
        public IInputValidationLogic _validation;

        public TagAccountListController(ITaggingLogic taggingLogic,
            IIndexLogic indexLogic,
            IInputValidationLogic inputValidationLogic)
        {
            _taggingLogic = taggingLogic;
            _indexLogic = indexLogic;
            _validation = inputValidationLogic;
        }

        /// <summary>
        /// GET
        /// /tags/ns/:ns/t/:t/accounts/list
        /// -B, SAFE
        /// Get list of accounts tagged with a specific tag (paginated, limited number)
        /// </summary>
        [HttpGet("tags/ns/{ns}/t/{tag}/accounts/list")]
        public async Task<IActionResult> GetTagAccountList(string ns, string tag,
            int count = 50, string continueFrom = null)
        {
            var validationResult = ValidateForGetTagAccountList(ns, tag, count);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var query = new AccountQuery
            {
                TaggedWithAll = new List<FqTag> { new FqTag { Ns = ns, Tag = tag } }
            };

            var result = await _indexLogic.List(query, count, continueFrom);

            return Ok(new GetTagAccountListResponse
            {
                RequestedTagNs = ns,
                RequestedTag = tag,
                RequestedCount = count,
                RequestedContinueFrom = continueFrom,
                Accounts = result.AccountIds.Select(aid => new GetTagAccountListResponseItem { AccountId = aid }).ToList(),
                TotalNumberOfResults = result.TotalNumberOfResults,
                ContinueWith = result.ContinueWith
            });
        }

        /// <summary>
        /// PUT
        /// /tags/ns/:ns/t/:t/accounts/list
        /// +B, ASYNC, IDMP
        /// Add a specific tag to multiple accounts if not already present.If adding, sets the weight to 1
        /// </summary>
        [HttpPut("tags/ns/{ns}/t/{tag}/accounts/list")]
        public async Task<IActionResult> PutTagInAccounts(string ns, string tag, PutTagInAccountsRequest input)
        {
            var validationResult = ValidateForPutTagInAccounts(ns, tag, input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await Task.WhenAll(input.AccountIds.EmptyIfNull().Select(aid => _taggingLogic.AddTag(aid, ns, tag)));
            return Ok();
        }

        /// <summary>
        /// DELETE
        /// /tags/ns/:ns/t/:t/accounts/list
        /// +B, ASYNC, IDMP
        /// Remove a specific tag from multiple accounts
        /// </summary>
        [HttpDelete("tags/ns/{ns}/t/{tag}/accounts/list")]
        public async Task<IActionResult> DeleteTagFromAccounts(string ns, string tag,
            DeleteTagFromAccountsRequest input)
        {
            if (input == null)
                return ValidationError(ErrorKeys.InputCanNotBeEmpty);

            var validationResult = ApiValidationResult.Aggregate(
                input.AccountIds.SafeSelect(_validation.ValidateAccountId).SafeToArray());
            validationResult.Append(_validation.ValidateExistingTagNs(ns));
            validationResult.Append(_validation.ValidateTag(tag));
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            await Task.WhenAll(input.AccountIds.EmptyIfNull()
                .SafeSelect(aid => _taggingLogic.SetTagWeight(aid, ns, tag, 0d)));
            return Ok();
        }

        #region Validation methods

        private ApiValidationResult ValidateForPutTagInAccounts(string ns, string tag, PutTagInAccountsRequest input)
        {
            if (input == null)
                return ApiValidationResult.Failure(ErrorKeys.InputCanNotBeEmpty);

            var validationResult = ApiValidationResult.Aggregate(
                input.AccountIds.SafeSelect(_validation.ValidateAccountId).SafeToArray());

            validationResult.Append(_validation.ValidateExistingTagNs(ns));
            validationResult.Append(_validation.ValidateTag(tag));

            return validationResult;
        }

        private ApiValidationResult ValidateForGetTagAccountList(string ns, string tag, int count)
        {
            var result = ApiValidationResult.Aggregate(
                _validation.ValidateExistingTagNs(ns),
                _validation.ValidateTag(tag),
                _validation.ValidateRange(nameof(count), count, 1, 1000)
            );

            return result;
        }

        #endregion
    }
}
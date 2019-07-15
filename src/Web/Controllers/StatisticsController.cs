using System.Collections.Generic;
using System.Threading.Tasks;
using Appson.Tesseract.Web.Base;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.General;
using Tesseract.ApiModel.Statistics;
using Tesseract.Common.Results;
using Tesseract.Core;
using Tesseract.Core.Logic;

namespace Appson.Tesseract.Web.Controllers
{
    public class StatisticsController : ApiControllerBase
    {
        // GET	/tags/stats	-B, SAFE	Get an overall statistics about all tags
        // GET	/tags/ns/:ns/count	-B, SAFE	Return number of tags present in the specified namespace
        // GET	/tags/ns/:ns/histogram	-B, SAFE	Get list of tags in a namespace with their cardinality
        // GET	/fields/f/:f/histogram	-B, SAFE	Calculate and return distribution of field values
        // GET	/tags/ns/:ns/accounts/count	-B, SAFE	Return number of accounts tagged in the specified namespace
        // GET	/tags/ns/:ns/t/:t/accounts/count	-B, SAFE Return number of accounts that include the specified tag
        // GET	/accounts/count	-B, SAFE	Return number of all known accounts


        private readonly IIndexLogic _indexLogic;
        private readonly IInputValidationLogic _validation;

        public StatisticsController(IIndexLogic indexLogic, IInputValidationLogic inputValidationLogic)
        {
            _indexLogic = indexLogic;
            _validation = inputValidationLogic;
        }

        /// <summary>
        /// GET
        /// /tags/stats
        /// -B, SAFE
        /// Get an overall statistics about all tags
        /// </summary>
        [HttpGet("tags/stats")]
        public IActionResult GetTagStatistics()
        {
            return NotImplemented();
        }

        /// <summary>
        /// GET
        /// /tags/ns/:ns/count
        /// -B, SAFE
        /// Return number of tags present in the specified namespace
        /// </summary>
        [HttpGet("tags/ns/{ns}/count")]
        public IActionResult CountTagsInNamespace(string ns)
        {
            return NotImplemented();
        }

        /// <summary>
        /// GET
        /// /tags/ns/:ns/histogram
        /// -B, SAFE
        /// Get list of tags in a namespace with their cardinality
        /// </summary>
        [HttpGet("tags/ns/{ns}/histogram")]
        public IActionResult GetTagNamespaceHistogram(string ns)
        {
            return NotImplemented();
        }

        /// <summary>
        /// GET
        /// /fields/f/:f/histogram
        /// -B, SAFE
        /// Calculate and return distribution of field values
        /// </summary>
        [HttpGet("fields/f/{fieldId}/histogram")]
        public IActionResult GetFieldHistogram(string fieldId)
        {
            return NotImplemented();
        }

        /// <summary>
        /// GET
        /// /tags/ns/:ns/accounts/count
        /// -B, SAFE
        /// Return number of accounts tagged in the specified namespace
        /// </summary>
        [HttpGet("tags/ns/{ns}/accounts/count")]
        public async Task<IActionResult> GetTagNsAccountCount(string ns)
        {
            var validationResult = ValidateForGetTagNsAccountCount(ns);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var count = await _indexLogic.Count(new AccountQuery
            {
                TaggedInNs = ns
            });

            return Ok(new GetTagNsAccountCountResponse
            {
                TagNs = ns,
                TotalCount = count
            });
        }

        /// <summary>
        /// GET
        /// /tags/ns/:ns/t/:t/accounts/count
        /// -B, SAFE
        /// Return number of accounts that include the specified tag
        /// </summary>
        [HttpGet("tags/ns/{ns}/t/{tag}/accounts/count")]
        public async Task<IActionResult> GetTagAccountCount(string ns, string tag)
        {
            var validationResult = ValidateForGetTagAccountCount(ns, tag);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var count = await _indexLogic.Count(new AccountQuery
            {
                TaggedWithAll = new List<FqTag> { new FqTag { Ns = ns, Tag = tag } }
            });

            return Ok(new GetTagAccountCountResponse
            {
                TagNs = ns,
                Tag = tag,
                TotalCount = count
            });
        }

        /// <summary>
        /// GET
        /// /accounts/count
        /// -B, SAFE
        /// Return number of all known accounts
        /// </summary>
        [HttpGet("accounts/count")]
        public async Task<IActionResult> GetCountOfAllAccounts()
        {
            var count = await _indexLogic.Count(new AccountQuery());
            return Ok(new GetCountOfAllAccountsResponse { Count = count });
        }

        /// <summary>
        /// POST
        /// /accounts/query/count
        /// +B, SAFE
        /// Return the number of accounts matching a given query
        /// </summary>
        [HttpPost("accounts/query/count")]
        public async Task<IActionResult> GetAccountQueryResultCount(GetAccountQueryResultCountRequest input)
        {
            var validationResult = ValidateForGetAccountQueryResultCount(input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var count = await _indexLogic.Count(input.Query);
            return Ok(new GetAccountQueryResultCountResponse { MatchCount = count });
        }


        #region Validation methods

        private ApiValidationResult ValidateForGetTagAccountCount(string ns, string tag)
        {
            return ApiValidationResult.Aggregate(
                _validation.ValidateTag(tag),
                _validation.ValidateExistingTagNs(ns)
            );
        }

        private ApiValidationResult ValidateForGetTagNsAccountCount(string ns)
        {
            return ApiValidationResult.Aggregate(
                _validation.ValidateExistingTagNs(ns)
            );
        }

        private ApiValidationResult ValidateForGetAccountQueryResultCount(GetAccountQueryResultCountRequest input)
        {
            if (input == null)
                return ApiValidationResult.Failure(nameof(input), ErrorKeys.ArgumentCanNotBeEmpty);

            return _validation.ValidateAccountQuery(input.Query);
        }

        #endregion
    }
}

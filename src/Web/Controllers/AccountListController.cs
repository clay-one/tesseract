using System.Threading.Tasks;
using System.Linq;
using Appson.Tesseract.Web.Base;
using Tesseract.Core.Logic;
using Microsoft.AspNetCore.Mvc;
using Tesseract.ApiModel.Accounts;
using Tesseract.Common.Results;

namespace Appson.Tesseract.Web.Controllers
{
    public class AccountListController : ApiControllerBase
    {
        private const int DefaultResultCount = 50;

        // PUT	/accounts/query/list	+B, SAFE Get list of accounts from a query(paginated, limited results)

        private readonly IInputValidationLogic _validation;
        private readonly IIndexLogic _indexLogic;

        public AccountListController(IInputValidationLogic inputValidationLogic, IIndexLogic indexLogic)
        {
            _validation = inputValidationLogic;
            _indexLogic = indexLogic;
        }

        /// <summary>
        /// PUT
        /// /accounts/query/list
        /// +B, SAFE
        /// Get list of accounts from a query(paginated, limited results)
        /// </summary>
        [HttpPost("accounts/query/list")]
        public async Task<IActionResult> GetAccountQueryResults(GetAccountQueryResultsRequest input)
        {
            var validationResult = ValidateForGetAccountQueryResults(input);
            if (!validationResult.Success)
                return ValidationResult(validationResult);

            var result = await _indexLogic.List(input.Query, input.Count ?? DefaultResultCount, input.ContinueFrom);

            return Ok(new GetAccountQueryResultsResponse
            {
                RequestedCount = input.Count ?? DefaultResultCount,
                RequestedContinueFrom = input.ContinueFrom,
                Accounts = result.AccountIds
                    .Select(aid => new GetAccountQueryResultsResponseItem { AccountId = aid }).ToList(),
                TotalNumberOfResults = result.TotalNumberOfResults,
                ContinueWith = result.ContinueWith
            });
        }

        #region Validation methods

        private ApiValidationResult ValidateForGetAccountQueryResults(GetAccountQueryResultsRequest input)
        {
            var result = _validation.ValidateAccountQuery(input.Query);

            if (input.Count.HasValue)
                result.Append(_validation.ValidateRange(nameof(input.Count), input.Count.Value, 1, 1000));
                    
            return result;
        }

        #endregion
    }
}

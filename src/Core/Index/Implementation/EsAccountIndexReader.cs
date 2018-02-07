using System;
using System.Threading.Tasks;
using Tesseract.Core.Connection;
using Tesseract.Core.Index.Model;

namespace Tesseract.Core.Index.Implementation
{
    [Component]
    public class EsAccountIndexReader : IAccountIndexReader
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [ComponentPlug]
        public IEsManager EsManager { get; set; }

        [ComponentPlug]
        public EsQueryBuilder QueryBuilder { get; set; }

        public async Task<long> Count(string tenantId, AccountQuery query)
        {
            var countResult = await EsManager.Client.CountAsync<AccountIndexModel>(c => c
                .Index(EsManager.GetTenantIndexName(tenantId))
                .Type(typeof(AccountIndexModel))
                .Query(d => QueryBuilder.BuildEsQuery(null, query) ?? new MatchAllQuery())
            );

            LogAndThrowIfNotValid(countResult);
            
            return countResult.Count;
        }

        public async Task<AccountQueryResultPage> List(string tenantId, AccountQuery query, 
            int count, string continueFrom)
        {
            var decodedContinueFrom = DecodeContinueFrom(continueFrom);

            var searchResult = await EsManager.Client.SearchAsync<AccountIndexModel>(s => s
                .Index(EsManager.GetTenantIndexName(tenantId))
                .Type(typeof(AccountIndexModel))
                .Query(d => QueryBuilder.BuildEsQuery(null, query) ?? new MatchAllQuery())
                .Size(count + 1)
                .Source(false)
                .Sort(o => o.Ascending(IndexNaming.CreationTimeFieldName).Descending(IndexNaming.AccountIdFieldName))
                .SearchAfter(decodedContinueFrom)
            );

            LogAndThrowIfNotValid(searchResult);
            
            var result = new AccountQueryResultPage
            {
                AccountIds = searchResult.Hits.Select(h => h.Id).Take(count).ToList(),
                TotalNumberOfResults = searchResult.Total
            };

            if (searchResult.Hits.Count > count)
            {
                result.ContinueWith = EncodeContinueWith(searchResult.Hits.Reverse().Skip(1).First().Sorts.ToArray());
            }

            return result;
        }

        public async Task<AccountQueryScrollPage> StartScroll(string tenantId, AccountQuery query, int count, 
            int timeoutSeconds, int sliceCount, int sliceId)
        {
            var scrollResult = await EsManager.Client.SearchAsync<AccountIndexModel>(s => s
                .Index(EsManager.GetTenantIndexName(tenantId))
                .Type(typeof(AccountIndexModel))
                .Query(d => QueryBuilder.BuildEsQuery(null, query) ?? new MatchAllQuery())
                .Size(count)
                .Source(false)
                .Scroll(TimeSpan.FromSeconds(timeoutSeconds))
                .Slice(sl => sl.Id(sliceId).Max(sliceCount))
                .Sort(sort => sort.Ascending(SortSpecialField.DocumentIndexOrder))
            );

            LogAndThrowIfNotValid(scrollResult);

            var result = new AccountQueryScrollPage
            {
                AccountIds = scrollResult.Hits.Select(h => h.Id).ToList(),
                ScrollId = scrollResult.ScrollId
            };

            return result;
        }

        public async Task<AccountQueryScrollPage> ContinueScroll(string scrollId, int timeoutSeconds)
        {
            var scrollResult = await EsManager.Client.ScrollAsync<AccountIndexModel>(
                TimeSpan.FromSeconds(timeoutSeconds), scrollId);

            LogAndThrowIfNotValid(scrollResult);

            var result = new AccountQueryScrollPage
            {
                AccountIds = scrollResult.Hits.Select(h => h.Id).ToList(),
                ScrollId = scrollResult.ScrollId
            };

            return result;
        }

        public async Task<ApiValidationResult> TerminateScroll(string scrollId)
        {
            var response = await EsManager.Client.ClearScrollAsync(s => s.ScrollId(scrollId));
            
            LogAndThrowIfNotValid(response);
            return ApiValidationResult.Ok();
        }

        private string EncodeContinueWith(object[] lastSorts)
        {
            var encodedCreationTime = Base32.ToString(BitConverter.GetBytes((long)lastSorts[0]));
            return $"{encodedCreationTime},{(string)lastSorts[1]}";
        }

        private object[] DecodeContinueFrom(string continueFrom)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(continueFrom))
                    return null;

                var firstDashIndex = continueFrom.IndexOf(",", StringComparison.Ordinal);
                if (firstDashIndex < 0)
                    return null;

                var accountId = continueFrom.Substring(firstDashIndex + 1);
                var encodedCreationTime = continueFrom.Substring(0, firstDashIndex);
                var creationTime = BitConverter.ToInt64(Base32.ToBytes(encodedCreationTime), 0);

                return new object[] {creationTime, accountId};
            }
            catch (Exception)
            {
                // If any exception occurs during the decode, it means the format was not correct and
                // the value is modified. In such a case, ignore the "continueFrom" by returning null.
                return null;
            }
        }

        private void LogAndThrowIfNotValid(IResponse response)
        {
            if (response.IsValid)
                return;
            
            Log.Error(response.DebugInformation);
            throw new InvalidOperationException("Invalid response returned from Elasticsearch");
        }        
    }
}
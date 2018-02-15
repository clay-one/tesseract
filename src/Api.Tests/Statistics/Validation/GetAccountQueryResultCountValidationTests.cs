using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.General;
using Tesseract.ApiModel.Statistics;

namespace Appson.Tesseract.Tests.Api.Statistics.Validation
{
    [TestClass]
    public class GetAccountQueryResultCountValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullRequestThrows()
        {
            await Client.Statistics.GetAccountQueryResultCount(null);
        }

        [TestMethod]
        public async Task NullQueryShouldBeRejected()
        {
            var request = new GetAccountQueryResultCountRequest
            {
                Query = null
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task EmptyFieldNameInFieldQueryFails()
        {
            var request = new GetAccountQueryResultCountRequest
            {
                Query = new AccountQuery
                {
                    FieldWithin = new AccountFieldQuery { LowerBound = 1d, UpperBound = 2d }
                }
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
                
        [TestMethod]
        public async Task UndefinedFieldNameInFieldQueryFails()
        {
            var request = new GetAccountQueryResultCountRequest
            {
                Query = new AccountQuery
                {
                    FieldWithin = new AccountFieldQuery { FieldName = BuildUniqueString(), LowerBound = 1d }
                }
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task UndefinedNsForTaggedInNsFails()
        {
            var request = new GetAccountQueryResultCountRequest
            {
                Query = new AccountQuery
                {
                    TaggedInNs = BuildUniqueString()
                }
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task UndefinedNsInTaggedWithAllFails()
        {
            var request = new GetAccountQueryResultCountRequest
            {
                Query = new AccountQuery
                {
                    TaggedWithAll = new List<FqTag> { new FqTag { Ns = BuildUniqueString(), Tag = TestTag }}
                }
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task UndefinedNsInTaggedWithAnyFails()
        {
            var request = new GetAccountQueryResultCountRequest
            {
                Query = new AccountQuery
                {
                    TaggedWithAny = new List<FqTag> { new FqTag { Ns = BuildUniqueString(), Tag = TestTag }}
                }
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task NullTagInTaggedWithAllFails()
        {
            var request = new GetAccountQueryResultCountRequest
            {
                Query = new AccountQuery
                {
                    TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = null }}
                }
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task NullTagInTaggedWithAnyFails()
        {
            var request = new GetAccountQueryResultCountRequest
            {
                Query = new AccountQuery
                {
                    TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = null }}
                }
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task EmptyTagInTaggedWithAllFails()
        {
            var request = new GetAccountQueryResultCountRequest
            {
                Query = new AccountQuery
                {
                    TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "" }}
                }
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task EmptyTagInTaggedWithAnyFails()
        {
            var request = new GetAccountQueryResultCountRequest
            {
                Query = new AccountQuery
                {
                    TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "" }}
                }
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task TooDeepAndsResultsInError()
        {
            var query = new AccountQuery
            {
                TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = TestTag }}
            };

            for (var i = 0; i < 9; i++)
            {
                query = new AccountQuery
                {
                    And = new List<AccountQuery> {query}
                };
            }
            
            var request = new GetAccountQueryResultCountRequest
            {
                Query = query
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task TooDeepOrsResultsInError()
        {
            var query = new AccountQuery
            {
                TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = TestTag }}
            };

            for (var i = 0; i < 9; i++)
            {
                query = new AccountQuery
                {
                    Or = new List<AccountQuery> {query}
                };
            }
            
            var request = new GetAccountQueryResultCountRequest
            {
                Query = query
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task TooDeepNotsResultsInError()
        {
            var query = new AccountQuery
            {
                TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = TestTag }}
            };

            for (var i = 0; i < 9; i++)
            {
                query = new AccountQuery {Not = query};
            }
            
            var request = new GetAccountQueryResultCountRequest
            {
                Query = query
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task TooManyConditionsResultsInError()
        {
            // A query with more than 512 conditions
            
            var query = new AccountQuery
            {
                And = Enumerable.Range(1, 16).Select(x => new AccountQuery
                {
                    TaggedWithAll = Enumerable.Range(1, 8)
                        .Select(y => new FqTag {Ns = TestNs, Tag = BuildUniqueString()})
                        .ToList(),
                    TaggedWithAny = Enumerable.Range(1, 8)
                        .Select(y => new FqTag {Ns = TestNs, Tag = BuildUniqueString()})
                        .ToList(),
                }).ToList(),

                Or = Enumerable.Range(1, 16).Select(x => new AccountQuery
                {
                    TaggedWithAll = Enumerable.Range(1, 8)
                        .Select(y => new FqTag {Ns = TestNs, Tag = BuildUniqueString()})
                        .ToList(),
                    TaggedWithAny = Enumerable.Range(1, 8)
                        .Select(y => new FqTag {Ns = TestNs, Tag = BuildUniqueString()})
                        .ToList(),
                }).ToList(),

                Not = new AccountQuery {TaggedInNs = TestNs}
            };
            
            var request = new GetAccountQueryResultCountRequest
            {
                Query = query
            };
            
            CheckFailure(await Client.Statistics.GetAccountQueryResultCount(request), HttpStatusCode.BadRequest);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.General;
using Tesseract.ApiModel.Tags;
using Tesseract.Common.Extensions;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class GetAccountQueryResultsTests : TestClassBase
    {
        private const int CountA = 1;
        private const int CountB = 2;
        private const int CountC = 4;
        private const int CountD = 8;
        private const int CountE = 16;
        private const int CountF = 32;
        private const int CountAll = CountA + CountB + CountC + CountD + CountE + CountF;

        private List<string> _listA;
        private List<string> _listB;
        private List<string> _listC;
        private List<string> _listD;
        private List<string> _listE;
        private List<string> _listF;
        private List<string> _listAb;
        private List<string> _listAc;
        private List<string> _listAd;
        private List<string> _listAe;
        private List<string> _listAf;
        private List<string> _listAbc;
        private List<string> _listAbd;
        private List<string> _listAbe;
        private List<string> _listAbf;
        private List<string> _listAbcd;
        private List<string> _listAbce;
        private List<string> _listAbcf;
        private List<string> _listAbcde;
        private List<string> _listAbcdf;
        private List<string> _listAbcdef;

        [TestMethod]
        public async Task TestEmptyQuery()
        {
            await BuildUserBase();

            await Test(new AccountQuery(),
                _listAbcdef,
                null,
                CountAll);
        }
        
        [TestMethod]
        public async Task TestTaggedWithAll()
        {
            await BuildUserBase();

            await Test(new AccountQuery
                {
                    TaggedWithAll = new List<FqTag>
                    {
                        new FqTag {Ns = TestNs, Tag = "abcdef"}
                    }
                },
                _listAbcdef,
                null,
                CountAll);

            await Test(new AccountQuery
                {
                    TaggedWithAll = new List<FqTag>
                    {
                        new FqTag {Ns = TestNs, Tag = "abcde"},
                        new FqTag {Ns = TestNs, Tag = "f"}
                    }
                },
                null,
                null,
                0);

            await Test(new AccountQuery
                {
                    TaggedWithAll = new List<FqTag>
                    {
                        new FqTag { Ns = TestNs, Tag = "ab"},
                        new FqTag { Ns = TestNs, Tag = "abc"}
                    }
                },
                _listAb,
                _listC,
                CountA + CountB);
            
            await Test(new AccountQuery
                {
                    TaggedWithAll = new List<FqTag>
                    {
                        new FqTag { Ns = TestNs, Tag = "ab"},
                        new FqTag { Ns = TestNs, Tag = "ac"},
                        new FqTag { Ns = TestNs, Tag = "ad"},
                        new FqTag { Ns = TestNs, Tag = "ae"},
                        new FqTag { Ns = TestNs, Tag = "af"}
                    }
                },
                _listA,
                _listB.Concat(_listC).Concat(_listD).Concat(_listE).Concat(_listF),
                CountA);
            
            await Test(new AccountQuery
                {
                    TaggedWithAll = new List<FqTag>
                    {
                        new FqTag { Ns = TestNs, Tag = "ab"},
                        new FqTag { Ns = TestNs, Tag = "abc"},
                        new FqTag { Ns = TestNs, Tag = "abcd"},
                        new FqTag { Ns = TestNs, Tag = "abcde"},
                        new FqTag { Ns = TestNs, Tag = "abcdef"}
                    }
                },
                _listAb,
                _listC.Concat(_listD).Concat(_listE).Concat(_listF),
                CountA + CountB);
        }

        [TestMethod]
        public async Task TestTaggedWithAny()
        {
            await BuildUserBase();

            await Test(new AccountQuery
                {
                    TaggedWithAny = new List<FqTag>
                    {
                        new FqTag {Ns = TestNs, Tag = "abcdef"}
                    }
                },
                _listAbcdef,
                null,
                CountAll);
            
            await Test(new AccountQuery
                {
                    TaggedWithAny = new List<FqTag>
                    {
                        new FqTag { Ns = TestNs, Tag = "abcde"},
                        new FqTag { Ns = TestNs, Tag = "f"}
                    }
                },
                _listAbcdef,
                null,
                CountAll);
            
            await Test(new AccountQuery
                {
                    TaggedWithAny = new List<FqTag>
                    {
                        new FqTag { Ns = TestNs, Tag = "ab"},
                        new FqTag { Ns = TestNs, Tag = "abc"}
                    }
                },
                _listAbc,
                _listD.Concat(_listE).Concat(_listF),
                CountA + CountB + CountC);
            
            await Test(new AccountQuery
                {
                    TaggedWithAny = new List<FqTag>
                    {
                        new FqTag { Ns = TestNs, Tag = "ab"},
                        new FqTag { Ns = TestNs, Tag = "ac"},
                        new FqTag { Ns = TestNs, Tag = "ad"},
                        new FqTag { Ns = TestNs, Tag = "ae"},
                        new FqTag { Ns = TestNs, Tag = "af"}
                    }
                },
                _listAbcdef,
                null,
                CountAll);
            
            await Test(new AccountQuery
                {
                    TaggedWithAny = new List<FqTag>
                    {
                        new FqTag { Ns = TestNs, Tag = "a"},
                        new FqTag { Ns = TestNs, Tag = "b"},
                        new FqTag { Ns = TestNs, Tag = "d"},
                        new FqTag { Ns = TestNs, Tag = "e"},
                        new FqTag { Ns = TestNs, Tag = "f"}
                    }
                },
                _listAb.Concat(_listD).Concat(_listE).Concat(_listF),
                _listC,
                CountAll - CountC);
        }

        [TestMethod]
        public async Task TestTaggedInNs()
        {
            await BuildUserBase();

            var otherNs = "other-ns"; 
            CheckSuccess(await Client.Tags.PutTagNsDefinition(otherNs, new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), otherNs, "a"));
            RefreshIndex();

            await Test(new AccountQuery
                {
                    TaggedInNs = TestNs
                },
                _listAbcdef,
                null,
                CountAll);
            
            await Test(new AccountQuery
                {
                    TaggedInNs = otherNs
                },
                null,
                _listAbcdef,
                1);
            
            await Test(new AccountQuery
                {
                    Or = new List<AccountQuery>
                    {
                        new AccountQuery {TaggedInNs = TestNs},
                        new AccountQuery {TaggedInNs = otherNs}
                    }
                },
                _listAbcdef,
                null,
                CountAll + 1);
            
            await Test(new AccountQuery
                {
                    And = new List<AccountQuery>
                    {
                        new AccountQuery {TaggedInNs = TestNs},
                        new AccountQuery {TaggedInNs = otherNs}
                    }
                },
                null,
                _listAbcdef,
                0);
        }

        [TestMethod]
        public Task TestFieldWithin()
        {
            Assert.Inconclusive();
            return Task.CompletedTask;
        }

        [TestMethod]
        public async Task TestAnd()
        {
            await BuildUserBase();

            await Test(new AccountQuery
                {
                    And = new List<AccountQuery>()
                },
                _listAbcdef,
                null,
                CountAll);
            
            await Test(new AccountQuery
                {
                    And = new List<AccountQuery>
                    {
                        new AccountQuery { TaggedInNs = TestNs }
                    }
                },
                _listAbcdef,
                null,
                CountAll);
            
            await Test(new AccountQuery
                {
                    And = new List<AccountQuery>
                    {
                        new AccountQuery { TaggedInNs = TestNs },
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "abc"}}}
                    }
                },
                _listAbc,
                _listD.Concat(_listE).Concat(_listF),
                CountA + CountB + CountC);
            
            await Test(new AccountQuery
                {
                    And = new List<AccountQuery>
                    {
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "a"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "b"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "c"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "d"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "e"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "f"}}}
                    }
                },
                null,
                _listAbcdef,
                0);
            
            await Test(new AccountQuery
                {
                    And = new List<AccountQuery>
                    {
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "a"}}},
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "b"}}},
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "c"}}},
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "d"}}},
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "e"}}},
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "f"}}}
                    }
                },
                null,
                _listAbcdef,
                0);
            
            await Test(new AccountQuery
                {
                    And = new List<AccountQuery>
                    {
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "ab"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "abc"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "abcd"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "abcde"}}}
                    }
                },
                _listAb,
                _listC.Concat(_listD).Concat(_listE).Concat(_listF),
                CountA + CountB);
        }
        
        [TestMethod]
        public async Task TestOr()
        {
            await BuildUserBase();
            
            await Test(new AccountQuery
                {
                    Or = new List<AccountQuery>()
                },
                _listAbcdef,
                null,
                CountAll);
            
            await Test(new AccountQuery
                {
                    Or = new List<AccountQuery>
                    {
                        new AccountQuery { TaggedInNs = TestNs }
                    }
                },
                _listAbcdef,
                null,
                CountAll);
            
            await Test(new AccountQuery
                {
                    Or = new List<AccountQuery>
                    {
                        new AccountQuery { TaggedInNs = TestNs },
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "abc"}}}
                    }
                },
                _listAbcdef,
                null,
                CountAll);
            
            await Test(new AccountQuery
                {
                    Or = new List<AccountQuery>
                    {
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "a"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "b"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "c"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "d"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "e"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "f"}}}
                    }
                },
                _listAbcdef,
                null,
                CountAll);
            
            await Test(new AccountQuery
                {
                    Or = new List<AccountQuery>
                    {
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "a"}}},
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "b"}}},
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "c"}}},
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "d"}}},
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "e"}}},
                        new AccountQuery { TaggedWithAny = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "f"}}}
                    }
                },
                _listAbcdef,
                null,
                CountAll);
            
            await Test(new AccountQuery
                {
                    Or = new List<AccountQuery>
                    {
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "ab"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "abc"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "abcd"}}},
                        new AccountQuery { TaggedWithAll = new List<FqTag> { new FqTag { Ns = TestNs, Tag = "abcde"}}}
                    }
                },
                _listAbcde,
                _listF,
                CountAll - CountF);
        }

        [TestMethod]
        public async Task TestNot()
        {
            await BuildUserBase();

            await Test(new AccountQuery
                {
                    Not = new AccountQuery()
                },
                null,
                _listAbcdef,
                0);
            
            await Test(new AccountQuery
                {
                    Not = new AccountQuery {TaggedInNs = TestNs}
                },
                null,
                _listAbcdef,
                0);

            await Test(new AccountQuery
                {
                    Not = new AccountQuery
                    {
                        And = new List<AccountQuery>
                        {
                            new AccountQuery {TaggedInNs = TestNs},
                            new AccountQuery {TaggedWithAll = new List<FqTag> {new FqTag {Ns = TestNs, Tag = "abc"}}}
                        }

                    }
                },
                _listD.Concat(_listE).Concat(_listF),
                _listAbc,
                CountAll - CountA - CountB - CountC);
            
            await Test(new AccountQuery
                {
                    Not = new AccountQuery
                    {
                        And = new List<AccountQuery>
                        {
                            new AccountQuery {TaggedWithAll = new List<FqTag> {new FqTag {Ns = TestNs, Tag = "a"}}},
                            new AccountQuery {TaggedWithAll = new List<FqTag> {new FqTag {Ns = TestNs, Tag = "b"}}},
                            new AccountQuery {TaggedWithAll = new List<FqTag> {new FqTag {Ns = TestNs, Tag = "c"}}},
                            new AccountQuery {TaggedWithAll = new List<FqTag> {new FqTag {Ns = TestNs, Tag = "d"}}},
                            new AccountQuery {TaggedWithAll = new List<FqTag> {new FqTag {Ns = TestNs, Tag = "e"}}},
                            new AccountQuery {TaggedWithAll = new List<FqTag> {new FqTag {Ns = TestNs, Tag = "f"}}}
                        }
                    }
                },
                _listAbcdef,
                null,
                CountAll);
        }

        [TestMethod]
        public async Task TestComplexCombinations()
        {
            await BuildUserBase();
            
            await Test(new AccountQuery
                {
                    Not = new AccountQuery
                    {
                        Not = new AccountQuery
                        {
                            Not = new AccountQuery
                            {
                                Not = new AccountQuery
                                {
                                    Not = new AccountQuery
                                    {
                                        TaggedWithAny = new List<FqTag>
                                        {
                                            new FqTag { Ns = TestNs, Tag = "a"},
                                            new FqTag { Ns = TestNs, Tag = "b"},
                                            new FqTag { Ns = TestNs, Tag = "c"},
                                            new FqTag { Ns = TestNs, Tag = "e"}
                                        }
                                    }
                                }                            
                            }
                        }
                    },
                    TaggedInNs = TestNs,
                    TaggedWithAny = new List<FqTag>
                    {
                        new FqTag { Ns = TestNs, Tag = "abc" },
                        new FqTag { Ns = TestNs, Tag = "abcd" }
                    }
                },
                _listD,
                _listAbc.Concat(_listE).Concat(_listF),
                CountD);
        }
        
        private async Task BuildUserBase()
        {
            _listA = Enumerable.Range(1, CountA).Select(_ => BuildUniqueString()).ToList();
            _listB = Enumerable.Range(1, CountB).Select(_ => BuildUniqueString()).ToList();
            _listC = Enumerable.Range(1, CountC).Select(_ => BuildUniqueString()).ToList();
            _listD = Enumerable.Range(1, CountD).Select(_ => BuildUniqueString()).ToList();
            _listE = Enumerable.Range(1, CountE).Select(_ => BuildUniqueString()).ToList();
            _listF = Enumerable.Range(1, CountF).Select(_ => BuildUniqueString()).ToList();
            _listAb = _listA.Concat(_listB).ToList();
            _listAc = _listA.Concat(_listC).ToList();
            _listAd = _listA.Concat(_listD).ToList();
            _listAe = _listA.Concat(_listE).ToList();
            _listAf = _listA.Concat(_listF).ToList();
            _listAbc = _listAb.Concat(_listC).ToList();
            _listAbd = _listAb.Concat(_listD).ToList();
            _listAbe = _listAb.Concat(_listE).ToList();
            _listAbf = _listAb.Concat(_listF).ToList();
            _listAbcd = _listAbc.Concat(_listD).ToList();
            _listAbce = _listAbc.Concat(_listE).ToList();
            _listAbcf = _listAbc.Concat(_listF).ToList();
            _listAbcde = _listAbcd.Concat(_listE).ToList();
            _listAbcdf = _listAbcd.Concat(_listF).ToList();
            _listAbcdef = _listAbcde.Concat(_listF).ToList();

            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "a",
                new PutTagInAccountsRequest {AccountIds = _listA}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "b",
                new PutTagInAccountsRequest {AccountIds = _listB}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "c",
                new PutTagInAccountsRequest {AccountIds = _listC}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "d",
                new PutTagInAccountsRequest {AccountIds = _listD}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "e",
                new PutTagInAccountsRequest {AccountIds = _listE}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "f",
                new PutTagInAccountsRequest {AccountIds = _listF}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "ab",
                new PutTagInAccountsRequest {AccountIds = _listAb}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "ac",
                new PutTagInAccountsRequest {AccountIds = _listAc}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "ad",
                new PutTagInAccountsRequest {AccountIds = _listAd}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "ae",
                new PutTagInAccountsRequest {AccountIds = _listAe}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "af",
                new PutTagInAccountsRequest {AccountIds = _listAf}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "abc",
                new PutTagInAccountsRequest {AccountIds = _listAbc}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "abd",
                new PutTagInAccountsRequest {AccountIds = _listAbd}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "abe",
                new PutTagInAccountsRequest {AccountIds = _listAbe}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "abf",
                new PutTagInAccountsRequest {AccountIds = _listAbf}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "abcd",
                new PutTagInAccountsRequest {AccountIds = _listAbcd}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "abce",
                new PutTagInAccountsRequest {AccountIds = _listAbce}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "abcf",
                new PutTagInAccountsRequest {AccountIds = _listAbcf}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "abcde",
                new PutTagInAccountsRequest {AccountIds = _listAbcde}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "abcdf",
                new PutTagInAccountsRequest {AccountIds = _listAbcdf}));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, "abcdef",
                new PutTagInAccountsRequest {AccountIds = _listAbcdef}));
            
            RefreshIndex();
        }

        private async Task Test(AccountQuery query,
            IEnumerable<string> included, IEnumerable<string> excluded, int expectedCount)
        {
            var result = CheckSuccess(await Client.Accounts.GetAccountQueryResults(
                new GetAccountQueryResultsRequest {Query = query, Count = 1000, ContinueFrom = null}));

            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.RequestedContinueFrom));
            Assert.IsTrue(string.IsNullOrEmpty(result.ContinueWith));
            Assert.AreEqual(1000, result.RequestedCount);
            Assert.AreEqual(expectedCount, result.TotalNumberOfResults);

            var resultList = result.Accounts?.Select(a => a.AccountId).ToList() ?? new List<string>();
            Assert.AreEqual(expectedCount, resultList.Count);

            Assert.IsFalse(included.EmptyIfNull().Except(resultList).Any());
            Assert.IsFalse(excluded.EmptyIfNull().Intersect(resultList).Any());
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;
using Tesseract.Common.Extensions;
using Tesseract.Common.Utils;

namespace Appson.Tesseract.Tests.Api.Tags
{
    [TestClass]
    public class GetTagAccountListTests : TestClassBase
    {
        [TestMethod]
        public async Task NonExistingTagShouldReturnZeroResults()
        {
            var tag = BuildUniqueString();
            var result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 10));

            Assert.IsNotNull(result);
            Assert.AreEqual(TestNs, result.RequestedTagNs);
            Assert.AreEqual(tag, result.RequestedTag);
            Assert.AreEqual(10, result.RequestedCount);
            Assert.IsNull(result.RequestedContinueFrom);

            Assert.IsNotNull(result.Accounts);
            Assert.AreEqual(0, result.Accounts.Count);
            Assert.AreEqual(0, result.TotalNumberOfResults);
            Assert.IsNull(result.ContinueWith);
        }
        
        [TestMethod]
        public async Task ListSingleAccount()
        {
            var tag = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, tag));

            RefreshIndex();
            var result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag));

            Assert.AreEqual(1, result.Accounts.Count);
            Assert.IsNotNull(result.Accounts[0]);
            Assert.AreEqual(TestAccount, result.Accounts[0].AccountId);
            Assert.AreEqual(1, result.TotalNumberOfResults);
            Assert.IsNull(result.ContinueWith);
        }

        [TestMethod]
        public async Task ListSingleAccountWithLimitedCount()
        {
            var tag = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, tag));

            RefreshIndex();
            var result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 1));

            Assert.AreEqual(1, result.RequestedCount);
            Assert.AreEqual(1, result.Accounts.Count);
            Assert.IsNotNull(result.Accounts[0]);
            Assert.AreEqual(TestAccount, result.Accounts[0].AccountId);
            Assert.AreEqual(1, result.TotalNumberOfResults);
            Assert.IsNull(result.ContinueWith);
        }

        [TestMethod]
        public async Task ListFirstAccountFromMany()
        {
            var tag = BuildUniqueString();
            var accountIds = Enumerable.Range(1, 20).Select(a => BuildUniqueString()).ToArray();
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, tag, new PutTagInAccountsRequest
            {
                AccountIds = accountIds.ToList()
            }));

            RefreshIndex();
            var result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 1));

            Assert.AreEqual(1, result.RequestedCount);
            Assert.AreEqual(1, result.Accounts.Count);
            Assert.IsNotNull(result.Accounts[0]);
            Assert.AreEqual(1, accountIds.Count(accountId => accountId == result.Accounts[0].AccountId));
            Assert.AreEqual(20, result.TotalNumberOfResults);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ContinueWith));
        }

        [TestMethod]
        public async Task ListFirstIncompletePage()
        {
            var tag = BuildUniqueString();
            var accountIds = Enumerable.Range(1, 10).Select(a => BuildUniqueString()).ToArray();
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, tag, new PutTagInAccountsRequest
            {
                AccountIds = accountIds.ToList()
            }));

            RefreshIndex();
            var result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 20));

            Assert.AreEqual(20, result.RequestedCount);
            Assert.AreEqual(10, result.Accounts.Count);
            Assert.AreEqual(10, result.TotalNumberOfResults);
            Assert.IsNull(result.ContinueWith);

            var outputIds = result.Accounts.Select(a => a.AccountId).OrderBy(aid => aid).ToList();
            var inputIds = accountIds.OrderBy(aid => aid).ToList();
            Assert.IsTrue(outputIds.SequenceEqual(inputIds));
        }

        [TestMethod]
        public async Task ListFirstPageOfMany()
        {
            var tag = BuildUniqueString();
            var accountIds = new HashSet<string>(Enumerable.Range(1, 50).Select(a => BuildUniqueString()));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, tag, new PutTagInAccountsRequest
            {
                AccountIds = accountIds.ToList()
            }));

            RefreshIndex();
            var result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 20));

            Assert.AreEqual(20, result.RequestedCount);
            Assert.AreEqual(20, result.Accounts.Count);
            Assert.AreEqual(50, result.TotalNumberOfResults);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ContinueWith));

            result.Accounts.ForEach(resultItem => Assert.IsTrue(accountIds.Contains(resultItem.AccountId)));
        }

        [TestMethod]
        public async Task ListSecondPageOfMany()
        {
            var tag = BuildUniqueString();
            var accountIds = new HashSet<string>(Enumerable.Range(1, 50).Select(a => BuildUniqueString()));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, tag, new PutTagInAccountsRequest
            {
                AccountIds = accountIds.ToList()
            }));

            RefreshIndex();
            var result1 = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 20));

            Assert.AreEqual(20, result1.RequestedCount);
            Assert.AreEqual(20, result1.Accounts.Count);
            Assert.AreEqual(50, result1.TotalNumberOfResults);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result1.ContinueWith));

            var result2 = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 20, result1.ContinueWith));

            Assert.AreEqual(20, result2.RequestedCount);
            Assert.AreEqual(20, result2.Accounts.Count);
            Assert.AreEqual(50, result2.TotalNumberOfResults);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result2.ContinueWith));

            result1.Accounts.ForEach(resultItem => Assert.IsTrue(accountIds.Contains(resultItem.AccountId)));
            result2.Accounts.ForEach(resultItem => Assert.IsTrue(accountIds.Contains(resultItem.AccountId)));
        }

        [TestMethod]
        public async Task ListRemainingInSecondPage()
        {
            var tag = BuildUniqueString();
            var accountIds = new HashSet<string>(Enumerable.Range(1, 50).Select(a => BuildUniqueString()));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, tag, new PutTagInAccountsRequest
            {
                AccountIds = accountIds.ToList()
            }));

            int firstPageSize = RandomProvider.GetThreadRandom().Next(39) + 1;
            int secondPageSize = 50 - firstPageSize;

            RefreshIndex();
            var result1 = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, firstPageSize));

            Assert.AreEqual(firstPageSize, result1.RequestedCount);
            Assert.AreEqual(firstPageSize, result1.Accounts.Count);
            Assert.AreEqual(50, result1.TotalNumberOfResults);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result1.ContinueWith));

            var result2 = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag,
                secondPageSize, result1.ContinueWith));

            Assert.AreEqual(secondPageSize, result2.RequestedCount);
            Assert.AreEqual(secondPageSize, result2.Accounts.Count);
            Assert.AreEqual(50, result2.TotalNumberOfResults);
            Assert.IsNull(result2.ContinueWith);

            var allAccounts = result1.Accounts.Select(a => a.AccountId)
                .Union(result2.Accounts.Select(a => a.AccountId))
                .ToList();
                
            Assert.AreEqual(50, allAccounts.Count);
            allAccounts.ForEach(aid => Assert.IsTrue(accountIds.Contains(aid)));
        }

        [TestMethod]
        public async Task ListAllPagesToTheEnd()
        {
            var tag = BuildUniqueString();
            var accountIds = new HashSet<string>(Enumerable.Range(1, 50).Select(a => a.ToString()));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, tag, new PutTagInAccountsRequest
            {
                AccountIds = accountIds.ToList()
            }));

            RefreshIndex();
            string continueFrom = null;
            var returnedAccountIds = new List<string>();

            for(var i = 0; i < 5; i++)
            {
                var result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 10, continueFrom));

                Assert.AreEqual(10, result.RequestedCount);
                Assert.AreEqual(continueFrom, result.RequestedContinueFrom);
                Assert.AreEqual(50, result.TotalNumberOfResults);
                Assert.IsNotNull(result.Accounts);
                Assert.AreEqual(10, result.Accounts.Count);

                continueFrom = result.ContinueWith;
                returnedAccountIds.AddRange(result.Accounts.Select(a => a.AccountId));
            }

            Assert.IsNull(continueFrom);
            Assert.AreEqual(50, returnedAccountIds.Count);

            returnedAccountIds.ForEach(aid => Assert.IsTrue(accountIds.Contains(aid)));
            accountIds.ForEach(aid => Assert.IsTrue(returnedAccountIds.Contains(aid)));
        }

        [TestMethod]
        public async Task ListMisalignedPageSizesToTheEnd()
        {
            var tag = BuildUniqueString();
            var accountIds = new HashSet<string>(Enumerable.Range(1, 50).Select(a => BuildUniqueString()));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, tag, new PutTagInAccountsRequest
            {
                AccountIds = accountIds.ToList()
            }));

            RefreshIndex();
            string continueFrom = null;
            var returnedAccountIds = new List<string>();

            do
            {
                var pageSize = RandomProvider.GetThreadRandom().Next(9) + 1;
                var result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, pageSize, continueFrom));

                Assert.AreEqual(pageSize, result.RequestedCount);
                Assert.AreEqual(continueFrom, result.RequestedContinueFrom);
                Assert.IsNotNull(result.Accounts);
                Assert.IsTrue(result.Accounts.Count > 0);
                Assert.AreEqual(50, result.TotalNumberOfResults);

                continueFrom = result.ContinueWith;
                returnedAccountIds.AddRange(result.Accounts.Select(a => a.AccountId));

            } while (!string.IsNullOrWhiteSpace(continueFrom));


            Assert.AreEqual(50, returnedAccountIds.Count);
            returnedAccountIds.ForEach(aid => Assert.IsTrue(accountIds.Contains(aid)));
            accountIds.ForEach(aid => Assert.IsTrue(returnedAccountIds.Contains(aid)));
        }

        [TestMethod]
        public async Task ListLastIncompletePage()
        {
            var tag = BuildUniqueString();
            var accountIds = new HashSet<string>(Enumerable.Range(1, 50).Select(a => BuildUniqueString()));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, tag, new PutTagInAccountsRequest
            {
                AccountIds = accountIds.ToList()
            }));

            RefreshIndex();
            var result1 = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 20));
            var result2 = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 20, result1.ContinueWith));
            var result3 = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 20, result2.ContinueWith));

            Assert.AreEqual(20, result3.RequestedCount);
            Assert.AreEqual(10, result3.Accounts.Count);
            Assert.AreEqual(50, result3.TotalNumberOfResults);
            Assert.IsNull(result3.ContinueWith);

            result1.Accounts.ForEach(resultItem => Assert.IsTrue(accountIds.Contains(resultItem.AccountId)));
            result2.Accounts.ForEach(resultItem => Assert.IsTrue(accountIds.Contains(resultItem.AccountId)));
            result3.Accounts.ForEach(resultItem => Assert.IsTrue(accountIds.Contains(resultItem.AccountId)));
        }

        [TestMethod]
        public async Task CheckIfOtherTagsInSameNsExcluded()
        {
            var tag = BuildUniqueString();
            var otherTag = BuildUniqueString();
            var accountIds = new HashSet<string>(Enumerable.Range(1, 50).Select(a => BuildUniqueString()));
            var otherAccountIds = new HashSet<string>(Enumerable.Range(1, 50).Select(a => BuildUniqueString()));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, tag, new PutTagInAccountsRequest
            {
                AccountIds = accountIds.ToList()
            }));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, otherTag, new PutTagInAccountsRequest
            {
                AccountIds = otherAccountIds.ToList()
            }));

            RefreshIndex();
            var result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, tag, 100));

            Assert.AreEqual(100, result.RequestedCount);
            Assert.AreEqual(50, result.Accounts.Count);
            Assert.AreEqual(50, result.TotalNumberOfResults);
            Assert.IsNull(result.ContinueWith);

            result.Accounts.ForEach(resultItem => Assert.IsTrue(accountIds.Contains(resultItem.AccountId)));
            result.Accounts.ForEach(resultItem => Assert.IsFalse(otherAccountIds.Contains(resultItem.AccountId)));
        }

        [TestMethod]
        public async Task CheckIfSameTagInOtherNsExcluded()
        {
            var tag = BuildUniqueString();
            var ns = BuildUniqueString();
            var otherNs = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Tags.PutTagNsDefinition(otherNs, new PutTagNsDefinitionRequest()));

            var accountIds = new HashSet<string>(Enumerable.Range(1, 50).Select(a => BuildUniqueString()));
            var otherAccountIds = new HashSet<string>(Enumerable.Range(1, 50).Select(a => BuildUniqueString()));
            CheckSuccess(await Client.Tags.PutTagInAccounts(ns, tag, new PutTagInAccountsRequest
            {
                AccountIds = accountIds.ToList()
            }));
            CheckSuccess(await Client.Tags.PutTagInAccounts(otherNs, tag, new PutTagInAccountsRequest
            {
                AccountIds = otherAccountIds.ToList()
            }));

            RefreshIndex();
            var result = CheckSuccess(await Client.Tags.GetTagAccountList(ns, tag, 100));

            Assert.AreEqual(100, result.RequestedCount);
            Assert.AreEqual(50, result.Accounts.Count);
            Assert.AreEqual(50, result.TotalNumberOfResults);
            Assert.IsNull(result.ContinueWith);

            result.Accounts.ForEach(resultItem => Assert.IsTrue(accountIds.Contains(resultItem.AccountId)));
            result.Accounts.ForEach(resultItem => Assert.IsFalse(otherAccountIds.Contains(resultItem.AccountId)));
        }

    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Tags
{
    [TestClass]
    public class DeleteTagFromAccountsTests : TestClassBase
    {
        [TestMethod]
        public async Task DeleteTagFromSingleAccount()
        {
            var account1 = BuildUniqueString();
            var account2 = BuildUniqueString();

            CheckSuccess(await Client.Accounts.PutTagOnAccount(account1, TestNs, TestTag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(account2, TestNs, TestTag));

            CheckSuccess(await Client.Tags.DeleteTagFromAccounts(TestNs, TestTag, new DeleteTagFromAccountsRequest
            {
                AccountIds = new List<string> { account1 }
            }));

            Assert.AreEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(account1, TestNs, TestTag)).Weight);
            Assert.AreNotEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(account2, TestNs, TestTag)).Weight);
        }

        [TestMethod]
        public async Task DeleteTagFromMultipleAccounts()
        {
            var accounts = Enumerable.Range(0, 50).Select(i => BuildUniqueString()).ToList();
            var anotherAccount = BuildUniqueString();

            CheckSuccess(await Client.Accounts.PutTagOnAccount(anotherAccount, TestNs, TestTag));
            await Task.WhenAll(accounts.Select(async a =>
            {
                CheckSuccess(await Client.Accounts.PutTagOnAccount(a, TestNs, TestTag));
            }));

            CheckSuccess(await Client.Tags.DeleteTagFromAccounts(TestNs, TestTag, new DeleteTagFromAccountsRequest
            {
                AccountIds = accounts
            }));

            await Task.WhenAll(accounts.Select(async a =>
            {
                Assert.AreEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(a, TestNs, TestTag)).Weight);
            }));

            Assert.AreNotEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(anotherAccount, TestNs, TestTag)).Weight);
        }

        [TestMethod]
        public async Task UnknownAccountsShouldBeIgnored()
        {
            var account1 = BuildUniqueString();
            var account2 = BuildUniqueString();
            var account3 = BuildUniqueString();

            CheckSuccess(await Client.Accounts.PutTagOnAccount(account1, TestNs, TestTag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(account2, TestNs, TestTag));

            CheckSuccess(await Client.Tags.DeleteTagFromAccounts(TestNs, TestTag, new DeleteTagFromAccountsRequest
            {
                AccountIds = new List<string> { account1, account3 }
            }));

            Assert.AreEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(account1, TestNs, TestTag)).Weight);
            Assert.AreNotEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(account2, TestNs, TestTag)).Weight);
        }
    }
}
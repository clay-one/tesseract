using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tesseract.ApiModel.Tags;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Tags
{
    [TestClass]
    public class PutTagInAccountsTests : TestClassBase
    {
        [TestMethod]
        public async Task NewTagShouldBeSetToOne()
        {
            var tag = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, tag,
                new PutTagInAccountsRequest {AccountIds = new List<string> {TestAccount}}));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, tag);
            Assert.IsNotNull(weightResponse);
            Assert.AreEqual(weightResponse.Weight, 1.0d);
        }

        [TestMethod]
        public async Task ExistingTagShouldNotBeChanged()
        {
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(TestAccount, TestNs, TestTag, 25.0d));
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, TestTag,
                new PutTagInAccountsRequest {AccountIds = new List<string> {TestAccount}}));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag);
            Assert.IsNotNull(weightResponse);
            Assert.AreEqual(weightResponse.Weight, 25.0d);
        }

        [TestMethod]
        public async Task ShouldAcceptArbitraryInput()
        {
            var account = BuildUniqueString();
            var tagNs = BuildUniqueString();
            var tag = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(tagNs, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Tags.PutTagInAccounts(tagNs, tag,
                new PutTagInAccountsRequest {AccountIds = new List<string> {account}}));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(account, tagNs, tag);
            Assert.IsNotNull(weightResponse);
            Assert.AreEqual(weightResponse.Weight, 1.0d);
        }

        [TestMethod]
        public async Task PutTagOnMultipleAccounts()
        {
            var accounts = Enumerable.Range(0, 50).Select(i => BuildUniqueString()).ToList();

            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, TestTag, new PutTagInAccountsRequest
            {
                AccountIds = accounts
            }));

            await Task.WhenAll(accounts.Select(async a =>
            {
                Assert.AreNotEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(a, TestNs, TestTag)).Weight);
            }));
        }
    }
}
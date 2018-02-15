using System;
using System.Net;
using System.Threading.Tasks;
using Tesseract.ApiModel.Tags;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.Common.Utils;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class PutTagWeightOnAccountTests : TestClassBase
    {

        [TestMethod]
        public async Task SetWeightAndRetrieve()
        {
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(TestAccount, TestNs, TestTag, 25.0d));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag);
            Assert.IsNotNull(weightResponse);
            Assert.AreEqual(weightResponse.Weight, 25.0d);
        }

        [TestMethod]
        public async Task SetWeightAndCheckAccountInfo()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 25.0d));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.IsNotNull(accountInfo);
            Assert.IsNotNull(accountInfo.TagNamespaces);
            Assert.AreEqual(1, accountInfo.TagNamespaces.Count);
            Assert.AreEqual(TestNs, accountInfo.TagNamespaces[0].Namespace);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags);
            Assert.AreEqual(1, accountInfo.TagNamespaces[0].Tags.Count);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags[0]);
            Assert.AreEqual(TestTag, accountInfo.TagNamespaces[0].Tags[0].Tag);
            Assert.AreEqual(25.0d, accountInfo.TagNamespaces[0].Tags[0].Weight);
        }

        [TestMethod]
        public async Task SettingToZeroShouldRemove()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, TestNs, TestTag));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(account, TestNs, TestTag);
            Assert.IsNotNull(weightResponse);
            Assert.AreNotEqual(0d, weightResponse.Weight);

            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 0d));

            weightResponse = await Client.Accounts.GetTagWeightOnAccount(account, TestNs, TestTag);
            Assert.IsNotNull(weightResponse);
            Assert.AreEqual(0d, weightResponse.Weight);

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.AreEqual(0, accountInfo.TagNamespaces.Count);
        }

        [TestMethod]
        public async Task ShouldNotAcceptNegativeWeight()
        {
            CheckFailure(await Client.Accounts.PutTagWeightOnAccount(TestAccount, TestNs, TestTag, -1d), HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task ShouldAcceptArbitraryInput()
        {
            var account = BuildUniqueString();
            var tagNs = BuildUniqueString();
            var tag = BuildUniqueString();
            var weight = RandomProvider.GetThreadRandom().NextDouble();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(tagNs, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, tagNs, tag, weight));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(account, tagNs, tag);
            Assert.IsNotNull(weightResponse);
            Assert.IsFalse(Math.Abs(weight - weightResponse.Weight) > 1e-15);
        }
    }
}
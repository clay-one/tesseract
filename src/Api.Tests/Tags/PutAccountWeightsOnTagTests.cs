using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;
using Tesseract.Common.Utils;

namespace Appson.Tesseract.Tests.Api.Tags
{
    [TestClass]
    public class PutAccountWeightsOnTagTests : TestClassBase
    {
        [TestMethod]
        public async Task SetWeightAndRetrieve()
        {
            CheckSuccess(await Client.Tags.PutAccountWeightsOnTag(TestNs, TestTag, new PutAccountWeightsOnTagRequest
            {
                Accounts = new List<AccountWeightOnTag>
                {
                    new AccountWeightOnTag { AccountId = TestAccount, Weight = 25.0d }
                }
            }));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag);
            Assert.IsNotNull(weightResponse);
            Assert.AreEqual(25.0d, weightResponse.Weight);
        }

        [TestMethod]
        public async Task SetWeightAndCheckAccountInfo()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutAccountWeightsOnTag(TestNs, TestTag, new PutAccountWeightsOnTagRequest
            {
                Accounts = new List<AccountWeightOnTag>
                {
                    new AccountWeightOnTag { AccountId = account, Weight = 25.0d }
                }
            }));

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
            CheckSuccess(await Client.Tags.PutAccountWeightsOnTag(TestNs, TestTag, new PutAccountWeightsOnTagRequest
            {
                Accounts = new List<AccountWeightOnTag>
                {
                    new AccountWeightOnTag { AccountId = account, Weight = 5d }
                }
            }));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(account, TestNs, TestTag);
            Assert.IsNotNull(weightResponse);
            Assert.AreNotEqual(0d, weightResponse.Weight);

            CheckSuccess(await Client.Tags.PutAccountWeightsOnTag(TestNs, TestTag, new PutAccountWeightsOnTagRequest
            {
                Accounts = new List<AccountWeightOnTag>
                {
                    new AccountWeightOnTag { AccountId = account, Weight = 0d }
                }
            }));

            weightResponse = await Client.Accounts.GetTagWeightOnAccount(account, TestNs, TestTag);
            Assert.IsNotNull(weightResponse);
            Assert.AreEqual(0d, weightResponse.Weight);

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.AreEqual(0, accountInfo.TagNamespaces.Count);
        }

        [TestMethod]
        public async Task ShouldNotAcceptNegativeWeight()
        {
            var account1 = BuildUniqueString();
            var account2 = BuildUniqueString();

            CheckSuccess(await Client.Tags.PutAccountWeightsOnTag(TestNs, TestTag, new PutAccountWeightsOnTagRequest
            {
                Accounts = new List<AccountWeightOnTag>
                {
                    new AccountWeightOnTag { AccountId = account1, Weight = 5d },
                    new AccountWeightOnTag { AccountId = account2, Weight = 10d }
                }
            }));

            CheckFailure(await Client.Tags.PutAccountWeightsOnTag(TestNs, TestTag, new PutAccountWeightsOnTagRequest
            {
                Accounts = new List<AccountWeightOnTag>
                {
                    new AccountWeightOnTag { AccountId = account1, Weight = 20d },
                    new AccountWeightOnTag { AccountId = account2, Weight = -1d }
                }
            }), HttpStatusCode.BadRequest);

            // Make sure the other account is not affected
            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(account1, TestNs, TestTag);
            Assert.IsNotNull(weightResponse);
            Assert.AreEqual(5d, weightResponse.Weight);
        }

        [TestMethod]
        public async Task ShouldAcceptArbitraryInput()
        {
            var account = BuildUniqueString();
            var tagNs = BuildUniqueString();
            var tag = BuildUniqueString();
            var weight = RandomProvider.GetThreadRandom().NextDouble();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(tagNs, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Tags.PutAccountWeightsOnTag(tagNs, tag, new PutAccountWeightsOnTagRequest
            {
                Accounts = new List<AccountWeightOnTag>
                {
                    new AccountWeightOnTag { AccountId = account, Weight = weight }
                }
            }));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(account, tagNs, tag);
            Assert.IsNotNull(weightResponse);
            Assert.IsTrue(weight - weightResponse.Weight < 1e-15);
        }

        [TestMethod]
        public async Task SetMultipleWeights()
        {
            var accounts = Enumerable.Range(0, 50).Select(i => BuildUniqueString()).ToList();
            var accountWeights = accounts.Select(a => new AccountWeightOnTag {AccountId = a, Weight = a[0] })
                .ToList();

            CheckSuccess(await Client.Tags.PutAccountWeightsOnTag(TestNs, TestTag, new PutAccountWeightsOnTagRequest
            {
                Accounts = accountWeights
            }));

            await Task.WhenAll(accounts.Select(async a =>
            {
                Assert.AreEqual(a[0], (await Client.Accounts.GetTagWeightOnAccount(a, TestNs, TestTag)).Weight);
            }));
        }
    }
}
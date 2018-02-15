using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;
using Tesseract.Common.Utils;

namespace Appson.Tesseract.Tests.Api.Tags
{
    [TestClass]
    public class PatchAccountWeightsOnTagTests : TestClassBase
    {
        [TestMethod]
        public async Task PatchWeightAndRetrieve()
        {
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(TestAccount, TestNs, TestTag, 11.0d));

            CheckSuccess(await Client.Tags.PatchAccountWeightsOnTag(TestNs, TestTag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = new List<PatchAccountWeightsOnTagItem>
                {
                    new PatchAccountWeightsOnTagItem {AccountId = TestAccount, WeightDelta = 25.0d}
                }
            }));

            Assert.AreEqual(36.0d, (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);
        }

        [TestMethod]
        public async Task PatchWeightAndCheckAccountInfo()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Tags.PatchAccountWeightsOnTag(TestNs, TestTag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = new List<PatchAccountWeightsOnTagItem>
                {
                    new PatchAccountWeightsOnTagItem {AccountId = account, WeightDelta = 25.0d}
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
        public async Task PatchingToZeroShouldRemove()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Tags.PatchAccountWeightsOnTag(TestNs, TestTag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = new List<PatchAccountWeightsOnTagItem>
                {
                    new PatchAccountWeightsOnTagItem {AccountId = account, WeightDelta = 5d}
                }
            }));

            Assert.AreNotEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(account, TestNs, TestTag)).Weight);

            CheckSuccess(await Client.Tags.PatchAccountWeightsOnTag(TestNs, TestTag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = new List<PatchAccountWeightsOnTagItem>
                {
                    new PatchAccountWeightsOnTagItem {AccountId = account, WeightDelta = -5d}
                }
            }));

            Assert.AreEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(account, TestNs, TestTag)).Weight);

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.AreEqual(0, accountInfo.TagNamespaces.Count);
        }

        [TestMethod]
        public async Task PatchingToNegativeShouldBehaveAsZero()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Tags.PatchAccountWeightsOnTag(TestNs, TestTag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = new List<PatchAccountWeightsOnTagItem>
                {
                    new PatchAccountWeightsOnTagItem {AccountId = account, WeightDelta = 5d}
                }
            }));

            Assert.AreNotEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(account, TestNs, TestTag)).Weight);

            CheckSuccess(await Client.Tags.PatchAccountWeightsOnTag(TestNs, TestTag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = new List<PatchAccountWeightsOnTagItem>
                {
                    new PatchAccountWeightsOnTagItem {AccountId = account, WeightDelta = -10d}
                }
            }));

            Assert.AreEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(account, TestNs, TestTag)).Weight);

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.AreEqual(0, accountInfo.TagNamespaces.Count);
        }

        [TestMethod]
        public async Task ShouldAcceptArbitraryInput()
        {
            var account = BuildUniqueString();
            var tagNs = BuildUniqueString();
            var tag = BuildUniqueString();
            var weightDelta = RandomProvider.GetThreadRandom().NextDouble();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(tagNs, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Tags.PatchAccountWeightsOnTag(tagNs, tag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = new List<PatchAccountWeightsOnTagItem>
                {
                    new PatchAccountWeightsOnTagItem {AccountId = account, WeightDelta = weightDelta}
                }
            }));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(account, tagNs, tag);
            Assert.IsNotNull(weightResponse);
            Assert.IsTrue(weightDelta - weightResponse.Weight < 1e-15);
        }

        [TestMethod]
        public async Task PatchMultipleWeights()
        {
            var accounts = Enumerable.Range(0, 100).Select(i => BuildUniqueString()).ToList();
            var accountWeights = accounts.Select(a => new PatchAccountWeightsOnTagItem {AccountId = a, WeightDelta = a[0]})
                .ToList();

            CheckSuccess(await Client.Tags.PatchAccountWeightsOnTag(TestNs, TestTag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = accountWeights
            }));

            await Task.WhenAll(accounts.Select(async a =>
            {
                Assert.AreEqual(a[0], (await Client.Accounts.GetTagWeightOnAccount(a, TestNs, TestTag)).Weight);
            }));
        }
    }
}
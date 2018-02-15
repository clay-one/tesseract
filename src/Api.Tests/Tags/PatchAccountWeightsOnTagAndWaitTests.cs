using System.Collections.Generic;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Tags
{
    [TestClass]
    public class PatchAccountWeightsOnTagAndWaitTests : TestClassBase
    {
        [TestMethod]
        public async Task PatchSingleAccountWeight()
        {
            var account = BuildUniqueString();

            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 10d));
            var result = await Client.Tags.PatchAccountWeightsOnTagAndWait(TestNs, TestTag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = new List<PatchAccountWeightsOnTagItem>
                {
                    new PatchAccountWeightsOnTagItem { AccountId = account, WeightDelta = 15d }
                }
            });

            CheckSuccess(result);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(TestNs, result.Result.TagNs);
            Assert.AreEqual(TestTag, result.Result.Tag);

            Assert.IsNotNull(result.Result.Accounts);
            Assert.AreEqual(1, result.Result.Accounts.Count);

            Assert.IsNotNull(result.Result.Accounts[0]);
            Assert.AreEqual(account, result.Result.Accounts[0].AccountId);
            Assert.AreEqual(25d, result.Result.Accounts[0].Weight);
        }

        [TestMethod]
        public async Task RemovedTagShouldReturnAsZero()
        {
            var account = BuildUniqueString();

            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 10d));
            var result = await Client.Tags.PatchAccountWeightsOnTagAndWait(TestNs, TestTag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = new List<PatchAccountWeightsOnTagItem>
                {
                    new PatchAccountWeightsOnTagItem { AccountId = account, WeightDelta = -15d }
                }
            });

            CheckSuccess(result);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(TestNs, result.Result.TagNs);
            Assert.AreEqual(TestTag, result.Result.Tag);

            Assert.IsNotNull(result.Result.Accounts);
            Assert.AreEqual(1, result.Result.Accounts.Count);

            Assert.IsNotNull(result.Result.Accounts[0]);
            Assert.AreEqual(account, result.Result.Accounts[0].AccountId);
            Assert.AreEqual(0d, result.Result.Accounts[0].Weight);
        }

    }
}
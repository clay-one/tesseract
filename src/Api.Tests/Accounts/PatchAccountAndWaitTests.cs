using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.Tags;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class PatchAccountAndWaitTests : TestClassBase
    {
        [TestMethod]
        public async Task SetSingleTagWeight()
        {
            var account = BuildUniqueString();
            var result = await Client.Accounts.PatchAccountAndWait(account, new PatchAccountRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>
                {
                    new AccountTagChangeInstruction
                    {
                        TagNs = TestNs,
                        Tag = TestTag,
                        Weight = 27d
                    }
                }
            });

            CheckSuccess(result);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(1, result.Result.TagNamespaces.Count);
            Assert.AreEqual(1, result.Result.TagNamespaces[0].Tags.Count);
            Assert.AreEqual(27d, result.Result.TagNamespaces[0].Tags[0].Weight);
        }

        [TestMethod]
        public async Task PatchSingleTagWeight()
        {
            var account = BuildUniqueString();

            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 10d));
            var result = await Client.Accounts.PatchAccountAndWait(account, new PatchAccountRequest
            {
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction
                    {
                        TagNs = TestNs,
                        Tag = TestTag,
                        WeightDelta = 15d
                    }
                }
            });

            CheckSuccess(result);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(1, result.Result.TagNamespaces.Count);
            Assert.AreEqual(1, result.Result.TagNamespaces[0].Tags.Count);
            Assert.AreEqual(25d, result.Result.TagNamespaces[0].Tags[0].Weight);
        }

        [TestMethod]
        public async Task UntouchedTagsShouldBeIncludedInResult()
        {
            var account = BuildUniqueString();
            var anotherNs = BuildUniqueString();
            var anotherTag = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(anotherNs, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 10d));
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, anotherNs, anotherTag, 20d));

            var result = await Client.Accounts.PatchAccountAndWait(account, new PatchAccountRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>
                {
                    new AccountTagChangeInstruction
                    {
                        TagNs = TestNs,
                        Tag = TestTag,
                        Weight = 27d
                    }
                }
            });

            CheckSuccess(result);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(2, result.Result.TagNamespaces.Count);

            var nsInfo = result.Result.TagNamespaces.SingleOrDefault(tn => tn.Namespace == anotherNs);
            Assert.IsNotNull(nsInfo);
            Assert.IsNotNull(nsInfo.Tags);
            Assert.AreEqual(1, nsInfo.Tags.Count);
            Assert.IsNotNull(nsInfo.Tags[0]);
            Assert.AreEqual(anotherTag, nsInfo.Tags[0].Tag);
            Assert.AreEqual(20d, nsInfo.Tags[0].Weight);
        }

        [TestMethod]
        public async Task SetSingleField()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task PatchSingleField()
        {
            Assert.Inconclusive();
        }
    }
}
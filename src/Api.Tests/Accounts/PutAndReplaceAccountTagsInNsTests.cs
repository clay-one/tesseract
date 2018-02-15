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
    public class PutAndReplaceAccountTagsInNsTests : TestClassBase
    {

        [TestMethod]
        public async Task ReplaceExistingNamespace()
        {
            // Make sure TestNs exists

            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, BuildUniqueString()));

            // Replace

            var weight = 5d;
            CheckSuccess(await Client.Accounts.PutAndReplaceAccountTagsInNs(TestAccount, TestNs,
                new PutAndReplaceAccountTagsInNsRequest
                {
                    Tags = new List<TagWeightOnAccount>
                    {
                        new TagWeightOnAccount {Tag = TestTag, Weight = weight}
                    }
                }));

            // Check

            var returnedTags = await Client.Accounts.GetTagsOfAccountInNs(TestAccount, TestNs);
            Assert.IsNotNull(returnedTags);
            Assert.IsNotNull(returnedTags.Tags);
            Assert.AreEqual(1, returnedTags.Tags.Count);
            Assert.IsNotNull(returnedTags.Tags[0]);
            Assert.AreEqual(TestTag, returnedTags.Tags[0].Tag);
            Assert.AreEqual(weight, returnedTags.Tags[0].Weight);
        }

        [TestMethod]
        public async Task ReplaceNonExistingNamespaceOnExistingAccount()
        {
            // Make sure account exists

            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, BuildUniqueString()));

            // Replace 

            var weight = 5d;
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Accounts.PutAndReplaceAccountTagsInNs(TestAccount, ns,
                new PutAndReplaceAccountTagsInNsRequest
                {
                    Tags = new List<TagWeightOnAccount>
                    {
                        new TagWeightOnAccount {Tag = TestTag, Weight = weight}
                    }
                }));

            // Check

            var returnedTags = await Client.Accounts.GetTagsOfAccountInNs(TestAccount, ns);
            Assert.IsNotNull(returnedTags);
            Assert.IsNotNull(returnedTags.Tags);
            Assert.AreEqual(1, returnedTags.Tags.Count);
            Assert.IsNotNull(returnedTags.Tags[0]);
            Assert.AreEqual(TestTag, returnedTags.Tags[0].Tag);
            Assert.AreEqual(weight, returnedTags.Tags[0].Weight);
        }

        [TestMethod]
        public async Task ReplaceOnNonExistingAccount()
        {
            // Replace 

            var weight = 5d;
            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutAndReplaceAccountTagsInNs(account, TestNs,
                new PutAndReplaceAccountTagsInNsRequest
                {
                    Tags = new List<TagWeightOnAccount>
                    {
                        new TagWeightOnAccount {Tag = TestTag, Weight = weight}
                    }
                }));

            // Check

            var returnedTags = await Client.Accounts.GetTagsOfAccountInNs(account, TestNs);
            Assert.IsNotNull(returnedTags);
            Assert.IsNotNull(returnedTags.Tags);
            Assert.AreEqual(1, returnedTags.Tags.Count);
            Assert.IsNotNull(returnedTags.Tags[0]);
            Assert.AreEqual(TestTag, returnedTags.Tags[0].Tag);
            Assert.AreEqual(weight, returnedTags.Tags[0].Weight);
        }

        [TestMethod]
        public async Task ReplaceWithManyTags()
        {
            var numberOfTags = 100;
            var tagWeights = Enumerable.Range(0, numberOfTags)
                .Select(i => BuildUniqueString())
                .Select(t => new TagWeightOnAccount
                {
                    Tag = t,
                    Weight = t[0]
                })
                .ToList();

            CheckSuccess(await Client.Accounts.PutAndReplaceAccountTagsInNs(TestAccount, TestNs,
                new PutAndReplaceAccountTagsInNsRequest { Tags = tagWeights }));

            var returnedTags = await Client.Accounts.GetTagsOfAccountInNs(TestAccount, TestNs);
            Assert.IsNotNull(returnedTags);
            Assert.IsNotNull(returnedTags.Tags);
            Assert.AreEqual(numberOfTags, returnedTags.Tags.Count);

            returnedTags.Tags.ForEach(tw =>
            {
                var original = tagWeights.SingleOrDefault(o => o.Tag == tw.Tag);
                Assert.IsNotNull(original);
                Assert.AreEqual(original.Weight, tw.Weight);
            });
        }

        [TestMethod]
        public async Task ReplaceExistingWithEmpty()
        {
            // Make sure account and ns exists

            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, TestNs, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, TestNs, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, TestNs, BuildUniqueString()));

            // Replace with empty

            CheckSuccess(await Client.Accounts.PutAndReplaceAccountTagsInNs(account, TestNs,
                new PutAndReplaceAccountTagsInNsRequest
                {
                    Tags = new List<TagWeightOnAccount>()
                }));

            // Check

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.AreEqual(0, accountInfo.TagNamespaces.Count);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class GetTagsOfAccountInNsTests : TestClassBase
    {
        [TestMethod]
        public async Task UnknownAccountReturnsNotFound()
        {
            var account = BuildUniqueString();
            Assert.IsNull(await Client.Accounts.GetTagsOfAccountInNs(account, TestNs));
        }

        [TestMethod]
        public async Task KnownAccountShouldReturnResults()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, TestNs, TestTag));

            var tags = await Client.Accounts.GetTagsOfAccountInNs(account, TestNs);
            Assert.IsNotNull(tags);
        }

        [TestMethod]
        public async Task KnownAccountReturnsEmptyForNewNs()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, TestNs, TestTag));

            var newNs = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(newNs, new PutTagNsDefinitionRequest()));

            var tags = await Client.Accounts.GetTagsOfAccountInNs(account, newNs);
            Assert.IsNotNull(tags);
        }

        [TestMethod]
        public async Task EmptiedAccountShouldBeKnown()
        {
            var account = BuildUniqueString();
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, ns, TestTag));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(account, ns, TestTag));

            var tags = await Client.Accounts.GetTagsOfAccountInNs(account, ns);
            Assert.IsNotNull(tags);
        }

        [TestMethod]
        public async Task EmptiedAccountDataShouldBeCorrect()
        {
            var account = BuildUniqueString();
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, ns, TestTag));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(account, ns, TestTag));

            var tags = await Client.Accounts.GetTagsOfAccountInNs(account, ns);
            Assert.IsNotNull(tags);
            Assert.IsNotNull(tags.AccountId);
            Assert.IsNotNull(tags.Namespace);
            Assert.IsNotNull(tags.Tags);

            Assert.AreEqual(account, tags.AccountId);
            Assert.AreEqual(ns, tags.Namespace);
            Assert.AreEqual(0, tags.Tags.Count);
        }

        [TestMethod]
        public async Task TestSingleTagData()
        {
            var account = BuildUniqueString();
            var ns = BuildUniqueString();
            var tag = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, ns, tag));

            var tags = await Client.Accounts.GetTagsOfAccountInNs(account, ns);

            Assert.AreEqual(ns, tags.Namespace);
            Assert.IsNotNull(tags.Tags);
            Assert.AreEqual(1, tags.Tags.Count);
            Assert.IsNotNull(tags.Tags[0]);
            Assert.AreEqual(tag, tags.Tags[0].Tag);
            Assert.AreNotEqual(0d, tags.Tags[0].Weight);
        }

        [TestMethod]
        public async Task TestManyTagsAndWeightsInSingleNamespace()
        {
            var numberOfTags = 200;

            var account = BuildUniqueString();
            var tags = new List<TagWeightOnAccount>();
            for (int i = 0; i < numberOfTags; i++)
                tags.Add(new TagWeightOnAccount
                {
                    Tag = BuildUniqueString(),
                    Weight = i + 10
                });

            CheckSuccess(await Client.Accounts.PutAndReplaceAccountTagsInNs(account, TestNs,
                new PutAndReplaceAccountTagsInNsRequest { Tags = tags }));

            var returnedTags = await Client.Accounts.GetTagsOfAccountInNs(account, TestNs);

            Assert.AreEqual(numberOfTags, returnedTags.Tags.Count);

            returnedTags.Tags.ForEach(tw =>
            {
                var tag = tags.SingleOrDefault(t => t.Tag == tw.Tag);
                Assert.IsNotNull(tag);
                Assert.AreEqual(tag.Weight, tw.Weight);
            });
        }

        [TestMethod]
        public async Task TestManyTagsAndWeightsInMultiNamespaces()
        {
            var numberOfTags = 100;
            var ns1 = BuildUniqueString();
            var ns2 = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns1, new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns2, new PutTagNsDefinitionRequest()));

            var account = BuildUniqueString();

            var tags1 = new List<string>();
            for (int i = 0; i < numberOfTags; i++)
                tags1.Add(BuildUniqueString());

            var tags2 = new List<string>();
            for (int i = 0; i < numberOfTags; i++)
                tags2.Add(BuildUniqueString());

            var tagWeights1 = tags1.Select(t => new TagWeightOnAccount
            {
                Tag = t,
                Weight = t[0]
            }).ToList();

            var tagWeights2 = tags2.Select(t => new TagWeightOnAccount
            {
                Tag = t,
                Weight = t[0] * 2
            }).ToList();

            CheckSuccess(await Client.Accounts.PutAndReplaceAccountTagsInNs(account, ns1,
                new PutAndReplaceAccountTagsInNsRequest { Tags = tagWeights1 }));
            CheckSuccess(await Client.Accounts.PutAndReplaceAccountTagsInNs(account, ns2,
                new PutAndReplaceAccountTagsInNsRequest { Tags = tagWeights2 }));

            var returnedTags1 = await Client.Accounts.GetTagsOfAccountInNs(account, ns1);
            Assert.AreEqual(numberOfTags, returnedTags1.Tags.Count);

            returnedTags1.Tags.ForEach(tw =>
            {
                var tag = tagWeights1.SingleOrDefault(t => t.Tag == tw.Tag);
                Assert.IsNotNull(tag);
                Assert.AreEqual(tag.Weight, tw.Weight);
            });

            tags2.ForEach(t =>
            {
                Assert.IsTrue(returnedTags1.Tags.All(tt => !tt.Tag.Equals(t)));
            });
        }
    }
}
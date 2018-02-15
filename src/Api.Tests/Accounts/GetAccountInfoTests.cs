using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class GetAccountInfoTests : TestClassBase
    {
        [TestMethod]
        public async Task UnknownAccountReturnsNull()
        {
            var account = BuildUniqueString();

            Assert.IsNull(await Client.Accounts.GetAccountInfo(account));
        }

        [TestMethod]
        public async Task KnownAccountShouldReturnResults()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, TestNs, TestTag));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.IsNotNull(accountInfo);
        }

        [TestMethod]
        public async Task EmptiedAccountShouldBeKnown()
        {
            var account = BuildUniqueString();
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, ns, TestTag));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(account, ns, TestTag));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.IsNotNull(accountInfo);
        }

        [TestMethod]
        public async Task EmptiedAccountDataShouldBeCorrect()
        {
            var account = BuildUniqueString();
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, ns, TestTag));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(account, ns, TestTag));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.IsNotNull(accountInfo);
            Assert.IsNotNull(accountInfo.AccountId);
            Assert.IsNotNull(accountInfo.Fields);
            Assert.IsNotNull(accountInfo.TagNamespaces);

            Assert.AreEqual(account, accountInfo.AccountId);
            Assert.AreEqual(0, accountInfo.Fields.Count);
            Assert.AreEqual(0, accountInfo.TagNamespaces.Count);
        }

        [TestMethod]
        public async Task TestSingleTagData()
        {
            var account = BuildUniqueString();
            var ns = BuildUniqueString();
            var tag = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, ns, tag));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            
            Assert.AreEqual(1, accountInfo.TagNamespaces.Count);
            Assert.IsNotNull(accountInfo.TagNamespaces[0]);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Namespace);
            Assert.AreEqual(ns, accountInfo.TagNamespaces[0].Namespace);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags);
            Assert.AreEqual(1, accountInfo.TagNamespaces[0].Tags.Count);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags[0]);
            Assert.AreEqual(tag, accountInfo.TagNamespaces[0].Tags[0].Tag);
            Assert.AreNotEqual(0d, accountInfo.TagNamespaces[0].Tags[0].Weight);
        }

        [TestMethod]
        public async Task TestManyTagsAndWeightsInSingleNamespace()
        {
            var numberOfTags = 200;

            var account = BuildUniqueString();
            var tags = Enumerable.Range(0, numberOfTags)
                .Select(i => new TagWeightOnAccount
                {
                    Tag = BuildUniqueString(),
                    Weight = i + 10
                }).ToList();

            CheckSuccess(await Client.Accounts.PutAndReplaceAccountTagsInNs(account, TestNs,
                new PutAndReplaceAccountTagsInNsRequest() {Tags = tags}));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);

            Assert.AreEqual(1, accountInfo.TagNamespaces.Count);
            Assert.AreEqual(numberOfTags, accountInfo.TagNamespaces[0].Tags.Count);

            accountInfo.TagNamespaces[0].Tags.ForEach(tw =>
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
            var ns3 = BuildUniqueString();

            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns1, new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns2, new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns3, new PutTagNsDefinitionRequest()));

            var account = BuildUniqueString();
            var tags = Enumerable.Range(0, numberOfTags).Select(i => BuildUniqueString()).ToList();

            var tagWeights1 = tags.Select(t => new TagWeightOnAccount
                {
                    Tag = t,
                    Weight = t[0]
                }).ToList();

            var tagWeights2 = tags.Select(t => new TagWeightOnAccount
                {
                    Tag = t,
                    Weight = t[0] * 2
                }).ToList();

            var tagWeights3 = tags.Select(t => new TagWeightOnAccount
                {
                    Tag = t,
                    Weight = t[0] * 3
                }).ToList();

            CheckSuccess(await Client.Accounts.PutAndReplaceAccountTagsInNs(account, ns1,
                new PutAndReplaceAccountTagsInNsRequest {Tags = tagWeights1}));

            await Task.WhenAll(tagWeights2.Select(async tw =>
            {
                CheckSuccess(await Client.Accounts.PatchTagWeightOnAccount(account, ns2, tw.Tag, tw.Weight));
            }));

            CheckSuccess(await Client.Accounts.PutAccountChange(account, new PutAccountChangeRequest
            {
                TagChanges = tagWeights3.Select(tw => new AccountTagChangeInstruction
                    {
                        Tag = tw.Tag,
                        TagNs = ns3,
                        Weight = tw.Weight
                    })
                    .ToList()
            }));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.AreEqual(3, accountInfo.TagNamespaces.Count);
            Assert.AreEqual(numberOfTags, accountInfo.TagNamespaces[0].Tags.Count);
            Assert.AreEqual(numberOfTags, accountInfo.TagNamespaces[1].Tags.Count);
            Assert.AreEqual(numberOfTags, accountInfo.TagNamespaces[2].Tags.Count);

            var resultNs1 = accountInfo.TagNamespaces.Single(tn => tn.Namespace == ns1);
            var resultNs2 = accountInfo.TagNamespaces.Single(tn => tn.Namespace == ns2);
            var resultNs3 = accountInfo.TagNamespaces.Single(tn => tn.Namespace == ns3);

            resultNs1.Tags.ForEach(tw =>
            {
                var tag = tagWeights1.SingleOrDefault(t => t.Tag == tw.Tag);
                Assert.IsNotNull(tag);
                Assert.AreEqual(tag.Weight, tw.Weight);
            });
            resultNs2.Tags.ForEach(tw =>
            {
                var tag = tagWeights2.SingleOrDefault(t => t.Tag == tw.Tag);
                Assert.IsNotNull(tag);
                Assert.AreEqual(tag.Weight, tw.Weight);
            });
            resultNs3.Tags.ForEach(tw =>
            {
                var tag = tagWeights3.SingleOrDefault(t => t.Tag == tw.Tag);
                Assert.IsNotNull(tag);
                Assert.AreEqual(tag.Weight, tw.Weight);
            });
        }
    }
}
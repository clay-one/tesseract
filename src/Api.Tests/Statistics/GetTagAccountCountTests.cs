using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Statistics
{
    [TestClass]
    public class GetTagAccountCountTests : TestClassBase
    {
        [TestMethod]
        public async Task NonExistingTagShouldReturnZero()
        {
            var tag = BuildUniqueString();
            var result = CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, tag));

            Assert.IsNotNull(result);
            Assert.AreEqual(tag, result.Tag);
            Assert.AreEqual(TestNs, result.TagNs);
            Assert.AreEqual(0, result.TotalCount);
        }

        [TestMethod]
        public async Task SingleAccount()
        {
            var ns = BuildUniqueString();
            var tag = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));

            RefreshIndex();
            var result = CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag));

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
        }

        [TestMethod]
        public async Task AccountInOtherTagsSameNs()
        {
            var ns = BuildUniqueString();
            var tag = BuildUniqueString();
            var otherTag = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, otherTag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, otherTag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, otherTag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, otherTag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, otherTag));

            RefreshIndex();
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag)).TotalCount);

            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));

            RefreshIndex();
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag)).TotalCount);
        }

        [TestMethod]
        public async Task AccountInSameTagOtherNs()
        {
            var ns = BuildUniqueString();
            var otherNs = BuildUniqueString();
            var tag = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Tags.PutTagNsDefinition(otherNs, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), otherNs, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), otherNs, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), otherNs, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), otherNs, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), otherNs, tag));

            RefreshIndex();
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag)).TotalCount);

            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));

            RefreshIndex();
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag)).TotalCount);
        }

        [TestMethod]
        public async Task MultipleAccounts()
        {
            var tag = BuildUniqueString();
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));

            RefreshIndex();
            var result = CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag));

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.TotalCount);
        }

        [TestMethod]
        public async Task RemoveSingleAccounts()
        {
            var tag = BuildUniqueString();
            var ns = BuildUniqueString();
            var accountId = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns, tag));

            RefreshIndex();
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag)).TotalCount);

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountId, ns, tag));

            RefreshIndex();
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag)).TotalCount);
        }

        [TestMethod]
        public async Task RemoveSomeAccounts()
        {
            var tag = BuildUniqueString();
            var ns = BuildUniqueString();
            var accountId = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, tag));

            RefreshIndex();
            Assert.AreEqual(6, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag)).TotalCount);

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountId, ns, tag));

            RefreshIndex();
            Assert.AreEqual(5, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag)).TotalCount);
        }

        [TestMethod]
        public async Task RemoveAllAccounts()
        {
            var tag = BuildUniqueString();
            var ns = BuildUniqueString();
            var accountIds = Enumerable.Range(1, 5).Select(i => BuildUniqueString()).ToArray();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            await Task.WhenAll(accountIds.Select(async accountId => CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns, tag))));

            RefreshIndex();
            Assert.AreEqual(5, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag)).TotalCount);

            await Task.WhenAll(accountIds.Select(async accountId => CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountId, ns, tag))));

            RefreshIndex();
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag)).TotalCount);
        }

        [TestMethod]
        public async Task RemoveSomeTags()
        {
            var tag1 = BuildUniqueString();
            var tag2 = BuildUniqueString();
            var accountId1 = BuildUniqueString();
            var accountId2 = BuildUniqueString();
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId1, ns, tag1));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId2, ns, tag1));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId1, ns, tag2));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId2, ns, tag2));

            RefreshIndex();
            Assert.AreEqual(2, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag1)).TotalCount);
            Assert.AreEqual(2, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag2)).TotalCount);

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountId1, ns, tag1));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountId2, ns, tag2));

            RefreshIndex();
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag1)).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(ns, tag2)).TotalCount);
        }

    }
}
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Statistics
{
    [TestClass]
    public class GetTagNsAccountCountTests : TestClassBase
    {
        [TestMethod]
        public async Task NonExistingNsShouldReturnZero()
        {
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            var result = CheckSuccess(await Client.Statistics.GetTagNsAccountCount(ns));

            Assert.IsNotNull(result);
            Assert.AreEqual(ns, result.TagNs);
            Assert.AreEqual(0, result.TotalCount);
        }

        [TestMethod]
        public async Task SingleAccountInSingleTag()
        {
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, BuildUniqueString()));

            RefreshIndex();
            var result = CheckSuccess(await Client.Statistics.GetTagNsAccountCount(ns));

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
        }

        [TestMethod]
        public async Task SingleAccountInMultipleTags()
        {
            var accountId = BuildUniqueString();
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns, BuildUniqueString()));

            RefreshIndex();
            var result = CheckSuccess(await Client.Statistics.GetTagNsAccountCount(ns));

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
        }

        [TestMethod]
        public async Task SingleAccountInDifferentNs()
        {
            var accountId = BuildUniqueString();
            var ns1 = BuildUniqueString();
            var ns2 = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns1, new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns2, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns1, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns1, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns2, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns2, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns2, BuildUniqueString()));

            RefreshIndex();
            var result = CheckSuccess(await Client.Statistics.GetTagNsAccountCount(ns1));

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
        }

        [TestMethod]
        public async Task MultipleAccountsInSameTag()
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
            var result = CheckSuccess(await Client.Statistics.GetTagNsAccountCount(ns));

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.TotalCount);
        }

        [TestMethod]
        public async Task MultipleAccountsInDifferentTags()
        {
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, BuildUniqueString()));

            RefreshIndex();
            var result = CheckSuccess(await Client.Statistics.GetTagNsAccountCount(ns));

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.TotalCount);
        }

        [TestMethod]
        public async Task RemoveSomeAccounts()
        {
            var ns = BuildUniqueString();
            var tag = BuildUniqueString();
            var accountId = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns, tag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), ns, BuildUniqueString()));

            RefreshIndex();
            Assert.AreEqual(6, CheckSuccess(await Client.Statistics.GetTagNsAccountCount(ns)).TotalCount);

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountId, ns, tag));

            RefreshIndex();
            Assert.AreEqual(5, CheckSuccess(await Client.Statistics.GetTagNsAccountCount(ns)).TotalCount);
        }

        [TestMethod]
        public async Task RemoveAllAccounts()
        {
            var ns = BuildUniqueString();
            var tags = Enumerable.Range(1, 5).Select(i => BuildUniqueString()).ToArray();
            var accountIds = Enumerable.Range(1, 5).Select(i => BuildUniqueString()).ToArray();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest()));

            await Task.WhenAll(accountIds.Select(async accountId =>
            {
                await Task.WhenAll(tags.Select(
                    async tag => CheckSuccess(await Client.Accounts.PutTagOnAccount(accountId, ns, tag))));

            }));

            RefreshIndex();
            Assert.AreEqual(5, CheckSuccess(await Client.Statistics.GetTagNsAccountCount(ns)).TotalCount);

            await Task.WhenAll(accountIds.Select(async accountId =>
            {
                await Task.WhenAll(tags.Select(
                    async tag => CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountId, ns, tag))));

            }));

            RefreshIndex();
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagNsAccountCount(ns)).TotalCount);
        }
    }
}
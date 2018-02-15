using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class DeleteTagNsFromAccountTests : TestClassBase
    {
        [TestMethod]
        public async Task DeleteNonExistantNsShouldSucceed()
        {
            var account = BuildUniqueString();
            var tagNs = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(tagNs, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.DeleteTagNsFromAccount(TestAccount, tagNs));
            CheckSuccess(await Client.Accounts.DeleteTagNsFromAccount(account, tagNs));
        }

        [TestMethod]
        public async Task DeletedNsShouldHaveZeroWeight()
        {
            // First, make sure tag exists on account
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, TestTag));
            Assert.AreNotEqual(0.0d,
                (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);

            // Then delete tag and make sure the weight has become zero
            CheckSuccess(await Client.Accounts.DeleteTagNsFromAccount(TestAccount, TestNs));
            Assert.AreEqual(0.0d,
                (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);
        }

        [TestMethod]
        public async Task DeletedNsShouldNotAppearInAccountInfo()
        {
            // First, make sure tag exists on account
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, TestTag));
            Assert.AreNotEqual(0.0d,
                (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);

            CheckSuccess(await Client.Accounts.DeleteTagNsFromAccount(TestAccount, TestNs));
            var account = await Client.Accounts.GetAccountInfo(TestAccount);

            var returnedNs = account.TagNamespaces.SingleOrDefault(ns => ns.Namespace == TestNs);
            Assert.IsNull(returnedNs);
        }

        [TestMethod]
        public async Task DeleteNsWithManyTags()
        {
            var randomNs = BuildUniqueString();
            var count = 50;

            CheckSuccess(await Client.Tags.PutTagNsDefinition(randomNs, new PutTagNsDefinitionRequest()));

            // Add many random tags to the NS
            for (int i = 0; i < count; i++)
                CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, randomNs, BuildUniqueString()));

            var account = await Client.Accounts.GetAccountInfo(TestAccount);
            var returnedNs = account.TagNamespaces.SingleOrDefault(ns => ns.Namespace == randomNs);
            Assert.IsNotNull(returnedNs);
            Assert.AreEqual(count, returnedNs.Tags.Count);

            // Then remove the NS and check
            CheckSuccess(await Client.Accounts.DeleteTagNsFromAccount(TestAccount, randomNs));
            account = await Client.Accounts.GetAccountInfo(TestAccount);
            returnedNs = account.TagNamespaces.SingleOrDefault(ns => ns.Namespace == randomNs);
            Assert.IsNull(returnedNs);
        }
    }
}
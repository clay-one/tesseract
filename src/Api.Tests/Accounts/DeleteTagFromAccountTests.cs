using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class DeleteTagFromAccountTests : TestClassBase
    {
        [TestMethod]
        public async Task DeleteNonExistantTagShouldSucceed()
        {
            var account = BuildUniqueString();
            var tagNs = BuildUniqueString();
            var tag = BuildUniqueString();

            // Define required namespaces
            CheckSuccess(await Client.Tags.PutTagNsDefinition(tagNs, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, tag));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, tagNs, tag));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(account, tagNs, tag));
        }

        [TestMethod]
        public async Task DeletedTagShouldHaveZeroWeight()
        {
            // First, make sure tag exists on account
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, TestTag));
            Assert.AreNotEqual(0.0d,
                (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);

            // Then delete tag and make sure the weight has become zero
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, TestTag));
            Assert.AreEqual(0.0d,
                (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);
        }

        [TestMethod]
        public async Task DeletedTagShouldNotAppearInAccountInfo()
        {
            var randomTag = BuildUniqueString();

            // First, make sure tag exists on account
            // Add a random other tag, to make sure the namsepace doesn't become empty after removing the tag
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, TestTag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, randomTag));
            Assert.AreNotEqual(0.0d,
                (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, TestTag));
            var account = await Client.Accounts.GetAccountInfo(TestAccount);

            var returnedNs = account.TagNamespaces.SingleOrDefault(ns => ns.Namespace == TestNs);
            Assert.IsNotNull(returnedNs);
            Assert.IsFalse(returnedNs.Tags.Any(tw => tw.Tag == TestTag));
            Assert.IsTrue(returnedNs.Tags.Any(tw => tw.Tag == randomTag));
        }

        [TestMethod]
        public async Task EmptiedNamespaceShouldNotAppearInAccountInfo()
        {
            var randomNs = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(randomNs, new PutTagNsDefinitionRequest()));

            // First, make sure a single-tag NS exists on account
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, randomNs, TestTag));
            Assert.AreNotEqual(0.0d,
                (await Client.Accounts.GetTagWeightOnAccount(TestAccount, randomNs, TestTag)).Weight);

            // Then delete tag and make sure the weight has become zero
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, randomNs, TestTag));
            var account = await Client.Accounts.GetAccountInfo(TestAccount);

            var returnedNs = account.TagNamespaces.SingleOrDefault(ns => ns.Namespace == randomNs);
            Assert.IsNull(returnedNs);
        }
    }
}
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class GetTagWeightOnAccountTests : TestClassBase
    {
        [TestMethod]
        public async Task ResponseDataShouldMatchInput()
        {
            var account = BuildUniqueString();
            var tagNs = BuildUniqueString();
            var tag = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(tagNs, new PutTagNsDefinitionRequest()));

            var response = await Client.Accounts.GetTagWeightOnAccount(account, tagNs, tag);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.AccountId, account);
            Assert.AreEqual(response.TagNs, tagNs);
            Assert.AreEqual(response.Tag, tag);
        }

        [TestMethod]
        public async Task NonExistantTagShouldBeZero()
        {
            var tag = BuildUniqueString();

            var response = await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, tag);
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Weight, 0d);
        }

        [TestMethod]
        public async Task ExistingTagShouldBeNonZero()
        {
            // Make sure test tag exists on test account
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, TestTag));

            var response = await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag);
            Assert.IsNotNull(response);
            Assert.AreNotEqual(response.Weight, 0d);
        }
    }
}
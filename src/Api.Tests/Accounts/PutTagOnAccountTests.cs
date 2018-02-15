using System.Threading.Tasks;
using Tesseract.ApiModel.Tags;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class PutTagOnAccountTests : TestClassBase
    {
        [TestMethod]
        public async Task NewTagShouldBeSetToOne()
        {
            var tag = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, tag));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, tag);
            Assert.IsNotNull(weightResponse);
            Assert.AreEqual(weightResponse.Weight, 1.0d);
        }

        [TestMethod]
        public async Task ExistingTagShouldNotBeChanged()
        {
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(TestAccount, TestNs, TestTag, 25.0d));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, TestTag));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag);
            Assert.IsNotNull(weightResponse);
            Assert.AreEqual(weightResponse.Weight, 25.0d);
        }

        [TestMethod]
        public async Task ShouldAcceptArbitraryInput()
        {
            var account = BuildUniqueString();
            var tagNs = BuildUniqueString();
            var tag = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(tagNs, new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, tagNs, tag));

            var weightResponse = await Client.Accounts.GetTagWeightOnAccount(account, tagNs, tag);
            Assert.IsNotNull(weightResponse);
            Assert.AreEqual(weightResponse.Weight, 1.0d);
        }
    }
}
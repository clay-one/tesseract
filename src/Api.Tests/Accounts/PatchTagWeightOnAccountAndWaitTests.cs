using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class PatchTagWeightOnAccountAndWaitTests : TestClassBase
    {
        [TestMethod]
        public async Task PatchSingleTagWeight()
        {
            var account = BuildUniqueString();

            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 10d));
            var result = await Client.Accounts.PatchTagWeightOnAccountAndWait(account, TestNs, TestTag, 15d);

            CheckSuccess(result);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(account, result.Result.AccountId);
            Assert.AreEqual(TestNs, result.Result.TagNs);
            Assert.AreEqual(TestTag, result.Result.Tag);
            Assert.AreEqual(25d, result.Result.Weight);
        }

        [TestMethod]
        public async Task RemovedTagShouldReturnAsZero()
        {
            var account = BuildUniqueString();

            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 10d));
            var result = await Client.Accounts.PatchTagWeightOnAccountAndWait(account, TestNs, TestTag, -15d);

            CheckSuccess(result);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(account, result.Result.AccountId);
            Assert.AreEqual(TestNs, result.Result.TagNs);
            Assert.AreEqual(TestTag, result.Result.Tag);
            Assert.AreEqual(0d, result.Result.Weight);
        }
    }
}
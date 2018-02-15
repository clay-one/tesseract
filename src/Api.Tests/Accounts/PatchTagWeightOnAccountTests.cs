using System;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class PatchTagWeightOnAccountTests : TestClassBase
    {
        [TestMethod]
        public async Task PatchSingleTagWeight()
        {
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(TestAccount, TestNs, TestTag, 10d));
            CheckSuccess(await Client.Accounts.PatchTagWeightOnAccount(TestAccount, TestNs, TestTag, 15d));

            Assert.AreEqual(25d, (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);
        }

        [TestMethod]
        public async Task PatchToZeroShouldRemoveTag()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 17d));
            CheckSuccess(await Client.Accounts.PatchTagWeightOnAccount(account, TestNs, TestTag, -17d));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.AreEqual(0, accountInfo.TagNamespaces.Count);
        }

        [TestMethod]
        public async Task PatchToNegativeShouldBehaveAsZero()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 23d));
            CheckSuccess(await Client.Accounts.PatchTagWeightOnAccount(account, TestNs, TestTag, -1000d));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.AreEqual(0, accountInfo.TagNamespaces.Count);
        }

        [TestMethod]
        public async Task PatchNonExistantAccountShouldCreateIt()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PatchTagWeightOnAccount(account, TestNs, TestTag, 5d));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.IsNotNull(accountInfo);

            Assert.IsNotNull(accountInfo.TagNamespaces);
            Assert.AreEqual(1, accountInfo.TagNamespaces.Count);
            Assert.IsNotNull(accountInfo.TagNamespaces[0]);
            Assert.AreEqual(TestNs, accountInfo.TagNamespaces[0].Namespace);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags);
            Assert.AreEqual(1, accountInfo.TagNamespaces[0].Tags.Count);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags[0]);
            Assert.AreEqual(TestTag, accountInfo.TagNamespaces[0].Tags[0].Tag);
            Assert.AreEqual(5d, accountInfo.TagNamespaces[0].Tags[0].Weight);
        }

        [TestMethod]
        public async Task PatchWithZeroDeltaShouldNotCreateAccount()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PatchTagWeightOnAccount(account, TestNs, TestTag, 0d));

            try
            {
                await Client.Accounts.GetAccountInfo(account);
                Assert.Fail("Exception should have been thrown as a result of 404 status code, but didn't.");
            }
            catch (Exception )
            {
                // The exception should be thrown, this is the expected result.
            }
        }
    }
}
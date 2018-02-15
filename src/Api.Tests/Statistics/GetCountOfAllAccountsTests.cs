using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Statistics
{
    [TestClass]
    public class GetCountOfAllAccountsTests : TestClassBase
    {
        [TestMethod]
        public async Task FreshDatabase()
        {
            var countResponse = await Client.Statistics.GetCountOfAllAccounts();
            CheckSuccess(countResponse);
            
            Assert.IsNotNull(countResponse.Result);
            Assert.AreEqual(0L, countResponse.Result.Count);
        }

        public async Task AddingSingleAccount()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, TestTag));
            RefreshIndex();
            
            Assert.AreEqual(1L, (await Client.Statistics.GetCountOfAllAccounts()).Result.Count);
        }

        public async Task AddingSingleAccountMultipleTimes()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, BuildUniqueString()));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, BuildUniqueString()));
            RefreshIndex();
            
            Assert.AreEqual(1L, (await Client.Statistics.GetCountOfAllAccounts()).Result.Count);
        }
        
        public async Task AddingDifferentAccounts()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), TestNs, TestTag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), TestNs, TestTag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), TestNs, TestTag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), TestNs, TestTag));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), TestNs, TestTag));
            RefreshIndex();
            
            Assert.AreEqual(5L, (await Client.Statistics.GetCountOfAllAccounts()).Result.Count);
        }

        
    }
}
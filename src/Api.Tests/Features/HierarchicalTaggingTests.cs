using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Features
{
    [TestClass]
    public class HierarchicalTaggingTests : TestClassBase
    {
        [TestMethod]
        public async Task SingleLevelHierarchyTest()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:b"));
            RefreshIndex();
            
            var result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, "a"));
            Assert.AreEqual(1, result.Accounts.Count);
            Assert.AreEqual(1, result.TotalNumberOfResults);
            Assert.AreEqual(TestAccount, result.Accounts[0].AccountId);
            
            result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, "a:b"));
            Assert.AreEqual(1, result.Accounts.Count);
            Assert.AreEqual(1, result.TotalNumberOfResults);
            Assert.AreEqual(TestAccount, result.Accounts[0].AccountId);
            
            result = CheckSuccess(await Client.Tags.GetTagAccountList(TestNs, "b"));
            Assert.AreEqual(0, result.TotalNumberOfResults);
        }

        [TestMethod]
        public async Task MultiLevelHierarchyTest()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:b:c:d"));
            RefreshIndex();

            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b:c:d")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b:c")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a")).TotalCount);

            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "b")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "b:c")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "b:c:d")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "c")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "c:d")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "d")).TotalCount);
        }
        
        [TestMethod]
        public async Task SingleAccountMultipleBranchingTest()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:b:x"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:b:y"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:b:z"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:c:x"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:c:y"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:c:z"));
            RefreshIndex();
            
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:c")).TotalCount);

            var accountInfo = await Client.Accounts.GetAccountInfo(TestAccount);
            
            Assert.IsNotNull(accountInfo.TagNamespaces);
            Assert.AreEqual(1, accountInfo.TagNamespaces.Count);
            Assert.IsNotNull(accountInfo.TagNamespaces[0]);
            Assert.AreEqual(TestNs, accountInfo.TagNamespaces[0].Namespace);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags);
            Assert.AreEqual(6, accountInfo.TagNamespaces[0].Tags.Count);
            
            Assert.IsFalse(accountInfo.TagNamespaces[0].Tags.Any(t => t.Tag == "a"));
            Assert.IsFalse(accountInfo.TagNamespaces[0].Tags.Any(t => t.Tag == "a:b"));
            Assert.IsTrue(accountInfo.TagNamespaces[0].Tags.Any(t => t.Tag == "a:b:x"));
        }
        
        [TestMethod]
        public async Task MultipleAccountMultipleBranchingTest()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), TestNs, "a:b:x"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), TestNs, "a:b:y"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), TestNs, "a:b:z"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), TestNs, "a:c:x"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), TestNs, "a:c:y"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(BuildUniqueString(), TestNs, "a:c:z"));
            RefreshIndex();
            
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b:x")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b:y")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b:z")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:c:x")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:c:y")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:c:z")).TotalCount);
            Assert.AreEqual(3, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(3, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:c")).TotalCount);
            Assert.AreEqual(6, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a")).TotalCount);
        }
        
        [TestMethod]
        public async Task RemoveHierarchyTest()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:b:c:d"));
            RefreshIndex();

            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b:c:d")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b:c")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a")).TotalCount);

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, "a"));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, "a:b"));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, "a:b:c"));
            RefreshIndex();
            
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b:c:d")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b:c")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a")).TotalCount);

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, "a:b:c:d"));
            RefreshIndex();
            
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b:c:d")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b:c")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a")).TotalCount);
        }
        
        [TestMethod]
        public async Task RemoveFromSingleAccountTest()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:b:x"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:b:y"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:b:z"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:c:x"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:c:y"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, "a:c:z"));
            RefreshIndex();
            
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:c")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a")).TotalCount);

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, "a:b:x"));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, "a:b:y"));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, "a:b:z"));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, "a:c:x"));
            RefreshIndex();
            
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:c")).TotalCount);
            Assert.AreEqual(1, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a")).TotalCount);

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, "a:c:y"));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(TestAccount, TestNs, "a:c:z"));
            RefreshIndex();

            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:c")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a")).TotalCount);
        }
        
        [TestMethod]
        public async Task RemoveFromMultipleAccountsTest()
        {
            var accountIds = Enumerable.Range(1, 6).Select(i => BuildUniqueString()).ToList();
            
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountIds[0], TestNs, "a:b:x"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountIds[1], TestNs, "a:b:y"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountIds[2], TestNs, "a:b:z"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountIds[3], TestNs, "a:c:x"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountIds[4], TestNs, "a:c:y"));
            CheckSuccess(await Client.Accounts.PutTagOnAccount(accountIds[5], TestNs, "a:c:z"));
            RefreshIndex();
            
            Assert.AreEqual(3, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(3, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:c")).TotalCount);
            Assert.AreEqual(6, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a")).TotalCount);

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountIds[0], TestNs, "a:b:x"));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountIds[1], TestNs, "a:b:y"));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountIds[2], TestNs, "a:b:z"));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountIds[3], TestNs, "a:c:x"));
            RefreshIndex();
            
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(2, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:c")).TotalCount);
            Assert.AreEqual(2, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a")).TotalCount);

            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountIds[4], TestNs, "a:c:y"));
            CheckSuccess(await Client.Accounts.DeleteTagFromAccount(accountIds[5], TestNs, "a:c:z"));
            RefreshIndex();

            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:b")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a:c")).TotalCount);
            Assert.AreEqual(0, CheckSuccess(await Client.Statistics.GetTagAccountCount(TestNs, "a")).TotalCount);
        }
        
 
        
    }
}
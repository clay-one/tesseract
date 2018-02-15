using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.Core.JobTypes.AccountIndexing;
using Tesseract.Core.Queue;
using Tesseract.Core.Queue.Implementation;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class MarkAccountForReindexingTests : TestClassBase
    {
        protected override void ApplyCompositionForTest()
        {
            Composer.Register(typeof(InMemoryJobQueue<AccountIndexingStep>));
        }

        [TestMethod]
        public async Task ArbitraryAccountShouldBeAddedToQueue()
        {
            var queueLength = await Composer.GetComponent<IJobQueue<AccountIndexingStep>>().GetQueueLength();
            Assert.AreEqual(0L, queueLength);
            
            CheckSuccess(await Client.Accounts.MarkAccountForReindexing(BuildUniqueString()));

            queueLength = await Composer.GetComponent<IJobQueue<AccountIndexingStep>>().GetQueueLength();
            Assert.AreEqual(1L, queueLength);
        }
        
    }
}
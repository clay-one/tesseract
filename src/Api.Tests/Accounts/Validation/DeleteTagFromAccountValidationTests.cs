using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts.Validation
{
    [TestClass]
    public class DeleteTagFromAccountValidationTests : TestClassBase
    {
        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            CheckFailure(
                await Client.Accounts.DeleteTagFromAccount(TestAccount, BuildUniqueString(), TestTag),
                HttpStatusCode.BadRequest);
        }
    }
}
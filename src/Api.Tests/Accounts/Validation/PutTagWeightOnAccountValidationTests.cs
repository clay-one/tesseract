using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts.Validation
{
    [TestClass]
    public class PutTagWeightOnAccountValidationTests : TestClassBase
    {
        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            CheckFailure(
                await Client.Accounts.PutTagWeightOnAccount(TestAccount, BuildUniqueString(), TestTag, 5d),
                HttpStatusCode.BadRequest);
        }
    }
}
using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts.Validation
{
    [TestClass]
    public class PatchTagWeightOnAccountAndWaitValidationTests : TestClassBase
    {
        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            CheckFailure(
                await Client.Accounts.PatchTagWeightOnAccountAndWait(TestAccount, BuildUniqueString(), TestTag, 1d),
                HttpStatusCode.BadRequest);
        }
    }
}
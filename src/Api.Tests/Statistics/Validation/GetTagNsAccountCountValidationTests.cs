using System;
using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Statistics.Validation
{
    [TestClass]
    public class GetTagNsAccountCountValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullTagNsThrows()
        {
            await Client.Statistics.GetTagNsAccountCount(null);
        }

        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            CheckFailure(
                await Client.Statistics.GetTagNsAccountCount(BuildUniqueString()), 
                HttpStatusCode.BadRequest);
        }
    }
}
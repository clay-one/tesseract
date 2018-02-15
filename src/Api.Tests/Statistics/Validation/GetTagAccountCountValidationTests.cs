using System;
using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Statistics.Validation
{
    [TestClass]
    public class GetTagAccountCountValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullTagNsThrows()
        {
            await Client.Statistics.GetTagAccountCount(null, BuildUniqueString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullTagThrows()
        {
            await Client.Statistics.GetTagAccountCount(BuildUniqueString(), null);
        }

        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            CheckFailure(
                await Client.Statistics.GetTagAccountCount(BuildUniqueString(), TestTag), 
                HttpStatusCode.BadRequest);
        }
    }
}
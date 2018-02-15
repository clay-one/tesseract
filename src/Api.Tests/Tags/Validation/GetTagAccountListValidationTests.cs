using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Tags.Validation
{
    [TestClass]
    public class GetTagAccountListValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullTagNsThrows()
        {
            await Client.Tags.GetTagAccountList(null, BuildUniqueString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullTagThrows()
        {
            await Client.Tags.GetTagAccountList(BuildUniqueString(), null);
        }

        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            var result = await Client.Tags.GetTagAccountList(BuildUniqueString(), TestTag);
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Any());
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        public async Task NegativeCountIsRejected()
        {
            CheckFailure(await Client.Tags.GetTagAccountList(TestNs, BuildUniqueString(), -10), HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task LargeCountIsRejected()
        {
            CheckFailure(await Client.Tags.GetTagAccountList(TestNs, BuildUniqueString(), 1001), HttpStatusCode.BadRequest);
        }

    }
}
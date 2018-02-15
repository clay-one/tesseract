using System;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts.Validation
{
    [TestClass]
    public class GetTagsOfAccountInNsValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UndefinedNsShouldBeRejected()
        {
            await Client.Accounts.GetTagsOfAccountInNs(TestAccount, BuildUniqueString());
        }
    }
}
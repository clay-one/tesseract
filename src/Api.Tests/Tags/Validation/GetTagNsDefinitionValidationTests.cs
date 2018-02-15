using System;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Tags.Validation
{
    [TestClass]
    public class GetTagNsDefinitionValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullNsThrows()
        {
            await Client.Tags.GetTagNsDefinition(null);
        }

    }
}
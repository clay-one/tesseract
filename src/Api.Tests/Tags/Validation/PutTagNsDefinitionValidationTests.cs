using System;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Tags.Validation
{
    [TestClass]
    public class PutTagNsDefinitionValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullNsThrows()
        {
            await Client.Tags.PutTagNsDefinition(null, new PutTagNsDefinitionRequest());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullRequestThrows()
        {
            await Client.Tags.PutTagNsDefinition(TestNs, null);
        }

    }
}
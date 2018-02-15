using System;
using System.Threading.Tasks;
using Tesseract.ApiModel.Fields;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Fields.Validation
{
    [TestClass]
    public class PutFieldDefinitionValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullFieldNameThrows()
        {
            await Client.Fields.PutFieldDefinition(null, new PutFieldDefinitionRequest());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullRequestThrows()
        {
            await Client.Fields.PutFieldDefinition(TestField, null);
        }
    }
}
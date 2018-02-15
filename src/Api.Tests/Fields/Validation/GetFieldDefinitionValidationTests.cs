using System;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Fields.Validation
{
    [TestClass]
    public class GetFieldDefinitionValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullFieldNameThrows()
        {
            await Client.Fields.GetFieldDefinition(null);
        }

    }
}
using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Fields;

namespace Appson.Tesseract.Tests.Api.Fields
{
    [TestClass]
    public class PutFieldDefinitionTests : TestClassBase
    {
        [TestMethod]
        public async Task MultipleCallsWithSameInputIsOkay()
        {
            var field = BuildUniqueString();
            var putFieldDefinitionRequest = new PutFieldDefinitionRequest
            {
                KeepHistory = true
            };

            CheckSuccess(await Client.Fields.PutFieldDefinition(field, putFieldDefinitionRequest));
            CheckSuccess(await Client.Fields.PutFieldDefinition(field, putFieldDefinitionRequest));
            CheckSuccess(await Client.Fields.PutFieldDefinition(field, putFieldDefinitionRequest));
            CheckSuccess(await Client.Fields.PutFieldDefinition(field, putFieldDefinitionRequest));
            CheckSuccess(await Client.Fields.PutFieldDefinition(field, putFieldDefinitionRequest));
        }

        [TestMethod]
        public async Task NewlyCreatedNsAppearsInOutput()
        {
            var field = BuildUniqueString();
            var putFieldDefinitionRequest = new PutFieldDefinitionRequest
            {
                KeepHistory = false
            };

            CheckSuccess(await Client.Fields.PutFieldDefinition(field, putFieldDefinitionRequest));

            Assert.IsNotNull(await Client.Fields.GetFieldDefinition(field));
            Assert.IsTrue((await Client.Fields.GetFieldDefinitionList()).Fields.Any(i => i.FieldName == field));
        }
    }
}
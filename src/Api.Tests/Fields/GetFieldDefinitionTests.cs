using System.Threading.Tasks;
using Tesseract.ApiModel.Fields;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Fields
{
    [TestClass]
    public class GetFieldDefinitionTests : TestClassBase
    {
        [TestMethod]
        public async Task UnknownFieldReturnedAsNull()
        {
            var fieldDef = await Client.Fields.GetFieldDefinition(BuildUniqueString());
            Assert.IsNull(fieldDef);
        }

        [TestMethod]
        public async Task KnownFieldReturnsResults()
        {
            CheckSuccess(await Client.Fields.PutFieldDefinition(TestField, new PutFieldDefinitionRequest()));

            var fieldDef = await Client.Fields.GetFieldDefinition(TestField);
            Assert.IsNotNull(fieldDef);
            Assert.IsNotNull(fieldDef.FieldName);
            Assert.AreEqual(TestField, fieldDef.FieldName);
        }

        [TestMethod]
        public async Task KeepHistoryFieldIsStored()
        {
            var field1 = BuildUniqueString();
            var field2 = BuildUniqueString();

            CheckSuccess(await Client.Fields.PutFieldDefinition(field1, new PutFieldDefinitionRequest
            {
                KeepHistory = true
            }));
            CheckSuccess(await Client.Fields.PutFieldDefinition(field2, new PutFieldDefinitionRequest
            {
                KeepHistory = false
            }));

            Assert.IsTrue((await Client.Fields.GetFieldDefinition(field1)).KeepHistory);
            Assert.IsFalse((await Client.Fields.GetFieldDefinition(field2)).KeepHistory);
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using Tesseract.ApiModel.Fields;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Fields
{
    [TestClass]
    public class GetFieldDefinitionListTests : TestClassBase
    {
        [TestMethod]
        public async Task CallReturnsNonNullResults()
        {
            var result = await Client.Fields.GetFieldDefinitionList();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Fields);
        }

        [TestMethod]
        public async Task ListGrowsWhenNewFieldIsDefined()
        {
            var initialSize = (await Client.Fields.GetFieldDefinitionList()).Fields.Count;

            var field = BuildUniqueString();
            CheckSuccess(await Client.Fields.PutFieldDefinition(field, new PutFieldDefinitionRequest
            {
                KeepHistory = false
            }));

            var largerSize = (await Client.Fields.GetFieldDefinitionList()).Fields.Count;

            Assert.IsTrue(largerSize > initialSize);
            Assert.IsTrue(largerSize == initialSize + 1);
        }

        [TestMethod]
        public async Task NewFieldAppearsInList()
        {
            var field = BuildUniqueString();
            CheckSuccess(await Client.Fields.PutFieldDefinition(field, new PutFieldDefinitionRequest
            {
                KeepHistory = false
            }));

            var list = await Client.Fields.GetFieldDefinitionList();
            Assert.IsTrue(list.Fields.Any(i => i.FieldName == field));
        }
    }
}
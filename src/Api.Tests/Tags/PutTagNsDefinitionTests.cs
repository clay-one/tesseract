using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Tags
{
    [TestClass]
    public class PutTagNsDefinitionTests : TestClassBase
    {
        [TestMethod]
        public async Task MultipleCallsWithSameInputIsOkay()
        {
            var ns = BuildUniqueString();
            var putTagNsDefinitionRequest = new PutTagNsDefinitionRequest
            {
                KeepHistory = true
            };

            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, putTagNsDefinitionRequest));
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, putTagNsDefinitionRequest));
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, putTagNsDefinitionRequest));
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, putTagNsDefinitionRequest));
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, putTagNsDefinitionRequest));
        }

        [TestMethod]
        public async Task NewlyCreatedNsAppearsInOutput()
        {
            var ns = BuildUniqueString();
            var putTagNsDefinitionRequest = new PutTagNsDefinitionRequest
            {
                KeepHistory = false
            };

            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, putTagNsDefinitionRequest));

            Assert.IsNotNull(await Client.Tags.GetTagNsDefinition(ns));
            Assert.IsTrue((await Client.Tags.GetTagNsList()).TagNamespaces.Any(i => i.TagNamespace == ns));
        }
    }
}
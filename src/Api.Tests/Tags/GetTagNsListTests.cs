using System.Linq;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Tags
{
    [TestClass]
    public class GetTagNsListTests : TestClassBase
    {
        [TestMethod]
        public async Task CallReturnsNonNullResults()
        {
            var result = await Client.Tags.GetTagNsList();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TagNamespaces);
        }

        [TestMethod]
        public async Task ListGrowsWhenNewNsIsDefined()
        {
            var initialSize = (await Client.Tags.GetTagNsList()).TagNamespaces.Count;

            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest
            {
                KeepHistory = false
            }));

            var largerSize = (await Client.Tags.GetTagNsList()).TagNamespaces.Count;

            Assert.IsTrue(largerSize > initialSize);
            Assert.IsTrue(largerSize == initialSize + 1);
        }

        [TestMethod]
        public async Task NewNsAppearsInList()
        {
            var ns = BuildUniqueString();
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns, new PutTagNsDefinitionRequest
            {
                KeepHistory = false
            }));

            var list = await Client.Tags.GetTagNsList();
            Assert.IsTrue(list.TagNamespaces.Any(i => i.TagNamespace == ns));
        }
    }
}
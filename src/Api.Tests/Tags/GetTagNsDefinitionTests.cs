using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Tags
{
    [TestClass]
    public class GetTagNsDefinitionTests : TestClassBase
    {
        [TestMethod]
        public async Task UnknownNsReturnedAsNull()
        {
            var nsDef = await Client.Tags.GetTagNsDefinition(BuildUniqueString());
            Assert.IsNull(nsDef);
        }

        [TestMethod]
        public async Task KnownNsReturnsResults()
        {
            CheckSuccess(await Client.Tags.PutTagNsDefinition(TestNs, new PutTagNsDefinitionRequest()));

            var nsDef = await Client.Tags.GetTagNsDefinition(TestNs);
            Assert.IsNotNull(nsDef);
            Assert.IsNotNull(nsDef.TagNamespace);
            Assert.AreEqual(TestNs, nsDef.TagNamespace);
        }

        [TestMethod]
        public async Task KeepHistoryFieldIsStored()
        {
            var ns1 = BuildUniqueString();
            var ns2 = BuildUniqueString();

            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns1, new PutTagNsDefinitionRequest
            {
                KeepHistory = true
            }));
            CheckSuccess(await Client.Tags.PutTagNsDefinition(ns2, new PutTagNsDefinitionRequest
            {
                KeepHistory = false
            }));

            Assert.IsTrue((await Client.Tags.GetTagNsDefinition(ns1)).KeepHistory);
            Assert.IsFalse((await Client.Tags.GetTagNsDefinition(ns2)).KeepHistory);
        }
    }
}
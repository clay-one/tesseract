using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.Core.Utility;

namespace Tesseract.Core.Tests.Utility
{
    [TestClass]
    public class TagUtilsTests
    {
        [TestMethod]
        public void TestFqTag()
        {
            var ab = TagUtils.FqTag("a", "b");
            Assert.IsNotNull(ab);
            Assert.IsTrue(ab.StartsWith("a"));
            Assert.IsTrue(ab.EndsWith("b"));

            Assert.AreEqual("a", TagUtils.GetNsFromFqTag(ab));
            Assert.AreEqual("b", TagUtils.GetTagFromFqTag(ab));
            Assert.AreEqual(ab, TagUtils.FqTag(TagUtils.GetNsFromFqTag(ab), TagUtils.GetTagFromFqTag(ab)));
        }
    }
}
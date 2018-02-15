using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Tags.Validation
{
    [TestClass]
    public class PatchAccountWeightsOnTagAndWaitValidationTests : TestClassBase
    {
        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            CheckFailure(
                await Client.Tags.PatchAccountWeightsOnTagAndWait(BuildUniqueString(), TestTag, new PatchAccountWeightsOnTagRequest
                {
                    AccountPatches = new List<PatchAccountWeightsOnTagItem>
                    {
                        new PatchAccountWeightsOnTagItem { AccountId = TestAccount, WeightDelta = 2d}
                    }
                }),
                HttpStatusCode.BadRequest);
        }
    }
}
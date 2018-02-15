using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Tags.Validation
{
    [TestClass]
    public class PatchAccountWeightsOnTagValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullInputThrowsException()
        {
            await Client.Tags.PatchAccountWeightsOnTag(TestNs, TestTag, null);
        }

        [TestMethod]
        public async Task NullListIsOkay()
        {
            CheckSuccess(await Client.Tags.PatchAccountWeightsOnTag(TestNs, TestTag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = null
            }));
        }

        [TestMethod]
        public async Task EmptyListIsOkay()
        {
            CheckSuccess(await Client.Tags.PatchAccountWeightsOnTag(TestNs, TestTag, new PatchAccountWeightsOnTagRequest
            {
                AccountPatches = new List<PatchAccountWeightsOnTagItem>()
            }));
        }

        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            CheckFailure(
                await Client.Tags.PatchAccountWeightsOnTag(BuildUniqueString(), TestTag, new PatchAccountWeightsOnTagRequest
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
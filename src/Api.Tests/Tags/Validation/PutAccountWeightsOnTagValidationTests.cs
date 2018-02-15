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
    public class PutAccountWeightsOnTagValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullInputThrowsException()
        {
            await Client.Tags.PutAccountWeightsOnTag(TestNs, TestTag, null);
        }

        [TestMethod]
        public async Task NullListIsOkay()
        {
            CheckSuccess(await Client.Tags.PutAccountWeightsOnTag(TestNs, TestTag, new PutAccountWeightsOnTagRequest
            {
                Accounts = null
            }));
        }

        [TestMethod]
        public async Task EmptyListIsOkay()
        {
            CheckSuccess(await Client.Tags.PutAccountWeightsOnTag(TestNs, TestTag, new PutAccountWeightsOnTagRequest
            {
                Accounts = new List<AccountWeightOnTag>()
            }));
        }

        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            CheckFailure(
                await Client.Tags.PutAccountWeightsOnTag(BuildUniqueString(), TestTag, new PutAccountWeightsOnTagRequest
                {
                    Accounts = new List<AccountWeightOnTag>
                    {
                        new AccountWeightOnTag { AccountId = TestAccount, Weight = 5d }
                    }
                }),
                HttpStatusCode.BadRequest);
        }
    }
}
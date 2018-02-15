using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Accounts;

namespace Appson.Tesseract.Tests.Api.Accounts.Validation
{
    [TestClass]
    public class PutAndReplaceAccountTagsInNsValidationTests : TestClassBase
    {
        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            CheckFailure(
                await Client.Accounts.PutAndReplaceAccountTagsInNs(TestAccount, BuildUniqueString(), new PutAndReplaceAccountTagsInNsRequest
                {
                    Tags = new List<TagWeightOnAccount>
                    {
                        new TagWeightOnAccount { Tag = TestTag, Weight = 4d }
                    }
                }),
                HttpStatusCode.BadRequest);
        }
    }
}
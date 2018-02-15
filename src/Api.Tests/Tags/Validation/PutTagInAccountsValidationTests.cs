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
    public class PutTagInAccountsValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullInputThrowsException()
        {
            await Client.Tags.PutTagInAccounts(TestNs, TestTag, null);
        }

        [TestMethod]
        public async Task NullListIsOkay()
        {
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, TestTag, new PutTagInAccountsRequest
            {
                AccountIds = null
            }));
        }

        [TestMethod]
        public async Task EmptyListIsOkay()
        {
            CheckSuccess(await Client.Tags.PutTagInAccounts(TestNs, TestTag, new PutTagInAccountsRequest
            {
                AccountIds = new List<string>()
            }));
        }

        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            CheckFailure(
                await Client.Tags.PutTagInAccounts(BuildUniqueString(), TestTag, new PutTagInAccountsRequest
                {
                    AccountIds = new List<string>
                    {
                        TestAccount
                    }
                }),
                HttpStatusCode.BadRequest);
        }
    }
}
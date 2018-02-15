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
    public class DeleteTagFromAccountsValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullInputThrowsException()
        {
            await Client.Tags.DeleteTagFromAccounts(TestNs, TestTag, null);
        }

        [TestMethod]
        public async Task NullListIsOkay()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, TestTag));
            CheckSuccess(await Client.Tags.DeleteTagFromAccounts(TestNs, TestTag, new DeleteTagFromAccountsRequest
            {
                AccountIds = null
            }));
            Assert.AreNotEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);
        }

        [TestMethod]
        public async Task EmptyListIsOkay()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, TestTag));
            CheckSuccess(await Client.Tags.DeleteTagFromAccounts(TestNs, TestTag, new DeleteTagFromAccountsRequest
            {
                AccountIds = new List<string>()
            }));
            Assert.AreNotEqual(0d, (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);
        }

        [TestMethod]
        public async Task UndefinedNsShouldBeRejected()
        {
            CheckFailure(
                await Client.Tags.DeleteTagFromAccounts(BuildUniqueString(), TestTag, new DeleteTagFromAccountsRequest
                {
                    AccountIds = new List<string> { TestAccount, BuildUniqueString() }
                }),
                HttpStatusCode.BadRequest);
        }
    }
}
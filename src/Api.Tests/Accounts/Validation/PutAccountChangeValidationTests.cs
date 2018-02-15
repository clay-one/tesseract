using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.Tags;

namespace Appson.Tesseract.Tests.Api.Accounts.Validation
{
    [TestClass]
    public class PutAccountChangeValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullInputThrowsException()
        {
            await Client.Accounts.PutAccountChange(TestAccount, null);
        }

        [TestMethod]
        public async Task NullListsAreOkay()
        {
            CheckSuccess(await Client.Accounts.PutAccountChange(TestAccount, new PutAccountChangeRequest
            {
                FieldChanges = null,
                TagChanges = null
            }));
        }

        [TestMethod]
        public async Task EmptyListsAreOkay()
        {
            CheckSuccess(await Client.Accounts.PutAccountChange(TestAccount, new PutAccountChangeRequest
            {
                FieldChanges = new List<AccountFieldChangeInstruction>(),
                TagChanges = new List<AccountTagChangeInstruction>()
            }));
        }

        [TestMethod]
        public async Task UndefinedNsInChangesShouldBeRejected()
        {
            CheckFailure(
                await Client.Accounts.PutAccountChange(TestAccount, new PutAccountChangeRequest
                {
                    TagChanges = new List<AccountTagChangeInstruction>
                    {
                        new AccountTagChangeInstruction {Tag = TestTag, TagNs = BuildUniqueString(), Weight = 1d}
                    }
                }),
                HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void UndefinedFieldInChangesShouldBeRejected()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task AtomicValidationWithUndefinedNsInChanges()
        {
            var accountId = BuildUniqueString();
            var tag = BuildUniqueString();

            CheckSuccess(await Client.Tags.PutTagNsDefinition(TestNs, new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(accountId, TestNs, tag, 5d));

            CheckFailure(
                await Client.Accounts.PutAccountChange(TestAccount, new PutAccountChangeRequest
                {
                    TagChanges = new List<AccountTagChangeInstruction>
                    {
                        new AccountTagChangeInstruction {Tag = tag, TagNs = TestNs, Weight = 1d},
                        new AccountTagChangeInstruction {Tag = BuildUniqueString(), TagNs = BuildUniqueString(), Weight = 1d}
                    }
                }),
                HttpStatusCode.BadRequest);

            Assert.AreEqual(5d, (await Client.Accounts.GetTagWeightOnAccount(accountId, TestNs, tag)).Weight);
        }

        [TestMethod]
        public void AtomicValidationWithUndefinedFieldInChanges()
        {
            Assert.Inconclusive();
        }
    }
}
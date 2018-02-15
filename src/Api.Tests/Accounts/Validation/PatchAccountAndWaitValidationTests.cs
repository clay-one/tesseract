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
    public class PatchAccountAndWaitValidationTests : TestClassBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullInputThrowsException()
        {
            await Client.Accounts.PatchAccountAndWait(TestAccount, null);
        }

        [TestMethod]
        public async Task NullListsAreOkay()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, TestTag));

            var result = await Client.Accounts.PatchAccountAndWait(TestAccount, new PatchAccountRequest
            {
                TagChanges = null,
                TagPatches = null,
                FieldChanges = null,
                FieldPatches = null
            });

            CheckSuccess(result);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public async Task EmptyListsAreOkay()
        {
            CheckSuccess(await Client.Accounts.PutTagOnAccount(TestAccount, TestNs, TestTag));

            var result = await Client.Accounts.PatchAccountAndWait(TestAccount, new PatchAccountRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>(),
                TagPatches = new List<AccountTagPatchInstruction>(),
                FieldChanges = new List<AccountFieldChangeInstruction>(),
                FieldPatches = new List<AccountFieldPatchInstruction>()
            });

            CheckSuccess(result);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public async Task UndefinedNsInChangesShouldBeRejected()
        {
            CheckFailure(
                await Client.Accounts.PatchAccountAndWait(TestAccount, new PatchAccountRequest
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
        public async Task UndefinedNsInPatchesShouldBeRejected()
        {

            CheckFailure(
                await Client.Accounts.PatchAccountAndWait(TestAccount, new PatchAccountRequest
                {
                    TagPatches = new List<AccountTagPatchInstruction>
                    {
                        new AccountTagPatchInstruction {Tag = TestTag, TagNs = BuildUniqueString(), WeightDelta = 1d }
                    }
                }),
                HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void UndefinedFieldInPatchesShouldBeRejected()
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
                await Client.Accounts.PatchAccountAndWait(TestAccount, new PatchAccountRequest
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

        [TestMethod]
        public async Task AtomicValidationWithUndefinedNsInPatches()
        {
            var accountId = BuildUniqueString();
            var tag = BuildUniqueString();

            CheckSuccess(await Client.Tags.PutTagNsDefinition(TestNs, new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(accountId, TestNs, tag, 5d));

            CheckFailure(
                await Client.Accounts.PatchAccountAndWait(TestAccount, new PatchAccountRequest
                {
                    TagPatches = new List<AccountTagPatchInstruction>
                    {
                        new AccountTagPatchInstruction {Tag = tag, TagNs = TestNs, WeightDelta = 1d },
                        new AccountTagPatchInstruction {Tag = TestTag, TagNs = BuildUniqueString(), WeightDelta = 1d }
                    }
                }),
                HttpStatusCode.BadRequest);

            Assert.AreEqual(5d, (await Client.Accounts.GetTagWeightOnAccount(accountId, TestNs, tag)).Weight);
        }

        [TestMethod]
        public void AtomicValidationWithUndefinedFieldInPatches()
        {
            Assert.Inconclusive();
        }
    }
}
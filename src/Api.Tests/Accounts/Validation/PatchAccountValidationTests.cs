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
    public class PatchAccountValidationTests : TestClassBase
    {
        [TestMethod]
        public async Task UndefinedNsInChangesShouldBeRejected()
        {
            CheckFailure(
                await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
                {
                    TagChanges = new List<AccountTagChangeInstruction>
                    {
                        new AccountTagChangeInstruction {Tag = TestTag, TagNs = BuildUniqueString(), Weight = 1d}
                    }
                }),
                HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public async Task SetNegativeWeightShouldBeRejected()
        {
            // First, set a field and tag weight to a specific value

            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(TestAccount, TestNs, TestTag, 5d));
            Assert.Inconclusive("Until PutFieldValueOnAccount is implemented");
            CheckSuccess(await Client.Accounts.PutFieldValueOnAccount(TestAccount, TestField, 5d));

            // Then, try to change them along with a negative weight

            CheckFailure(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                FieldChanges = new List<AccountFieldChangeInstruction>
                {
                    new AccountFieldChangeInstruction
                    {
                        FieldName = TestField,
                        FieldValue = 10d
                    }
                },
                TagChanges = new List<AccountTagChangeInstruction>
                {
                    new AccountTagChangeInstruction
                    {
                        TagNs = TestNs,
                        Tag = TestTag,
                        Weight = 10d
                    },
                    new AccountTagChangeInstruction
                    {
                        TagNs = TestNs,
                        Tag = BuildUniqueString(),
                        Weight = -1d
                    }
                }
            }), HttpStatusCode.BadRequest);

            // Original field and tag weights should not have been changed

            Assert.AreEqual(5d,
                (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);
            Assert.AreEqual(5d,
                (await Client.Accounts.GetFieldValueOnAccount(TestAccount, TestField)).FieldValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NullInputThrowsException()
        {
            await Client.Accounts.PatchAccount(TestAccount, null);
        }

        [TestMethod]
        public async Task NullListsAreOkay()
        {
            CheckSuccess(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                TagChanges = null,
                TagPatches = null,
                FieldChanges = null,
                FieldPatches = null
            }));
        }

        [TestMethod]
        public async Task EmptyListsAreOkay()
        {
            CheckSuccess(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>(),
                TagPatches = new List<AccountTagPatchInstruction>(),
                FieldChanges = new List<AccountFieldChangeInstruction>(),
                FieldPatches = new List<AccountFieldPatchInstruction>()
            }));
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
                await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
                {
                    TagPatches = new List<AccountTagPatchInstruction>
                    {
                        new AccountTagPatchInstruction {Tag = TestTag, TagNs = BuildUniqueString(), WeightDelta = 1d}
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
                await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
                {
                    TagChanges = new List<AccountTagChangeInstruction>
                    {
                        new AccountTagChangeInstruction {Tag = tag, TagNs = TestNs, Weight = 1d},
                        new AccountTagChangeInstruction
                        {
                            Tag = BuildUniqueString(),
                            TagNs = BuildUniqueString(),
                            Weight = 1d
                        }
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
                await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
                {
                    TagPatches = new List<AccountTagPatchInstruction>
                    {
                        new AccountTagPatchInstruction {Tag = tag, TagNs = TestNs, WeightDelta = 1d},
                        new AccountTagPatchInstruction {Tag = TestTag, TagNs = BuildUniqueString(), WeightDelta = 1d}
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
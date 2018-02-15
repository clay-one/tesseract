using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract.ApiModel.Accounts;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class PatchAccountTests : TestClassBase
    {
        [TestMethod]
        public async Task EmptyListsShouldNotCreateNewAccount()
        {
            var account = BuildUniqueString();

            CheckSuccess(await Client.Accounts.PatchAccount(account, new PatchAccountRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>(),
                TagPatches = new List<AccountTagPatchInstruction>(),
                FieldChanges = new List<AccountFieldChangeInstruction>(),
                FieldPatches = new List<AccountFieldPatchInstruction>()
            }));

            try
            {
                await Client.Accounts.GetAccountInfo(account);
                Assert.Fail("Exception should have been thrown as a result of 404 status code, but didn't.");
            }
            catch (Exception)
            {
                // The exception should be thrown, this is the expected result.
            }
        }

        [TestMethod]
        public async Task SetNonExistantAccountShouldCreateIt()
        {
            var account = BuildUniqueString();
            await Client.Accounts.PatchAccount(account, new PatchAccountRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>
                {
                    new AccountTagChangeInstruction { Tag = TestTag, TagNs = TestNs, Weight = 5d }
                },
                FieldChanges = new List<AccountFieldChangeInstruction>
                {
                    new AccountFieldChangeInstruction { FieldName = TestField, FieldValue = 10d }
                }
            });

            var accountInfo = await Client.Accounts.GetAccountInfo(account);

            Assert.IsNotNull(accountInfo);

            Assert.IsNotNull(accountInfo.TagNamespaces);
            Assert.AreEqual(1, accountInfo.TagNamespaces.Count);
            Assert.IsNotNull(accountInfo.TagNamespaces[0]);
            Assert.AreEqual(TestNs, accountInfo.TagNamespaces[0].Namespace);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags);
            Assert.AreEqual(1, accountInfo.TagNamespaces[0].Tags.Count);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags[0]);
            Assert.AreEqual(TestTag, accountInfo.TagNamespaces[0].Tags[0].Tag);
            Assert.AreEqual(5d, accountInfo.TagNamespaces[0].Tags[0].Weight);

            Assert.IsNotNull(accountInfo.Fields);
            Assert.AreEqual(1, accountInfo.Fields.Count);
            Assert.IsNotNull(accountInfo.Fields[0]);
            Assert.AreEqual(TestField, accountInfo.Fields[0].FieldName);
            Assert.AreEqual(10d, accountInfo.Fields[0].FieldValue);
        }

        [TestMethod]
        public async Task PatchNonExistantAccountShouldCreateIt()
        {
            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PatchAccount(account, new PatchAccountRequest
            {
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction { Tag = TestTag, TagNs = TestNs, WeightDelta = 5d }
                },
                FieldPatches = new List<AccountFieldPatchInstruction>
                {
                    new AccountFieldPatchInstruction { FieldName = TestField, FieldValueDelta = 10d }
                }
            }));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);

            Assert.IsNotNull(accountInfo);

            Assert.IsNotNull(accountInfo.TagNamespaces);
            Assert.AreEqual(1, accountInfo.TagNamespaces.Count);
            Assert.IsNotNull(accountInfo.TagNamespaces[0]);
            Assert.AreEqual(TestNs, accountInfo.TagNamespaces[0].Namespace);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags);
            Assert.AreEqual(1, accountInfo.TagNamespaces[0].Tags.Count);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags[0]);
            Assert.AreEqual(TestTag, accountInfo.TagNamespaces[0].Tags[0].Tag);
            Assert.AreEqual(5d, accountInfo.TagNamespaces[0].Tags[0].Weight);

            Assert.IsNotNull(accountInfo.Fields);
            Assert.AreEqual(1, accountInfo.Fields.Count);
            Assert.IsNotNull(accountInfo.Fields[0]);
            Assert.AreEqual(TestField, accountInfo.Fields[0].FieldName);
            Assert.AreEqual(10d, accountInfo.Fields[0].FieldValue);
        }

        [TestMethod]
        public async Task SetAndPatchFieldsAndTags()
        {
            Assert.Inconclusive("Until field operations are implemented.");

            await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>
                {
                    new AccountTagChangeInstruction { Tag = TestTag, TagNs = TestNs, Weight = 1d }
                },
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction { Tag = TestTag, TagNs = TestNs, WeightDelta = 0d }
                },
                FieldChanges = new List<AccountFieldChangeInstruction>
                {
                    new AccountFieldChangeInstruction { FieldName = TestField, FieldValue = 2d }
                },
                FieldPatches = new List<AccountFieldPatchInstruction>
                {
                    new AccountFieldPatchInstruction { FieldName = TestField, FieldValueDelta = 0d }
                }
            });
        }
    }
}
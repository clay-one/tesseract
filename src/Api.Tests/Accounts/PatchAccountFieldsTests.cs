using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tesseract.ApiModel.Accounts;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class PatchAccountFieldsTests : TestClassBase
    {
        [TestMethod]
        public async Task SetSingleField()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task SetMultipleFields()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task SetFieldToZero()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task SetFieldToNegative()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task PatchSingleField()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task PatchMultipleFields()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task PatchFieldToZero()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task PatchFieldToNegative()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task SetAndPatchSameField()
        {
            CheckFailure(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                FieldChanges = new List<AccountFieldChangeInstruction>
                {
                    new AccountFieldChangeInstruction { FieldName = TestField, FieldValue = 2d }
                },
                FieldPatches = new List<AccountFieldPatchInstruction>
                {
                    new AccountFieldPatchInstruction { FieldName = TestField, FieldValueDelta = 2d }
                }
            }), HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task SetDuplicateField()
        {
            CheckFailure(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                FieldChanges = new List<AccountFieldChangeInstruction>
                {
                    new AccountFieldChangeInstruction { FieldName = TestField, FieldValue = 2d },
                    new AccountFieldChangeInstruction { FieldName = TestField, FieldValue = 2d }
                }
            }), HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task PatchDuplicateField()
        {
            CheckFailure(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                FieldPatches = new List<AccountFieldPatchInstruction>
                {
                    new AccountFieldPatchInstruction { FieldName = TestField, FieldValueDelta = 2d },
                    new AccountFieldPatchInstruction { FieldName = TestField, FieldValueDelta = 2d }
                }
            }), HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task SetAndPatchDifferentFields()
        {
            Assert.Inconclusive();
        }
    }
}
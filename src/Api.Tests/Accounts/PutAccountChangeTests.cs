using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tesseract.Common.Utils;
using Tesseract.ApiModel.Accounts;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class PutAccountChangeTests : TestClassBase
    {
        [TestMethod]
        public async Task EmptyListsShouldNotCreateNewAccount()
        {
            var account = BuildUniqueString();

            CheckSuccess(await Client.Accounts.PutAccountChange(account, new PutAccountChangeRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>(),
                FieldChanges = new List<AccountFieldChangeInstruction>()
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
        public async Task SetSingleTagWeight()
        {
            CheckSuccess(await Client.Accounts.PutAccountChange(TestAccount, new PutAccountChangeRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>
                {
                    new AccountTagChangeInstruction
                    {
                        TagNs = TestNs,
                        Tag = TestTag,
                        Weight = 27d
                    }
                }
            }));

            Assert.AreEqual(27d, (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);
        }

        [TestMethod]
        public async Task SetMultipleTagWeights()
        {
            var account = BuildUniqueString();
            var numberOfTags = 50;

            var tags = Enumerable.Range(0, numberOfTags)
                .Select(i => new Tuple<string, double>(BuildUniqueString(),
                    RandomProvider.GetThreadRandom().NextDouble() * 1000))
                .ToList();

            CheckSuccess(await Client.Accounts.PutAccountChange(account, new PutAccountChangeRequest
            {
                TagChanges = tags.Select(tag => new AccountTagChangeInstruction
                    {
                        TagNs = TestNs,
                        Tag = tag.Item1,
                        Weight = tag.Item2
                    }) .ToList()
            }));

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.IsNotNull(accountInfo);
            Assert.IsNotNull(accountInfo.TagNamespaces);
            Assert.AreEqual(1, accountInfo.TagNamespaces.Count);
            Assert.IsNotNull(accountInfo.TagNamespaces[0]);
            Assert.AreEqual(TestNs, accountInfo.TagNamespaces[0].Namespace);
            Assert.IsNotNull(accountInfo.TagNamespaces[0].Tags);
            Assert.AreEqual(numberOfTags, accountInfo.TagNamespaces[0].Tags.Count);

            tags.ForEach(tag =>
            {
                var tagInfo = accountInfo.TagNamespaces[0].Tags.SingleOrDefault(t => t.Tag == tag.Item1);
                Assert.IsNotNull(tagInfo);
                Assert.IsTrue(tag.Item2 - tagInfo.Weight < 1e-12);
            });
        }

        [TestMethod]
        public async Task NegativeWeightShouldBeRejected()
        {
            // First, set a field and tag weight to a specific value

            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(TestAccount, TestNs, TestTag, 5d));
            Assert.Inconclusive("Until PutFieldOnAccount is implemented");
            CheckSuccess(await Client.Accounts.PutFieldValueOnAccount(TestAccount, TestField, 5d));

            // Then, try to change them along with a negative weight

            CheckFailure(await Client.Accounts.PutAccountChange(TestAccount, new PutAccountChangeRequest
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
        public async Task ZeroWeightShouldRemoveTag()
        {
            // First, add a tag to an account

            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, TestNs, TestTag));

            // Set weight to zero

            CheckSuccess(await Client.Accounts.PutAccountChange(account, new PutAccountChangeRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>
                {
                    new AccountTagChangeInstruction
                    {
                        TagNs = TestNs,
                        Tag = TestTag,
                        Weight = 0d
                    }
                }
            }));

            // Make sure tag does not exist

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.AreEqual(0, accountInfo.TagNamespaces.Count);
        }

        [TestMethod]
        public void SetSingleField()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void SetMultipleFields()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void SetFieldToZero()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void SetFieldToNegative()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void SetMultipleTagsAndFields()
        {
            Assert.Inconclusive();
        }
    }
}
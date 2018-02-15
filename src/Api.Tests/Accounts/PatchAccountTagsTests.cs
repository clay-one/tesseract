using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tesseract.Common.Utils;
using Tesseract.ApiModel.Accounts;
using Tesseract.ApiModel.Tags;
using Appson.Tesseract.Tests.Api.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Tesseract.Tests.Api.Accounts
{
    [TestClass]
    public class PatchAccountTagsTests : TestClassBase
    {
        [TestMethod]
        public async Task SetSingleTagWeight()
        {
            CheckSuccess(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
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

            CheckSuccess(await Client.Accounts.PatchAccount(account, new PatchAccountRequest
            {
                TagChanges = tags.Select(tag => new AccountTagChangeInstruction
                {
                    TagNs = TestNs,
                    Tag = tag.Item1,
                    Weight = tag.Item2
                }).ToList()
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
                Assert.IsTrue(tag.Item2 - tagInfo.Weight < 1E-12);
            });
        }

        [TestMethod]
        public async Task SetZeroWeightShouldRemoveTag()
        {
            // First, add a tag to an account

            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagOnAccount(account, TestNs, TestTag));

            // Set weight to zero

            CheckSuccess(await Client.Accounts.PatchAccount(account, new PatchAccountRequest
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
        public async Task PatchSingleTagWeight()
        {
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(TestAccount, TestNs, TestTag, 10d));
            CheckSuccess(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction
                    {
                        TagNs = TestNs,
                        Tag = TestTag,
                        WeightDelta = 15d
                    }
                }
            }));

            Assert.AreEqual(25d, (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, TestTag)).Weight);
        }

        [TestMethod]
        public async Task PatchMultipleTagWeights()
        {
            var account = BuildUniqueString();
            var numberOfTags = 50;

            var tags = Enumerable.Range(0, numberOfTags)
                .Select(i => new Tuple<string, double>(BuildUniqueString(),
                    RandomProvider.GetThreadRandom().NextDouble() * 1000))
                .ToList();

            // First, initialize account with PutAccountChange API to predefined weights

            CheckSuccess(await Client.Accounts.PutAccountChange(account, new PutAccountChangeRequest
            {
                TagChanges = tags.Select(tag => new AccountTagChangeInstruction
                {
                    TagNs = TestNs,
                    Tag = tag.Item1,
                    Weight = tag.Item2
                }).ToList()
            }));

            // Then, patch using 2X predefined weights

            CheckSuccess(await Client.Accounts.PatchAccount(account, new PatchAccountRequest
            {
                TagPatches = tags.Select(tag => new AccountTagPatchInstruction
                {
                    TagNs = TestNs,
                    Tag = tag.Item1,
                    WeightDelta = tag.Item2*2
                }).ToList()
            }));

            // Finally, check the patch results

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
                Assert.IsTrue(tag.Item2*3 - tagInfo.Weight < 1e-11);
            });
        }

        [TestMethod]
        public async Task PatchToZeroShouldRemoveTag()
        {
            // First, add a tag to an account

            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 17d));

            // Patch weight with the negated same number

            CheckSuccess(await Client.Accounts.PatchAccount(account, new PatchAccountRequest
            {
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction
                    {
                        TagNs = TestNs,
                        Tag = TestTag,
                        WeightDelta = -17d
                    }
                }
            }));

            // Make sure tag does not exist

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.AreEqual(0, accountInfo.TagNamespaces.Count);
        }

        [TestMethod]
        public async Task PatchToNegativeShouldBehaveAsZero()
        {
            // First, add a tag to an account

            var account = BuildUniqueString();
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(account, TestNs, TestTag, 23d));

            // Patch weight with a larger negative delta

            CheckSuccess(await Client.Accounts.PatchAccount(account, new PatchAccountRequest
            {
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction
                    {
                        TagNs = TestNs,
                        Tag = TestTag,
                        WeightDelta = -1000d
                    }
                }
            }));

            // Make sure tag does not exist

            var accountInfo = await Client.Accounts.GetAccountInfo(account);
            Assert.AreEqual(0, accountInfo.TagNamespaces.Count);
        }

        [TestMethod]
        public async Task SetAndPatchSameTag()
        {
            CheckFailure(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>
                {
                    new AccountTagChangeInstruction { Tag = TestTag, TagNs = TestNs, Weight = 1d }
                },
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction { Tag = TestTag, TagNs = TestNs, WeightDelta = 1d }
                }
            }), HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task SetDuplicateTag()
        {
            CheckFailure(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>
                {
                    new AccountTagChangeInstruction { Tag = TestTag, TagNs = TestNs, Weight = 1d },
                    new AccountTagChangeInstruction { Tag = TestTag, TagNs = TestNs, Weight = 1d }
                }
            }), HttpStatusCode.BadRequest);

            CheckSuccess(await Client.Tags.PutTagNsDefinition("ns1", new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Tags.PutTagNsDefinition("ns2", new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>
                {
                    new AccountTagChangeInstruction { Tag = TestTag, TagNs = "ns1", Weight = 1d },
                    new AccountTagChangeInstruction { Tag = TestTag, TagNs = "ns2", Weight = 1d }
                }
            }));
        }

        [TestMethod]
        public async Task PatchDuplicateTag()
        {
            CheckFailure(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction { Tag = TestTag, TagNs = TestNs, WeightDelta = 1d },
                    new AccountTagPatchInstruction { Tag = TestTag, TagNs = TestNs, WeightDelta = 1d }
                }
            }), HttpStatusCode.BadRequest);

            CheckSuccess(await Client.Tags.PutTagNsDefinition("ns1", new PutTagNsDefinitionRequest()));
            CheckSuccess(await Client.Tags.PutTagNsDefinition("ns2", new PutTagNsDefinitionRequest()));

            CheckSuccess(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction { Tag = TestTag, TagNs = "ns1", WeightDelta = 1d },
                    new AccountTagPatchInstruction { Tag = TestTag, TagNs = "ns2", WeightDelta = 1d }
                }
            }));
        }

        [TestMethod]
        public async Task SetAndPatchDifferentTags()
        {
            var tag1 = BuildUniqueString();
            var tag2 = BuildUniqueString();

            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(TestAccount, TestNs, tag1, 10d));
            CheckSuccess(await Client.Accounts.PutTagWeightOnAccount(TestAccount, TestNs, tag2, 20d));
            CheckSuccess(await Client.Accounts.PatchAccount(TestAccount, new PatchAccountRequest
            {
                TagChanges = new List<AccountTagChangeInstruction>
                {
                    new AccountTagChangeInstruction
                    {
                        TagNs = TestNs,
                        Tag = tag1,
                        Weight = 30d
                    }  
                },
                TagPatches = new List<AccountTagPatchInstruction>
                {
                    new AccountTagPatchInstruction
                    {
                        TagNs = TestNs,
                        Tag = tag2,
                        WeightDelta = -5d
                    }
                }
            }));

            Assert.AreEqual(30d, (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, tag1)).Weight);
            Assert.AreEqual(15d, (await Client.Accounts.GetTagWeightOnAccount(TestAccount, TestNs, tag2)).Weight);
        }
    }
}
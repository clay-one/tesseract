using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tesseract.Common.Extensions;
using Tesseract.Common.Results;
using Tesseract.Core.Logic.Implementation;

namespace Tesseract.Core.Tests.Utility
{
    [TestClass]
    public class InputUtilsTests
    {
        [TestMethod]
        public void TestAccountIdValidation()
        {
            ApiValidationError Validate(string accountId)
            {
                return new DefaultInputValidationLogic().ValidateAccountId(accountId);
            }

            // Typical cases
            new[]
            {
                0.ToString(), int.MinValue.ToString(), int.MaxValue.ToString(), long.MinValue.ToString(),
                long.MaxValue.ToString(), Guid.Empty.ToString(), Guid.NewGuid().ToString()
            }.ForEach(s => { Assert.IsNull(Validate(s)); });

            // Length
            for (var i = 1; i <= 50; i++)
            {
                Assert.IsNull(Validate(new string('a', i)), $"Length of '{i}' should be acceptable, but it is not.");
            }

            for (var i = 51; i <= 500; i++)
            {
                Assert.IsNotNull(Validate(new string('a', i)), $"Length of '{i}' should not be acceptable, but it is.");
            }

            // Empty or null
            new[] {null, "", " ", "\n", "\r", "\t"}.ForEach(s =>
            {
                Assert.IsNotNull(Validate(s), "Whitespace should not be acceptable as AccountID");
            });

            // Character tests
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ:~-_".ForEach(c =>
            {
                Assert.IsNull(Validate(new string(c, 1)), $"Character '{c}' should be acceptable, but it is not.");
            });

            "`!@#$%^&*()=+{}[]\\|\"';?/><.,".ForEach(c =>
            {
                Assert.IsNotNull(Validate(new string(c, 1)),
                    $"Character '{c}' should not be acceptable, but it is.");
            });
        }

        [TestMethod]
        public void TestTagNsValidation()
        {
            ApiValidationError Validate(string ns)
            {
                return new DefaultInputValidationLogic().ValidateTagNs(ns);
            }

            // Typical cases
            new[]
            {
                0.ToString(), int.MinValue.ToString(), int.MaxValue.ToString(), long.MinValue.ToString(),
                long.MaxValue.ToString(), Guid.Empty.ToString(), Guid.NewGuid().ToString(),
                "SomeString", "Some-String", "Company_Product_SubProduct_Service_ServiceItem",
                "some-string", "good-company_some-product_sub-product_service_some-service-item"
            }.ForEach(s => { Assert.IsNull(Validate(s)); });

            // Length
            for (var i = 1; i <= 100; i++)
            {
                Assert.IsNull(Validate(new string('a', i)), $"Length of '{i}' should be acceptable, but it is not.");
            }

            for (var i = 101; i <= 500; i++)
            {
                Assert.IsNotNull(Validate(new string('a', i)), $"Length of '{i}' should not be acceptable, but it is.");
            }

            // Empty or null
            new[] {null, "", " ", "\n", "\r", "\t"}.ForEach(s =>
            {
                Assert.IsNotNull(Validate(s), "Whitespace should not be acceptable as Namespace");
            });

            // Character tests
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_".ForEach(c =>
            {
                Assert.IsNull(Validate(new string(c, 1)), $"Character '{c}' should be acceptable, but it is not.");
            });

            "`~!@#$%^&*()=+{}[]\\|\"';:?/><.,".ForEach(c =>
            {
                Assert.IsNotNull(Validate(new string(c, 1)),
                    $"Character '{c}' should not be acceptable, but it is.");
            });
        }

        [TestMethod]
        public void TestTagValidation()
        {
            ApiValidationError Validate(string tag)
            {
                return new DefaultInputValidationLogic().ValidateTag(tag);
            }

            // Typical cases
            new[]
            {
                0.ToString(), int.MinValue.ToString(), int.MaxValue.ToString(), long.MinValue.ToString(),
                long.MaxValue.ToString(), Guid.Empty.ToString(), Guid.NewGuid().ToString(),
                "SomeString", "Some:String", "Company:Product:SubProduct:Service:ServiceItem",
                "some-string", "good-company:some-product:sub-product:service:some-service-item",
                "service-id:7488", "receive:2017:06:06", "cluster:12345:from-service:6789", "192:168:1:1"
            }.ForEach(s => { Assert.IsNull(Validate(s), $"Input '{s}' should be acceptable, but it's not."); });

            // Part length
            for (var i = 1; i <= 80; i++)
            {
                Assert.IsNull(Validate("a:" + new string('a', i) + ":a"),
                    $"Part length of '{i}' should be acceptable, but it is not.");
            }

            for (var i = 81; i <= 500; i++)
            {
                Assert.IsNotNull(Validate("a:" + new string('a', i) + ":a"),
                    $"Part length of '{i}' should not be acceptable, but it is.");
            }

            // Depth
            for (var i = 1; i <= 6; i++)
            {
                var parts = Enumerable.Repeat("a", i);
                Assert.IsNull(Validate(string.Join(":", parts)),
                    $"Depth of '{i}' should be acceptable, but it is not.");
            }

            for (var i = 7; i <= 30; i++)
            {
                var parts = Enumerable.Repeat("a", i);
                Assert.IsNotNull(Validate(string.Join(":", parts)),
                    $"Depth of '{i}' should not be acceptable, but it is.");
            }

            // Full length
            var prefix = new string('a', 59) + ":" +
                         new string('b', 59) + ":" +
                         new string('c', 59) + ":";

            for (var i = 1; i <= 20; i++)
            {
                Assert.IsNull(Validate(prefix + new string('d', i)),
                    $"Length of '{prefix.Length + i}' should be acceptable, but it is not.");
            }

            for (var i = 21; i <= 80; i++)
            {
                Assert.IsNotNull(Validate(prefix + new string('d', i)),
                    $"Length of '{prefix.Length + i}' should not be acceptable, but it is.");
            }


            // Whitespace, empty or null
            new[] {null, "", " ", "\n", "\t", "\r"}.ForEach(s =>
            {
                Assert.IsNotNull(Validate(s), "Should not accept empty input");
            });

            // Whitespace or empty part
            new[] {"", " ", "\n", "\t", "\r"}.ForEach(s =>
            {
                Assert.IsNotNull(Validate("a:" + s + ":a"), "Should not accept empty part");
            });

            // Character tests
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_:".ForEach(c =>
            {
                Assert.IsNull(Validate("a" + new string(c, 1) + "a"),
                    $"Character '{c}' should be acceptable, but it is not.");
            });

            "`~!@#$%^&*()=+{}[]\\|\"'.;?/><,".ForEach(c =>
            {
                Assert.IsNotNull(Validate("a" + new string(c, 1) + "a"),
                    $"Character '{c}' should not be acceptable, but it is.");
            });
        }

        [TestMethod]
        public void TestAbsoluteTagWeightValidation()
        {
            var validation = new DefaultInputValidationLogic();

            Assert.IsNull(validation.ValidateAbsoluteTagWeight(0d));
            Assert.IsNull(validation.ValidateAbsoluteTagWeight(1d));
            Assert.IsNull(validation.ValidateAbsoluteTagWeight(double.MaxValue));
            Assert.IsNotNull(validation.ValidateAbsoluteTagWeight(-1d));
            Assert.IsNotNull(validation.ValidateAbsoluteTagWeight(double.MinValue));
            Assert.IsNotNull(validation.ValidateAbsoluteTagWeight(-double.Epsilon));
            Assert.IsNotNull(validation.ValidateAbsoluteTagWeight(double.NaN));
            Assert.IsNotNull(validation.ValidateAbsoluteTagWeight(double.NegativeInfinity));
            Assert.IsNotNull(validation.ValidateAbsoluteTagWeight(double.PositiveInfinity));
        }

        [TestMethod]
        public void TestFieldNameValidation()
        {
            ApiValidationError Validate(string fn)
            {
                return new DefaultInputValidationLogic().ValidateFieldName(fn);
            }

            // Typical cases
            new[]
            {
                0.ToString(), int.MinValue.ToString(), int.MaxValue.ToString(), long.MinValue.ToString(),
                long.MaxValue.ToString(), Guid.Empty.ToString(), Guid.NewGuid().ToString(),
                "SomeString", "Some-String", "some_field_name"
            }.ForEach(s => { Assert.IsNull(Validate(s), $"String '{s}' should have been acceptable, but it's not."); });

            // Length
            for (var i = 1; i <= 100; i++)
            {
                Assert.IsNull(Validate(new string('a', i)), $"Length of '{i}' should be acceptable, but it is not.");
            }

            for (var i = 101; i <= 500; i++)
            {
                Assert.IsNotNull(Validate(new string('a', i)), $"Length of '{i}' should not be acceptable, but it is.");
            }

            // Empty or null
            new[] {null, "", " ", "\n", "\r", "\t"}.ForEach(s =>
            {
                Assert.IsNotNull(Validate(s), "Whitespace should not be acceptable as Namespace");
            });

            // Character tests
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_".ForEach(c =>
            {
                Assert.IsNull(Validate(new string(c, 1)), $"Character '{c}' should be acceptable, but it is not.");
            });

            "`~!@#$%^&*()=+{}[]\\|\"';:?/><.,".ForEach(c =>
            {
                Assert.IsNotNull(Validate(new string(c, 1)),
                    $"Character '{c}' should not be acceptable, but it is.");
            });
        }

        [TestMethod]
        public void TestFieldValueValidation()
        {
            var validation = new DefaultInputValidationLogic();

            Assert.IsNull(validation.ValidateFieldValue(0d));
            Assert.IsNull(validation.ValidateFieldValue(1d));
            Assert.IsNull(validation.ValidateFieldValue(double.MaxValue));
            Assert.IsNull(validation.ValidateFieldValue(-1d));
            Assert.IsNull(validation.ValidateFieldValue(double.MinValue));
            Assert.IsNull(validation.ValidateFieldValue(-double.Epsilon));
            Assert.IsNotNull(validation.ValidateFieldValue(double.NaN));
            Assert.IsNotNull(validation.ValidateFieldValue(double.NegativeInfinity));
            Assert.IsNotNull(validation.ValidateFieldValue(double.PositiveInfinity));
        }

        [TestMethod]
        public void TestHasDuplicates()
        {
            var validation = new DefaultInputValidationLogic();

            Assert.IsFalse(validation.HasDuplicates(null));
            Assert.IsFalse(validation.HasDuplicates(new string[] { }));
            Assert.IsFalse(validation.HasDuplicates(new string[] {null}));
            Assert.IsFalse(validation.HasDuplicates(new[] {""}));
            Assert.IsFalse(validation.HasDuplicates(new[] {" "}));
            Assert.IsFalse(validation.HasDuplicates(new[] {"", null, " "}));
            Assert.IsFalse(validation.HasDuplicates(new[] {"a", "b", "c"}));
            Assert.IsFalse(validation.HasDuplicates(Enumerable.Range(1, 500).Select(i => i.ToString())));

            Assert.IsTrue(validation.HasDuplicates(new string[] {null, null}));
            Assert.IsTrue(validation.HasDuplicates(new[] {"", ""}));
            Assert.IsTrue(validation.HasDuplicates(new[] {" ", " "}));
            Assert.IsTrue(validation.HasDuplicates(new[] {"-", "-"}));
            Assert.IsTrue(validation.HasDuplicates(new[] {"a", "a"}));
            Assert.IsTrue(validation.HasDuplicates(new[] {"", "a", "a"}));
        }
    }
}
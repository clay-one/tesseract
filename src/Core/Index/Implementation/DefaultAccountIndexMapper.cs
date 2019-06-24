using System;
using System.Collections.Generic;
using System.Text;
using Tesseract.Common.Extensions;
using Tesseract.Core.Index.Model;
using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Index.Implementation
{
    public class DefaultAccountIndexMapper : IAccountIndexMapper
    {
        public AccountIndexModel MapAccountData(AccountData accountData)
        {
            if (accountData == null)
            {
                return null;
            }

            var result = new AccountIndexModel
            {
                AccountId = accountData.AccountId,
                CreationTime = EncodeCreationTime(accountData.CreationTime)
            };

            accountData.TagNamespaces.SafeForEach(nsPair =>
            {
                if (nsPair.Value == null || nsPair.Value.Count < 1)
                {
                    return;
                }

                result.SetTagNsText(nsPair.Key, BuildTagNsText(nsPair.Value.Keys));
            });

            accountData.Fields.SafeForEach(fieldPair => { result.SetFieldValue(fieldPair.Key, fieldPair.Value); });

            return result;
        }

        private static int EncodeCreationTime(DateTime time)
        {
            time = time.ToUniversalTime();
            return
                (time.Year - 2017) * 366 * 24 * 60 * 60 +
                time.DayOfYear * 24 * 60 * 60 +
                time.Hour * 60 * 60 +
                time.Minute * 60 +
                time.Second;
        }

        private static string BuildTagNsText(IEnumerable<string> tags)
        {
            var builder = new StringBuilder();

            foreach (var tag in tags.EmptyIfNull())
            {
                if (string.IsNullOrWhiteSpace(tag))
                {
                    continue;
                }

                var startIndex = 0;
                while (startIndex < tag.Length)
                {
                    var nextIndex = tag.IndexOf(':', startIndex);
                    if (nextIndex < 0)
                    {
                        break;
                    }

                    builder.Append(" ").Append(tag.Substring(0, nextIndex));
                    startIndex = nextIndex + 1;
                }

                builder.Append(" ").Append(tag);
            }

            return builder.ToString();
        }
    }
}
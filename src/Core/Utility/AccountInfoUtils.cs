using Tesseract.Core.Storage.Model;

namespace Tesseract.Core.Utility
{
    public static class AccountInfoUtils
    {
        public static double GetTagWeight(this AccountData account, string ns, string tag)
        {
            if (account?.TagNamespaces == null)
            {
                return 0d;
            }

            if (!account.TagNamespaces.TryGetValue(ns, out var nsTags))
            {
                return 0d;
            }

            if (nsTags == null)
            {
                return 0d;
            }

            return nsTags.TryGetValue(tag, out var weight) ? weight : 0d;
        }

        public static double GetFieldValue(this AccountData account, string fieldName)
        {
            if (account?.Fields == null)
            {
                return 0d;
            }

            return account.Fields.TryGetValue(fieldName, out var fieldValue) ? fieldValue : 0d;
        }
    }
}
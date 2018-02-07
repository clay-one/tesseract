using System;

namespace Tesseract.Core.Utility
{
    public static class TagUtils
    {
        private const char TagNsSeparator = '/';

        public static string FqTag(string ns, string tag)
        {
            return ns + TagNsSeparator + tag;
        }

        public static string GetNsFromFqTag(string fqTag)
        {
            if (fqTag == null)
                throw new ArgumentNullException(nameof(fqTag));

            var split = fqTag.Split(TagNsSeparator);
            if (split == null || split.Length != 2)
                throw new ArgumentException("fqTag is not in the correct format.");

            return split[0];
        }

        public static string GetTagFromFqTag(string fqTag)
        {
            if (fqTag == null)
                throw new ArgumentNullException(nameof(fqTag));

            var split = fqTag.Split(TagNsSeparator);
            if (split == null || split.Length != 2)
                throw new ArgumentException("fqTag is not in the correct format.");

            return split[1];
        }
    }
}
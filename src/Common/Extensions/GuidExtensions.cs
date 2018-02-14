using System;

namespace Tesseract.Common.Extensions
{
    public static class GuidExtensions
    {
        public static string ToUrlFriendly(this Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray()).Replace("/", "_").Replace("+", "-").Substring(0, 22);
        }

        public static Guid ParseUrlFriendlyGuid(string urlFriendlyGuid)
        {
            urlFriendlyGuid = urlFriendlyGuid.Replace("_", "/");
            urlFriendlyGuid = urlFriendlyGuid.Replace("-", "+");
            return new Guid(Convert.FromBase64String(urlFriendlyGuid + "=="));
        }
    }
}
using System;
using System.Collections.Generic;

namespace Tesseract.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object self)
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
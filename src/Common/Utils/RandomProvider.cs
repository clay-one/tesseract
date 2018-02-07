using System;
using System.Threading;

namespace Tesseract.Common.Utils
{
    public static class RandomProvider
    {
        private static int _seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> RandomWrapper = new ThreadLocal<Random>((Func<Random>) (() => new Random(Interlocked.Increment(ref RandomProvider._seed))));

        public static Random GetThreadRandom()
        {
            return RandomProvider.RandomWrapper.Value;
        }
    }
}
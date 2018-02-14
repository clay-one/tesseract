using System;
using Tesseract.Common.Utils;

namespace Tesseract.Cli.Generator.Base.Generators
{
    public class RandomIntegerProducer : InputProducerBase<int>
    {
        private readonly int _max;
        private readonly int _min;

        public RandomIntegerProducer(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public override bool ValidateSettings()
        {
            if (_min > _max)
            {
                Console.WriteLine("ERROR: minimum cannot be larger than the maximum.");
                return false;
            }

            return true;
        }

        public override int GetNext()
        {
            return RandomProvider.GetThreadRandom().Next(_min, _max + 1);
        }
    }
}
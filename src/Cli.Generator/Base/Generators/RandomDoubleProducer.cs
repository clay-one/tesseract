using System;
using Tesseract.Common.Utils;

namespace Tesseract.Cli.Generator.Base.Generators
{
    public class RandomDoubleProducer : InputProducerBase<double>
    {
        private readonly double _max;
        private readonly double _min;
        private readonly double _randomCoefficient;

        public RandomDoubleProducer(double min, double max)
        {
            _min = min;
            _max = max;
            _randomCoefficient = _max - _min;
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

        public override double GetNext()
        {
            return RandomProvider.GetThreadRandom().NextDouble() * _randomCoefficient + _min;
        }
    }
}
using System.Threading;

namespace Tesseract.Cli.Generator.Base.Generators
{
    public class SequentialNumberProducer : InputProducerBase<int>
    {
        private int _next;

        public SequentialNumberProducer(int start)
        {
            _next = start;
        }

        public override bool ValidateSettings()
        {
            return true;
        }

        public override int GetNext()
        {
            return Interlocked.Increment(ref _next);
        }
    }
}
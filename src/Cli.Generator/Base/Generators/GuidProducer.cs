using System;

namespace Tesseract.Cli.Generator.Base.Generators
{
    public class GuidProducer : InputProducerBase<string>
    {
        private readonly Func<Guid, string> _formatter;

        public GuidProducer(Func<Guid, string> formatter)
        {
            _formatter = formatter;
        }

        public override string GetNext()
        {
            return _formatter(Guid.NewGuid());
        }
    }
}
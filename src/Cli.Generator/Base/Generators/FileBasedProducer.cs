using System;

namespace Tesseract.Cli.Generator.Base.Generators
{
    public class FileBasedProducer : InputProducerBase<string>
    {
        public override bool ValidateSettings()
        {
            Console.WriteLine("ERROR: File-based data producer is not implemented yet.");
            return false;
        }

        public override string GetNext()
        {
            throw new NotImplementedException();
        }
    }
}
using System;

namespace Tesseract.Cli.Generator.Base.Generators
{
    public class TesseractAccountIdProducer : InputProducerBase<string>
    {
        public override bool ValidateSettings()
        {
            Console.WriteLine("ERROR: Tesseracr-query-based account id producer is not implemented yet.");
            return false;
        }

        public override string GetNext()
        {
            throw new NotImplementedException();
        }
    }
}
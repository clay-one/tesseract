using System.Threading.Tasks;

namespace Tesseract.Cli.Generator.Base.Generators
{
    public abstract class InputProducerBase
    {
        public virtual bool ValidateSettings()
        {
            return true;
        }

        public virtual Task Prepare()
        {
            return Task.CompletedTask;
        }

        public abstract string GetNextString();
    }

    public abstract class InputProducerBase<T> : InputProducerBase
    {
        public override string GetNextString()
        {
            return GetNext().ToString();
        }

        public abstract T GetNext();
    }
}
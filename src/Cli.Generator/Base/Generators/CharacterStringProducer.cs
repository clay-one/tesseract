using System;
using System.Text;
using System.Threading.Tasks;
using Tesseract.Common.Utils;

namespace Tesseract.Cli.Generator.Base.Generators
{
    public class CharacterStringProducer : InputProducerBase<string>
    {
        private const string ViableCharacters = "abcdefghijklmnopqrstuvwxyz1234567890";

        private readonly int _length;
        private readonly object _lockObject;
        private readonly bool _sequential;

        private int[] _sequentialIndexes;

        public CharacterStringProducer(int length, bool sequential)
        {
            _length = length;
            _sequential = sequential;
            _lockObject = new object();
        }

        public override bool ValidateSettings()
        {
            if (_length < 1 || _length > 50)
            {
                Console.WriteLine("ERROR: character string length is out of bounds.");
                return false;
            }

            return true;
        }

        public override async Task Prepare()
        {
            await base.Prepare();

            if (!_sequential)
                return;

            _sequentialIndexes = new int[_length];
        }

        public override string GetNext()
        {
            if (_sequential)
                return GetNextSequential();

            return BuildRandomString();
        }

        private string GetNextSequential()
        {
            lock (_lockObject)
            {
                var result = BuildSequentialString();
                IncreaseIndex(0);

                return result;
            }
        }

        private void IncreaseIndex(int indexIndex)
        {
            if (indexIndex >= _sequentialIndexes.Length)
                return;

            _sequentialIndexes[indexIndex] = (_sequentialIndexes[indexIndex] + 1) % ViableCharacters.Length;
            if (_sequentialIndexes[indexIndex] == 0)
                IncreaseIndex(indexIndex + 1);
        }

        private string BuildSequentialString()
        {
            var builder = new StringBuilder(new string(' ', _length));
            for (var i = 0; i < _length; i++)
            {
                var randomCharIndex = RandomProvider.GetThreadRandom().Next(0, ViableCharacters.Length);
                builder[_length - i - 1] = ViableCharacters[randomCharIndex];
            }

            return builder.ToString();
        }

        private string BuildRandomString()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < _length; i++)
            {
                var randomCharIndex = RandomProvider.GetThreadRandom().Next(0, ViableCharacters.Length);
                builder.Append(ViableCharacters[randomCharIndex]);
            }

            return builder.ToString();
        }
    }
}
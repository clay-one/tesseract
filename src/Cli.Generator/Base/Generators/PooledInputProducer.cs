using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Tesseract.Cli.Generator.Base.Generators
{
    public class PooledInputProducer<T> : InputProducerBase<T>
    {
        private readonly List<T> _pool;
        private int _nextIndex;


        public PooledInputProducer()
        {
            _pool = new List<T>();
        }

        public PooledInputProducer<T> Append(IEnumerable<T> items)
        {
            _pool.AddRange(items.Where(i => i != null));
            return this;
        }

        public override bool ValidateSettings()
        {
            if (_pool.Count <= 0)
            {
                Console.WriteLine("ERROR: pooled input producer is empty.");
                return false;
            }

            return true;
        }

        public override T GetNext()
        {
            var index = Interlocked.Increment(ref _nextIndex);
            index %= _pool.Count;

            return _pool[index];
        }
    }
}
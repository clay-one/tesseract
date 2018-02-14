using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Tesseract.Cli.Pit.Http
{
    public class DataCollector
    {
        private ConcurrentBag<RequestData> _bag;

        public DataCollector()
        {
            _bag = new ConcurrentBag<RequestData>();
        }

        public void Add(RequestData data)
        {
            _bag.Add(data);
        }

        public List<RequestData> Rotate()
        {
            var oldBag = Interlocked.Exchange(ref _bag, new ConcurrentBag<RequestData>());
            return oldBag.ToList();
        }
    }
}
using System;

namespace Tesseract.Cli.Pit.Http
{
    public class DataReporter
    {
        private readonly DataCollector _collector;

        public DataReporter(DataCollector collector)
        {
            _collector = collector;
        }

        public void Report()
        {
            var data = _collector.Rotate();
            Console.WriteLine($"{data.Count}");
        }
    }
}
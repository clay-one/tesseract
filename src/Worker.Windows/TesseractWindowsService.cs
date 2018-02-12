using System.ServiceProcess;
using NLog;
using Tesseract.Worker.Core;

namespace Tesseract.Worker.Windows
{
    public class TesseractWindowsService : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private WorkerService _service;

        public TesseractWindowsService()
        {
            ServiceName = "Tesseract Worker";
        }

        protected override void OnStart(string[] args)
        {
            _service = new WorkerService();
            _service.Start();
        }

        protected override void OnStop()
        {
            _service.Stop();
        }
    }
}
using System.ServiceProcess;
using log4net;
using Tesseract.Worker.Core;

namespace Tesseract.Worker.Windows
{
    public class TesseractWindowsService : ServiceBase
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
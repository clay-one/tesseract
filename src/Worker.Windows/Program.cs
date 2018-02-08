using System;
using System.ServiceProcess;
using log4net;
using Tesseract.Worker.Core;

namespace Tesseract.Worker.Windows
{
    class Program
    {
        // BkgJob tasks:
        // - Log errors on Faulty job runners to DB and terminate
        // - Log processing errors and make sure runner doesn't terminate
        
        // - Separate queue and processor for one-off jobs
        // - Implement some kind of guard, for detecting if a single job task schduler 
        //       or job runner is being run in more that one thread/instance
        
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
     
        static void Main()
        {
            if (!Environment.UserInteractive)
            {
                // running as service
                
                using (var windowsService = new TesseractWindowsService())
                    ServiceBase.Run(windowsService);
            }
            else
            {
                // running as console app
                
                Console.WriteLine("Starting tesseract worker service...");

                var service = new WorkerService();
                service.Start();

                Console.WriteLine("Service started. Press ENTER to stop.");
                Console.ReadLine();

                Console.WriteLine("Stopping the serivce...");
                service.Stop();
                Console.WriteLine("Service stopped, everything looks clean.");
            }
        }
    }
}

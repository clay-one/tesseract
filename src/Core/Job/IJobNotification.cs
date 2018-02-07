﻿using System.Threading.Tasks;

namespace Tesseract.Core.Job
{
    [Contract]
    public interface IJobNotification
    {
        Task StartNotificationTargetThread();
        Task StopNotificationTargetThread();

        Task NotifyJobUpdated(string jobId);
    }
}
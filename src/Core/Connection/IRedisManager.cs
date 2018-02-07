﻿namespace Tesseract.Core.Connection
{
    [Contract]
    public interface IRedisManager
    {
        IDatabase GetDatabase();
        ISubscriber GetSubscriber();
    }
}
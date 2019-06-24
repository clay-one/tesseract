using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Tesseract.Core.Connection;
using Tesseract.Core.Connection.Implementation;
using Tesseract.Core.Index;
using Tesseract.Core.Index.Implementation;
using Tesseract.Core.JobTypes.AccountIndexing;
using Tesseract.Core.Queue;
using Tesseract.Core.Storage;
using Tesseract.Core.Storage.Implementation;

namespace Tesseract.Core
{
    public static class ServiceProviderExtensions
    {
        public static void AddTesseractCoreServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEsManager, DefaultEsManager>();
            serviceCollection.AddSingleton<IAccountIndexMapper, DefaultAccountIndexMapper>();
            serviceCollection.AddSingleton<IAccountIndexReader, EsAccountIndexReader>();
            serviceCollection.AddSingleton<IAccountIndexWriter, EsAccountIndexWriter>();
            serviceCollection.AddSingleton<EsQueryBuilder>();

            serviceCollection.AddSingleton<IRedisManager, DefaultRedisManager>();

            serviceCollection.AddSingleton<IMongoManager, DefaultMongoManager>();
            serviceCollection.AddSingleton<IAccountStore, MongoAccountStore>();


            serviceCollection.AddTransient<IJobProcessor<AccountIndexingStep>, AccountIndexingProcessor>();

        }
    }
}

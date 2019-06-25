using Microsoft.Extensions.DependencyInjection;
using Tesseract.Core.Connection;
using Tesseract.Core.Connection.Implementation;
using Tesseract.Core.Index;
using Tesseract.Core.Index.Implementation;
using Tesseract.Core.Job;
using Tesseract.Core.Job.Implementation;
using Tesseract.Core.JobTypes.AccountIndexing;
using Tesseract.Core.JobTypes.FetchAllForReindex;
using Tesseract.Core.JobTypes.FetchFromIndex;
using Tesseract.Core.Logic;
using Tesseract.Core.Logic.Implementation;
using Tesseract.Core.Queue;
using Tesseract.Core.Queue.Implementation;
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

            serviceCollection.AddSingleton<IJobStore, MongoJobStore>();
            serviceCollection.AddSingleton<IJobManager, DefaultJobManager>();
            serviceCollection.AddSingleton<IJobNotification, RedisJobNotification>();
            serviceCollection.AddSingleton<IJobNotificationTarget, DefaultJobNotificationTarget>();
            serviceCollection.AddSingleton<IJobRunnerManager, DefaultJobRunnerManager>();
            serviceCollection.AddSingleton<IStaticJob, AccountIndexingStaticJob>();

            serviceCollection.AddSingleton<ICurrentTenantLogic, DefaultCurrentTenantLogic>();

            serviceCollection.AddSingleton<IRedisManager, DefaultRedisManager>();

            serviceCollection.AddSingleton<IMongoManager, DefaultMongoManager>();
            serviceCollection.AddSingleton<IAccountStore, MongoAccountStore>();


            serviceCollection.AddTransient<IJobProcessor<AccountIndexingStep>, AccountIndexingProcessor>();
            serviceCollection.AddTransient<IJobProcessor<FetchForReindexStep>, FetchForReindexProcessor>();
            serviceCollection.AddTransient<IJobProcessor<FetchFromIndexStep>, FetchFromIndexProcessor>();

            serviceCollection.AddSingleton(typeof(IJobQueue<>),typeof(RedisJobQueue<>));
            
            // TODO: register IJobQueue<AccountIndexingStep>
            // TODO: register IJobQueue<FetchForReindexStep>

        }
    }
}
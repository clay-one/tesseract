using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Tesseract.Core.Index;
using Tesseract.Core.Index.Model;
using Tesseract.Core.Storage;


namespace Tesseract.Core.Connection.Implementation
{
    public class DefaultEsManager : IEsManager
    {
        private readonly IFieldDefinitionStore _fieldDefinitionStore;

        private readonly ITagNsDefinitionStore _tagNsDefinitionStore;
        private readonly ILogger<DefaultEsManager> _logger;
        private readonly IConnectionPool _connectionPool;

        private readonly string _indexNamePrefix;

        #region Initialization

        public DefaultEsManager(IFieldDefinitionStore fieldDefinitionStore,
            ITagNsDefinitionStore tagNsDefinitionStore,
            IOptions<ElasticsearchConfig> options,
            ILogger<DefaultEsManager> logger)
        {
            _fieldDefinitionStore = fieldDefinitionStore;
            _tagNsDefinitionStore = tagNsDefinitionStore;
            _logger = logger;
            _indexNamePrefix = options.Value.IndexNamePrefix;
            var uris = options.Value.Uris.Select(u => new Uri(u));

            _connectionPool = new StaticConnectionPool(uris);

            var settings = new ConnectionSettings(_connectionPool)
#if DEBUG
                    .EnableDebugMode()
                    .OnRequestCompleted(h =>
                    {
                        Debug.WriteLine($"EsLog: {h.HttpMethod} {h.Uri} -> ${h.HttpStatusCode}");
                        if (h.RequestBodyInBytes != null)
                        {
                            var reqBody = Encoding.Default.GetString(h.RequestBodyInBytes);
                            Debug.WriteLine($"REQUEST:\n{reqBody}");
                        }
                    })
                    .PrettyJson()
#endif
                    .EnableHttpCompression()
                    .RequestTimeout(TimeSpan.FromSeconds(10));

            Client = new ElasticClient(settings);
        }


        #endregion

        #region IIndexManager implementation

        public ElasticClient Client { get; private set; }

        public string GetTenantIndexName(string tenantId)
        {
            return tenantId == null ? _indexNamePrefix : _indexNamePrefix + "_" + tenantId;
        }

        public async Task DeleteTenantIndex(string tenantId)
        {
            var exists = await Client.Indices.ExistsAsync(GetTenantIndexName(tenantId)); ;
            if (!exists.Exists)
            {
                return;
            }

            await Client.Indices.DeleteAsync(GetTenantIndexName(tenantId));
        }

        public async Task CreateTenantIndex(string tenantId)
        {
            var properties = new Properties
            {
                [IndexNaming.AccountIdFieldName] = new KeywordProperty(),
                [IndexNaming.CreationTimeFieldName] = new NumberProperty(NumberType.Integer)
            };

            await Client.Indices.CreateAsync(new CreateIndexRequest(GetTenantIndexName(tenantId), new IndexState
            {
                //Mappings = new Mappings
                //{
                //    [typeof(AccountIndexModel)] = new TypeMapping
                //    {
                //        Properties = properties
                //    }
                //}
                Mappings = new TypeMapping()
                {
                    Properties = properties
                }
            }));
        }

        public async Task EnsureTenantIndex(string tenantId)
        {
            var exists = await Client.Indices.ExistsAsync(GetTenantIndexName(tenantId));
            if (!exists.Exists)
            {
                await CreateTenantIndex(tenantId);
            }

            await EnsureIndexTagNsAndFieldMappings(tenantId);
        }

        public async Task EnsureIndexTagNsAndFieldMappings(string tenantId)
        {
            var mappings = await Client.Indices
                .GetMappingAsync(new GetMappingRequest(GetTenantIndexName(tenantId)));
            //.GetMappingAsync(new GetMappingRequest(GetTenantIndexName(tenantId), typeof(AccountIndexModel)));

            var nsList = await _tagNsDefinitionStore.LoadAll(tenantId);
            foreach (var ns in nsList)
            /*
                 * Kia:
                 * IGetMappingResponse's `Mapping` property is deleted in newer versions of NEST.
                 * Also the `Mappings` property is of type `IReadOnlyDictionary<IndexName, IndexMappings>`
                 * hence the deletion of `.Properties` expression.
                 */
            {
                if (!mappings.Indices.ContainsKey(IndexNaming.Namespace(ns.Namespace)))
                {
                    await SetTagNsMapping(tenantId, ns.Namespace);
                }
            }

            var fieldList = await _fieldDefinitionStore.LoadAll(tenantId);
            foreach (var field in fieldList)
            /*
                 * Kia:
                 * IGetMappingResponse's `Mapping` property is deleted in newer versions of NEST.
                 * Also the `Mappings` property is of type `IReadOnlyDictionary<IndexName, IndexMappings>`
                 * hence the deletion of `.Properties` expression.
                 */
            {
                if (!mappings.Indices.ContainsKey(IndexNaming.Field(field.FieldName)))
                {
                    await SetFieldMapping(tenantId, field.FieldName);
                }
            }
        }

        public async Task SetTagNsMapping(string tenantId, string ns)
        {
            try
            {
                await Client.MapAsync(
                //new PutMappingRequest(GetTenantIndexName(tenantId), typeof(AccountIndexModel))
                new PutMappingRequest(GetTenantIndexName(tenantId))
                {
                    Properties = new Properties
                    {
                        [IndexNaming.Namespace(ns)] = new TextProperty { Analyzer = "whitespace" }
                    }
                });
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, e.ToString());
                throw;
            }
        }

        public async Task SetFieldMapping(string tenantId, string fieldName)
        {
            await Client.MapAsync(
                //new PutMappingRequest(GetTenantIndexName(tenantId), typeof(AccountIndexModel))
                new PutMappingRequest(GetTenantIndexName(tenantId))
                {
                    Properties = new Properties
                    {
                        [IndexNaming.Field(fieldName)] = new NumberProperty(NumberType.Double)
                    }
                });
        }

        #endregion
    }
}
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Tesseract.Common.ComposerImposter;
using Tesseract.Core.Index;
using Tesseract.Core.Index.Model;
using Tesseract.Core.Storage;

namespace Tesseract.Core.Connection.Implementation
{
    [Component]
    public class DefaultEsManager : IEsManager
    {
        [ComponentPlug]
        public IFieldDefinitionStore FieldDefinitionStore { get; set; }

        [ComponentPlug]
        public ITagNsDefinitionStore TagNsDefinitionStore { get; set; }

        [ConfigurationPoint("elasticsearch.connectionPool")]
        public IConnectionPool ConnectionPool { get; set; }

        [ConfigurationPoint("elasticsearch.indexNamePrefix")]
        public string IndexNamePrefix { get; set; }

        #region Initialization

        [OnCompositionComplete]
        public void OnCompositionComplete()
        {
            var settings = new ConnectionSettings(ConnectionPool)
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

                        //                        if (h.ResponseBodyInBytes != null)
                        //                            Debug.WriteLine($"RESPONSE:\n{Encoding.Default.GetString(h.ResponseBodyInBytes)}");
                    })
                    .PrettyJson()
#endif
                    .EnableHttpCompression()
                    .RequestTimeout(TimeSpan.FromSeconds(10))
                //                    .BasicAuthentication("elastic", "changeme")
                ;

            Client = new ElasticClient(settings);
        }

        #endregion

        #region IIndexManager implementation

        public ElasticClient Client { get; private set; }

        public string GetTenantIndexName(string tenantId)
        {
            return tenantId == null ? IndexNamePrefix : IndexNamePrefix + "_" + tenantId;
        }

        public async Task DeleteTenantIndex(string tenantId)
        {
            var exists = await Client.IndexExistsAsync(GetTenantIndexName(tenantId));
            if (!exists.Exists)
            {
                return;
            }

            await Client.DeleteIndexAsync(GetTenantIndexName(tenantId));
        }

        public async Task CreateTenantIndex(string tenantId)
        {
            var properties = new Properties
            {
                [IndexNaming.AccountIdFieldName] = new KeywordProperty(),
                [IndexNaming.CreationTimeFieldName] = new NumberProperty(NumberType.Integer)
            };

            await Client.CreateIndexAsync(new CreateIndexRequest(GetTenantIndexName(tenantId), new IndexState
            {
                Mappings = new Mappings
                {
                    [typeof(AccountIndexModel)] = new TypeMapping
                    {
                        Properties = properties
                    }
                }
            }));
        }

        public async Task EnsureTenantIndex(string tenantId)
        {
            var exists = await Client.IndexExistsAsync(GetTenantIndexName(tenantId));
            if (!exists.Exists)
            {
                await CreateTenantIndex(tenantId);
            }

            await EnsureIndexTagNsAndFieldMappings(tenantId);
        }

        public async Task EnsureIndexTagNsAndFieldMappings(string tenantId)
        {
            var mappings =
                await Client.GetMappingAsync(new GetMappingRequest(GetTenantIndexName(tenantId),
                    typeof(AccountIndexModel)));

            var nsList = await TagNsDefinitionStore.LoadAll(tenantId);
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

            var fieldList = await FieldDefinitionStore.LoadAll(tenantId);
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
            await Client.MapAsync(
                new PutMappingRequest(GetTenantIndexName(tenantId), typeof(AccountIndexModel))
                {
                    Properties = new Properties
                    {
                        [IndexNaming.Namespace(ns)] = new TextProperty {Analyzer = "whitespace"}
                    }
                });
        }

        public async Task SetFieldMapping(string tenantId, string fieldName)
        {
            await Client.MapAsync(
                new PutMappingRequest(GetTenantIndexName(tenantId), typeof(AccountIndexModel))
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
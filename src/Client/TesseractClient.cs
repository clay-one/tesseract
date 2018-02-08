using System;
using System.Net.Http;
using Tesseract.Client.Proxies;

namespace Tesseract.Client
{
    public class TesseractClient
    {
        private readonly TesseractProxyInternalData _internalData;

        public TesseractClientSettings Settings => _internalData.Settings;

        public AccountsProxy Accounts { get; private set; }
        public TagsProxy Tags { get; private set; }
        public FieldsProxy Fields { get; private set; }
        public StatisticsProxy Statistics { get; private set; }
        public JobsProxy Jobs { get; private set; }

        #region Initialization

        public TesseractClient()
        {
            throw new NotImplementedException();
        }

        public TesseractClient(HttpClient httpClient)
        {
            _internalData = new TesseractProxyInternalData
            {
                ClientInstance = this,
                Settings = new TesseractClientSettings(),
                HttpClient = httpClient
            };

            InitializeClients();
        }

        public TesseractClient(Uri baseUri)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = baseUri;

            _internalData = new TesseractProxyInternalData
            {
                ClientInstance = this,
                Settings = new TesseractClientSettings(),
                HttpClient = httpClient
            };

            InitializeClients();
        }

        private void InitializeClients()
        {
            Accounts = new AccountsProxy(_internalData);
            Tags = new TagsProxy(_internalData);
            Fields = new FieldsProxy(_internalData);
            Statistics = new StatisticsProxy(_internalData);
            Jobs = new JobsProxy(_internalData);
        }

        #endregion
    }
}
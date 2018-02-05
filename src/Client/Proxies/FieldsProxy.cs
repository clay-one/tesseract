using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tesseract.Client.Utils;

namespace Tesseract.Client.Proxies
{
    public class FieldsProxy : TesseractProxyBase
    {
        internal FieldsProxy(TesseractProxyInternalData internalData) : base(internalData)
        {
        }

        public async Task<GetFieldDefinitionListResponse> GetFieldDefinitionList()
        {
            // GET	/fields/list	-B, SAFE	Get list of defined fields

            var response = await GenericSendRequestAsync(HttpMethod.Get, $"/api/fields/list");
            return await response.DeserializeAsync<GetFieldDefinitionListResponse>();
        }

        public async Task<GetFieldDefinitionResponse> GetFieldDefinition(string fieldName)
        {
            // GET	/fields/f/:f	-B, SAFE	Get information about a field definition

            if (fieldName.IsNullOrWhitespace())
                throw new ArgumentNullException(nameof(fieldName));

            var response = await GenericSendRequestAsync(HttpMethod.Get, $"/api/fields/f/{fieldName}");
            return await response.DeserializeAsync<GetFieldDefinitionResponse>(true);
        }

        public async Task<ApiValidationResult> PutFieldDefinition(string fieldName, PutFieldDefinitionRequest request)
        {
            // PUT	/fields/f/:f	+B, IDMP	Define a new field, or update its parameters and settings

            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (fieldName.IsNullOrWhitespace())
                throw new ArgumentNullException(nameof(fieldName));

            return await InternalSendRequest(HttpMethod.Put, $"/api/fields/f/{fieldName}", request);
        }
    }
}
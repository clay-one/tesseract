using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ServiceStack.Text;
using Tesseract.Common.Extensions;
using Tesseract.Common.Results;

namespace Tesseract.Client.Utils
{
    public static class ResponseMessageExtensions
    {
        public static ApiValidationResult ToValidationResult(this HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode
                ? ApiValidationResult.Ok()
                : ApiValidationResult.Failure(response.ToValidationError());
        }

        public static async Task<ApiValidatedResult<TResponse>> ToValidatedResult<TResponse>(
            this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var content = await response.DeserializeAsync<TResponse>();
                return ApiValidatedResult<TResponse>.Ok(content);
            }

            return ApiValidatedResult<TResponse>.Failure(response.ToValidationError());
        }

        public static ApiValidationError ToValidationError(this HttpResponseMessage response)
        {
            return new ApiValidationError(response.StatusCode.ToString(), response.ReasonPhrase.Yield());
        }

        public static async Task<T> DeserializeAsync<T>(this HttpResponseMessage self,
            bool acceptNotFoundResult = false)
        {
            if (!self.IsSuccessStatusCode)
            {
                if (acceptNotFoundResult && self.StatusCode == HttpStatusCode.NotFound)
                {
                    return default(T);
                }

                // todo: use a better type rather than ArgumentException
                throw new ArgumentException(
                    $@"the HttpResponseMessage which the type {
                            typeof(T).FullName
                        } is expected to deserialize from was unsuccessful.");
            }

            try
            {
                var serializedContent = await self.Content.ReadAsStringAsync();
                var value = JsonSerializer.DeserializeFromString<T>(serializedContent);

                return value;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    $@"Error occured while trying to convert HttpResponseMessage to {typeof(T).FullName}", e);
            }
        }
    }
}
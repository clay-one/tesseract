using System.Net.Http;

namespace Tesseract.Client
{
    internal class TesseractProxyInternalData
    {
        public TesseractClient ClientInstance { get; set; }
        public TesseractClientSettings Settings { get; set; }
        public HttpClient HttpClient { get; set; }
    }
}
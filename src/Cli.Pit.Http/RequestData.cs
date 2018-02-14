namespace Tesseract.Cli.Pit.Http
{
    public class RequestData
    {
        public string Verb { get; set; }
        public string Uri { get; set; }
        public string RemoteIp { get; set; }
        public int NumberOfAccounts { get; set; }
    }
}
using System.Collections.Generic;

namespace Tesseract.Core.Index
{
    public class AccountQueryResultPage
    {
        public List<string> AccountIds { get; set; }
        public long TotalNumberOfResults { get; set; }

        public string ContinueWith { get; set; }
    }
}
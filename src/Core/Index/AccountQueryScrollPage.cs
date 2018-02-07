using System.Collections.Generic;

namespace Tesseract.Core.Index
{
    public class AccountQueryScrollPage
    {
        public List<string> AccountIds { get; set; }
        public string ScrollId{ get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract.Core
{
    public class ElasticsearchConfig
    {
        public string IndexNamePrefix { get; set; }
        public string[] Uris { get; set; }
    }
}

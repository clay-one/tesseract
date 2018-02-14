using System;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Tesseract.ApiModel.Tags;
using Tesseract.Cli.Generator.Base;

namespace Tesseract.Cli.Generator.PutTag
{
    [Verb("put-tag", HelpText = "Sets the weight of tags to 1 (if tags do not already exist) of accounts")]
    public class PutTagVerb : TagVerbBase
    {
        protected override async Task<long> ExecuteNextBatch()
        {
            var batchSize = GetNextBatchSize();

            var result = await Client.Tags.PutTagInAccounts(TagNs, GetNextTag(), new PutTagInAccountsRequest
            {
                AccountIds = Enumerable.Range(1, batchSize).Select(x => GetNextAccountId()).ToList()
            });

            if (!result.Success)
            {
                Console.WriteLine("Error!");
                return 0;
            }

            return batchSize;
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Tesseract.ApiModel.Tags;
using Tesseract.Cli.Generator.Base;
using Tesseract.Cli.Generator.Base.Generators;

namespace Tesseract.Cli.Generator.SetTagWeight
{
    [Verb("set-tag-weight", HelpText = "Sets the weight of tags on accounts to a number determined by input")]
    public class SetTagWeightVerb : TagVerbBase
    {
        private InputProducerBase<double> _weightProducer;

        protected override async Task<bool> ValidateParametersAsync()
        {
            if (!await base.ValidateParametersAsync())
                return false;

            switch (WeightDistribution)
            {
                case NumberDistributionType.Constant:
                    _weightProducer = new PooledInputProducer<double>().Append(WeightList);
                    break;

                case NumberDistributionType.Random:
                    _weightProducer = new RandomDoubleProducer(WeightMinRandom, WeightMaxRandom);
                    break;
            }

            return _weightProducer.ValidateSettings();
        }

        protected virtual double GetNextWeight()
        {
            return _weightProducer.GetNext();
        }

        protected override async Task<long> ExecuteNextBatch()
        {
            var batchSize = GetNextBatchSize();

            var result = await Client.Tags.PutAccountWeightsOnTag(TagNs, GetNextTag(),
                new PutAccountWeightsOnTagRequest
                {
                    Accounts = Enumerable.Range(1, batchSize)
                        .Select(x => new AccountWeightOnTag {AccountId = GetNextAccountId(), Weight = GetNextWeight()})
                        .ToList()
                });

            if (!result.Success)
            {
                Console.WriteLine("Error!");
                return 0;
            }

            return batchSize;
        }

        #region Command-line options

        [Option("weight-distribution", Default = NumberDistributionType.Random, HelpText = "")]
        public NumberDistributionType WeightDistribution { get; set; }

        [Option("weight-constants-list", Default = new[] {5.0d},
            HelpText =
                "When using constant weight distribution, specifies the list of weights to randomly choose from.")]
        public double[] WeightList { get; set; }

        [Option("weight-min-random", Default = 1.0d, HelpText = "")]
        public double WeightMinRandom { get; set; }

        [Option("weight-max-random", Default = 100.0d, HelpText = "")]
        public double WeightMaxRandom { get; set; }

        #endregion
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Tesseract.ApiModel.Tags;
using Tesseract.Cli.Generator.Base.Generators;
using Tesseract.Common.Extensions;
using Tesseract.Common.Text;

namespace Tesseract.Cli.Generator.Base
{
    public abstract class TagVerbBase : AccountVerbBase
    {
        private InputProducerBase _tagProducer;

        protected override async Task<bool> ValidateParametersAsync()
        {
            if (!await base.ValidateParametersAsync())
                return false;

            TagPrefix = TagPrefix ?? "";
            TagSuffix = TagSuffix ?? "";

            switch (TagSource)
            {
                case TagSourceType.Static:
                    _tagProducer = new PooledInputProducer<string>().Append(TagStaticList);
                    break;

                case TagSourceType.File:
                    _tagProducer = new FileBasedProducer();
                    break;

                case TagSourceType.SequentialNumbers:
                    _tagProducer = new SequentialNumberProducer(TagNumberStart);
                    break;

                case TagSourceType.RandomNumbers:
                    _tagProducer = new RandomIntegerProducer(TagNumberMinimum, TagNumberMaximum);
                    break;

                case TagSourceType.SequentialCharancters:
                    _tagProducer = new CharacterStringProducer(TagCharactersLength, true);
                    break;

                case TagSourceType.RandomCharacters:
                    _tagProducer = new CharacterStringProducer(TagCharactersLength, false);
                    break;

                case TagSourceType.Base32Guid:
                    _tagProducer = new GuidProducer(id => Base32Url.ToBase32String(id.ToByteArray()));
                    break;

                case TagSourceType.Base64Guid:
                    _tagProducer = new GuidProducer(id => id.ToUrlFriendly());
                    break;

                case TagSourceType.HexGuid:
                    _tagProducer = new GuidProducer(id => id.ToString("N"));
                    break;
            }

            if (TagPoolSize > 5000000)
            {
                Console.WriteLine("ERROR: Pool size too large.");
                return false;
            }

            return _tagProducer.ValidateSettings();
        }

        protected override async Task PrepareAsync()
        {
            await base.PrepareAsync();
            await Client.Tags.PutTagNsDefinition(TagNs, new PutTagNsDefinitionRequest());

            if (TagPoolSize <= 0)
                return;

            var pool = new PooledInputProducer<string>();
            pool.Append(Enumerable.Repeat(1, TagPoolSize).Select(i => _tagProducer.GetNextString()));

            _tagProducer = pool;
        }

        protected virtual string GetNextTag()
        {
            return $"{TagPrefix}{_tagProducer.GetNextString()}{TagSuffix}";
        }

        #region Command line options

        [Option('n', "namespace", Default = "generator-ns", HelpText = "Tag Namespace used to tag the accounts with")]
        public string TagNs { get; set; }

        [Option('t', "tags", Default = TagSourceType.RandomCharacters, HelpText = "")]
        public TagSourceType TagSource { get; set; }

        [Option("tags-static-list", Default = new[] {"generator-tag"},
            HelpText = "When using Static tag source, specifies the list of tags to randomly choose from.")]
        public string[] TagStaticList { get; set; }

        [Option("tags-file-path", HelpText = "")]
        public string TagFilePath { get; set; }

        [Option("tags-number-start", Default = 1, HelpText = "")]
        public int TagNumberStart { get; set; }

        [Option("tags-number-min", Default = 1, HelpText = "")]
        public int TagNumberMinimum { get; set; }

        [Option("tags-number-max", Default = int.MaxValue, HelpText = "")]
        public int TagNumberMaximum { get; set; }

        [Option("tags-char-length", Default = 4, HelpText = "")]
        public int TagCharactersLength { get; set; }

        [Option("tags-pool-size", Default = -1,
            HelpText =
                "For Guid or Random tags, you can limit the number of uniquely generated values by setting this parameter.")]
        public int TagPoolSize { get; set; }

        [Option("tags-prefix", HelpText =
            "If specified, the value will be added to the beginning of all generated or queried tags")]
        public string TagPrefix { get; set; }

        [Option("tags-suffix", HelpText =
            "If specified, the value will be added to the end of all generated or queried tags")]
        public string TagSuffix { get; set; }

        #endregion
    }
}
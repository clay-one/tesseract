using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Tesseract.Cli.Generator.Base.Generators;
using Tesseract.Common.Extensions;
using Tesseract.Common.Text;

namespace Tesseract.Cli.Generator.Base
{
    public abstract class AccountVerbBase : VerbBase
    {
        private InputProducerBase _accountIdProducer;

        protected override async Task<bool> ValidateParametersAsync()
        {
            if (!await base.ValidateParametersAsync())
                return false;

            AccountIdPrefix = AccountIdPrefix ?? "";
            AccountIdSuffix = AccountIdSuffix ?? "";

            switch (AccountSource)
            {
                case AccountSourceType.Static:
                    _accountIdProducer = new PooledInputProducer<string>().Append(AccountIdStaticList);
                    break;

                case AccountSourceType.Tesseract:
                    _accountIdProducer = new TesseractAccountIdProducer();
                    break;

                case AccountSourceType.File:
                    _accountIdProducer = new FileBasedProducer();
                    break;

                case AccountSourceType.SequentialNumbers:
                    _accountIdProducer = new SequentialNumberProducer(AccountIdNumberStart);
                    break;

                case AccountSourceType.RandomNumbers:
                    _accountIdProducer = new RandomIntegerProducer(AccountIdNumberMinimum, AccountIdNumberMaximum);
                    break;

                case AccountSourceType.SequentialCharancters:
                    _accountIdProducer = new CharacterStringProducer(AccountIdCharactersLength, true);
                    break;

                case AccountSourceType.RandomCharacters:
                    _accountIdProducer = new CharacterStringProducer(AccountIdCharactersLength, false);
                    break;

                case AccountSourceType.Base32Guid:
                    _accountIdProducer = new GuidProducer(id => Base32Url.ToBase32String(id.ToByteArray()));
                    break;

                case AccountSourceType.Base64Guid:
                    _accountIdProducer = new GuidProducer(id => id.ToUrlFriendly());
                    break;

                case AccountSourceType.HexGuid:
                    _accountIdProducer = new GuidProducer(id => id.ToString("N"));
                    break;
            }

            if (AccountIdPoolSize > 5000000)
            {
                Console.WriteLine("ERROR: Pool size too large.");
                return false;
            }

            return _accountIdProducer.ValidateSettings();
        }

        protected override async Task PrepareAsync()
        {
            await base.PrepareAsync();

            if (AccountIdPoolSize <= 0)
                return;

            var pool = new PooledInputProducer<string>();
            pool.Append(Enumerable.Repeat(1, AccountIdPoolSize).Select(i => _accountIdProducer.GetNextString()));

            _accountIdProducer = pool;
        }

        protected virtual string GetNextAccountId()
        {
            return $"{AccountIdPrefix}{_accountIdProducer.GetNextString()}{AccountIdSuffix}";
        }

        #region Command line options

        [Option('a', "accounts", Default = AccountSourceType.HexGuid, HelpText = "")]
        public AccountSourceType AccountSource { get; set; }

        [Option("accounts-static-list", Default = new[] {"generator-account"}, Separator = ',',
            HelpText = "When using Static account source, specifies the list of account ids to randomly choose from.")]
        public IEnumerable<string> AccountIdStaticList { get; set; }

        [Option("accounts-tesseract-query", HelpText = "")]
        public string AccountIdTesseractQuery { get; set; }

        [Option("accounts-file-path", HelpText = "")]
        public string AccountIdFilePath { get; set; }

        [Option("accounts-number-start", Default = 1, HelpText = "")]
        public int AccountIdNumberStart { get; set; }

        [Option("accounts-number-min", Default = 1, HelpText = "")]
        public int AccountIdNumberMinimum { get; set; }

        [Option("accounts-number-max", Default = int.MaxValue, HelpText = "")]
        public int AccountIdNumberMaximum { get; set; }

        [Option("accounts-char-length", Default = 8, HelpText = "")]
        public int AccountIdCharactersLength { get; set; }

        [Option("accounts-pool-size", Default = -1,
            HelpText =
                "For Guid or Random account ids, you can limit the number of uniquely generated values by setting this parameter.")]
        public int AccountIdPoolSize { get; set; }

        [Option("accounts-prefix", HelpText =
            "If specified, the value will be added to the beginning of all generated or queried account ids")]
        public string AccountIdPrefix { get; set; }

        [Option("accounts-suffix", HelpText =
            "If specified, the value will be added to the end of all generated or queried account ids")]
        public string AccountIdSuffix { get; set; }

        #endregion
    }
}
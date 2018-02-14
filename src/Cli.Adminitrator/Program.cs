using System;
using CommandLine;
using Tesseract.Cli.Adminitrator.Base;
using Tesseract.Cli.Adminitrator.DefineTenant;
using Tesseract.Cli.Adminitrator.ReindexAll;

namespace Tesseract.Cli.Adminitrator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var parser = new Parser(s =>
            {
                s.CaseInsensitiveEnumValues = true;
                s.CaseSensitive = false;
                s.HelpWriter = Console.Out;
            });

            parser.ParseArguments<ReindexAllVerb, DefineTenantVerb>(args)
                .WithParsed<VerbBase>(verb => { verb.Run(); })
                .WithNotParsed(errors => { });
        }
    }
}
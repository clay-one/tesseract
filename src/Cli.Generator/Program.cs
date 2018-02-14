using System;
using CommandLine;
using Tesseract.Cli.Generator.Base;
using Tesseract.Cli.Generator.PutTag;
using Tesseract.Cli.Generator.SetTagWeight;

namespace Tesseract.Cli.Generator
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

            parser.ParseArguments<PutTagVerb, SetTagWeightVerb>(args)
                .WithParsed<VerbBase>(verb => { verb.Run(); })
                .WithNotParsed(errors => { });
        }
    }
}
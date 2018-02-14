using CommandLine;

namespace Tesseract.Cli.Generator.Base
{
    public abstract class FieldVerbBase : VerbBase
    {
        [Option('f', "field", Default = "gen-field", HelpText = "The Field to be set or changed")]
        public string FieldName { get; set; }
    }
}
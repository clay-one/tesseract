using System;
using System.Threading.Tasks;
using CommandLine;
using Tesseract.Cli.Adminitrator.Base;

namespace Tesseract.Cli.Adminitrator.DefineTenant
{
    [Verb("define-tenant", HelpText = "Defines a new tenant in the database")]
    public class DefineTenantVerb : VerbBase
    {
        protected override Task<bool> ValidateParametersAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task PrepareAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task StartJobAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> ReportProgressAsync()
        {
            throw new NotImplementedException();
        }
    }
}
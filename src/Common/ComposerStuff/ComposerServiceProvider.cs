using System;
using ComposerCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tesseract.Common.ComposerStuff
{
    public class ComposerServiceProvider : IServiceProvider, ISupportRequiredService
    {
        private readonly IComposer _composer;

        public ComposerServiceProvider(IComposer composer)
        {
            _composer = composer;
        }

        public object GetService(Type serviceType)
        {
            return _composer.GetComponent(serviceType);
        }

        public object GetRequiredService(Type serviceType)
        {
            return _composer.GetComponent(serviceType);
        }
    }
}
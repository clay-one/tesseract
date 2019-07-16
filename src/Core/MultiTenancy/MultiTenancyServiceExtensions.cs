using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tesseract.Core.MultiTenancy
{
    public static class MultiTenancyServiceExtensions
    {
        public static IServiceCollection AddMultiTenancyServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ITenantContextAccessor, TenantContextAccessor>();
            serviceCollection.TryAddTransient<ITenantContextFactory, TenantContextFactory>();

            return serviceCollection;
        }
    }
}

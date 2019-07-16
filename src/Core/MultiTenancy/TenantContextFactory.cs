using System;
using System.Collections.Generic;
using System.Text;

namespace Tesseract.Core.MultiTenancy
{
    public class TenantContextFactory : ITenantContextFactory
    {
        private readonly ITenantContextAccessor _tenantContextAccessor;

        public TenantContextFactory(ITenantContextAccessor tenantContextAccessor)
        {
            _tenantContextAccessor = tenantContextAccessor;
        }

        public TenantContext Create()
        {
            var context = new TenantContext(new Context.TenantContextInfo("fanap-plus"));

            if (_tenantContextAccessor != null)
                _tenantContextAccessor.TenantContext = context;

            return context;
        }
    }
}

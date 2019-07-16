using System;
using System.Collections.Generic;
using System.Text;
using Tesseract.Core.Context;

namespace Tesseract.Core.MultiTenancy
{
    public class TenantContext
    {
        public TenantContext(TenantContextInfo tenant)
        {
            Tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        }
        public TenantContextInfo Tenant { get; }
    }
}

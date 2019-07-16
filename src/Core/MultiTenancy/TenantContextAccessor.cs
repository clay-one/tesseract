using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Tesseract.Core.MultiTenancy
{
    public class TenantContextAccessor : ITenantContextAccessor
    {
        private static readonly AsyncLocal<TenantContext> ctx = new AsyncLocal<TenantContext>();

        public TenantContext TenantContext
        {
            get => ctx.Value;
            set => ctx.Value = value;
        }
    }
}

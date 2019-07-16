namespace Tesseract.Core.MultiTenancy
{
    public interface ITenantContextAccessor
    {
        TenantContext TenantContext { get; set; }
    }
}
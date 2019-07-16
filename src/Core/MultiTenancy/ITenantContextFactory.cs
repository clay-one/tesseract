namespace Tesseract.Core.MultiTenancy
{
    public interface ITenantContextFactory
    {
        TenantContext Create();
    }
}
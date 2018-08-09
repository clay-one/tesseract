using System.Threading;

namespace Tesseract.Web.CorrelationId
{
    /// <inheritdoc />
    public class CorrelationContextAccessor : ICorrelationContextAccessor
    {
        private static readonly AsyncLocal<CorrelationContext> Context = new AsyncLocal<CorrelationContext>();

        public CorrelationContext CorrelationContext
        {
            get => Context.Value;
            set => Context.Value = value;
        }
    }
}
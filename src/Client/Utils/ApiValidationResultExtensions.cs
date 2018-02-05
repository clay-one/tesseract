namespace Tesseract.Client.Utils
{
    public static class ApiValidationResultExtensions
    {
        public static string ToDebugMessage(this ApiValidationResult result)
        {
            if (result.Success)
                return "OK";

            return $"Failure with {result.Errors.Count} errors: {string.Join("; ", result.Errors.Select(e => e.ToDebugMessage()))}";
        }

        public static string ToDebugMessage(this ApiValidationError error)
        {
            if (error == null)
                return "<null>";

            if (error.ErrorKey == null)
                return "<null-key>";

            if (error.ErrorKey.Length == 0)
                return "<empty-key>";

            var result = error.ErrorKey;
            if (error.ErrorParameters != null)
                result += $"({string.Join(", ", error.ErrorParameters)})";

            if (!string.IsNullOrWhiteSpace(error.PropertyPath))
                result += $" on {error.PropertyPath}";

            return result;
        }
    }
}
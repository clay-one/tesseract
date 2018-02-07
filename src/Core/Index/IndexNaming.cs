namespace Tesseract.Core.Index
{
    public static class IndexNaming
    {
        public const string NamespacePrefix = "ns_";
        public const string FieldPrefix = "f_";

        public const string AccountIdFieldName = "accountId";
        public const string CreationTimeFieldName = "creationTime";

        public static string Namespace(string ns)
        {
            return NamespacePrefix + ns;
        }

        public static string Field(string fieldName)
        {
            return FieldPrefix + fieldName;
        }
    }
}
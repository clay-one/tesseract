using System.Collections.Generic;

namespace Tesseract.Core.Index.Model
{
    public class AccountIndexModel : Dictionary<string, object>
    {
        // ReSharper disable once UnusedMember.Global - Used by NEST
        public string Id
        {
            get => AccountId;
            set => AccountId = value;
        }
        
        public string AccountId
        {
            get
            {
                TryGetValue(IndexNaming.AccountIdFieldName, out var accountId);
                return accountId as string;
            }
            set => this[IndexNaming.AccountIdFieldName] = value;
        }

        public int CreationTime
        {
            get
            {
                TryGetValue(IndexNaming.CreationTimeFieldName, out var creationTime);
                return creationTime as int? ?? 0;
            }
            set => this[IndexNaming.CreationTimeFieldName] = value;
        }

        public void SetTagNsText(string ns, string tagsText)
        {
            this[IndexNaming.Namespace(ns)] = tagsText;
        }

        public void SetFieldValue(string fieldName, double fieldValue)
        {
            this[IndexNaming.Field(fieldName)] = fieldValue;
        }
    }
}
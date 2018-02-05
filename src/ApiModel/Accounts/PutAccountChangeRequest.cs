using System.Collections.Generic;

namespace Tesseract.ApiModel.Accounts
{
    public class PutAccountChangeRequest
    {
        public List<AccountTagChangeInstruction> TagChanges { get; set; }
        public List<AccountFieldChangeInstruction> FieldChanges { get; set; }
    }
}
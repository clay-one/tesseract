using System.Collections.Generic;

namespace Tesseract.ApiModel.Accounts
{
    public class PatchAccountRequest
    {
        public List<AccountTagChangeInstruction> TagChanges { get; set; }
        public List<AccountFieldChangeInstruction> FieldChanges { get; set; }
        public List<AccountTagPatchInstruction> TagPatches { get; set; }
        public List<AccountFieldPatchInstruction> FieldPatches { get; set; }
    }
}
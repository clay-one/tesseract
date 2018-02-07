using System.Collections.Generic;
using Tesseract.ApiModel.General;
using Tesseract.Common.ComposerImposter;
using Tesseract.Common.Results;

namespace Tesseract.Core.Logic
{
    [Contract]
    public interface IInputValidationLogic
    {
        ApiValidationError ValidateTag(string tag);
        ApiValidationError ValidateAccountId(string accountId);

        ApiValidationError ValidateExistingTagNs(string ns);
        ApiValidationError ValidateTagNs(string ns);
        ApiValidationError ValidateAbsoluteTagWeight(double weight);

        ApiValidationError ValidateExistingFieldName(string fieldName);
        ApiValidationError ValidateFieldName(string fieldName);
        ApiValidationError ValidateFieldValue(double fieldValue);

        ApiValidationError ValidateRange(string fieldName, int input, int min, int max);

        bool HasDuplicates(IEnumerable<string> inputs);

        ApiValidationError ValidateTargetUrl(string url);
        ApiValidationResult ValidateAccountQuery(AccountQuery query);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tesseract.ApiModel.General;
using Tesseract.Common.ComposerImposter;
using Tesseract.Common.Extensions;
using Tesseract.Common.Results;

namespace Tesseract.Core.Logic.Implementation
{
    [Component]
    public class DefaultInputValidationLogic : IInputValidationLogic
    {
        private const int MaxAccountIdLength = 50;
        private const int MaxNamespaceLength = 100;
        private const int MaxTagLength = 200;
        private const int MaxTagPartLength = 80;
        private const int MaxTagDepth = 6;
        private const int MaxFieldNameLength = 100;
        private static readonly Regex AccountIdRegex = new Regex("^[\\w\\-:~]*$");
        private static readonly Regex NamespaceRegex = new Regex("^[\\w\\-]*$");
        private static readonly Regex TagRegex = new Regex("^[\\w\\-\\:]*$");
        private static readonly Regex FieldNameRegex = new Regex("^[\\w\\-]*$");

        [ComponentPlug]
        public ICurrentTenantLogic Tenant { get; set; }

        public ApiValidationError ValidateTag(string tag)
        {
            if (tag.IsNullOrWhitespace())
            {
                return new ApiValidationError(nameof(tag), ErrorKeys.ArgumentCanNotBeEmpty);
            }

            if (tag.Length > MaxTagLength)
            {
                return new ApiValidationError(nameof(tag), ErrorKeys.ArgumentLengthExceedsMaximum);
            }

            if (!TagRegex.IsMatch(tag))
            {
                return new ApiValidationError(nameof(tag), ErrorKeys.InvalidCharactersInArgument);
            }

            var tagHierarchy = tag.Split(':');
            if (tagHierarchy.Length > MaxTagDepth)
            {
                return new ApiValidationError(nameof(tag), ErrorKeys.TagHierarchyIsTooDeep);
            }

            if (tagHierarchy.Any(tagPart => tagPart.IsNullOrWhitespace()))
            {
                return new ApiValidationError(nameof(tag), ErrorKeys.TagHierarchyNodeCanNotBeEmpty);
            }

            if (tagHierarchy.Any(tagPart => tagPart.Length > MaxTagPartLength))
            {
                return new ApiValidationError(nameof(tag), ErrorKeys.TagHierarchyNodeLengthExceedsMaximum);
            }

            return null;
        }

        public ApiValidationError ValidateAccountId(string accountId)
        {
            if (accountId.IsNullOrWhitespace())
            {
                return new ApiValidationError(nameof(accountId), ErrorKeys.ArgumentCanNotBeEmpty);
            }

            if (accountId.Length > MaxAccountIdLength)
            {
                return new ApiValidationError(nameof(accountId), ErrorKeys.ArgumentLengthExceedsMaximum);
            }

            if (!AccountIdRegex.IsMatch(accountId))
            {
                return new ApiValidationError(nameof(accountId), ErrorKeys.InvalidCharactersInArgument);
            }

            return null;
        }

        public ApiValidationError ValidateExistingTagNs(string ns)
        {
            var result = ValidateTagNs(ns);
            if (result != null)
            {
                return result;
            }

            return Tenant.DoesTagNsExist(ns)
                ? null
                : new ApiValidationError(nameof(ns), ErrorKeys.TagNamespaceIsNotDefined);
        }

        public ApiValidationError ValidateTagNs(string ns)
        {
            if (ns.IsNullOrWhitespace())
            {
                return new ApiValidationError(nameof(ns), ErrorKeys.ArgumentCanNotBeEmpty);
            }

            if (ns.Length > MaxNamespaceLength)
            {
                return new ApiValidationError(nameof(ns), ErrorKeys.ArgumentLengthExceedsMaximum);
            }

            if (!NamespaceRegex.IsMatch(ns))
            {
                return new ApiValidationError(nameof(ns), ErrorKeys.InvalidCharactersInArgument);
            }

            return null;
        }

        public ApiValidationError ValidateAbsoluteTagWeight(double weight)
        {
            if (weight < 0d)
            {
                return new ApiValidationError(nameof(weight), ErrorKeys.ArgumentCanNotBeNegative);
            }

            if (double.IsNaN(weight))
            {
                return new ApiValidationError(nameof(weight), ErrorKeys.ArgumentIsNotANumber);
            }

            if (double.IsPositiveInfinity(weight) || double.IsNegativeInfinity(weight))
            {
                return new ApiValidationError(nameof(weight), ErrorKeys.InfinityIsNotAllowed);
            }

            return null;
        }

        public ApiValidationError ValidateExistingFieldName(string fieldName)
        {
            var result = ValidateFieldName(fieldName);
            if (result != null)
            {
                return result;
            }

            return Tenant.DoesFieldExist(fieldName)
                ? null
                : new ApiValidationError(nameof(fieldName), ErrorKeys.FieldIsNotDefined);
        }

        public ApiValidationError ValidateFieldName(string fieldName)
        {
            if (fieldName.IsNullOrWhitespace())
            {
                return new ApiValidationError(nameof(fieldName), ErrorKeys.ArgumentCanNotBeEmpty);
            }

            if (fieldName.Length > MaxFieldNameLength)
            {
                return new ApiValidationError(nameof(fieldName), ErrorKeys.ArgumentLengthExceedsMaximum);
            }

            if (!FieldNameRegex.IsMatch(fieldName))
            {
                return new ApiValidationError(nameof(fieldName), ErrorKeys.InvalidCharactersInArgument);
            }

            return null;
        }

        public ApiValidationError ValidateFieldValue(double fieldValue)
        {
            if (double.IsNaN(fieldValue))
            {
                return new ApiValidationError(nameof(fieldValue), ErrorKeys.ArgumentIsNotANumber);
            }

            if (double.IsPositiveInfinity(fieldValue) || double.IsNegativeInfinity(fieldValue))
            {
                return new ApiValidationError(nameof(fieldValue), ErrorKeys.InfinityIsNotAllowed);
            }

            return null;
        }

        public ApiValidationError ValidateRange(string fieldName, int input, int min, int max)
        {
            if (input < min)
            {
                return new ApiValidationError(fieldName, ErrorKeys.ArgumentIsSmallerThanMinimum);
            }

            if (input > max)
            {
                return new ApiValidationError(fieldName, ErrorKeys.ArgumentIsLargerThanMaximum);
            }

            return null;
        }

        public bool HasDuplicates(IEnumerable<string> inputs)
        {
            if (inputs == null)
            {
                return false;
            }

            var hash = new HashSet<string>();
            return inputs.Any(input => !hash.Add(input));
        }

        public ApiValidationError ValidateTargetUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return new ApiValidationError(nameof(url), ErrorKeys.ArgumentCanNotBeEmpty);
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return new ApiValidationError(nameof(url), ErrorKeys.TargetUriIsInvalid);
            }

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                return new ApiValidationError(nameof(url), ErrorKeys.TargetUriSchemeIsInvalid);
            }

            return null;
        }

        public ApiValidationResult ValidateAccountQuery(AccountQuery query)
        {
            var numberOfConditions = 0;
            var result = ValidateAccountQuery(query, 0, ref numberOfConditions);

            if (numberOfConditions > 512)
            {
                result.Append(new ApiValidationError(ErrorKeys.QueryHasTooManyConditions));
            }

            return result;
        }

        private ApiValidationResult ValidateAccountQuery(AccountQuery query, int depth, ref int numberOfConditions)
        {
            if (query == null)
            {
                return ApiValidationResult.Failure(nameof(query), ErrorKeys.ArgumentCanNotBeEmpty);
            }

            if (depth >= 8)
            {
                return ApiValidationResult.Failure(ErrorKeys.QueryNestingIsTooDeep);
            }

            var result = new ApiValidationResult();

            foreach (var innerQuery in query.And.EmptyIfNull())
            {
                result.Append(ValidateAccountQuery(innerQuery, depth + 1, ref numberOfConditions));
            }

            foreach (var innerQuery in query.Or.EmptyIfNull())
            {
                result.Append(ValidateAccountQuery(innerQuery, depth + 1, ref numberOfConditions));
            }

            if (query.Not != null)
            {
                result.Append(ValidateAccountQuery(query.Not, depth + 1, ref numberOfConditions));
            }

            foreach (var fqTag in query.TaggedWithAll.EmptyIfNull())
            {
                numberOfConditions++;
                result.Append(ValidateExistingTagNs(fqTag.Ns));
                result.Append(ValidateTag(fqTag.Tag));
            }

            foreach (var fqTag in query.TaggedWithAny.EmptyIfNull())
            {
                numberOfConditions++;
                result.Append(ValidateExistingTagNs(fqTag.Ns));
                result.Append(ValidateTag(fqTag.Tag));
            }

            if (!string.IsNullOrWhiteSpace(query.TaggedInNs))
            {
                numberOfConditions++;
                result.Append(ValidateExistingTagNs(query.TaggedInNs));
            }

            if (query.FieldWithin != null)
            {
                numberOfConditions++;
                result.Append(ValidateExistingFieldName(query.FieldWithin.FieldName));

                if (query.FieldWithin.LowerBound.HasValue)
                {
                    result.Append(ValidateFieldValue(query.FieldWithin.LowerBound.Value));
                }

                if (query.FieldWithin.UpperBound.HasValue)
                {
                    result.Append(ValidateFieldValue(query.FieldWithin.UpperBound.Value));
                }
            }

            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using ComposerCore.Attributes;
using Nest;
using Tesseract.ApiModel.General;
using Tesseract.Common.Extensions;

namespace Tesseract.Core.Index.Implementation
{
    [Contract]
    [Component]
    public class EsQueryBuilder
    {
        public QueryContainer BuildEsQuery(QueryContainer query, AccountQuery accountQuery)
        {
            if (accountQuery == null)
            {
                throw new ArgumentNullException(nameof(accountQuery));
            }

            query = AddAllTagTermClauses(query, accountQuery.TaggedWithAll);
            query = AddAnyTagTermClauses(query, accountQuery.TaggedWithAny);
            query = AddTagNamespaceClause(query, accountQuery.TaggedInNs);
            query = AddFieldRangeClauses(query, accountQuery.FieldWithin);

            query = AddAndClauses(query, accountQuery.And);
            query = AddOrClauses(query, accountQuery.Or);
            query = AddNotClause(query, accountQuery.Not);

            return query;
        }

        private QueryContainer AddAllTagTermClauses(QueryContainer query, List<FqTag> tags)
        {
            foreach (var fqTag in tags.EmptyIfNull())
            {
                query &= new TermQuery {Field = IndexNaming.Namespace(fqTag.Ns), Value = fqTag.Tag};
            }

            return query;
        }

        private QueryContainer AddAnyTagTermClauses(QueryContainer query, List<FqTag> tags)
        {
            if (tags.SafeAny())
            {
                var innerQuery = new BoolQuery
                {
                    Should = tags.Select(t => (QueryContainer) new TermQuery
                    {
                        Field = IndexNaming.Namespace(t.Ns),
                        Value = t.Tag
                    })
                };

                return query & innerQuery;
            }

            return query;
        }

        private QueryContainer AddTagNamespaceClause(QueryContainer query, string tagNs)
        {
            if (!string.IsNullOrWhiteSpace(tagNs))
            {
                return query & new ExistsQuery {Field = IndexNaming.Namespace(tagNs)};
            }

            return query;
        }

        private QueryContainer AddFieldRangeClauses(QueryContainer query, AccountFieldQuery accountFieldQuery)
        {
            if (accountFieldQuery == null)
            {
                return query;
            }

            if (!accountFieldQuery.LowerBound.HasValue && !accountFieldQuery.UpperBound.HasValue)
            {
                return query & new ExistsQuery {Field = IndexNaming.Field(accountFieldQuery.FieldName)};
            }

            var innerQuery = new NumericRangeQuery
            {
                Field = IndexNaming.Field(accountFieldQuery.FieldName)
            };

            if (accountFieldQuery.LowerBound.HasValue)
            {
                innerQuery.GreaterThanOrEqualTo = accountFieldQuery.LowerBound.Value;
            }

            if (accountFieldQuery.UpperBound.HasValue)
            {
                innerQuery.LessThanOrEqualTo = accountFieldQuery.UpperBound.Value;
            }

            return query & innerQuery;
        }

        private QueryContainer AddAndClauses(QueryContainer query, List<AccountQuery> accountQueries)
        {
            foreach (var and in accountQueries.EmptyIfNull())
            {
                query = BuildEsQuery(query, and);
            }

            return query;
        }

        private QueryContainer AddOrClauses(QueryContainer query, List<AccountQuery> accountQueries)
        {
            if (!accountQueries.SafeAny())
            {
                return query;
            }

            var innerQueries = accountQueries
                .Select(aq => BuildEsQuery(null, aq))
                .Where(qc => qc != null)
                .ToList();

            if (innerQueries.Any())
            {
                return query & new BoolQuery {Should = innerQueries};
            }

            return query;
        }

        private QueryContainer AddNotClause(QueryContainer query, AccountQuery accountQuery)
        {
            if (accountQuery == null)
            {
                return query;
            }

            var innerQuery = BuildEsQuery(null, accountQuery);
            return query & !(innerQuery ?? new MatchAllQuery());
        }
    }
}
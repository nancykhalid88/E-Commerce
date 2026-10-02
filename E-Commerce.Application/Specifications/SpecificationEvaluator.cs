using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    public class SpecificationEvaluator
    {
        public static IQueryable<TEntity>CreateQuery <TEntity,TKey>(IQueryable<TEntity> inputQuery,ISpecifications<TEntity,TKey> Spec) where TEntity:BaseEntity<TKey>
        {
            var query = inputQuery;
            if(Spec.Criteria != null)
            {
                query = query.Where(Spec.Criteria);
            }
            if (Spec.IncludeExpressions.Any())
            {
                query = Spec.IncludeExpressions.Aggregate(query, (current, NextExp) => current.Include(NextExp));
            }
            if (Spec.OrderBy != null)
            {
                query = query.OrderBy(Spec.OrderBy);
            }
            if (Spec.OrderByDescending != null)
            {
                query = query.OrderBy(Spec.OrderByDescending);
            }
            if (Spec.IsPagenated)
            {
                query = query.Skip(Spec.Skip).Take(Spec.Take);
            }

            return query;
        }
    }
}

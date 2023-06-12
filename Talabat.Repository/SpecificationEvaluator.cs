using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    public static class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        // This Method Will Build Our Query
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            // Comments For Just Clarification
            var query = inputQuery; // query = _dbContext.Products

            if(spec.Criteria is not null) // P => P.Id == 1
                query = query.Where(spec.Criteria);

            if(spec.OrderBy is not null) // OrderBy(P => P.Price)
                query = query.OrderBy(spec.OrderBy);

            if (spec.OrderByDescending is not null) // OrderByDescending(P => P.Price)
                query = query.OrderByDescending(spec.OrderByDescending);

            if(spec.IsPaginationEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            // We Used Aggregate To Combine Query
            query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

            return query;
        }
    }
}

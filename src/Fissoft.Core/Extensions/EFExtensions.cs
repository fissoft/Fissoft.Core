using System;
using System.Linq;
using System.Linq.Expressions;

namespace Fissoft.Framework
{
    public static class EFExtensions
    {
        public static IQueryable<TDestination> SelectAndMapper<TDestination>(
            this IQueryable source,
            params Expression<Func<TDestination, object>>[] membersToExpand)
        {
            return AutoMapper.QueryableExtensions.Extensions.ProjectTo(
                source,
                MapperUtil.Mapper.ConfigurationProvider,
                null,
                membersToExpand
                );
        }
    }
}
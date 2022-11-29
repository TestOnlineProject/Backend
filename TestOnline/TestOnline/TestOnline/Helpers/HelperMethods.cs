using System.Linq.Expressions;

namespace TestOnline.Helpers;
public static class HelperMethods
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool searchCondition, Expression<Func<T, bool>> predicate)
    {
        return searchCondition ? query.Where(predicate) : query;
    }

    public static IQueryable<T> PageBy<T, Type>(this IQueryable<T> query, Expression<Func<T, Type>> orderBy, int page, int pageSize, bool orderByDescending = true) //add Tkey into PageBy if needed
    {
        const int defaultPageNumber = 1;

        if (query == null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        // Check if the page number is greater then zero - otherwise use default page number
        if (page <= 0)
        {
            page = defaultPageNumber;
        }

        // It is necessary sort items before it
        query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public static double GetPriceByQuantity(int quantity, double price, double price50, double price100)
    {
        if (quantity <= 50) return price;
        else
        {
            if (quantity <= 100) return price50;
            else
                return price100;
        }
    }
}

using System.Collections.Generic;

namespace Vortex.GenerativeArtSuite.Common.Extensions
{
    public static class IListExtensions
    {
        public static bool ContainsEqualItems<T>(this IList<T> collection, IList<T> other)
        {
            if (collection.Count != other.Count)
            {
                return false;
            }

            for (int i = 0; i < collection.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(collection[i], other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static void AddUnique<T>(this IList<T> collection, T item)
        {
            if (!collection.Contains(item))
            {
                collection.Add(item);
            }
        }
    }
}

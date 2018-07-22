
using System.Collections.Generic;
using System.Linq;

namespace TransactionalList.Test
{
    public static class ListExtensions
    {
        public static bool CompareList<T>(this List<T> baseList, params T[] paramsList)
        {
            var temp1 = baseList.All(x => paramsList.Any(y => CompareObj(x, y)));
            var temp2 = paramsList.All(x => baseList.Any(y => CompareObj(x, y)));

            return temp1 && temp2;
        }

        private static bool CompareObj<T>(T obj1, T obj2)
        {
            var props = typeof(T).GetProperties().Where(x => x.GetAccessors().Any(y => y.IsPublic));
            foreach (var prop in props)
            {
                var prop1 = prop.GetValue(obj1);
                var prop2 = prop.GetValue(obj2);
                if (prop1?.Equals(prop2) == false)
                    return false;
            }

            return true;
        }
    }
}

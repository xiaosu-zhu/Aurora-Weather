using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.Shared.Extensions
{
    public static class EnumrableExtesion
    {
        public static bool IsNullorEmpty(this ICollection e)
        {
            return (e == null || e.Count == 0);
        }

        public static bool IsNullorEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }
    }
}

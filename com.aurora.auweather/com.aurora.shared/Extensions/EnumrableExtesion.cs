// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.Shared.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Com.Aurora.Shared.Extensions
{
    public static class EnumrableExtesion
    {
        public static bool IsNullorEmpty(this ICollection collection)
        {
            return (collection == null || collection.Count == 0);
        }

        public static bool IsNullorEmpty(this IDictionary list)
        {
            return (list == null || list.Count == 0);
        }

        public static bool IsNullorEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }

        public static string SelectRandomString(this string[] s)
        {
            if (!s.IsNullorEmpty())
            {
                var index = Tools.Random.Next(s.Length);
                return s[index];
            }
            return null;
        }
    }
}

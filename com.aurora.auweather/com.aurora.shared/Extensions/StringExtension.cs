// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;

namespace Com.Aurora.Shared.Extensions
{
    public static class StringExtension
    {
        public static string Reverse(this string s)
        {
            return new string(s.ToCharArray().Reverse().ToArray());
        }

        public static long HextoDec(this string s)
        {
            var c = s.ToCharArray();
            long res = 0, k = 1;
            for (int i = c.Length - 1; i >= 0; i--)
            {
                res += (Array.IndexOf(LongExtension.HexSet, c[i])) * k;
                k *= 16;
            }
            return res;
        }

        public static bool IsNullorEmpty(this string s)
        {
            return (s == null || s == "" || s == string.Empty);
        }

        public static long OcttoDec(this string s)
        {
            var c = s.ToCharArray();
            long res = 0, k = 1;
            for (int i = c.Length - 1; i >= 0; i--)
            {
                res += (Array.IndexOf(LongExtension.HexSet, c[i])) * k;
                k *= 8;
            }
            return res;
        }

        public static long BintoDec(this string s)
        {
            var c = s.ToCharArray();
            long res = 0, k = 1;
            for (int i = c.Length - 1; i >= 0; i--)
            {
                res += (Array.IndexOf(LongExtension.HexSet, c[i])) * k;
                k *= 2;
            }
            return res;
        }

    }
}

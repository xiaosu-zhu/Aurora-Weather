// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace UnitTest
{
    internal class ReadWrtieSettingGroupTestClass
    {
        public ReadWrtieSettingGroupTestClass()
        {

        }

        public ReadWrtieSettingGroupTestClass(TimeSpan timeSpan, int v1, string v2, double v3, bool v4, DateTime v5, DateTime[] v6, string[] v7)
        {
            TimeSpan = timeSpan;
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
            V5 = v5;
            V6 = v6;
            V7 = v7;
        }

        public TimeSpan TimeSpan
        {
            get; private set;
        }

        public int V1
        {
            get; private set;
        }

        public string V2
        {
            get; private set;
        }

        public double V3
        {
            get; private set;
        }

        public bool V4
        {
            get; private set;
        }
        public DateTime V5
        {
            get; private set;
        }
        public DateTime[] V6
        {
            get; private set;
        }
        public string[] V7
        {
            get; private set;
        }
    }
}
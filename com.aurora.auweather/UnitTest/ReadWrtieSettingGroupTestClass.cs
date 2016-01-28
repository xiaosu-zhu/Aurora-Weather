using System;

namespace UnitTest
{
    internal class ReadWrtieSettingGroupTestClass
    {
        private TimeSpan timeSpan;
        private int v1;
        private string v2;
        private double v3;
        private bool v4;

        public ReadWrtieSettingGroupTestClass(int v1, string v2, double v3, bool v4, TimeSpan timeSpan)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
            this.V4 = v4;
            this.TimeSpan = timeSpan;
        }

        public ReadWrtieSettingGroupTestClass()
        {

        }

        public TimeSpan TimeSpan
        {
            get
            {
                return timeSpan;
            }

            set
            {
                timeSpan = value;
            }
        }

        public int V1
        {
            get
            {
                return v1;
            }

            set
            {
                v1 = value;
            }
        }

        public string V2
        {
            get
            {
                return v2;
            }

            set
            {
                v2 = value;
            }
        }

        public double V3
        {
            get
            {
                return v3;
            }

            set
            {
                v3 = value;
            }
        }

        public bool V4
        {
            get
            {
                return v4;
            }

            set
            {
                v4 = value;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Models
{
    public class Speed
    {
        private double kmph;

        public double KMPH
        {
            get
            {
                return kmph;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                kmph = value;
            }
        }

        public double MPS
        {
            get
            {
                return kmph / 3.6;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                kmph = value * 3.6;
            }
        }

        public double Knot
        {
            get
            {
                return kmph / 1.852;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                kmph = value * 1.852;
            }
        }

        public static Speed FromKMPH(double kmph)
        {
            var speed = new Speed();
            speed.KMPH = kmph;
            return speed;
        }

        public static Speed FromMPS(double mps)
        {
            var speed = new Speed();
            speed.MPS = mps;
            return speed;
        }

        public static Speed FromKnot(double knot)
        {
            var speed = new Speed();
            speed.Knot = knot;
            return speed;
        }
    }
}

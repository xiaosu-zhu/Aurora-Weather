using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.aurora.auweather.Models
{
    internal class Wind
    {
        private uint degree;
        private WindDirection direction;
        private WindScale scale;
        private uint speed;

        public uint Degree
        {
            get
            {
                return degree;
            }

            set
            {
                if (value > 360)
                    throw new ArgumentOutOfRangeException();
                else
                {
                    degree = value;
                    if (degree < 16)
                        Direction = WindDirection.north;
                    else if (degree < 75)
                        Direction = WindDirection.northeast;
                    else if (degree < 106)
                        Direction = WindDirection.east;
                    else if (degree < 165)
                        Direction = WindDirection.southeast;
                    else if (degree < 196)
                        Direction = WindDirection.south;
                    else if (degree < 255)
                        Direction = WindDirection.southwest;
                    else if (degree < 286)
                        Direction = WindDirection.west;
                    else if (degree < 345)
                        Direction = WindDirection.northwest;
                    else if (degree < 361)
                        Direction = WindDirection.north;
                    else Direction = WindDirection.unknown;
                }
            }
        }

        public uint Speed
        {
            get
            {
                return speed;
            }

            set
            {
                if (value >= 221)
                    throw new ArgumentOutOfRangeException();
                else
                {
                    speed = value;
                    if (speed < 3)
                        Scale = WindScale.zero;
                    else if (speed < 7)
                        Scale = WindScale.one;
                    else if (speed < 13)
                        Scale = WindScale.two;
                    else if (speed < 20)
                        Scale = WindScale.three;
                    else if (speed < 31)
                        Scale = WindScale.four;
                    else if (speed < 41)
                        Scale = WindScale.five;
                    else if (speed < 52)
                        Scale = WindScale.six;
                    else if (speed < 63)
                        Scale = WindScale.seven;
                    else if (speed < 76)
                        Scale = WindScale.eight;
                    else if (speed < 88)
                        Scale = WindScale.nine;
                    else if (speed < 104)
                        Scale = WindScale.ten;
                    else if (speed < 118)
                        Scale = WindScale.eleven;
                    else if (speed < 133)
                        Scale = WindScale.twelve;
                    else if (speed < 150)
                        Scale = WindScale.thirteen;
                    else if (speed < 167)
                        Scale = WindScale.fourteen;
                    else if (speed < 184)
                        Scale = WindScale.fifteen;
                    else if (speed < 202)
                        Scale = WindScale.sixteen;
                    else if (speed < 221)
                        Scale = WindScale.seventeen;
                    else
                        Scale = WindScale.unknown;
                }

            }
        }

        internal WindDirection Direction
        {
            get
            {
                return direction;
            }

            set
            {
                direction = value;
            }
        }

        internal WindScale Scale
        {
            get
            {
                return scale;
            }

            set
            {
                scale = value;
            }
        }

        public Wind(HeWeather.JsonContract.WindContract wind)
        {
            Degree = uint.Parse(wind.deg);
            Speed = uint.Parse(wind.spd);
        }
    }
}

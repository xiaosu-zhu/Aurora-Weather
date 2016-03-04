using System;

namespace Com.Aurora.AuWeather.Models
{
    public class Wind
    {
        private uint degree;
        private WindDirection direction;
        private WindScale scale;
        private Speed speed;

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

        public Speed Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = value;
                if (speed.KMPH < 3)
                    Scale = WindScale.zero;
                else if (speed.KMPH < 7)
                    Scale = WindScale.one;
                else if (speed.KMPH < 13)
                    Scale = WindScale.two;
                else if (speed.KMPH < 20)
                    Scale = WindScale.three;
                else if (speed.KMPH < 31)
                    Scale = WindScale.four;
                else if (speed.KMPH < 41)
                    Scale = WindScale.five;
                else if (speed.KMPH < 52)
                    Scale = WindScale.six;
                else if (speed.KMPH < 63)
                    Scale = WindScale.seven;
                else if (speed.KMPH < 76)
                    Scale = WindScale.eight;
                else if (speed.KMPH < 88)
                    Scale = WindScale.nine;
                else if (speed.KMPH < 104)
                    Scale = WindScale.ten;
                else if (speed.KMPH < 118)
                    Scale = WindScale.eleven;
                else if (speed.KMPH < 133)
                    Scale = WindScale.twelve;
                else if (speed.KMPH < 150)
                    Scale = WindScale.thirteen;
                else if (speed.KMPH < 167)
                    Scale = WindScale.fourteen;
                else if (speed.KMPH < 184)
                    Scale = WindScale.fifteen;
                else if (speed.KMPH < 202)
                    Scale = WindScale.sixteen;
                else if (speed.KMPH < 221)
                    Scale = WindScale.seventeen;
                else
                    Scale = WindScale.eighteen;

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
            Speed = Speed.FromKMPH(uint.Parse(wind.spd));
        }
    }
}

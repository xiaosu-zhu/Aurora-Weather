using Com.Aurora.AuWeather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Core.LunarCalendar
{
    public class SunRiseSet
    {
        #region 计算天文参数
        private readonly static int[] days_of_month_1 = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        private readonly static int[] days_of_month_2 = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        private const double h = -0.833;//日出日落时太阳的位置

        private const double UTo = 180.0;//上次计算的日落日出时间，初始迭代值180.0

        //输入日期

        //输入经纬度

        //判断是否为闰年：若为闰年，返回1；若不是闰年,返回0

        public static bool leap_year(int year)
        {

            if (((year % 400 == 0) || (year % 100 != 0) && (year % 4 == 0))) return true;

            else return false;

        }

        //求从格林威治时间公元2000年1月1日到计算日天数days 

        private static int days(int year, int month, int date)
        {

            int i, a = 0;

            for (i = 2000; i < year; i++)
            {

                if (leap_year(i)) a = a + 366;

                else a = a + 365;

            }

            if (leap_year(year))
            {

                for (i = 0; i < month - 1; i++)
                {

                    a = a + days_of_month_2[i];

                }

            }
            else
            {

                for (i = 0; i < month - 1; i++)
                {

                    a = a + days_of_month_1[i];

                }

            }

            a = a + date;

            return a;

        }

        //求格林威治时间公元2000年1月1日到计算日的世纪数t 

        private static double t_century(int days, double UTo)
        {

            return ((double)days + UTo / 360) / 36525;

        }

        //求太阳的平黄径

        private static double L_sun(double t_century)
        {

            return (280.460 + 36000.770 * t_century);

        }

        //求太阳的平近点角

        private static double G_sun(double t_century)
        {

            return (357.528 + 35999.050 * t_century);

        }

        //求黄道经度

        private static double ecliptic_longitude(double L_sun, double G_sun)
        {

            return (L_sun + 1.915 * Math.Sin(G_sun * Math.PI / 180) + 0.02 * Math.Sin(2 * G_sun * Math.PI / 180));

        }

        //求地球倾角

        private static double earth_tilt(double t_century)
        {

            return (23.4393 - 0.0130 * t_century);

        }

        //求太阳偏差

        private static double sun_deviation(double earth_tilt, double ecliptic_longitude)
        {

            return (180 / Math.PI * Math.Asin(Math.Sin(Math.PI / 180 * earth_tilt) * Math.Sin(Math.PI / 180 * ecliptic_longitude)));

        }

        //求格林威治时间的太阳时间角GHA

        private static double GHA(double UTo, double G_sun, double ecliptic_longitude)
        {

            return (UTo - 180 - 1.915 * Math.Sin(G_sun * Math.PI / 180) - 0.02 * Math.Sin(2 * G_sun * Math.PI / 180) + 2.466 * Math.Sin(2 * ecliptic_longitude * Math.PI / 180) - 0.053 * Math.Sin(4 * ecliptic_longitude * Math.PI / 180));

        }

        //求修正值e

        private static double e(double h, double glat, double sun_deviation)
        {

            return 180 / Math.PI * Math.Acos((Math.Sin(h * Math.PI / 180) - Math.Sin(glat * Math.PI / 180) * Math.Sin(sun_deviation * Math.PI / 180)) / (Math.Cos(glat * Math.PI / 180) * Math.Cos(sun_deviation * Math.PI / 180)));

        }
        #endregion

        //求日出时间

        private static double UT_rise(double UTo, double GHA, double glong, double e)
        {

            return (UTo - (GHA + glong + e));

        }

        //求日落时间

        private static double UT_set(double UTo, double GHA, double glong, double e)
        {

            return (UTo - (GHA + glong - e));

        }

        //判断并返回结果（日出）

        private static double result_rise(double UT, double UTo, double glong, double glat, int year, int month, int date)
        {

            double d;

            if (UT >= UTo) d = UT - UTo;

            else d = UTo - UT;

            if (d >= 0.5)
            {

                UTo = UT;

                UT = UT_rise(UTo,

                    GHA(UTo, G_sun(t_century(days(year, month, date), UTo)),

                            ecliptic_longitude(L_sun(t_century(days(year, month, date), UTo)),

                            G_sun(t_century(days(year, month, date), UTo)))),

                glong,

                e(h, glat, sun_deviation(earth_tilt(t_century(days(year, month, date), UTo)),

                    ecliptic_longitude(L_sun(t_century(days(year, month, date), UTo)),

                    G_sun(t_century(days(year, month, date), UTo))))));

                result_rise(UT, UTo, glong, glat, year, month, date);
            }

            return UT;

        }

        //判断并返回结果（日落）

        private static double result_set(double UT, double UTo, double glong, double glat, int year, int month, int date)
        {

            double d;

            if (UT >= UTo) d = UT - UTo;

            else d = UTo - UT;

            if (d >= 0.5)
            {

                UTo = UT;

                UT = UT_set(UTo,

                  GHA(UTo, G_sun(t_century(days(year, month, date), UTo)),

                  ecliptic_longitude(L_sun(t_century(days(year, month, date), UTo)),

                  G_sun(t_century(days(year, month, date), UTo)))),

                  glong,

                  e(h, glat, sun_deviation(earth_tilt(t_century(days(year, month, date), UTo)),

                  ecliptic_longitude(L_sun(t_century(days(year, month, date), UTo)),

                  G_sun(t_century(days(year, month, date), UTo))))));

                result_set(UT, UTo, glong, glat, year, month, date);

            }

            return UT;

        }

        //求时区

        public static int Zone(double glong)
        {

            if (glong >= 0) return ((int)(glong / 15.0) + 1);

            else return ((int)(glong / 15.0) - 1);

        }

        //打印结果 

        // public static void output(double rise, double set, double glong){  

        //     if((int)(60*(rise/15+Zone(glong)-(int)(rise/15+Zone(glong))))<10)  

        //         System.out.println("The time at which the sunrise is: "+(int)(rise/15+Zone(glong))+":"+(int)(60*(rise/15+Zone(glong)-(int)(rise/15+Zone(glong))))+" .\n");  

        //     else System.out.println("The time at which the sunrise is: "+(int)(rise/15+Zone(glong))+":"+(int)(60*(rise/15+Zone(glong)-(int)(rise/15+Zone(glong))))+" .\n");

        //      

        //     if((int)(60*(set/15+Zone(glong)-(int)(set/15+Zone(glong))))<10) 

        //         System.out.println("The time at which the sunset is: "+(int)(set/15+Zone(glong))+": "+(int)(60*(set/15+Zone(glong)-(int)(set/15+Zone(glong))))+" .\n");  

        //     else System.out.println("The time at which the sunset is: "+(int)(set/15+Zone(glong))+":"+(int)(60*(set/15+Zone(glong)-(int)(set/15+Zone(glong))))+" .\n"); 

        // } 



        public static TimeSpan GetRise(Location geoPoint, DateTime desiredTime)
        {

            double sunrise, glong, glat;

            int year, month, date;

            year = desiredTime.Year;

            month = desiredTime.Month;

            date = desiredTime.Day;

            glong = geoPoint.Longitude;

            glat = geoPoint.Latitude;

            sunrise = result_rise(UT_rise(UTo,

                             GHA(UTo, G_sun(t_century(days(year, month, date), UTo)),

                                    ecliptic_longitude(L_sun(t_century(days(year, month, date), UTo)),

                                    G_sun(t_century(days(year, month, date), UTo)))),

                        glong,

                        e(h, glat, sun_deviation(earth_tilt(t_century(days(year, month, date), UTo)),

                        ecliptic_longitude(L_sun(t_century(days(year, month, date), UTo)),

                    G_sun(t_century(days(year, month, date), UTo)))))), UTo, glong, glat, year, month, date);

            return new TimeSpan((int)(sunrise / 15 + (DateTime.Now - DateTime.UtcNow).Hours), (int)(60 * (sunrise / 15 + (DateTime.Now - DateTime.UtcNow).Hours - (int)(sunrise / 15 + (DateTime.Now - DateTime.UtcNow).Hours))) - 1, 0);
        }

        public static TimeSpan GetSet(Location geoPoint, DateTime desiredTime)
        {

            double sunset, glong, glat;

            int year, month, date;

            year = desiredTime.Year;

            month = desiredTime.Month;

            date = desiredTime.Day;

            glong = geoPoint.Longitude;

            glat = geoPoint.Latitude;

            sunset = result_set(UT_set(UTo,

                            GHA(UTo, G_sun(t_century(days(year, month, date), UTo)),

                              ecliptic_longitude(L_sun(t_century(days(year, month, date), UTo)),

                           G_sun(t_century(days(year, month, date), UTo)))),

                            glong,

                            e(h, glat, sun_deviation(earth_tilt(t_century(days(year, month, date), UTo)),

                              ecliptic_longitude(L_sun(t_century(days(year, month, date), UTo)),

                           G_sun(t_century(days(year, month, date), UTo)))))), UTo, glong, glat, year, month, date);
            return new TimeSpan((int)(sunset / 15 + (DateTime.Now - DateTime.UtcNow).Hours), (int)(60 * (sunset / 15 + (DateTime.Now - DateTime.UtcNow).Hours - (int)(sunset / 15 + (DateTime.Now - DateTime.UtcNow).Hours))) - 1, 0);
        }

    }
}

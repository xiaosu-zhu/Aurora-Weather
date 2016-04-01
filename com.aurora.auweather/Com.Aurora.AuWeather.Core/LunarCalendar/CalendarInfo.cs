// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Com.Aurora.AuWeather.LunarCalendar
{
    public class CalendarInfo
    {

        /// <summary>
        /// 内部公历
        /// </summary>
        private DateTime m_SolarDate;

        /// <summary>
        /// 内部阴历年月日
        /// </summary>
        private int m_LunarYear, m_LunarMonth, m_LunarDay;

        /// <summary>
        /// 内部闰月标志
        /// </summary>
        private bool m_IsLeapMonth;

        /// <summary>
        /// 天干地支，生肖
        /// </summary>
        private string m_LunarYearSexagenary, m_LunarYearAnimal;

        /// <summary>
        /// 阴历年月日显示
        /// </summary>
        private string m_LunarYearText, m_LunarMonthText, m_LunarDayText;

        /// <summary>
        /// 节气序号
        /// </summary>
        private int m_SolarTermNum = -1;

        /// <summary>
        /// 阳历星期，星座，生日石显示
        /// </summary>
        private string m_SolarWeekText;
        private int m_SolarConstellationNum;

        #region Constructor

        /// <summary>
        /// 默认构造体传入当前日期
        /// </summary>
        public CalendarInfo()
          : this(DateTime.Now.Date)
        {

        }

        /// <summary>
        /// 生成指定日期的公历和阴历
        /// </summary>
        /// <param name="date">specific date</param>
        public CalendarInfo(DateTime date)
        {
            m_SolarDate = date;
            LoadFromSolarDate();
        }

        private void LoadFromSolarDate()
        {
            m_IsLeapMonth = false;
            m_LunarYearSexagenary = null;
            m_LunarYearAnimal = null;
            m_LunarYearText = null;
            m_LunarMonthText = null;
            m_LunarDayText = null;
            m_SolarWeekText = null;

            // 使用内部库获取农历年月日
            m_LunarYear = calendar.GetYear(m_SolarDate);
            m_LunarMonth = calendar.GetMonth(m_SolarDate);
            m_LunarDay = calendar.GetDayOfMonth(m_SolarDate);
            //获取节气
            m_SolarTermNum = SolarTerm.GetSolarTerm(m_SolarDate);
            // 获取这一年的闰月
            int leapMonth = calendar.GetLeapMonth(m_LunarYear, m_SolarDate.Year % 100 / 10);

            if (leapMonth == m_LunarMonth)
            {
                // 如果当前月是闰月，实际月份与上一月相同
                m_IsLeapMonth = true;
                m_LunarMonth -= 1;
            }
            else if (leapMonth > 0 && leapMonth < m_LunarMonth)
            {
                // 闰月之后的月均减一
                m_LunarMonth -= 1;
            }

            CalcConstellation(m_SolarDate);
        }

        #endregion

        #region Solar 

        /// <summary>
        /// 公历日期，赋值后自动计算
        /// </summary>
        public DateTime SolarDate
        {
            get { return m_SolarDate; }
            set
            {
                if (m_SolarDate.Equals(value))
                    return;
                m_SolarDate = value;
                LoadFromSolarDate();
            }
        }
        /// <summary>
        /// 星期几
        /// </summary>
        public string SolarWeekText
        {
            get
            {
                if (string.IsNullOrEmpty(m_SolarWeekText))
                {
                    int i = (int)m_SolarDate.DayOfWeek;
                    m_SolarWeekText = ChineseWeekName[i];
                }
                return m_SolarWeekText;
            }
        }
        /// <summary>
        /// 星座字符串
        /// </summary>
        public string SolarConstellation
        {
            get { return m_SolarConstellationNum == -1 ? null : Constellations[m_SolarConstellationNum]; }
        }
        /// <summary>
        /// 生日石字符串
        /// </summary>
        public string SolarBirthStone
        {
            get { return BirthStones[m_SolarDate.Month - 1]; }
        }

        /// <summary>
        /// 星座对应编号
        /// </summary>
        public int SolarConstellationNum
        {
            get { return m_SolarConstellationNum; }
        }

        /// <summary>
        /// 农历年
        /// </summary>
        public int LunarYear
        {
            get { return m_LunarYear; }
        }
        /// <summary>
        /// 农历月
        /// </summary>
        public int LunarMonth
        {
            get { return m_LunarMonth; }
        }
        /// <summary>
        /// 闰月
        /// </summary>
        public bool IsLeapMonth
        {
            get { return m_IsLeapMonth; }
        }
        /// <summary>
        /// 农历日
        /// </summary>
        public int LunarDay
        {
            get { return m_LunarDay; }
        }

        /// <summary>
        /// 农历干支
        /// </summary>
        public string LunarYearSexagenary
        {
            get
            {
                if (string.IsNullOrEmpty(m_LunarYearSexagenary))
                {
                    int y = calendar.GetSexagenaryYear(this.SolarDate);
                    m_LunarYearSexagenary = CelestialStem.Substring((y - 1) % 10, 1) + TerrestrialBranch.Substring((y - 1) % 12, 1);
                }
                return m_LunarYearSexagenary;
            }
        }
        /// <summary>
        /// 农历生肖
        /// </summary>
        public string LunarYearAnimal
        {
            get
            {
                if (string.IsNullOrEmpty(m_LunarYearAnimal))
                {
                    int y = calendar.GetSexagenaryYear(this.SolarDate);
                    m_LunarYearAnimal = Animals.Substring((y - 1) % 12, 1);
                }
                return m_LunarYearAnimal;
            }
        }


        /// <summary>
        /// 农历年（大写）
        /// </summary>
        public string LunarYearText
        {
            get
            {
                if (string.IsNullOrEmpty(m_LunarYearText))
                {
                    m_LunarYearText = Animals.Substring(calendar.GetSexagenaryYear(new DateTime(m_LunarYear, 1, 1)) % 12 - 1, 1);
                    StringBuilder sb = new StringBuilder();
                    int year = this.LunarYear;
                    int d;
                    do
                    {
                        d = year % 10;
                        sb.Insert(0, ChineseNumber[d]);
                        year = year / 10;
                    } while (year > 0);
                    m_LunarYearText = sb.ToString();
                }
                return m_LunarYearText;
            }
        }
        /// <summary>
        /// 农历月（正-腊）
        /// </summary>
        public string LunarMonthText
        {
            get
            {
                if (string.IsNullOrEmpty(m_LunarMonthText))
                {
                    m_LunarMonthText = (this.IsLeapMonth ? "Leap Month" : "") + ChineseMonthName[this.LunarMonth - 1];
                }
                return m_LunarMonthText;
            }
        }

        /// <summary>
        /// 农历日
        /// </summary>
        public string LunarDayText
        {
            get
            {
                if (string.IsNullOrEmpty(m_LunarDayText))
                    m_LunarDayText = ChineseDayName[this.LunarDay - 1];
                return m_LunarDayText;
            }
        }

        /// <summary>
        /// 节气字符串
        /// </summary>
        public string SolarTermStr
        {
            get
            {
                return m_SolarTermNum == -1 ? null : SolarTerm.SolarTermString[m_SolarTermNum];
            }
        }

        public int SolarTermNum
        {
            get
            {
                return m_SolarTermNum;
            }
        }

        #endregion

        /// <summary>
        /// 获取指定日期的星座和生日石
        /// </summary>
        /// <param name="date">specifica date</param>
        /// <param name="constellation">constellation</param>
        /// <param name="birthstone">birthstone</param>
        public void CalcConstellation(DateTime date)
        {
            // 将X月Y日换为一个整数XY
            int i = Convert.ToInt32(date.ToString("MMdd"));
            int j;
            // 判断星座
            if (i >= 321 && i <= 419)
                j = 0;
            else if (i >= 420 && i <= 520)
                j = 1;
            else if (i >= 521 && i <= 621)
                j = 2;
            else if (i >= 622 && i <= 722)
                j = 3;
            else if (i >= 723 && i <= 822)
                j = 4;
            else if (i >= 823 && i <= 922)
                j = 5;
            else if (i >= 923 && i <= 1023)
                j = 6;
            else if (i >= 1024 && i <= 1121)
                j = 7;
            else if (i >= 1122 && i <= 1221)
                j = 8;
            else if (i >= 1222 || i <= 119)
                j = 9;
            else if (i >= 120 && i <= 218)
                j = 10;
            else if (i >= 219 && i <= 320)
                j = 11;
            else
            {
                m_SolarConstellationNum = -1;
                return;
            }
            // 换为对应字符串
            m_SolarConstellationNum = j;
        }


        #region convert Lunar to Solar

        /// <summary>
        /// 根据指定公历年查找农历新年
        /// </summary>
        /// <param name="year">specifica year</param>
        private static DateTime GetLunarNewYearDate(int year)
        {
            DateTime dt = new DateTime(year, 1, 1);
            int cnYear = calendar.GetYear(dt);
            int cnMonth = calendar.GetMonth(dt);

            int num1 = 0;
            int num2 = calendar.IsLeapYear(cnYear) ? 13 : 12;

            while (num2 >= cnMonth)
            {
                num1 += calendar.GetDaysInMonth(cnYear, num2--);
            }

            num1 = num1 - calendar.GetDayOfMonth(dt) + 1;
            return dt.AddDays(num1);
        }

        /// <summary>
        /// 农历转公历
        /// </summary>
        /// <param name="year">Lunar Year</param>
        /// <param name="month">Lunar Month</param>
        /// <param name="day">Lunar day</param>
        /// <param name="IsLeapMonth">IsLeapMonth</param>
        public static DateTime GetDateFromLunarDate(int year, int month, int day, bool IsLeapMonth)
        {
            if (year < 1902 || year > 2100)
                throw new Exception("Year Only Between 1902 And 2100");
            if (month < 1 || month > 12)
                throw new Exception("Month Only Between 1 And 12");

            if (day < 1 || day > calendar.GetDaysInMonth(year, month))
                throw new Exception("Wrong Date");

            int num1 = 0, num2 = 0;
            int leapMonth = calendar.GetLeapMonth(year, year % 100 / 10);

            if (((leapMonth == month + 1) && IsLeapMonth) || (leapMonth > 0 && leapMonth <= month))
                num2 = month;
            else
                num2 = month - 1;

            while (num2 > 0)
            {
                num1 += calendar.GetDaysInMonth(year, num2--);
            }

            DateTime dt = GetLunarNewYearDate(year);
            return dt.AddDays(num1 + day - 1);
        }

        /// <summary>
        /// Lunar To Solar
        /// </summary>
        /// <param name="date">Lunar Date</param>
        /// <param name="IsLeapMonth">IsLeapMonth</param>
        public static DateTime GetDateFromLunarDate(DateTime date, bool IsLeapMonth)
        {
            return GetDateFromLunarDate(date.Year, date.Month, date.Day, IsLeapMonth);
        }

        #endregion

        #region create calendarInfo 

        /// <summary>
        /// create calendarInfo of Lunar
        /// </summary>
        /// <param name="year">Lunar Year</param>
        /// <param name="month">Lunar Month</param>
        /// <param name="day">Lunar Date</param>
        /// <param name="IsLeapMonth">IsLeapMonth</param>
        public static CalendarInfo FromLunarDate(int year, int month, int day, bool IsLeapMonth)
        {
            DateTime dt = GetDateFromLunarDate(year, month, day, IsLeapMonth);
            return new CalendarInfo(dt);
        }
        /// <summary>
        /// create calendarInfo of Lunar
        /// </summary>
        /// <param name="date">Lunar Date</param>
        /// <param name="IsLeapMonth">IsLeapMonth</param>
        public static CalendarInfo FromLunarDate(DateTime date, bool IsLeapMonth)
        {
            return FromLunarDate(date.Year, date.Month, date.Day, IsLeapMonth);
        }

        /// <summary>
        /// create calendarInfo of Lunar
        /// </summary>
        /// <param name="date">ie: 20070209</param>
        /// <param name="IsLeapMonth">IsLeapMonth</param>
        public static CalendarInfo FromLunarDate(string date, bool IsLeapMonth)
        {
            Regex rg = new Regex(@"^\d{7}(\d)$");
            Match mc = rg.Match(date);
            if (!mc.Success)
            {
                throw new Exception("Date Fomat Wrong");
            }
            DateTime dt = DateTime.Parse(string.Format("{0}-{1}-{2}", date.Substring(0, 4), date.Substring(4, 2), date.Substring(6, 2)));
            return FromLunarDate(dt, IsLeapMonth);
        }


        #endregion

        private static ChineseLunisolarCalendar calendar = new ChineseLunisolarCalendar();
        public const string ChineseNumber = "〇一二三四五六七八九";
        public const string CelestialStem = "甲乙丙丁戊己庚辛壬癸";
        public const string TerrestrialBranch = "子丑寅卯辰巳午未申酉戌亥";
        public const string Animals = "鼠牛虎兔龙蛇马羊猴鸡狗猪";
        public static readonly string[] ChineseWeekName = new string[] { "星期天", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
        public static readonly string[] ChineseDayName = new string[] {
      "初一","初二","初三","初四","初五","初六","初七","初八","初九","初十",
      "十一","十二","十三","十四","十五","十六","十七","十八","十九","二十",
      "廿一","廿二","廿三","廿四","廿五","廿六","廿七","廿八","廿九","三十"};
        public static readonly string[] ChineseMonthName = new string[] { "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二" };
        public static readonly string[] Constellations = new string[] { "白羊座", "金牛座", "双子座", "巨蟹座", "狮子座", "处女座", "天秤座", "天蝎座", "射手座", "摩羯座", "水瓶座", "双鱼座" };
        public static readonly string[] BirthStones = new string[] { "石榴石", "紫水晶", "海蓝宝石", "钻石", "祖母绿", "月光石", "红宝石", "橄榄石", "蓝宝石", "蛋白石", "黄宝石", "萤石" };

    }
}

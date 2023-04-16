using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        private static string[] Days = { "یک شنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنج شنبه", "جمعه", "شنبه" };
        private static string[] Months = { "فروردین", "اریبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };

        private static string[] yakan = new string[10] { "صفر", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        private static string[] dahgan = new string[10] { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        private static string[] dahyek = new string[10] { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        private static string[] sadgan = new string[10] { "", "یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        private static string[] basex = new string[5] { "", "هزار", "میلیون", "میلیارد", "تریلیون" };

        private static PersianCalendar pc = new PersianCalendar();

        public static string ToPersianDateString(this DateTime date)
        {
            return ($"{pc.GetYear(date).ToString("0000")}/{pc.GetMonth(date).ToString("00")}/{pc.GetDayOfMonth(date).ToString("00")}");
        }


        public static string ToPersianMonthYearString(this DateTime date)
        {
            return ($"{Months[pc.GetMonth(date) - 1]}{ pc.GetYear(date)}");
        }


        public static string ToPersianDayMonthYearString(this DateTime date)
        {
            return ($"{pc.GetDayOfMonth(date)} {Months[pc.GetMonth(date) - 1]} { pc.GetYear(date).ToString("00")}");
        }

        public static string ToPersianDayString(this DateTime date)
        {
            return ($"{Days[pc.GetDayOfWeek(date).GetHashCode()]}");
        }

        public static string PersianToEnglish(this string strNum)
        {
            string[] pn = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string[] en = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string[] an = { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };
            var cache = strNum;
            for (var i = 0; i < 10; i++)
            {
                cache = cache.Replace(pn[i], en[i]);
                cache = cache.Replace(an[i], en[i]);
            }
            return cache;
        }

        public static string EnglishToPersian(this string data)

        {

            for (int i = 48; i < 58; i++)

            {

                data = data.Replace(Convert.ToChar(i), Convert.ToChar(1728 + i));

            }

            return data;



        }

        public static DateTime StringToDateTime(this string strDate)
        {

            int year = Int32.Parse(string.Join("",
                strDate.Split('/')[0].Select(c => char.GetNumericValue(c))));
            int month = Int32.Parse(string.Join("",
               strDate.Split('/')[1].Select(c => char.GetNumericValue(c))));
            int day = Int32.Parse(string.Join("",
                strDate.Split('/')[2].Select(c => char.GetNumericValue(c))));

            DateTime time = DateTime.Now;
            DateTime datetime = new DateTime(year, month, day, time.Hour, time.Minute, time.Second, time.Millisecond, new PersianCalendar());
            return datetime;
        }


        public static string FixPersianChars(this string str)
        {
            if (str == null) { return str; }
            return str.Replace("ﮎ", "ک")
                .Replace("ﮏ", "ک")
                .Replace("ﮐ", "ک")
                .Replace("ﮑ", "ک")
                .Replace("ك", "ک")
                .Replace("ي", "ی")
                .Replace(" ", " ")
                .Replace("‌", " ")
                .Replace("ھ", "ه");//.Replace("ئ", "ی");
        }

        public static string GenerateSlug(this string name)
        {
            string phrase = string.Format(name);
            string str = phrase.ToLower();

            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-");

            return str;
        }


        
        public static string Number2String(this string snum)
        {
            string stotal = "";
            if (snum == "") return "صفر";
            if (snum == "0")
            {
                return yakan[0];
            }
            else
            {
                snum = snum.PadLeft(((snum.Length - 1) / 3 + 1) * 3, '0');
                int L = snum.Length / 3 - 1;
                for (int i = 0; i <= L; i++)
                {
                    int b = int.Parse(snum.Substring(i * 3, 3));
                    if (b != 0)
                        stotal = stotal + GetNumber(b) + " " + basex[L - i] + " و ";
                }
                stotal = stotal.Substring(0, stotal.Length - 3);
            }
            return stotal;
        }

       private static string GetNumber(int num3)
        {
            string s = "";
            int d3, d12;
            d12 = num3 % 100;
            d3 = num3 / 100;
            if (d3 != 0)
                s = sadgan[d3] + " و ";
            if ((d12 >= 10) && (d12 <= 19))
            {
                s = s + dahyek[d12 - 10];
            }
            else
            {
                int d2 = d12 / 10;
                if (d2 != 0)
                    s = s + dahgan[d2] + " و ";
                int d1 = d12 % 10;
                if (d1 != 0)
                    s = s + yakan[d1] + " و ";
                s = s.Substring(0, s.Length - 3);
            };
            return s;
        }
    }
}





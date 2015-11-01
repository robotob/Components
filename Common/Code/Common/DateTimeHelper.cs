using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToB.Common
{
    public static class DateTimeHelper
    {
        public static DateTime GetFirstDateOfMonth()
        {
            return GetFirstDateOfMonth(DateTime.Now);
        }

        public static DateTime GetFirstDateOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime GetLastDateOfMonth()
        {
            return GetLastDateOfMonth(DateTime.Now);
        }

        public static DateTime GetLastDateOfMonth(DateTime dt)
        {
            int daysInMonth = DateTime.DaysInMonth(dt.Year, dt.Month);
            return new DateTime(dt.Year, dt.Month, daysInMonth);
        }

        public static DateTime GetFirstDateOfYear()
        {
            return GetFirstDateOfYear(DateTime.Now.Year);
        }

        public static DateTime GetFirstDateOfYear(int year)
        {
            return new DateTime(year, 1, 1);
        }

        public static DateTime GetLastDateOfYear()
        {
            return GetLastDateOfYear(DateTime.Now.Year);
        }

        public static DateTime GetLastDateOfYear(int year)
        {
            return new DateTime(year, 12, 31);
        }
    }
}

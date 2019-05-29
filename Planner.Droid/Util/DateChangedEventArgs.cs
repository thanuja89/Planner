using System;

namespace Planner.Droid.Util
{
    public class DateChangedEventArgs : EventArgs
    {
        public Date Date { get; set; }
    }

    public struct Date
    {
        public int Year { get; }
        public int Month { get; }
        public int Day { get; }

        public Date(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public static bool operator ==(Date date1, Date date2) => Equals(date1, date2);

        public static bool operator !=(Date date1, Date date2) => !Equals(date1, date2);

        public override bool Equals(object obj)
        {
            return obj is Date date &&
                   Year == date.Year &&
                   Month == date.Month &&
                   Day == date.Day;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year, Month, Day);
        }

        public override string ToString()
        {
            return new DateTime(Year, Month, Day).ToString("d MMM");
        }
    }
}
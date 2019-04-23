using System;
using System.Globalization;

namespace Planner.Droid.Util
{
    public class TimeChangedEventArgs : EventArgs
    {
        public Time Time { get; set; }
    }

    public struct Time
    {
        public int Hour { get; }
        public int Minute { get; }

        public Time(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
        }

        public static bool operator ==(Time time1, Time time2) 
            => Equals(time1, time2);

        public static bool operator !=(Time time1, Time time2) => !Equals(time1, time2);

        public override bool Equals(object obj)
        {
            return obj is Time time &&
                   Hour == time.Hour &&
                   Minute == time.Minute;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Hour, Minute);
        }

        public override string ToString()
        {
            int hr = Hour > 12 ? (24 - Hour) : Hour;
            string amPm = Hour > 12 ? "PM" : "AM";

            return $"{hr}:{Minute} {amPm}";
        }
    }
}
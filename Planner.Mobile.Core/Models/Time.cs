using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Mobile.Core.Models
{
    public struct Time
    {
        public int hour;
        public int minute;

        public Time(int hour, int minute)
        {
            this.hour = hour;
            this.minute = minute;
        }

        public override string ToString()
        {
            return $"{ hour } : { minute }";
        }
    }
}

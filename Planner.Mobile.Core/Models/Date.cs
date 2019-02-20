using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Mobile.Core.Models
{
    public struct Date
    {
        public int year;
        public int month;
        public int day;

        public Date(int year, int month, int day)
        {
            this.year = year;
            this.month = month;
            this.day = day;
        }

        public override string ToString()
        {
            return new DateTime(year, month, day).ToShortDateString();
        }
    }
}

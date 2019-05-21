using Android.App;
using Android.Content;
using Planner.Droid.Receivers;
using Planner.Mobile.Core.Data;
using System;

namespace Planner.Droid.Helpers
{
    public class AlarmHelper
    {
        public DateTime GetNextWeekDay(DateTime date) =>
            GetNextDay(date, d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday);

        public DateTime GetNextNonWeekDay(DateTime date) => 
            GetNextDay(date, d => d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday);

        public DateTime GetDayInNextMonth(DateTime date) => date.AddMonths(1);
        public DateTime GetDayInNextYear(DateTime date) => date.AddYears(1);

        public void SetAlarm(ScheduledTask task)
        {
            var context = Application.Context;
            var alarmManager = (AlarmManager) context.GetSystemService(Context.AlarmService);

            Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
            calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();

            calendar.Set(task.Start.Year, task.Start.Month - 1, task.Start.Day, task.Start.Hour, task.Start.Minute, 0);

            var pending = GetAlarmPendingIntent(context, task);

            switch (task.Repeat)
            {
                case Frequency.EveryDay:
                    alarmManager.SetRepeating(AlarmType.Rtc, calendar.TimeInMillis, 1000 * 60 * 60 * 24, pending);
                    break;
                case Frequency.Weekly:
                    alarmManager.SetRepeating(AlarmType.Rtc, calendar.TimeInMillis, 1000 * 60 * 60 * 24 * 7, pending);
                    break;
                default:
                    alarmManager.Set(AlarmType.Rtc, calendar.TimeInMillis, pending);
                    //SetNextRepeatingAlarm(task);
                    break;
            }           
        }

        public void SetNextRepeatingAlarm(ScheduledTask task)
        {
            var context = Application.Context;
            var alarmManager = (AlarmManager) context.GetSystemService(Context.AlarmService);

            DateTime nextDate = GetNextAlarmDate(task.Start, task.Repeat);

            if(nextDate != default)
            {
                var time = GetTimeDifferenceInMilliseconds(nextDate);
                var pending = GetAlarmPendingIntent(context, task);

                alarmManager.Set(AlarmType.Rtc, time, pending);
            }
        }

        public void CancelAlarm(ScheduledTask task)
        {
            var context = Application.Context;
            var alarmManager = (AlarmManager) context.GetSystemService(Context.AlarmService);

            var pending = GetAlarmPendingIntent(context, task);

            alarmManager.Cancel(pending);
        }

        public void UpdateAlarm(ScheduledTask task)
        {
            CancelAlarm(task);
            SetAlarm(task);
        }

        private DateTime GetNextAlarmDate(DateTime date, Frequency frequency)
        {
            DateTime nextDate = default;

            switch (frequency)
            {
                case Frequency.Weekdays:
                    nextDate = GetNextWeekDay(date);
                    break;
                case Frequency.Weekends:
                    nextDate = GetNextNonWeekDay(date);
                    break;
                case Frequency.Monthly:
                    nextDate = GetDayInNextMonth(date);
                    break;
                case Frequency.Yearly:
                    nextDate = GetDayInNextYear(date);
                    break;
            }

            return nextDate;
        }

        private long GetTimeDifferenceInMilliseconds(DateTime date)
        {
            Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
            calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();

            calendar.Set(date.Year, date.Month - 1, date.Day, date.Hour, date.Minute, 0);

            return calendar.TimeInMillis;
        }

        private PendingIntent GetAlarmPendingIntent(Context context, ScheduledTask task)
        {
            var alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));

            alarmIntent.PutExtra(AlarmReceiver.Constants.CLIENT_SIDE_ID_PARAM_NAME, task.ClientSideId);

            var pending = PendingIntent.GetBroadcast(context, task.ClientSideId, alarmIntent, PendingIntentFlags.UpdateCurrent);

            return pending;
        }

        private DateTime GetNextDay(DateTime originalDate, Func<DateTime, bool> predicate)
        {
            var day = DateTime.Today;

            do
            {
                day = day.AddDays(1);
            } while (!predicate(day));

            return day.Add(originalDate.TimeOfDay);
        }
    }
}
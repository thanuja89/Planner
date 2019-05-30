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
        public DateTime GetNextDay(DateTime date) => date.AddDays(1);
        public DateTime GetDayInNextWeek(DateTime date) => date.AddDays(7);

        public void SetAlarm(ScheduledTask task)
        {
            if (task.Start > DateTime.Now)
            {
                SetAlarm(task.ClientSideId, task.Start);
            }
            else
            {
                SetNextRepeatingAlarm(task);
            }            
        }

        private void SetAlarm(int clientSideId, DateTime start)
        {
            var context = Application.Context;
            var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

            var time = GetTimeDifferenceInMilliseconds(start);

            var pending = GetAlarmPendingIntent(context, clientSideId);

            alarmManager.SetExact(AlarmType.Rtc, time, pending);
        }

        public void SetNextRepeatingAlarm(ScheduledTask task)
        {
            var context = Application.Context;
            var alarmManager = (AlarmManager) context.GetSystemService(Context.AlarmService);

            DateTime nextDate = GetNextAlarmDate(task.Start, task.Repeat);

            if(nextDate != default)
            {
                var time = GetTimeDifferenceInMilliseconds(nextDate);
                var pending = GetAlarmPendingIntent(context, task.ClientSideId);

                alarmManager.Set(AlarmType.Rtc, time, pending);
            }
        }

        public void CancelAlarm(ScheduledTask task)
        {
            CancelAlarm(task.ClientSideId);
        }

        public void CancelAlarm(int clientSideId)
        {
            var context = Application.Context;
            var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

            var pending = GetAlarmPendingIntent(context, clientSideId);

            alarmManager.Cancel(pending);
        }

        public void UpdateAlarm(ScheduledTask task)
        {
            if(task.Start > DateTime.Now)
            {
                UpdateAlarm(task.ClientSideId, task.Start);
            }
            else
            {
                SetNextRepeatingAlarm(task);
            }
        }

        private void UpdateAlarm(int clientSideId, DateTime start)
        {
            CancelAlarm(clientSideId);
            SetAlarm(clientSideId, start);
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
                case Frequency.EveryDay:
                    nextDate = GetNextDay(date);
                    break;
                case Frequency.Weekly:
                    nextDate = GetDayInNextWeek(date);
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

        private PendingIntent GetAlarmPendingIntent(Context context, int clientSideId)
        {
            var alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));

            alarmIntent.PutExtra(AlarmReceiver.Constants.CLIENT_SIDE_ID_PARAM_NAME, clientSideId);

            var pending = PendingIntent.GetBroadcast(context, clientSideId, alarmIntent, PendingIntentFlags.UpdateCurrent);

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
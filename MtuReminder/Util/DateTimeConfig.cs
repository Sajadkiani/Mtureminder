using MD.PersianDateTime;
using System;
using System.Globalization;

namespace MtuReminder.Util
{
    public static class DateTimeConfig
    {

        public static string ConvertPersianToUtc(string StrPersianDate)
        {
            var ObjPersianDateTime = PersianDateTime.Parse(StrPersianDate);
            DateTime Dt = new DateTime(ObjPersianDateTime.Year, ObjPersianDateTime.Month, ObjPersianDateTime.Day, ObjPersianDateTime.Hour, ObjPersianDateTime.Minute, ObjPersianDateTime.Second, new PersianCalendar());
            DateTime UtcDt = Dt.ToUniversalTime();
            return UtcDt.ToString(CultureInfo.InvariantCulture);
        }


        public static bool Isdate(string StrPersianDate)
        {
            try
            {
                var ObjPersianDateTime = PersianDateTime.Parse(StrPersianDate);
                DateTime Dt = new DateTime(ObjPersianDateTime.Year, ObjPersianDateTime.Month, ObjPersianDateTime.Day, ObjPersianDateTime.Hour, ObjPersianDateTime.Minute, ObjPersianDateTime.Second, new PersianCalendar());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
         
        }


        //internal static string ConvertUtcToPersian(string StrUtcDt)
        //{
        //    DateTime dt = new DateTime();
        //    PersianCalendar pc = new PersianCalendar();



        //    var x = DateTime.TryParseExact(StrUtcDt, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
        //    TimeZoneInfo IranDaylightTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
        //    dt = TimeZoneInfo.ConvertTimeFromUtc(dt, IranDaylightTimeZone);
        //    PersianDateTime persianDateTime = new PersianDateTime(dt);

        //    string v = persianDateTime.ToString();
        //    return v;
        //}

        internal static PersianDateTime ConvertUtcToPersian(string StrUtcDt)
        {
            DateTime dt = new DateTime();
            PersianCalendar pc = new PersianCalendar();



            var x = DateTime.TryParseExact(StrUtcDt, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            TimeZoneInfo IranDaylightTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            dt = TimeZoneInfo.ConvertTimeFromUtc(dt, IranDaylightTimeZone);
            PersianDateTime ObjPersianDateTime = new PersianDateTime(dt);
           // DateTime PerDt = new DateTime(ObjPersianDateTime.Year, ObjPersianDateTime.Month, ObjPersianDateTime.Day, ObjPersianDateTime.Hour, ObjPersianDateTime.Minute, ObjPersianDateTime.Second, new PersianCalendar());

            
            return ObjPersianDateTime;
        }

    }
}

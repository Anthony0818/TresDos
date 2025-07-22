namespace TresDos.Helper
{
    public class DateTimeHelper
    {
        public DateTime GetPhilippineTime()
        {
            TimeZoneInfo phTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            DateTime utcNow = DateTime.UtcNow;
            DateTime phTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, phTimeZone);
            return phTime;
        }
    }
}

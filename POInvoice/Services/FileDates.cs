using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace POInvoice.Services
{
    public static class FileDates
    {
        public static async Task<DateTime> StreamToString(HttpContent stream)
        {
            var byteArray = await stream.ReadAsByteArrayAsync();
            var dateStringInMilliseconds = System.Text.Encoding.Default.GetString(byteArray);
            return StringToDateTime(dateStringInMilliseconds);
        }

        public static DateTime StringToDateTime(string str)
        {
            // Jan 1, 1970 12:00:00 AM
            var beginTime = new DateTime(1970, 1, 1, 0, 0, 0);

            var timeSinceBeginTime = (TimeSpan.FromMilliseconds(Convert.ToInt64(str)));

            var dateLastModified = beginTime + timeSinceBeginTime;

            // We have to subtract 5 hours to change from UTC time to local time
            return dateLastModified.Subtract(new TimeSpan(5, 0, 0));
        }
    }
}
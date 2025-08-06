using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Utilities
{
    public static class TimeHelper
    {
        public static DateTime VietnamTimeZone()
        {
            TimeZoneInfo vn = TimeZoneInfo.FindSystemTimeZoneById(GetVietnamTimeZoneId());

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vn);
        }

        public static string GetVietnamTimeZoneId()
        {
            if (OperatingSystem.IsWindows())
            {
                return "SE Asia Standard Time"; // Windows
            }
            else
            {
                return "Asia/Ho_Chi_Minh"; // Linux
            }
        }
    }
}

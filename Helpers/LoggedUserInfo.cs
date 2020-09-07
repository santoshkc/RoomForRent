using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RoomForRent.Helpers
{
    public class LoggedUserInfo
    {
        public string Email { get; set; }
        public bool IsLoggedIn { get; set; }
    }

    public static class LoggedInUserInfoHelper {

        private const string LoggedInUserInfo = "LoggedInUserInfo";
        public static void SetLoggedUserInfo(this ISession session, LoggedUserInfo userInfo)
        {
            session.Set(LoggedInUserInfo, userInfo);
        }

        public static LoggedUserInfo GetLoggedUserInfo(this ISession session)
        {
            return session.Get<LoggedUserInfo>(LoggedInUserInfo) ?? new LoggedUserInfo();
        }
    }
}

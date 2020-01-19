using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Infrastructure
{
    public static class SessionExtensions
    {
        public static T GetJson<T>(this ISession session, string key)
        {
            var sessiondata = session.GetString(key);
            return string.IsNullOrEmpty(sessiondata)?default(T):JsonConvert.DeserializeObject<T>(sessiondata);
        }
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BR.helper
{
    public static class SessionExtensions
    {
        public static void AddData<T>(this ISession session,string key, T value)
        {
            if (!string.IsNullOrEmpty(key) && session != null)
            {
                string json = JsonConvert.SerializeObject(value);
                session.SetString(key, json);
            }
            else
            {
                throw new ArgumentNullException($"--Exception in SessionExtensions.AddData : {nameof(session)} And {nameof(key)}, check if the session or key is null");
            }

        }

        public static T GetData<T>(this ISession session, string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(key) && session != null)
                {
                    string json = session.GetString(key);
                    return json == null ? default : JsonConvert.DeserializeObject<T>(json);
                }
                return default;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}

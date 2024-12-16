using Microsoft.AspNetCore.Http;
namespace BR.helper
{
    public interface ISessionHelper
    {
        bool ClearSession();
        T GetData<T>(string key);
        void AddData<T>(string key, T value);
        void SetMainData(string userId, string role, int language);

        string UserId { get; }
        string Role { get; }
        int Language { get; }
    }
}

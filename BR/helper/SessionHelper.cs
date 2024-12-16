namespace BR.helper
{
    public class SessionHelper : ISessionHelper
    {
        private readonly ISession _session;
        
        public string UserId => _session.GetData<string>("UserId");
        public string Role => _session.GetData<string>("Role");
        public int Language => _session.GetData<int>("Language");
        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor != null)
            {
                if (httpContextAccessor.HttpContext != null)
                {
                    _session = httpContextAccessor.HttpContext.Session;
                }
            }
        }

        public T GetData<T>(string key)
        {
            return _session.GetData<T>(key);
        }

        public void AddData<T>(string key, T value)
        {
            _session.AddData<T>(key, value);
        }

        public void SetMainData(string userId, string role, int language)
        {
            _session.AddData<string>("UserId", userId);
            _session.AddData<string>("Role", role);
            _session.AddData<int>("Language", language);
        }

        public bool ClearSession()
        {
            try
            {
                _session.Clear();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

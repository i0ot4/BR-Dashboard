using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;

namespace BR.helper
{
    public interface ISharedViewLocalizer
    {
        public LocalizedString this[string key]
        {
            get;
        }

        LocalizedString GetLocalizedString(string key);
    }
    public class SharedResource
    {

    }

    public class SharedViewLocalizer : ISharedViewLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public SharedViewLocalizer(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("Common", assemblyName.Name);
        }

        public LocalizedString this[string key] => _localizer[key];

        public LocalizedString GetLocalizedString(string key)
        {
            return _localizer[key];
        }
    }
    public interface ILocalizationService
    {
        IList<CultureInfo> GetSupportedCultures();
        void ConfigureLocalization(IApplicationBuilder app);
        string GetLocalizedResource(string key, string resource, CultureInfo culture = default);
        string GetMessagesResource(string key, CultureInfo culture = default);
        string GetMainResource(string key, CultureInfo culture = default);
        string GetCommonResource(string key, CultureInfo culture = default);
    }
}

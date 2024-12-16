using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace BR.helper
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly IStringLocalizer _localizer;
        private readonly IStringLocalizer _localizer2;

        public LocalizationService(IStringLocalizerFactory localizerFactory)
        {
            //var type = typeof(SharedResource);
            //var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            //_localizer = _localizerFactory.Create(type);
            _localizerFactory = localizerFactory;
            //_localizer2 = _localizerFactory.Create("SharedResource", assemblyName.Name);
        }

        public IList<CultureInfo> GetSupportedCultures()
        {
            return new List<CultureInfo>
            {
                // we can add any language code here for other languages
                new CultureInfo("ar-YE"),
                new CultureInfo("en-US")
            };
        }

        public void ConfigureLocalization(IApplicationBuilder app)
        {
            var supportedCultures = GetSupportedCultures();

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                //DefaultRequestCulture = new RequestCulture("en-US"),
                DefaultRequestCulture = new RequestCulture("ar-YE"),

                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,


            });
            Console.WriteLine($"=========in infra, current culture: {CultureInfo.CurrentCulture}");
            Console.WriteLine($"=========in infra, current UI culture: {CultureInfo.CurrentUICulture}");
        }

        public string GetLocalizedResource(string key, string resource, CultureInfo culture = default)
        {
            var localizer = _localizerFactory.Create(resource, "BRAplication");
            return localizer.GetString(key);
        }

        public string GetMessagesResource(string key, CultureInfo culture = default)
        {
            var localizer = _localizerFactory.Create("Messages", "BRAplication");
            return localizer.GetString(key);
        }

        public string GetMainResource(string key, CultureInfo culture = default)
        {
            var localizer = _localizerFactory.Create("Main", "BRAplication");
            return localizer.GetString(key);
        }

        public string GetCommonResource(string key, CultureInfo culture = default)
        {
            var localizer = _localizerFactory.Create("Common", "BRAplication");
            return localizer.GetString(key);
        }
    }
}

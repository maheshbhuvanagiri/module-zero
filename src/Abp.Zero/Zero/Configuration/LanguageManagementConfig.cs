using System.Linq;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization;
using Abp.Localization.Dictionaries;

namespace Abp.Zero.Configuration
{
    internal class LanguageManagementConfig : ILanguageManagementConfig
    {
        private readonly IIocManager _iocManager;
        private readonly IAbpStartupConfiguration _configuration;

        public LanguageManagementConfig(IIocManager iocManager, IAbpStartupConfiguration configuration)
        {
            _iocManager = iocManager;
            _configuration = configuration;
        }

        public void EnableDbLocalization()
        {
            _iocManager.Register<ILanguageProvider, ApplicationLanguageProvider>(DependencyLifeStyle.Transient);

            var sources = _configuration
                .Localization
                .Sources
                .Where(s => s is IDictionaryBasedLocalizationSource)
                .Cast<IDictionaryBasedLocalizationSource>()
                .ToList();
            
            foreach (var source in sources)
            {
                _configuration.Localization.Sources.Remove(source);
                _configuration.Localization.Sources.Add(
                    new MultiTenantLocalizationSource(
                        source.Name,
                        new MultiTenantLocalizationDictionaryProvider(
                            source.DictionaryProvider,
                            _iocManager
                            )
                        )
                    );
            }
        }
    }
}
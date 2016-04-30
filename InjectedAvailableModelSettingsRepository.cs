using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using EPiServer.ServiceLocation;

namespace JOS.InjectedAllowedTypes
{
    public class InjectedAvailableModelSettingsRepository : AvailableModelSettingsRepository
    {
        private static readonly Dictionary<Type, ContentTypeAvailableModelSetting> CustomSettings = InjectedAvailableModelSettings.GetCustomAvailableModelSettings();
        public InjectedAvailableModelSettingsRepository(ServiceAccessor<IContentTypeRepository> contentTypeRepository) : base(contentTypeRepository)
        {
        }

        public override IDictionary<Type, ContentTypeAvailableModelSetting> ListRuntimeSettings()
        {
            var runtimeSettings = base.ListRuntimeSettings();

            foreach (var customSetting in CustomSettings)
            {
                if (runtimeSettings.ContainsKey(customSetting.Key))
                {
                    var merged = MergeSettings(customSetting.Value, runtimeSettings[customSetting.Key]);
                    runtimeSettings[customSetting.Key] = merged;
                }
            }

            return runtimeSettings;
        }

        private ContentTypeAvailableModelSetting MergeSettings(ContentTypeAvailableModelSetting customSetting, ContentTypeAvailableModelSetting runtimeSetting)
        {
            var mergedSetting = new ContentTypeAvailableModelSetting
            {
                Excluded = new HashSet<Type>(customSetting.Excluded.Concat(runtimeSetting.Excluded).Distinct()),
                Included = new HashSet<Type>(customSetting.Included.Concat(runtimeSetting.Included).Distinct()),
                IncludedOn = new HashSet<Type>(customSetting.IncludedOn.Concat(runtimeSetting.IncludedOn).Distinct()),
                Availability = customSetting.Availability
            };

            return mergedSetting;
        }
    }
}
using System;
using System.Collections.Generic;
using EPiServer.DataAbstraction.RuntimeModel;

namespace JOS.InjectedAllowedTypes
{
    public static class InjectedAvailableModelSettings
    {
        private static Dictionary<Type, ContentTypeAvailableModelSetting> _customSettings;

        public static void RegisterCustomAvailableModelSettings(Dictionary<Type, ContentTypeAvailableModelSetting> customSettings)
        {
            _customSettings = customSettings;
        } 

        public static Dictionary<Type, ContentTypeAvailableModelSetting> GetCustomAvailableModelSettings()
        {
            return _customSettings ?? new Dictionary<Type, ContentTypeAvailableModelSetting>();
        }
    }
}
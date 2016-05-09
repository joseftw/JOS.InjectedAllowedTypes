using System;
using System.Collections.Generic;
using EPiServer.DataAbstraction.RuntimeModel;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace JOS.InjectedAllowedTypes
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Shell.ShellInitialization))]
    public class InjectedAllowedTypesInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            InjectedAvailableModelSettings.RegisterCustomAvailableModelSettings(new Dictionary<Type, ContentTypeAvailableModelSetting>
            {

                //{
                //    typeof(MediaPage) , new ContentTypeAvailableModelSetting
                //                        {
                //                            Availability = Availability.Specific,
                //                            Included = new HashSet<Type> {typeof(CoolPage) }
                //                        }
                //}
            });

            InjectedAllowedTypes.RegisterInjectedAllowedTypesAttributes(new Dictionary<string, InjectedAllowedTypesAttribute>
            {
                //{
                //    string.Format("{0}.{1}",typeof(MediaPage).Name, nameof(MediaPage.ContentArea)), new InjectedAllowedTypesAttribute
                //    {
                //        AllowedTypes = new [] {typeof(MusicBlock)}
                //    }
                //}
            });
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}

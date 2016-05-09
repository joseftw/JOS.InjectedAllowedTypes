using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace JOS.InjectedAllowedTypes
{
    public static class InjectedAllowedTypes
    {
        private static Dictionary<string, InjectedAllowedTypesAttribute> _injectedAllowedTypesAttributes = new Dictionary<string, InjectedAllowedTypesAttribute>();

        public static void RegisterInjectedAllowedTypesAttributes(Dictionary<string, InjectedAllowedTypesAttribute> customAllowedTypesAttributes)
        {
            _injectedAllowedTypesAttributes = customAllowedTypesAttributes;
        }

        public static AllowedTypesAttribute GetInjectedAllowedTypesAttribute(Type contentType,
            string propertyName)
        {
            var allInjectedAttributes = _injectedAllowedTypesAttributes;
            var key = string.Format("{0}.{1}", contentType.Name, propertyName);
            var injectedAttribute = allInjectedAttributes.FirstOrDefault(x => x.Key.Equals(key));

            if (injectedAttribute.Value == null)
            {
                return null;
            }

            var allowedTypeAttribute = new AllowedTypesAttribute
            {
                AllowedTypes = injectedAttribute.Value.AllowedTypes,
                RestrictedTypes = injectedAttribute.Value.RestrictedTypes
            };

            return allowedTypeAttribute;
        }

        /// <summary>
        /// Merges a AllowedTypesAttribute with a InjectedAllowedTypesAttribute
        /// </summary>
        /// <param name="injectedAllowedTypesAttribute">The AllowedTypesAttribute specified in the InitializationModule(RegisterInjectedAllowedTypesAttributes)</param>
        /// <param name="specifiedAllowedTypesAttribute">The InjectedAllowedTypesAttribute specified on a property with an attribute</param>
        /// <returns></returns>
        public static AllowedTypesAttribute MergeAttributes(
            AllowedTypesAttribute injectedAllowedTypesAttribute, InjectedAllowedTypesAttribute specifiedAllowedTypesAttribute)
        {
            if (injectedAllowedTypesAttribute == null)
            {
                injectedAllowedTypesAttribute = SetDefaultAllowedTypesAttributeValue();
            }

            var mergedAllowedTypesAttribute = new AllowedTypesAttribute
            {
                AllowedTypes = injectedAllowedTypesAttribute.AllowedTypes,
                RestrictedTypes = injectedAllowedTypesAttribute.RestrictedTypes
            };

            if (specifiedAllowedTypesAttribute != null)
            {
                mergedAllowedTypesAttribute = new AllowedTypesAttribute
                {
                    AllowedTypes =
                    injectedAllowedTypesAttribute.AllowedTypes.Concat(specifiedAllowedTypesAttribute.AllowedTypes).Distinct().ToArray(),
                    RestrictedTypes =
                    injectedAllowedTypesAttribute.RestrictedTypes.Concat(specifiedAllowedTypesAttribute.RestrictedTypes).Distinct().ToArray()
                };
            }

            //We(and episerver) are adding IContentData as default, here we make sure to remove IContentData if we have any specific AllowedTypes set.
            if (injectedAllowedTypesAttribute.AllowedTypes.Any())
            {
                mergedAllowedTypesAttribute.AllowedTypes =
                    mergedAllowedTypesAttribute.AllowedTypes.Where(x => x != typeof(IContentData)).ToArray();
            }

            if (mergedAllowedTypesAttribute.AllowedTypes.Any() || mergedAllowedTypesAttribute.RestrictedTypes.Any())
            {
                return mergedAllowedTypesAttribute;
            }

            return null;
        }

        private static AllowedTypesAttribute SetDefaultAllowedTypesAttributeValue()
        {
            return new AllowedTypesAttribute
            {
                AllowedTypes = new Type[0],
                RestrictedTypes = new Type[0]
            };
        }
    }
}
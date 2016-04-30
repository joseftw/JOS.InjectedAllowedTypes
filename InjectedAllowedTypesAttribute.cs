using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;

namespace JOS.InjectedAllowedTypes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]

    public class InjectedAllowedTypesAttribute : ValidationAttribute, IMetadataAware
    {
        internal static readonly IEnumerable<Type> SupportedTypes = new[]
        {
            typeof (ContentReference),
            typeof (ContentArea),
            typeof (IEnumerable<ContentReference>)
        };

        private Type[] _restrictedTypes = new Type[0];
        private Type[] _allowedTypes;

        public Type[] AllowedTypes
        {
            get
            {
                return (Type[])this._allowedTypes.Clone();
            }
            set
            {
                var typeArray = value ?? new[]
                {
                    typeof (IContentData)
                };

                this._allowedTypes = typeArray;
            }
        }

        public Type[] RestrictedTypes
        {
            get
            {
                return (Type[])this._restrictedTypes.Clone();
            }
            set
            {
                this._restrictedTypes = value ?? new Type[0];
            }
        }

        public string TypesFormatSuffix { get; set; }

        public InjectedAllowedTypesAttribute() : this(typeof(IContentData))
        {

        }

        public InjectedAllowedTypesAttribute(params Type[] allowedTypes)
        {
            this.AllowedTypes = allowedTypes;
        }

        public InjectedAllowedTypesAttribute(Type[] allowedTypes, Type[] restrictedTypes) : this(allowedTypes)
        {
            this.RestrictedTypes = restrictedTypes;
        }

        public InjectedAllowedTypesAttribute(Type[] allowedTypes, Type[] restrictedTypes, string typesFormatSuffix) : this(allowedTypes, restrictedTypes)
        {
            this.TypesFormatSuffix = typesFormatSuffix;
        }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            var extendedMetadata = metadata as ExtendedMetadata;
            if (extendedMetadata == null)
            {
                return;
            }

            var suffix = this.TypesFormatSuffix ?? (extendedMetadata.AdditionalValues.ContainsKey("TypesFormatSuffix") ?
                extendedMetadata.AdditionalValues["TypesFormatSuffix"] as string :
                null);

            var allowedTypes = this.AllowedTypes.Select(a => a.FullName.ToLowerInvariant()).ToList();
            var restrictedTypes = this.RestrictedTypes.Select(r => r.FullName.ToLowerInvariant()).ToList();

            var injectedAllowedTypesAttribute = InjectedAllowedTypes.GetInjectedAllowedTypesAttribute(extendedMetadata.ContainerType, extendedMetadata.PropertyName);

            if (injectedAllowedTypesAttribute != null)
            {
                allowedTypes =
                    allowedTypes.Concat(
                        injectedAllowedTypesAttribute.AllowedTypes.Select(x => x.FullName.ToLowerInvariant()))
                        .Distinct()
                        .ToList();

                restrictedTypes =
                    restrictedTypes.Concat(
                        injectedAllowedTypesAttribute.RestrictedTypes.Select(x => x.FullName.ToLowerInvariant()))
                            .Distinct()
                            .ToList();
            }


            if (!string.IsNullOrEmpty(suffix))
            {
                allowedTypes = allowedTypes.ToArray().Select(a => a + "." + suffix).ToList();
                restrictedTypes = restrictedTypes.ToArray().Select(a => a + "." + suffix).ToList();
            }

            var allowedTypesArray = allowedTypes.ToArray();
            var restrictedTypesArray = restrictedTypes.ToArray();

            extendedMetadata.EditorConfiguration["AllowedTypes"] = allowedTypesArray;
            extendedMetadata.EditorConfiguration["RestrictedTypes"] = restrictedTypesArray;
            extendedMetadata.EditorConfiguration["AllowedDndTypes"] = allowedTypesArray;
            extendedMetadata.EditorConfiguration["RestrictedDndTypes"] = restrictedTypesArray;
            extendedMetadata.OverlayConfiguration["AllowedDndTypes"] = allowedTypesArray;
            extendedMetadata.OverlayConfiguration["RestrictedDndTypes"] = restrictedTypesArray;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return null;

            var contentLocator = ServiceLocator.Current.GetInstance<IContentLoader>();
            var validationMessage = string.Empty;
            var stringBuilder = new StringBuilder();
            var allowedTypes = this.AllowedTypes;
            var restrictedTypes = this.RestrictedTypes;
            var contentReferences = new List<ContentReference>();

            if (value is ContentArea)
            {
                var contentArea = value as ContentArea;
                contentReferences = contentArea.Items.Select(x => x.ContentLink).ToList();
            }
            else if (value is IEnumerable<ContentReference>)
            {
                var references = value as IEnumerable<ContentReference>;
                contentReferences = references.ToList();
            }

            foreach (var contentReference in contentReferences)
            {
                var content = contentLocator.Get<IContent>(contentReference);
                var type = content.GetOriginalType();
                var injectedAllowedTypesAttribute = InjectedAllowedTypes.GetInjectedAllowedTypesAttribute(validationContext.ObjectInstance.GetOriginalType(),
                    validationContext.MemberName);

                if (injectedAllowedTypesAttribute != null)
                {
                    allowedTypes =
                        allowedTypes.Concat(injectedAllowedTypesAttribute.AllowedTypes).Distinct().ToArray();
                    restrictedTypes =
                        restrictedTypes.Concat(injectedAllowedTypesAttribute.RestrictedTypes).Distinct().ToArray();
                }

                if (restrictedTypes.Contains(type) || !allowedTypes.Contains(type))
                {
                    var message =
                        string.Format(
                            LocalizationService.Current.GetString("/injectedallowedtypes/errormessages/notallowed"), type.Name, validationContext.MemberName);
                    validationMessage = stringBuilder.Append(message).AppendLine(".").ToString();
                }
            }

            if (string.IsNullOrEmpty(validationMessage))
            {
                return null;
            }

            var validationResult = new ValidationResult(validationMessage);
            return validationResult;
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPiServer;
using EPiServer.Core;
using NSubstitute;
using Shouldly;
using Xunit;

namespace JOS.InjectedAllowedTypes.Tests
{
    public class InjectedAllowedTypesAttributeTests
    {
        private readonly List<ContentReference> _contentReferences;
        private readonly IContentLoader _contentLoader;
        public InjectedAllowedTypesAttributeTests()
        {
            _contentLoader = Substitute.For<IContentLoader>();

            _contentReferences = new List<ContentReference>
            {
                new ContentReference(1337),
                new ContentReference(1338),
                new ContentReference(1339)
            };

            foreach (var contentReference in _contentReferences)
            {
                _contentLoader.Get<IContent>(contentReference).Returns(new StandardPage());
            }
        }

        [Fact]
        public void ShouldAllowStandardPageType_BecauseOfAllowedTypes()
        {
            var page = new StandardPage
            {
                Items = _contentReferences
            };

            var attribute = new InjectedAllowedTypesAttribute(_contentLoader)
            {
                AllowedTypes = new[] { typeof(StandardPage) }
            };

            var validationContext = new ValidationContext(page) { MemberName = nameof(page.Items) };
            var result = attribute.RunValidation(page.Items, validationContext);
            result.ShouldBeNull();
        }

        [Fact]
        public void ShouldAllowStandardPageAndDifferentPageType_BecauseOfAllowedTypes()
        {
            var page = new StandardPage
            {
                Items = _contentReferences
            };

            var differentPageTypeContentReference = new ContentReference(2000);
            page.Items.Add(differentPageTypeContentReference);

            _contentLoader.Get<IContent>(differentPageTypeContentReference).Returns(new DifferentPage());


            var attribute = new InjectedAllowedTypesAttribute(_contentLoader)
            {
                AllowedTypes = new[] { typeof(StandardPage), typeof(DifferentPage) }
            };

            var validationContext = new ValidationContext(page) { MemberName = nameof(page.Items) };
            var result = attribute.RunValidation(page.Items, validationContext);
            result.ShouldBeNull();
        }

        [Fact]
        public void ShouldNotAllowDifferentPageType_BecauseOfAllowedTypes()
        {
            var page = new StandardPage
            {
                Items = _contentReferences
            };

            var differentPageTypeContentReference = new ContentReference(2000);
            page.Items.Add(differentPageTypeContentReference);

            _contentLoader.Get<IContent>(differentPageTypeContentReference).Returns(new DifferentPage());

            var attribute = new InjectedAllowedTypesAttribute(_contentLoader)
            {
                AllowedTypes = new[] { typeof(StandardPage) }
            };

            var validationContext = new ValidationContext(page) { MemberName = nameof(page.Items) };
            var result = attribute.RunValidation(page.Items, validationContext);
            result.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldAllowIContainableInterface_BecauseOfAllowedTypes()
        {
            var page = new StandardPage
            {
                Items = _contentReferences
            };

            var attribute = new InjectedAllowedTypesAttribute(_contentLoader)
            {
                AllowedTypes = new[] { typeof(IContainable) }
            };

            var validationContext = new ValidationContext(page) { MemberName = nameof(page.Items) };
            var result = attribute.RunValidation(page.Items, validationContext);
            result.ShouldBeNull();
        }

        [Fact]
        public void ShouldNotAllowDifferentPage_BecauseOfRestrictedTypes()
        {
            var page = new StandardPage
            {
                Items = _contentReferences
            };

            var differentPageTypeContentReference = new ContentReference(2000);
            page.Items.Add(differentPageTypeContentReference);

            _contentLoader.Get<IContent>(differentPageTypeContentReference).Returns(new DifferentPage());

            var attribute = new InjectedAllowedTypesAttribute(_contentLoader)
            {
                RestrictedTypes = new[] { typeof(DifferentPage) }
            };

            var validationContext = new ValidationContext(page) { MemberName = nameof(page.Items) };
            var result = attribute.RunValidation(page.Items, validationContext);
            result.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldNotAllowDifferentPage_BecauseOfRestrictedTypesInterface()
        {
            var page = new StandardPage
            {
                Items = _contentReferences
            };
            
            var attribute = new InjectedAllowedTypesAttribute(_contentLoader)
            {
                RestrictedTypes = new[] { typeof(IContainable) }
            };

            var validationContext = new ValidationContext(page) { MemberName = nameof(page.Items) };
            var result = attribute.RunValidation(page.Items, validationContext);
            result.ShouldNotBeNull();
        }
    }
}

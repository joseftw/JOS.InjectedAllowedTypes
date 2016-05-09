using EPiServer;
using EPiServer.DataAnnotations;
using NSubstitute;
using Shouldly;
using Xunit;

namespace JOS.InjectedAllowedTypes.Tests
{
    public class InjectedAllowedTypesTests
    {
        private readonly IContentLoader _contentLoader;

        public InjectedAllowedTypesTests()
        {
            _contentLoader = Substitute.For<IContentLoader>();
        }

        [Fact]
        public void MergeAttributes_ShouldContainBothSpecifiedAndInjectedAttribute()
        {
            var allowedTypesAttribute = new AllowedTypesAttribute
            {
                AllowedTypes = new[] {typeof (StandardPage)}
            };

            var injectedAllowedTypesAttribute = new InjectedAllowedTypesAttribute(_contentLoader)
            {
                AllowedTypes = new[] {typeof (DifferentPage)}
            };

            var result = InjectedAllowedTypes.MergeAttributes(allowedTypesAttribute, injectedAllowedTypesAttribute);
            result.AllowedTypes.ShouldBe(new []{typeof(StandardPage), typeof(DifferentPage)});
        }

        [Fact]
        public void MergeAttributes_ShouldContainBothSpecifiedAndInjectedAttribute_Distinct()
        {
            var allowedTypesAttribute = new AllowedTypesAttribute
            {
                AllowedTypes = new[] { typeof(StandardPage) }
            };

            var injectedAllowedTypesAttribute = new InjectedAllowedTypesAttribute(_contentLoader)
            {
                AllowedTypes = new[] { typeof(DifferentPage), typeof(StandardPage) }
            };

            var result = InjectedAllowedTypes.MergeAttributes(allowedTypesAttribute, injectedAllowedTypesAttribute);
            result.AllowedTypes.ShouldBe(new[] { typeof(StandardPage), typeof(DifferentPage) });
        }

        [Fact]
        public void MergeAttributes_FirstArgumentNull()
        {
            var injectedAllowedTypesAttribute = new InjectedAllowedTypesAttribute(_contentLoader)
            {
                AllowedTypes = new[] { typeof(DifferentPage) }
            };

            var result = InjectedAllowedTypes.MergeAttributes(null, injectedAllowedTypesAttribute);
            result.AllowedTypes.ShouldBe(new[] { typeof(DifferentPage) });
        }

        [Fact]
        public void MergeAttributes_SecondArgumentNull()
        {
            var allowedTypesAttribute = new AllowedTypesAttribute
            {
                AllowedTypes = new[] { typeof(StandardPage) }
            };

            var result = InjectedAllowedTypes.MergeAttributes(allowedTypesAttribute, null);
            result.AllowedTypes.ShouldBe(new[] { typeof(StandardPage) });
        }

        [Fact]
        public void MergeAttributes_BothArgumentNull()
        {
            var result = InjectedAllowedTypes.MergeAttributes(null, null);
            result.ShouldBeNull();
        }
    }
}

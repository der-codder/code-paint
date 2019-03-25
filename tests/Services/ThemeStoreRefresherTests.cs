using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using CodePaint.WebApi.Domain.Models;
using CodePaint.WebApi.Domain.Repositories;
using CodePaint.WebApi.Services;
using Moq;
using Xunit;

namespace CodePaint.WebApi.Tests.Services
{
    public class ThemeStoreRefresherTests : IDisposable
    {
        private readonly AutoMock _mock;

        public ThemeStoreRefresherTests()
        {
            _mock = AutoMock.GetLoose();
        }

        public void Dispose()
        {
            _mock.Dispose();
        }

        [Fact]
        public void GetSavedGalleryItemType_ReturnsProperType()
        {
            const string expectedId = "expectedId_test";
            const GalleryItemType expectedGalleryItemType = GalleryItemType.NoThemes;
            _mock.Mock<IGalleryItemsRepository>()
                .Setup(x => x.GetGalleryItem(expectedId))
                .ReturnsAsync(new GalleryItem { Type = expectedGalleryItemType });
            var mockRefresher = _mock.Create<ThemeStoreRefresher>();

            var actual = mockRefresher.GetSavedGalleryItemType(expectedId).Result;

            Assert.True(expectedGalleryItemType == actual);
        }

        [Fact]
        public void GetStoredTheme_ReturnsProperTheme()
        {
            const string expectedId = "expectedId_test";
            _mock.Mock<IVSCodeThemeStoreRepository>()
                .Setup(x => x.GetTheme(expectedId))
                .ReturnsAsync(new VSCodeTheme { Id = expectedId });
            var mockRefresher = _mock.Create<ThemeStoreRefresher>();

            var actual = mockRefresher.GetStoredTheme(expectedId).Result;

            Assert.Equal(expectedId, actual.Id);
        }

        [Fact]
        public void CheckAndUpdateFreshThemeType_FreshThemeContainsTokenColors_ReturnsDefaultType()
        {
            var freshTheme = new VSCodeTheme
            {
                Themes = new List<Theme>
                {
                    new Theme
                    {
                        TokenColors = new List<TokenColor>
                        {
                            new TokenColor { Name = "name_test" }
                        }
                    }
                }
            };
            var mockRefresher = _mock.Create<ThemeStoreRefresher>();

            var actualType = mockRefresher.CheckAndUpdateFreshThemeType(freshTheme).Result;

            Assert.Equal(GalleryItemType.Default, actualType);
        }

        [Fact]
        public void CheckAndUpdateFreshThemeType_FreshThemeDoesNotContainsAnyTokenColors_ReturnsNeedAttentionType_And_UpdatesMetadata()
        {
            const string expectedId = "expectedId_test";
            const GalleryItemType expectedType = GalleryItemType.NeedAttention;
            var freshTheme = new VSCodeTheme
            {
                Id = expectedId,
                Themes = new List<Theme>
                {
                    new Theme()
                }
            };

            _mock.Mock<IGalleryItemsRepository>()
                .Setup(x => x.ChangeGalleryItemType(expectedId, expectedType))
                .ReturnsAsync(true);
            var mockRefresher = _mock.Create<ThemeStoreRefresher>();

            var actualType = mockRefresher.CheckAndUpdateFreshThemeType(freshTheme).Result;

            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void CheckAndUpdateFreshThemeType_FreshThemeDoesNotContributesAnyThemes_ReturnsNoThemesType_And_UpdatesMetadata()
        {
            const string expectedId = "expectedId_test";
            const GalleryItemType expectedType = GalleryItemType.NoThemes;
            var freshTheme = new VSCodeTheme
            {
                Id = expectedId,
                Themes = new List<Theme>()
            };

            _mock.Mock<IGalleryItemsRepository>()
                .Setup(x => x.ChangeGalleryItemType(expectedId, expectedType))
                .ReturnsAsync(true);
            var mockRefresher = _mock.Create<ThemeStoreRefresher>();

            var actualType = mockRefresher.CheckAndUpdateFreshThemeType(freshTheme).Result;

            Assert.Equal(expectedType, actualType);
        }

        [Fact]
        public void CreateTheme_CreatesNewThemeInStore()
        {
            const string expectedId = "expectedId_test";
            var newTheme = new VSCodeTheme { Id = expectedId };
            _mock.Mock<IVSCodeThemeStoreRepository>()
                .Setup(x => x.Create(newTheme))
                .Returns(Task.CompletedTask);
            var mockRefresher = _mock.Create<ThemeStoreRefresher>();

            mockRefresher.CreateTheme(newTheme).Wait();

            _mock.Mock<IVSCodeThemeStoreRepository>()
                .Verify(x => x.Create(newTheme), Times.Once);
        }

        [Fact]
        public void UpdateTheme_UpdatesThemeInStore()
        {
            const string expectedId = "expectedId_test";
            var theme = new VSCodeTheme { Id = expectedId };
            _mock.Mock<IVSCodeThemeStoreRepository>()
                .Setup(x => x.Update(theme))
                .ReturnsAsync(true);
            var mockRefresher = _mock.Create<ThemeStoreRefresher>();

            mockRefresher.UpdateTheme(theme).Wait();

            _mock.Mock<IVSCodeThemeStoreRepository>()
                .Verify(x => x.Update(theme), Times.Once);
        }

        [Fact]
        public void DownloadFreshTheme_ReturnsProperTheme()
        {
            var metadata = new GalleryItem
            {
                Id = "espectedPublisher_test.espectedName_test",
                PublisherName = "espectedPublisher_test",
                Name = "espectedName_test",
                Version = "espectedVersion_test"
            };
            var expectedTheme = new VSCodeTheme
            {
                Id = metadata.Id,
                Version = metadata.Version
            };
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(""));

            _mock.Mock<IVSMarketplaceClient>()
                .Setup(x => x.GetVsixFileStream(metadata))
                .ReturnsAsync(stream);
            _mock.Mock<IVSExtensionHandler>()
                .Setup(x => x.ProcessExtension(expectedTheme.Id, stream))
                .ReturnsAsync(expectedTheme);
            var mockRefresher = _mock.Create<ThemeStoreRefresher>();

            var actualTheme = mockRefresher.DownloadFreshTheme(metadata).Result;

            Assert.Equal(expectedTheme.Id, actualTheme.Id);
            Assert.Equal(expectedTheme.Version, actualTheme.Version);
        }
    }
}

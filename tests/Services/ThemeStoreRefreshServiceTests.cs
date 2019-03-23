using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using CodePaint.WebApi.Domain.Models;
using CodePaint.WebApi.Services;
using Moq;
using Xunit;

namespace CodePaint.WebApi.Tests.Services
{
    public class ThemeStoreRefreshServiceTests : IDisposable
    {
        private readonly AutoMock _mock;

        public ThemeStoreRefreshServiceTests()
        {
            _mock = AutoMock.GetLoose();
        }

        public void Dispose()
        {
            _mock.Dispose();
        }

        [Fact]
        public void RefreshGalleryStore_ProcessesAllGivenItems()
        {
            var items = new List<GalleryItem>();
            const int itemsCount = 12;
            for (int i = 0; i < itemsCount; i++)
            {
                items.Add(new GalleryItem { Id = $"test_{i}" });
            }

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedGalleryItemType(It.IsAny<string>()))
                .ReturnsAsync(GalleryItemType.NoThemes);
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStore(items).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(
                    x => x.GetSavedGalleryItemType(It.IsAny<string>()),
                    Times.Exactly(itemsCount)
                );
        }

        [Theory]
        [InlineData(GalleryItemType.NoThemes)]
        [InlineData(GalleryItemType.NeedAttention)]
        public void RefreshGalleryStoreTheme_TakesThemeWithNonDefaultType_DoesNotDownloadsFreshTheme(
            GalleryItemType extensionType)
        {
            var galleryItem = new GalleryItem { Id = "expectedId_test" };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedGalleryItemType(galleryItem.Id))
                .ReturnsAsync(extensionType);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(galleryItem))
                .ReturnsAsync(new VSCodeTheme());
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(galleryItem).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedGalleryItemType(galleryItem.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(galleryItem), Times.Never);
        }

        [Theory]
        [InlineData(GalleryItemType.NoThemes)]
        [InlineData(GalleryItemType.NeedAttention)]
        public void RefreshGalleryStoreTheme_TakesNonDefaultTypeTheme_DoesNotRefreshTheme(
            GalleryItemType extensionType)
        {
            var galleryItem = new GalleryItem { Id = "expectedId_test" };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedGalleryItemType(galleryItem.Id))
                .ReturnsAsync(extensionType);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(It.IsAny<VSCodeTheme>()));
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()));
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(galleryItem).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedGalleryItemType(galleryItem.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CreateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CreateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
        }

        [Fact]
        public void RefreshGalleryStoreTheme_TakesThemeWithActualVersion_DoesNotDownloadsFreshTheme()
        {
            var galleryItem = new GalleryItem { Id = "expectedId_test", Version = "version_test" };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedGalleryItemType(galleryItem.Id))
                .ReturnsAsync(GalleryItemType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(galleryItem.Id))
                .ReturnsAsync(new VSCodeTheme { Version = galleryItem.Version });
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(galleryItem))
                .ReturnsAsync(new VSCodeTheme());
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(galleryItem).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedGalleryItemType(galleryItem.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetStoredTheme(galleryItem.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(galleryItem), Times.Never);
        }

        [Fact]
        public void RefreshGalleryStoreTheme_TakesThemeWithActualVersion_DoesNotRefreshTheme()
        {
            var galleryItem = new GalleryItem { Id = "expectedId_test", Version = "version_test" };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedGalleryItemType(galleryItem.Id))
                .ReturnsAsync(GalleryItemType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(galleryItem.Id))
                .ReturnsAsync(new VSCodeTheme { Version = galleryItem.Version });
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(It.IsAny<VSCodeTheme>()));
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()));
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(galleryItem).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedGalleryItemType(galleryItem.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetStoredTheme(galleryItem.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CreateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
        }

        [Fact]
        public void RefreshGalleryStoreTheme_TakesNewTheme_DownloadsAndCreateIt()
        {
            const string expectedId = "expectedId_test";
            const string expectedVersion = "version1";
            var newGalleryItem = new GalleryItem { Id = expectedId, Version = expectedVersion };
            var colorTheme = new Theme
            {
                TokenColors = new List<TokenColor> { new TokenColor { Name = "name_test" } }
            };
            var newTheme = new VSCodeTheme
            {
                Id = expectedId,
                Version = expectedVersion,
                Themes = new List<Theme> { colorTheme }
            };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedGalleryItemType(newGalleryItem.Id))
                .ReturnsAsync(GalleryItemType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(newGalleryItem.Id))
                .ReturnsAsync(() => null);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(newGalleryItem))
                .ReturnsAsync(newTheme);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(newTheme))
                .Returns(Task.CompletedTask);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()));
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(newGalleryItem).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedGalleryItemType(newGalleryItem.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(newGalleryItem), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CreateTheme(newTheme), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
        }

        [Fact]
        public void RefreshGalleryStoreTheme_TakesNewNoThemesExtension_UpdatesExtensionTypeMetadata_And_DoesNotRefreshExtension()
        {
            const string expectedId = "expectedId_test";
            const string expectedVersion = "version1";
            var newGalleryItem = new GalleryItem { Id = expectedId, Version = expectedVersion };
            var newTheme = new VSCodeTheme
            {
                Id = expectedId,
                Version = expectedVersion,
                Themes = new List<Theme>()
            };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedGalleryItemType(newGalleryItem.Id))
                .ReturnsAsync(GalleryItemType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(newGalleryItem.Id))
                .ReturnsAsync(() => null);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(newGalleryItem))
                .ReturnsAsync(newTheme);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CheckAndUpdateFreshThemeType(newTheme))
                .ReturnsAsync(GalleryItemType.NoThemes);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(It.IsAny<VSCodeTheme>()));
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()));
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(newGalleryItem).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedGalleryItemType(newGalleryItem.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(newGalleryItem), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CheckAndUpdateFreshThemeType(newTheme), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CreateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
        }

        [Fact]
        public void RefreshGalleryStoreTheme_TakesNewNeedAttentionExtension_UpdatesExtensionTypeMetadata_And_DoesNotRefreshExtension()
        {
            const string expectedId = "expectedId_test";
            const string expectedVersion = "version1";
            var newGalleryItem = new GalleryItem { Id = expectedId, Version = expectedVersion };
            var newTheme = new VSCodeTheme
            {
                Id = expectedId,
                Version = expectedVersion,
                Themes = new List<Theme> { new Theme() }
            };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedGalleryItemType(newGalleryItem.Id))
                .ReturnsAsync(GalleryItemType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(newGalleryItem.Id))
                .ReturnsAsync(() => null);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(newGalleryItem))
                .ReturnsAsync(newTheme);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CheckAndUpdateFreshThemeType(newTheme))
                .ReturnsAsync(GalleryItemType.NeedAttention);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(It.IsAny<VSCodeTheme>()));
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()));
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(newGalleryItem).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedGalleryItemType(newGalleryItem.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(newGalleryItem), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CheckAndUpdateFreshThemeType(newTheme), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CreateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
        }

        [Fact]
        public void RefreshGalleryStoreTheme_TakesFreshTheme_DownloadsAndUpdateIt()
        {
            const string expectedId = "expectedId_test";
            var colorTheme = new Theme
            {
                TokenColors = new List<TokenColor> { new TokenColor { Name = "name_test" } }
            };
            var freshGalleryItem = new GalleryItem { Id = expectedId, Version = "version2" };
            var oldTheme = new VSCodeTheme
            {
                Id = expectedId,
                Version = "version1",
                Themes = new List<Theme> { colorTheme }
            };
            var freshTheme = new VSCodeTheme
            {
                Id = expectedId,
                Version = "version2",
                Themes = new List<Theme> { colorTheme }
            };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedGalleryItemType(freshGalleryItem.Id))
                .ReturnsAsync(GalleryItemType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(freshGalleryItem.Id))
                .ReturnsAsync(oldTheme);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(freshGalleryItem))
                .ReturnsAsync(freshTheme);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(It.IsAny<VSCodeTheme>()));
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(freshTheme))
                .Returns(Task.CompletedTask);
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(freshGalleryItem).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedGalleryItemType(freshGalleryItem.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(freshGalleryItem), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CreateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.UpdateTheme(freshTheme), Times.Once);
        }
    }
}

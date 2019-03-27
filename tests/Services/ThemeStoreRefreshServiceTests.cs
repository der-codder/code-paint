using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using CodePaint.WebApi.Domain.Models;
using CodePaint.WebApi.Services;
using CodePaint.WebApi.Services.ThemeStoreRefreshing;
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
            var items = new List<ExtensionMetadata>();
            const int itemsCount = 12;
            for (int i = 0; i < itemsCount; i++)
            {
                items.Add(new ExtensionMetadata { Id = $"test_{i}" });
            }

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedExtensionType(It.IsAny<string>()))
                .ReturnsAsync(ExtensionType.NoThemes);
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStore(items).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(
                    x => x.GetSavedExtensionType(It.IsAny<string>()),
                    Times.Exactly(itemsCount)
                );
        }

        [Theory]
        [InlineData(ExtensionType.NoThemes)]
        [InlineData(ExtensionType.NeedAttention)]
        public void RefreshGalleryStoreTheme_TakesThemeWithNonDefaultType_DoesNotDownloadsFreshTheme(
            ExtensionType extensionType)
        {
            var extensionMetadata = new ExtensionMetadata { Id = "expectedId_test" };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedExtensionType(extensionMetadata.Id))
                .ReturnsAsync(extensionType);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(extensionMetadata))
                .ReturnsAsync(new VSCodeTheme());
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(extensionMetadata).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedExtensionType(extensionMetadata.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(extensionMetadata), Times.Never);
        }

        [Theory]
        [InlineData(ExtensionType.NoThemes)]
        [InlineData(ExtensionType.NeedAttention)]
        public void RefreshGalleryStoreTheme_TakesNonDefaultTypeTheme_DoesNotRefreshTheme(
            ExtensionType extensionType)
        {
            var extensionMetadata = new ExtensionMetadata { Id = "expectedId_test" };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedExtensionType(extensionMetadata.Id))
                .ReturnsAsync(extensionType);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(It.IsAny<VSCodeTheme>()));
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()));
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(extensionMetadata).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedExtensionType(extensionMetadata.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CreateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CreateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
        }

        [Fact]
        public void RefreshGalleryStoreTheme_TakesThemeWithActualVersion_DoesNotDownloadsFreshTheme()
        {
            var extensionMetadata = new ExtensionMetadata { Id = "expectedId_test", Version = "version_test" };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedExtensionType(extensionMetadata.Id))
                .ReturnsAsync(ExtensionType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(extensionMetadata.Id))
                .ReturnsAsync(new VSCodeTheme { Version = extensionMetadata.Version });
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(extensionMetadata))
                .ReturnsAsync(new VSCodeTheme());
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(extensionMetadata).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedExtensionType(extensionMetadata.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetStoredTheme(extensionMetadata.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(extensionMetadata), Times.Never);
        }

        [Fact]
        public void RefreshGalleryStoreTheme_TakesThemeWithActualVersion_DoesNotRefreshTheme()
        {
            var extensionMetadata = new ExtensionMetadata { Id = "expectedId_test", Version = "version_test" };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedExtensionType(extensionMetadata.Id))
                .ReturnsAsync(ExtensionType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(extensionMetadata.Id))
                .ReturnsAsync(new VSCodeTheme { Version = extensionMetadata.Version });
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(It.IsAny<VSCodeTheme>()));
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()));
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(extensionMetadata).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedExtensionType(extensionMetadata.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetStoredTheme(extensionMetadata.Id), Times.Once);
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
            var newExtansionMetadata = new ExtensionMetadata { Id = expectedId, Version = expectedVersion };
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
                .Setup(x => x.GetSavedExtensionType(newExtansionMetadata.Id))
                .ReturnsAsync(ExtensionType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(newExtansionMetadata.Id))
                .ReturnsAsync(() => null);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(newExtansionMetadata))
                .ReturnsAsync(newTheme);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(newTheme))
                .Returns(Task.CompletedTask);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()));
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(newExtansionMetadata).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedExtensionType(newExtansionMetadata.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(newExtansionMetadata), Times.Once);
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
            var newExtensionMetadata = new ExtensionMetadata { Id = expectedId, Version = expectedVersion };
            var newTheme = new VSCodeTheme
            {
                Id = expectedId,
                Version = expectedVersion,
                Themes = new List<Theme>()
            };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedExtensionType(newExtensionMetadata.Id))
                .ReturnsAsync(ExtensionType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(newExtensionMetadata.Id))
                .ReturnsAsync(() => null);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(newExtensionMetadata))
                .ReturnsAsync(newTheme);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CheckAndUpdateFreshExtensionType(newTheme))
                .ReturnsAsync(ExtensionType.NoThemes);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(It.IsAny<VSCodeTheme>()));
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()));
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(newExtensionMetadata).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedExtensionType(newExtensionMetadata.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(newExtensionMetadata), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CheckAndUpdateFreshExtensionType(newTheme), Times.Once);
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
            var newExtensionMetadata = new ExtensionMetadata { Id = expectedId, Version = expectedVersion };
            var newTheme = new VSCodeTheme
            {
                Id = expectedId,
                Version = expectedVersion,
                Themes = new List<Theme> { new Theme() }
            };

            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetSavedExtensionType(newExtensionMetadata.Id))
                .ReturnsAsync(ExtensionType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(newExtensionMetadata.Id))
                .ReturnsAsync(() => null);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(newExtensionMetadata))
                .ReturnsAsync(newTheme);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CheckAndUpdateFreshExtensionType(newTheme))
                .ReturnsAsync(ExtensionType.NeedAttention);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(It.IsAny<VSCodeTheme>()));
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(It.IsAny<VSCodeTheme>()));
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(newExtensionMetadata).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedExtensionType(newExtensionMetadata.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(newExtensionMetadata), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CheckAndUpdateFreshExtensionType(newTheme), Times.Once);
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
            var freshExtensionMetada = new ExtensionMetadata { Id = expectedId, Version = "version2" };
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
                .Setup(x => x.GetSavedExtensionType(freshExtensionMetada.Id))
                .ReturnsAsync(ExtensionType.Default);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.GetStoredTheme(freshExtensionMetada.Id))
                .ReturnsAsync(oldTheme);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.DownloadFreshTheme(freshExtensionMetada))
                .ReturnsAsync(freshTheme);
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.CreateTheme(It.IsAny<VSCodeTheme>()));
            _mock.Mock<IThemeStoreRefresher>()
                .Setup(x => x.UpdateTheme(freshTheme))
                .Returns(Task.CompletedTask);
            var mockRefreshService = _mock.Create<ThemeStoreRefreshService>();

            mockRefreshService.RefreshGalleryStoreTheme(freshExtensionMetada).Wait();

            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.GetSavedExtensionType(freshExtensionMetada.Id), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.DownloadFreshTheme(freshExtensionMetada), Times.Once);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.CreateTheme(It.IsAny<VSCodeTheme>()), Times.Never);
            _mock.Mock<IThemeStoreRefresher>()
                .Verify(x => x.UpdateTheme(freshTheme), Times.Once);
        }
    }
}

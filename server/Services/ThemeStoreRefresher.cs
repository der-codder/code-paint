using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodePaint.WebApi.Domain.Models;
using CodePaint.WebApi.Domain.Repositories;
using Polly;
using Polly.Retry;
using Serilog;

namespace CodePaint.WebApi.Services
{
    public interface IThemeStoreRefresher
    {
        Task<GalleryItemType> GetSavedGalleryItemType(string extensionId);
        Task<VSCodeTheme> GetStoredTheme(string extensionId);
        Task<VSCodeTheme> DownloadFreshTheme(GalleryItem metadata);
        Task<GalleryItemType> CheckAndUpdateFreshThemeType(VSCodeTheme freshTheme);
        Task CreateTheme(VSCodeTheme newTheme);
        Task UpdateTheme(VSCodeTheme theme);
    }

    public class ThemeStoreRefresher : IThemeStoreRefresher
    {
        private readonly IVSMarketplaceClient _marketplaceClient;
        private readonly IVSExtensionHandler _extensionHandler;
        private readonly IVSCodeThemeStoreRepository _storeRepository;
        private readonly IGalleryItemsRepository _galleryItemsRepository;

        public ThemeStoreRefresher(
            IVSMarketplaceClient marketplaceClient,
            IVSExtensionHandler extensionHandler,
            IVSCodeThemeStoreRepository storeRepository,
            IGalleryItemsRepository galleryInfoRepository)
        {
            _marketplaceClient = marketplaceClient;
            _extensionHandler = extensionHandler;
            _storeRepository = storeRepository;
            _galleryItemsRepository = galleryInfoRepository;
        }

        public async Task<GalleryItemType> GetSavedGalleryItemType(string extensionId)
        {
            var savedThemeMetadata = await _galleryItemsRepository.GetGalleryItem(extensionId);

            return savedThemeMetadata.Type;
        }

        public async Task<VSCodeTheme> GetStoredTheme(string extensionId)
        {
            return await _storeRepository.GetTheme(extensionId);
        }

        public async Task<VSCodeTheme> DownloadFreshTheme(GalleryItem metadata)
        {
            VSCodeTheme freshTheme;
            using (var stream = await _marketplaceClient.GetVsixFileStream(metadata))
            {
                freshTheme = await _extensionHandler.ProcessExtension(stream);
            }

            return freshTheme;
        }

        public async Task<GalleryItemType> CheckAndUpdateFreshThemeType(VSCodeTheme freshTheme)
        {
            if (freshTheme.Themes.Count == 0)
            {
                Log.Information($"Extension '{freshTheme.Id}' does not contribute any theme. Changing its Type.");
                const GalleryItemType themeType = GalleryItemType.NoThemes;
                var result = await _galleryItemsRepository.ChangeGalleryItemType(
                    freshTheme.Id,
                    themeType
                );

                if (result)
                {
                    Log.Information($"Successfully changed Type to {themeType}.");
                }
                else
                {
                    Log.Warning($"Type change unsuccessful: ExtensionId: '{freshTheme.Id}'.");
                }

                return themeType;
            }
            if (freshTheme.Themes.Any(t => t.TokenColors.Count == 0))
            {
                Log.Information($"Extension '{freshTheme.Id}' does not contains any TokenColor. Changing its Type.");
                const GalleryItemType themeType = GalleryItemType.NeedAttention;
                var result = await _galleryItemsRepository.ChangeGalleryItemType(
                    freshTheme.Id,
                    themeType
                );

                if (result)
                {
                    Log.Information($"Successfully changed Type to {themeType}.");
                }
                else
                {
                    Log.Warning($"Type change unsuccessful: ExtensionId: '{freshTheme.Id}'.");
                }

                return themeType;
            }

            return GalleryItemType.Default;
        }

        public async Task CreateTheme(VSCodeTheme newTheme)
        {
            await _storeRepository.Create(newTheme);
            Log.Information($"Created new theme in store: Id='{newTheme.Id}'.");
        }

        public async Task UpdateTheme(VSCodeTheme theme)
        {
            var result = await _storeRepository.Update(theme);

            if (result)
            {
                Log.Information($"Successfuly updated theme in store: ThemeId='{theme.Id}'.");
            }
            else
            {
                Log.Warning($"Update unsuccessful theme in store: ThemeId='{theme.Id}'.");
            }
        }
    }
}

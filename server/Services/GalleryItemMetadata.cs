using System;
using CodePaint.WebApi.Domain.Models;

namespace CodePaint.WebApi.Services
{
    public class GalleryItemMetadata
    {
        public GalleryItem ThemeInfo { get; set; }
        public GalleryItemStatistic ThemeStatistic { get; set; }
    }
}

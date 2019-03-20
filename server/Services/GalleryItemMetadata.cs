using System;
using CodePaint.WebApi.Domain.Models;

namespace CodePaint.WebApi.Services
{
    public class GalleryItemMetadata
    {
        public GalleryItem GalleryItem { get; set; }
        public GalleryItemStatistic GalleryItemStatistic { get; set; }
    }
}

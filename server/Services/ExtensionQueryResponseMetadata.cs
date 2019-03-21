using System;
using System.Collections.Generic;
using CodePaint.WebApi.Domain.Models;

namespace CodePaint.WebApi.Services
{
    public class ExtensionQueryResponseMetadata
    {
        public int RequestResultTotalCount { get; set; }
        public List<(GalleryItem GalleryItem, GalleryItemStatistic Statistic)> Items { get; }

        public ExtensionQueryResponseMetadata()
        {
            Items = new List<(GalleryItem, GalleryItemStatistic)>();
        }
    }
}

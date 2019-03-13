using System;
using MongoDB.Driver;

namespace CodePaint.WebApi.Models
{
    public interface IGalleryContext
    {
        IMongoCollection<ThemeInfo> ThemesInfo { get; }
    }
}

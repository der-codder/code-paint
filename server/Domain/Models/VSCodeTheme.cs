using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodePaint.WebApi.Domain.Models
{
    public class VSCodeTheme
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string GalleryItemId { get; set; }

        public string Version { get; set; }

        public List<Theme> Themes { get; set; }

        public VSCodeTheme() => Themes = new List<Theme>();
    }
}

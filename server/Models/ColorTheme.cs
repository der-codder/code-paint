using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodePaint.WebApi.Models
{
    public class ColorTheme
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string ThemeId { get; set; }
        public string Version { get; set; }
        public List<Theme> Themes { get; set; }

        public ColorTheme()
        {
            Themes = new List<Theme>();
        }
    }
}

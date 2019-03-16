using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodePaint.WebApi.Models
{
    public class ColorTheme
    {
        [BsonId]
        public ObjectId InternalId { get; set; }

        [BsonElement("themeId")]
        public string ThemeId { get; set; }

        [BsonElement("version")]
        public string Version { get; set; }

        [BsonElement("themes")]
        public List<Theme> Themes { get; set; }

        public ColorTheme()
        {
            Themes = new List<Theme>();
        }
    }
}

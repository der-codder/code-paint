using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace CodePaint.WebApi.Domain.Models
{
    // TODO: Change VSCodeTheme to VSCodeExtension
    public class VSCodeTheme
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        public string Version { get; set; }

        public List<Theme> Themes { get; set; } = new List<Theme>();
    }
}

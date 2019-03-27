using System;

namespace CodePaint.WebApi.Resources
{
    public class ExtensionMetadataResource
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string PublisherName { get; set; }
        public string PublisherDisplayName { get; set; }
        public string Version { get; set; }
        public DateTime LastUpdated { get; set; }
        public string IconDefault { get; set; }
        public string IconSmall { get; set; }
        public StatisticResource Statistics { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodePaint.WebApi.Controllers.Resources
{
    public class ExtensionResource : ExtensionMetadataResource
    {
        public List<ThemeResource> Themes { get; set; }
    }
}

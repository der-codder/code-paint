using System;
using System.Collections.Generic;

namespace CodePaint.WebApi.Controllers.Resources
{
    public class ThemeResource
    {
        public string Label { get; set; }
        public string Base { get; set; }
        public bool Inherit { get; set; } = true;
        public Dictionary<string, string> Colors { get; set; }
        public List<TokenColorResource> Rules { get; set; }
    }

    public class TokenColorResource
    {
        public string Token { get; set; }
        public string Foreground { get; set; }
        public string FontStyle { get; set; }
    }
}

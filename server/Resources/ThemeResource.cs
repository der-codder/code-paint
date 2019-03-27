using System;
using System.Collections.Generic;

namespace CodePaint.WebApi.Resources
{
    public class ThemeResource
    {
        public string Label { get; set; }
        public string ThemeType { get; set; }
        // public Dictionary<string, string> Colors { get; set; }
        public List<TokenColorResource> TokenColors { get; set; }
    }

    public class TokenColorResource
    {
        public string Name { get; set; }
        public string Scope { get; set; }
        public TokenColorSettingsResource Settings { get; set; }
    }

    public class TokenColorSettingsResource
    {
        public string Foreground { get; set; }
        public string FontStyle { get; set; }
    }
}

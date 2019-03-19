using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace CodePaint.WebApi.Domain.Models
{
    public class TokenColorSettings
    {
        public string Foreground { get; set; }

        public string FontStyle { get; set; }
    }

    public class TokenColor
    {
        public string Name { get; set; }

        public string Scope { get; set; }

        public TokenColorSettings Settings { get; set; }
    }

    public class Theme
    {
        public string Label { get; set; }
        public string ThemeType { get; set; }
        public Dictionary<string, string> Colors { get; set; }
        public List<TokenColor> TokenColors { get; set; }

        public Theme()
        {
            Colors = new Dictionary<string, string>();
            TokenColors = new List<TokenColor>();
        }

        public static Theme FromJson(
            JObject jObject,
            string label,
            string themeType)
        {
            var theme = new Theme { Label = label, ThemeType = themeType };

            var jColors = jObject.SelectToken("colors");
            if (jColors != null)
            {
                theme.Colors = jColors.ToObject<Dictionary<string, string>>();
            }

            var jTokenColors = jObject.SelectToken("tokenColors");
            if (jTokenColors != null)
            {
                foreach (var jTokenColor in jTokenColors)
                {
                    var tokenColor = ParseTokenColor((JObject) jTokenColor);
                    if (!string.IsNullOrWhiteSpace(tokenColor.Scope)
                        && (!string.IsNullOrWhiteSpace(tokenColor.Settings.FontStyle)
                            || !string.IsNullOrWhiteSpace(tokenColor.Settings.Foreground)))
                    {
                        theme.TokenColors.Add(tokenColor);
                    }
                }
            }

            return theme;
        }

        public static TokenColor ParseTokenColor(JObject jObject)
        {
            var tokenColor = new TokenColor { Settings = new TokenColorSettings() };

            var jScope = jObject.SelectToken("scope");
            if (jScope != null)
            {
                if (jScope.Type == JTokenType.String)
                {
                    tokenColor.Scope = jScope.ToString();
                }
                else if (jScope.Type == JTokenType.Array)
                {
                    tokenColor.Scope = string
                        .Join(
                            ',',
                            ((JArray) jScope).Select(item => item.ToString())
                        );
                }
                else
                {
                    throw new FormatException($"Scope contains inappropriate formate: {jScope}");
                }
            }

            tokenColor.Name = (string) jObject.SelectToken("name");
            tokenColor.Settings.Foreground = (string) jObject.SelectToken("settings.foreground");
            tokenColor.Settings.FontStyle = (string) jObject.SelectToken("settings.fontStyle");

            return tokenColor;
        }
    }
}

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

    public class ColorCustomization
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
    }

    public class Theme
    {
        public string Label { get; set; }
        public string ThemeType { get; set; }
        public List<ColorCustomization> Colors { get; set; }
        public List<TokenColor> TokenColors { get; set; }

        public Theme()
        {
            Colors = new List<ColorCustomization>();
            TokenColors = new List<TokenColor>();
        }

        public static List<ColorCustomization> ParseColors(JObject jObject)
        {
            var colors = new List<ColorCustomization>();

            var jColors = jObject.SelectToken("colors");
            if (jColors != null)
            {
                var dictionary = jColors.ToObject<Dictionary<string, string>>();

                foreach (var item in dictionary)
                {
                    var colorCustomization = new ColorCustomization
                    {
                        PropertyName = item.Key,
                        Value = item.Value
                    };
                    colors.Add(colorCustomization);
                }
            }

            return colors;
        }

        public static List<TokenColor> ParseTokenColors(JObject jObject)
        {
            var tokenColors = new List<TokenColor>();
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
                        tokenColors.Add(tokenColor);
                    }
                }
            }

            return tokenColors;
        }

        private static TokenColor ParseTokenColor(JObject jObject)
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

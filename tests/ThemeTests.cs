using System;
using System.Collections.Generic;
using System.Linq;
using CodePaint.WebApi.Domain.Models;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CodePaint.WebApi.Tests
{
    public class ThemeTests
    {
        [Fact]
        public void FromJson_SimpleValidJObject_ReturnsCorrectResult()
        {
            const string jsonString = @"{
                'colors': {
                    'activityBar.background': '#ff0000'
                },
                'tokenColors': [{
                        'name': 'tokenColor_name',
                        'scope': 'tokenColor_scope',
                        'settings': {
                            'foreground': 'settings_foreground',
                            'fontStyle': 'settings_fontStyle'
                        }
                    }
                ]
            }";

            var result = Theme.FromJson(
                JObject.Parse(jsonString),
                "label_test",
                "type_test"
            );

            Assert.Equal("label_test", result.Label);
            Assert.Equal("type_test", result.ThemeType);
            Assert.Equal("#ff0000", result.Colors["activityBar.background"]);
            Assert.Equal("tokenColor_name", result.TokenColors[0].Name);
            Assert.Equal("tokenColor_scope", result.TokenColors[0].Scope);
            Assert.Equal("settings_foreground", result.TokenColors[0].Settings.Foreground);
            Assert.Equal("settings_fontStyle", result.TokenColors[0].Settings.FontStyle);
        }

        [Fact]
        public void FromJson_TokenColorWithOnlyRequiredProperies_ReturnsCorrectResult()
        {
            const string jsonString = @"{
                'tokenColors': [{
                        'scope': 'tokenColor_scope',
                        'settings': {
                            'foreground': 'settings_foreground'
                        }
                    }
                ]
            }";

            var result = Theme.FromJson(
                JObject.Parse(jsonString),
                "label_test",
                "type_test"
            );

            Assert.Equal("tokenColor_scope", result.TokenColors[0].Scope);
            Assert.Equal("settings_foreground", result.TokenColors[0].Settings.Foreground);
        }

        [Theory]
        // Scope is empty
        [InlineData("{'tokenColors':[{'name':'tokenColor_name','settings':{'foreground':'settings_foreground','fontStyle':'settings_fontStyle'}}]}")]
        // Settings is empty
        [InlineData("{'tokenColors':[{'name':'tokenColor_name','scope':'tokenColor_scope'}]}")]
        public void FromJson_TokenColorWithInvalidProperies_ShouldNotReturnThisTokenColors(string jsonString)
        {
            var result = Theme.FromJson(
                JObject.Parse(jsonString),
                "label_test",
                "type_test"
            );

            Assert.Empty(result.TokenColors);
        }

        [Theory]
        [InlineData("{'tokenColors':[{'scope':'scope1,scope2,scope3','settings':{'foreground':'settings_foreground'}}]}")]
        [InlineData("{'tokenColors':[{'scope':['scope1','scope2','scope3'],'settings':{'foreground':'settings_foreground'}}]}")]
        public void FromJson_JObjectWithValidScope_ReturnsCorrectResult(string jsonString)
        {
            var result = Theme.FromJson(
                JObject.Parse(jsonString),
                "label_test",
                "type_test"
            );

            Assert.Single(result.TokenColors, token => token.Scope == "scope1,scope2,scope3");
        }
    }
}

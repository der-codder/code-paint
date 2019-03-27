using System;
using System.IO;
using Autofac.Extras.Moq;
using CodePaint.WebApi.Services;
using CodePaint.WebApi.Services.ThemeStoreRefreshing;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CodePaint.WebApi.Tests.Services
{
    public class VSCodeThemeParserTests : IDisposable
    {
        private readonly AutoMock _mock;

        public VSCodeThemeParserTests()
        {
            _mock = AutoMock.GetLoose();
        }

        public void Dispose()
        {
            _mock.Dispose();
        }

        [Fact]
        public void LoadTheme_ProperlyLoadsThemeWithNoIncludes()
        {
            var jObject = JObject.Parse(@"{
                'colors': {
                    'activityBar.background': '#ff0000'
                },
                'tokenColors': [{
                        'scope': 'tokenColor_scope',
                        'settings': {
                            'foreground': 'settings_foreground'
                        }
                    }
                ]
            }");
            var metadata = new ThemeMetadata
            {
                Label = "label_test",
                ThemeType = "type_test",
                Path = "test/theme.json"
            };

            _mock.Mock<IJsonFileLoader>()
                .Setup(x => x.Load(metadata.Path))
                .ReturnsAsync(jObject);
            var mockParser = _mock.Create<VSCodeThemeParser>();

            var actual = mockParser.LoadTheme(metadata).Result;

            Assert.Equal(metadata.Label, actual.Label);
            Assert.Equal(metadata.ThemeType, actual.ThemeType);
            Assert.Equal("activityBar.background", actual.Colors[0].PropertyName);
            Assert.Equal("#ff0000", actual.Colors[0].Value);
            Assert.Equal("tokenColor_scope", actual.TokenColors[0].Scope);
            Assert.Equal("settings_foreground", actual.TokenColors[0].Settings.Foreground);
        }

        [Fact]
        public void LoadTheme_ProperlyLoadsThemeWithIncludesInTheSameFolder()
        {
            var tmpFolder = Path.GetTempPath();
            var expectedRootThemePath = tmpFolder + "rootTheme.json";
            var expectedIncludedThemePath = tmpFolder + "includedTheme.json";
            var rootTheme = JObject.Parse(@"{
                'include': './includedTheme.json',
                'colors': {
                    'activityBar.background': '#ff0000'
                }
            }");
            var includedTheme = JObject.Parse(@"{
                'tokenColors': [{
                        'scope': 'tokenColor_scope',
                        'settings': {
                            'foreground': 'settings_foreground'
                        }
                    }
                ]
            }");
            var metadata = new ThemeMetadata
            {
                Label = "label_test",
                ThemeType = "type_test",
                Path = expectedRootThemePath
            };

            _mock.Mock<IJsonFileLoader>()
                .Setup(x => x.Load(expectedRootThemePath))
                .ReturnsAsync(rootTheme);
            _mock.Mock<IJsonFileLoader>()
                .Setup(x => x.Load(expectedIncludedThemePath))
                .ReturnsAsync(includedTheme);
            var mockParser = _mock.Create<VSCodeThemeParser>();

            var actual = mockParser.LoadTheme(metadata).Result;

            _mock.Mock<IJsonFileLoader>()
                .Verify(x => x.Load(expectedRootThemePath), Times.Once);
            _mock.Mock<IJsonFileLoader>()
                .Verify(x => x.Load(expectedIncludedThemePath), Times.Once);

            Assert.Equal(metadata.Label, actual.Label);
            Assert.Equal(metadata.ThemeType, actual.ThemeType);
            Assert.Equal("activityBar.background", actual.Colors[0].PropertyName);
            Assert.Equal("#ff0000", actual.Colors[0].Value);
            Assert.Equal("tokenColor_scope", actual.TokenColors[0].Scope);
            Assert.Equal("settings_foreground", actual.TokenColors[0].Settings.Foreground);
        }

        [Fact]
        public void LoadTheme_ProperlyLoadsThemeWithIncludesInTheDifferentFolders()
        {
            var tmpFolder = Path.GetTempPath();
            var separator = Path.DirectorySeparatorChar;
            var expectedRootThemePath = tmpFolder + "rootTheme.json";
            var expectedIncludedTheme1Path = tmpFolder + "include" + separator + "includedTheme1.json";
            var expectedIncludedTheme2Path = tmpFolder + "include" + separator + "includedTheme2.json";
            var rootTheme = JObject.Parse(@"{
                'include': './include/includedTheme1.json',
                'colors': {
                    'rootTheme': '#ff0000'
                }
            }");
            var includedTheme1 = JObject.Parse(@"{
                'include': './includedTheme2.json',
                'colors': {
                    'includedTheme1': '#00ff00'
                }
            }");
            var includedTheme2 = JObject.Parse(@"{
                'colors': {
                    'includedTheme2': '#0000ff'
                }
            }");
            var metadata = new ThemeMetadata
            {
                Label = "label_test",
                ThemeType = "type_test",
                Path = tmpFolder + "rootTheme.json"
            };

            _mock.Mock<IJsonFileLoader>()
                .Setup(x => x.Load(expectedRootThemePath))
                .ReturnsAsync(rootTheme);
            _mock.Mock<IJsonFileLoader>()
                .Setup(x => x.Load(expectedIncludedTheme1Path))
                .ReturnsAsync(includedTheme1);
            _mock.Mock<IJsonFileLoader>()
                .Setup(x => x.Load(expectedIncludedTheme2Path))
                .ReturnsAsync(includedTheme2);
            var mockParser = _mock.Create<VSCodeThemeParser>();

            var actual = mockParser.LoadTheme(metadata).Result;

            _mock.Mock<IJsonFileLoader>()
                .Verify(x => x.Load(expectedRootThemePath), Times.Once);
            _mock.Mock<IJsonFileLoader>()
                .Verify(x => x.Load(expectedIncludedTheme1Path), Times.Once);
            _mock.Mock<IJsonFileLoader>()
                .Verify(x => x.Load(expectedIncludedTheme2Path), Times.Once);

            Assert.Equal(metadata.Label, actual.Label);
            Assert.Equal(metadata.ThemeType, actual.ThemeType);
            Assert.Equal("rootTheme", actual.Colors[0].PropertyName);
            Assert.Equal("#ff0000", actual.Colors[0].Value);
            Assert.Equal("includedTheme1", actual.Colors[1].PropertyName);
            Assert.Equal("#00ff00", actual.Colors[1].Value);
            Assert.Equal("includedTheme2", actual.Colors[2].PropertyName);
            Assert.Equal("#0000ff", actual.Colors[2].Value);
        }

        [Fact]
        public void LoadTheme_ThemeFileDifferentFromJson_ReturnThemeWithoutTokenColorsAndColors()
        {
            var metadata = new ThemeMetadata
            {
                Label = "label_test",
                ThemeType = "type_test",
                Path = "test/theme.tmTheme"
            };
            var mockParser = _mock.Create<VSCodeThemeParser>();

            var actual = mockParser.LoadTheme(metadata).Result;

            Assert.Equal(metadata.Label, actual.Label);
            Assert.Equal(metadata.ThemeType, actual.ThemeType);
            Assert.Empty(actual.Colors);
            Assert.Empty(actual.TokenColors);
        }
    }
}

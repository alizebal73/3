using System.Globalization;
using GameNet.Desktop.Localization;

namespace GameNet.Desktop.Tests;

public sealed class LocalizationFoundationTests
{
    [Theory]
    [InlineData("fa-IR")]
    [InlineData("en-US")]
    public void Supported_cultures_are_accepted(string cultureName)
    {
        var service = new LanguageService();

        service.SetLanguage(CultureInfo.GetCultureInfo(cultureName));

        Assert.Equal(cultureName, service.CurrentCulture.Name);
    }

    [Fact]
    public void Unsupported_culture_is_rejected()
    {
        var service = new LanguageService();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.SetLanguage(CultureInfo.GetCultureInfo("de-DE")));
    }
}

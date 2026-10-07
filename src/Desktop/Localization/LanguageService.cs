using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace GameNet.Desktop.Localization;

public sealed class LanguageService
{
    private const string DictionaryPrefix =
        "/GameNet.Manager.Desktop;component/Resources/Languages/Strings.";

    private ResourceDictionary? _currentDictionary;

    public CultureInfo CurrentCulture { get; private set; } =
        CultureInfo.GetCultureInfo("fa-IR");

    public event EventHandler? LanguageChanged;

    public string GetString(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return Application.Current?.TryFindResource(key) as string
            ?? key;
    }

    public void SetLanguage(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        if (culture.Name is not ("fa-IR" or "en-US"))
            throw new ArgumentOutOfRangeException(
                nameof(culture),
                $"Unsupported UI culture: {culture.Name}");

        CurrentCulture = culture;

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        if (Application.Current is not null)
        {
            var dictionary = new ResourceDictionary
            {
                Source = new Uri(
                    $"{DictionaryPrefix}{culture.Name}.xaml",
                    UriKind.Relative)
            };

            if (_currentDictionary is not null)
                Application.Current.Resources.MergedDictionaries.Remove(
                    _currentDictionary);

            Application.Current.Resources.MergedDictionaries.Add(dictionary);
            _currentDictionary = dictionary;

            if (Application.Current.MainWindow is { } window)
            {
                window.FlowDirection = culture.Name.StartsWith("fa", StringComparison.Ordinal)
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight;

                window.Language = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
            }
        }

        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}

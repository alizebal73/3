using System.Globalization;
using System.Windows;
using GameNet.Desktop.Localization;

namespace GameNet.Desktop.Shell;

public partial class MainWindow : Window
{
    private readonly LanguageService _language;

    public MainWindow(LanguageService language)
    {
        InitializeComponent();
        _language = language;
    }

    private void EnglishClick(object sender, RoutedEventArgs e)
    {
        _language.SetLanguage(CultureInfo.GetCultureInfo("en-US"));
    }

    private void PersianClick(object sender, RoutedEventArgs e)
    {
        _language.SetLanguage(CultureInfo.GetCultureInfo("fa-IR"));
    }
}

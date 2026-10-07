using System.Globalization;
using System.Windows;
using GameNet.Desktop.Localization;
using GameNet.Desktop.UI.Shell;

namespace GameNet.Desktop.Shell;

public partial class MainWindow : Window
{
    private readonly LanguageService _language;
    private readonly ShellViewModel _viewModel;

    public MainWindow(
        LanguageService language,
        ShellViewModel viewModel)
    {
        InitializeComponent();

        _language = language;
        _viewModel = viewModel;
        DataContext = viewModel;

        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    private async void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync(CancellationToken.None);
    }

    private async void OnClosed(
        object? sender,
        EventArgs e)
    {
        await _viewModel.DisposeAsync();
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

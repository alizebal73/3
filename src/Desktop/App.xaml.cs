using System.Globalization;
using System.Windows;
using GameNet.Desktop.Localization;
using GameNet.Desktop.Shell;

namespace GameNet.Desktop;

public partial class App : Application
{
    public static LanguageService Language { get; } = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        MainWindow = new MainWindow();
        Language.SetLanguage(CultureInfo.GetCultureInfo("fa-IR"));
        MainWindow.Show();
    }
}

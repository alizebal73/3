using System.Globalization;
using System.Windows;

namespace GameNet.Desktop.Shell;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void EnglishClick(object sender, RoutedEventArgs e)
    {
        App.Language.SetLanguage(CultureInfo.GetCultureInfo("en-US"));
    }

    private void PersianClick(object sender, RoutedEventArgs e)
    {
        App.Language.SetLanguage(CultureInfo.GetCultureInfo("fa-IR"));
    }
}

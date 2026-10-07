using System.Windows;
using GameNet.Desktop.UI.Shell;

namespace GameNet.Desktop.Shell;

public partial class MainWindow : Window
{
    private readonly ShellViewModel _viewModel;

    public MainWindow(ShellViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = viewModel;

        Loaded += OnLoaded;
    }

    private async void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync(
            Application.Current.Dispatcher
                .AsTaskCancellationToken());
    }
}

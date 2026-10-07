using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using GameNet.Desktop.Api;
using GameNet.Desktop.Localization;
using GameNet.Desktop.UI.Commands;
using GameNet.Desktop.UI.Navigation;
using GameNet.Desktop.UI.Services;
using GameNet.Desktop.UI.State;

namespace GameNet.Desktop.UI.Shell;

public sealed class ShellViewModel : INotifyPropertyChanged, IAsyncDisposable
{
    private readonly LanguageService _language;
    private readonly ServerConnectivityMonitor _connectivity;

    public ShellViewModel(
        LanguageService language,
        ServerConnectivityMonitor connectivity)
    {
        _language = language;
        _connectivity = connectivity;

        RefreshCommand = new UiCommand(
            _ => _ = RefreshAsync(),
            _ => !IsBusy);

        SelectNavigationCommand = new UiCommand(
            parameter =>
            {
                if (parameter is NavigationItem item)
                    Select(item);
            });

        _language.LanguageChanged += OnLanguageChanged;
        _connectivity.StateChanged += OnConnectivityChanged;

        RebuildNavigation();

        if (NavigationItems.Count > 0)
            SelectedNavigation = NavigationItems[0];
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<NavigationItem> NavigationItems { get; private set; } = [];

    public NavigationItem? SelectedNavigation { get; private set; }

    public UiState UiState => _connectivity.State;

    public string WindowTitle =>
        _language.GetString("AppTitle");

    public string CurrentPageTitle =>
        SelectedNavigation is null
            ? WindowTitle
            : SelectedNavigation.DisplayName;

    public string ConnectionText =>
        UiState.Connection switch
        {
            UiConnectionState.Online => _language.GetString("StatusOnline"),
            UiConnectionState.Connecting => _language.GetString("StatusConnecting"),
            UiConnectionState.Offline => _language.GetString("StatusOffline"),
            _ => _language.GetString("StatusUnknown")
        };

    public bool IsBusy => UiState.IsBusy;

    public bool HasError => !string.IsNullOrWhiteSpace(UiState.ErrorMessage);

    public string ErrorMessage =>
        UiState.ErrorMessage ?? string.Empty;

    public UiCommand SelectNavigationCommand { get; }

    public UiCommand RefreshCommand { get; }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await RefreshAsync(cancellationToken);
    }

    public void Select(NavigationItem item)
    {
        SelectedNavigation = item;
        OnPropertyChanged(nameof(SelectedNavigation));
        OnPropertyChanged(nameof(CurrentPageTitle));
    }

    public void SetLanguage(CultureInfo culture) =>
        _language.SetLanguage(culture);

    private async Task RefreshAsync(
        CancellationToken cancellationToken = default)
    {
        await _connectivity.RefreshAsync(cancellationToken);
        OnConnectivityChanged(this, EventArgs.Empty);
    }

    private void RebuildNavigation()
    {
        var selectedId = SelectedNavigation?.Id;

        NavigationItems = NavigationCatalog.Items
            .Where(item => item.IsEnabled)
            .Select(item => item with
            {
                DisplayName = _language.GetString(item.ResourceKey)
            })
            .ToArray();

        SelectedNavigation =
            NavigationItems.FirstOrDefault(item => item.Id == selectedId)
            ?? NavigationItems.FirstOrDefault();

        OnPropertyChanged(nameof(NavigationItems));
        OnPropertyChanged(nameof(SelectedNavigation));
        OnPropertyChanged(nameof(CurrentPageTitle));
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        RebuildNavigation();
        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(CurrentPageTitle));
        OnPropertyChanged(nameof(ConnectionText));
    }

    private void OnConnectivityChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(UiState));
        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(ConnectionText));
        RefreshCommand.RaiseCanExecuteChanged();
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));

    public ValueTask DisposeAsync()
    {
        _language.LanguageChanged -= OnLanguageChanged;
        _connectivity.StateChanged -= OnConnectivityChanged;
        return ValueTask.CompletedTask;
    }
}

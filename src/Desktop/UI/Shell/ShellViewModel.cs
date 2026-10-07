using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
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
    private string _commandPaletteQuery = string.Empty;
    private bool _isCommandPaletteOpen;
    private bool _isHelpOpen;
    private bool _isNotificationCenterOpen;
    private IReadOnlyList<NavigationItem> _commandPaletteItems = [];

    public ShellViewModel(
        LanguageService language,
        ServerConnectivityMonitor connectivity)
    {
        _language = language;
        _connectivity = connectivity;

        RefreshCommand = new UiCommand(
            _ => _ = RefreshAsync(),
            _ => !IsBusy);

        ToggleLanguageCommand = new UiCommand(
            _ => ToggleLanguage());

        NavigateHomeCommand = new UiCommand(
            _ => NavigateHome());

        ToggleCommandPaletteCommand = new UiCommand(
            _ => ToggleCommandPalette());

        ToggleHelpCommand = new UiCommand(
            _ => ToggleHelp());

        ToggleNotificationCenterCommand = new UiCommand(
            _ => ToggleNotificationCenter(),
            _ => HasNotifications);

        CloseOverlaysCommand = new UiCommand(
            _ => CloseOverlays());

        SelectCommandPaletteItemCommand = new UiCommand(
            parameter =>
            {
                if (parameter is NavigationItem item)
                {
                    Select(item);
                    CloseOverlays();
                }
            });

        SelectNavigationCommand = new UiCommand(
            parameter =>
            {
                if (parameter is NavigationItem item)
                    Select(item);
            });

        _language.LanguageChanged += OnLanguageChanged;
        _connectivity.StateChanged += OnConnectivityChanged;

        RebuildNavigation();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<NavigationItem> NavigationItems { get; private set; } = [];

    public NavigationItem? SelectedNavigation { get; private set; }

    public UiState UiState => _connectivity.State;

    public string CommandPaletteQuery
    {
        get => _commandPaletteQuery;
        set
        {
            if (string.Equals(_commandPaletteQuery, value, StringComparison.Ordinal))
                return;

            _commandPaletteQuery = value ?? string.Empty;
            RebuildCommandPalette();
            OnPropertyChanged();
        }
    }

    public bool IsCommandPaletteOpen => _isCommandPaletteOpen;
    public bool IsHelpOpen => _isHelpOpen;
    public bool IsNotificationCenterOpen => _isNotificationCenterOpen;
    public IReadOnlyList<NavigationItem> CommandPaletteItems => _commandPaletteItems;
    public bool HasNotifications => HasError || UiState.IsOffline;

    public string NotificationSummary =>
        HasNotifications
            ? $"{_language.GetString("NotificationCenter")}: {ErrorMessage}"
            : _language.GetString("NoNotifications");

    public string WindowTitle => _language.GetString("AppTitle");

    public string CurrentPageTitle =>
        SelectedNavigation?.DisplayName ?? WindowTitle;

    public string ConnectionText =>
        UiState.Connection switch
        {
            UiConnectionState.Online => _language.GetString("StatusOnline"),
            UiConnectionState.Connecting => _language.GetString("StatusConnecting"),
            UiConnectionState.Offline => _language.GetString("StatusOffline"),
            _ => _language.GetString("StatusUnknown")
        };

    public string CurrentPageId =>
        SelectedNavigation?.Id ?? "home";

    public bool IsBusy => UiState.IsBusy;

    public bool HasError => !string.IsNullOrWhiteSpace(UiState.ErrorMessage);

    public string ErrorMessage => UiState.ErrorCode switch
    {
        "SERVER_NOT_READY" => _language.GetString("ErrorServerNotReady"),
        "SERVER_UNAVAILABLE" => _language.GetString("ErrorServerUnavailable"),
        _ when !string.IsNullOrWhiteSpace(UiState.ErrorMessage) =>
            UiState.ErrorMessage!,
        _ => string.Empty
    };

    public UiCommand SelectNavigationCommand { get; }

    public UiCommand RefreshCommand { get; }

    public UiCommand ToggleLanguageCommand { get; }

    public UiCommand NavigateHomeCommand { get; }
    public UiCommand ToggleCommandPaletteCommand { get; }
    public UiCommand ToggleHelpCommand { get; }
    public UiCommand ToggleNotificationCenterCommand { get; }
    public UiCommand CloseOverlaysCommand { get; }
    public UiCommand SelectCommandPaletteItemCommand { get; }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await _connectivity.StartAsync(cancellationToken);
        await RefreshAsync(cancellationToken);
    }

    public void Select(NavigationItem item)
    {
        SelectedNavigation = item;
        OnPropertyChanged(nameof(SelectedNavigation));
        OnPropertyChanged(nameof(CurrentPageTitle));
        OnPropertyChanged(nameof(CurrentPageId));
    }

    public void SetLanguage(CultureInfo culture) =>
        _language.SetLanguage(culture);

    private void ToggleLanguage()
    {
        var target = _language.CurrentCulture.Name == "fa-IR"
            ? CultureInfo.GetCultureInfo("en-US")
            : CultureInfo.GetCultureInfo("fa-IR");

        SetLanguage(target);
    }

    private void NavigateHome()
    {
        var home = NavigationItems.FirstOrDefault(
            item => item.Id == "home");

        if (home is not null)
            Select(home);
    }

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

        RebuildCommandPalette();

        SelectedNavigation =
            NavigationItems.FirstOrDefault(item => item.Id == selectedId)
            ?? NavigationItems.FirstOrDefault();

        OnPropertyChanged(nameof(NavigationItems));
        OnPropertyChanged(nameof(SelectedNavigation));
        OnPropertyChanged(nameof(CurrentPageTitle));
        OnPropertyChanged(nameof(CurrentPageId));
        OnPropertyChanged(nameof(CommandPaletteItems));
    }

    private void RebuildCommandPalette()
    {
        var query = _commandPaletteQuery.Trim();

        _commandPaletteItems = string.IsNullOrWhiteSpace(query)
            ? NavigationItems
            : NavigationItems
                .Where(item =>
                    item.DisplayName.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    item.Id.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToArray();

        OnPropertyChanged(nameof(CommandPaletteItems));
    }

    private void ToggleCommandPalette()
    {
        _isCommandPaletteOpen = !_isCommandPaletteOpen;

        if (_isCommandPaletteOpen)
        {
            _isHelpOpen = false;
            _isNotificationCenterOpen = false;
            CommandPaletteQuery = string.Empty;
        }

        OnPropertyChanged(nameof(IsCommandPaletteOpen));
        OnPropertyChanged(nameof(IsHelpOpen));
        OnPropertyChanged(nameof(IsNotificationCenterOpen));
    }

    private void ToggleHelp()
    {
        _isHelpOpen = !_isHelpOpen;

        if (_isHelpOpen)
        {
            _isCommandPaletteOpen = false;
            _isNotificationCenterOpen = false;
        }

        OnPropertyChanged(nameof(IsHelpOpen));
        OnPropertyChanged(nameof(IsCommandPaletteOpen));
        OnPropertyChanged(nameof(IsNotificationCenterOpen));
    }

    private void ToggleNotificationCenter()
    {
        if (!HasNotifications)
            return;

        _isNotificationCenterOpen = !_isNotificationCenterOpen;

        if (_isNotificationCenterOpen)
        {
            _isCommandPaletteOpen = false;
            _isHelpOpen = false;
        }

        OnPropertyChanged(nameof(IsNotificationCenterOpen));
        OnPropertyChanged(nameof(IsCommandPaletteOpen));
        OnPropertyChanged(nameof(IsHelpOpen));
    }

    private void CloseOverlays()
    {
        _isCommandPaletteOpen = false;
        _isHelpOpen = false;
        _isNotificationCenterOpen = false;
        OnPropertyChanged(nameof(IsCommandPaletteOpen));
        OnPropertyChanged(nameof(IsHelpOpen));
        OnPropertyChanged(nameof(IsNotificationCenterOpen));
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        RebuildNavigation();
        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(NotificationSummary));
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
        OnPropertyChanged(nameof(NotificationSummary));
        OnPropertyChanged(nameof(HasNotifications));
        ToggleNotificationCenterCommand.RaiseCanExecuteChanged();
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

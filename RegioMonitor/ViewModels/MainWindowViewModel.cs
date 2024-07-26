using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RegioMon.RegioJet;
using RegioMonitor.RegioJet.Models;

namespace RegioMon.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    const int RequestTimeout = 15000;

    [ObservableProperty]
    public DateTimeOffset _departureDate;

    [ObservableProperty]
    public long _fromId;

    [ObservableProperty]
    public long _toId;

    [ObservableProperty]
    public ObservableCollection<Trip> _trains = new();

    [ObservableProperty]
    public string _errorMessage = string.Empty;

    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly RegioJetApi _rjApi;

    private CancellationTokenSource? _cancelMonitoringTokenSource = new();

    [ObservableProperty]
    public bool _isSearchRunning = false;

    public IRelayCommand BeginMonitoringCommand { get; init; }
    public IRelayCommand CancelMonitoringCommand { get; init; }

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        RegioJetApi rjApi,
        IConfiguration conf
        )
    {
        _logger = logger;
        _rjApi = rjApi;

        BeginMonitoringCommand = new RelayCommand(StartMonitoring);
        CancelMonitoringCommand = new RelayCommand(StopMonitoring);

        DepartureDate = new DateTime(2024, 8, 7);
    }

    private async Task RequestTrainList(CancellationToken ct)
    {
        IsSearchRunning = true;
        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var resp = await _rjApi.SimpleRouteSearch(DepartureDate.ToString("yyyy-MM-dd"), 5990055004, 10202003);
                    if (resp != null)
                    {
                        _logger.LogInformation("Retrieved {Count} trains", resp.Routes.Length);
                        ErrorMessage = string.Empty;

                        Trains.Clear();
                        foreach (var trip in resp.Routes)
                        {
                            bool found = trip.DepartureTime.Date == DepartureDate.Date && trip.Bookable;
                            trip.SetIsRequestedFound(found);
                            Trains.Add(trip);

                            if (found)
                            {
                                _logger.LogInformation("Have train: {Train} with {FreeSeats} seats", trip.DepartureTime, trip.FreeSeatsCount);
                                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                                {
                                    if (desktop.MainWindow != null)
                                    {
                                        Dispatcher.UIThread.Invoke(() =>
                                        {
                                            desktop.MainWindow.Show();
                                            desktop.MainWindow.WindowState = WindowState.Normal;
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    _logger.LogError(ex, "Failed to retrieve trains: {Message}", ex.Message);
                }

                ct.WaitHandle.WaitOne(RequestTimeout);
            }
        }
        finally
        {
            IsSearchRunning = false;
            _logger.LogInformation("Monitoring task stopped");
        }
    }

    private void StopMonitoring()
    {
        _cancelMonitoringTokenSource?.Cancel();
        _logger.LogInformation("Monitoring stopped");
    }

    public void StartMonitoring()
    {
        _logger.LogInformation("Monitoring started");
        _cancelMonitoringTokenSource = new CancellationTokenSource();

        Task.Run(() => RequestTrainList(_cancelMonitoringTokenSource.Token));
    }
}

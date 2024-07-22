using System;
using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RegioMon.RegioJet;
using RegioMonitor.RegioJet.Models;

namespace RegioMon.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
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

    private CancellationTokenSource? _cancelInstallTokenSource = new();

    [ObservableProperty]
    public bool _isSearchRunning = false;

    public IRelayCommand RequestTrainListCommand { get; init; }

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        RegioJetApi rjApi,
        IConfiguration conf
        )
    {
        _logger = logger;
        _rjApi = rjApi;

        RequestTrainListCommand = new RelayCommand(RequestTrainList);
        DepartureDate = new DateTime(2024, 8, 7);
    }

    private async void RequestTrainList()
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
                    trip.SetIsRequestedFound(trip.DepartureTime.Date == DepartureDate.Date && trip.Bookable);
                    Trains.Add(trip);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.LogError(ex, "Failed to retrieve trains: {Message}", ex.Message);
        }
    }
}

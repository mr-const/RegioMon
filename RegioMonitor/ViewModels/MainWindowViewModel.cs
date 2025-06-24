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

        DepartureDate = new DateTime(2025, 7, 5);
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
                    // Kyiv - 271526028
                    // Przemysl - 5990055004
                    // Praha - 10202003
                    var resp = await _rjApi.SimpleRouteSearch(DepartureDate.ToString("yyyy-MM-dd"), 10202003, 5990055004);
                    if (resp != null)
                    {
                        _logger.LogDebug("Retrieved {Count} trains", resp.Routes.Length);
                        ErrorMessage = string.Empty;

                        Trains.Clear();
                        foreach (var trip in resp.Routes)
                        {
                            bool found = (trip.DepartureTime.Date == DepartureDate.Date
                                          || trip.DepartureTime.Date == DepartureDate.Date.AddDays(1)
                                          || trip.DepartureTime.Date == DepartureDate.Date.AddDays(2)
                                          )
                                         && trip.Bookable;
                            trip.SetIsRequestedFound(found);
                            Trains.Add(trip);

                            if (found)
                            {
                                _logger.LogInformation("Have train: {TrainId} {Departure} with {FreeSeats} seats", trip.Id, trip.DepartureTime, trip.FreeSeatsCount);

                                // retrieve details for the trip
                                RegioJetRouteDetailResponse? details = await _rjApi.GetRouteDetails(trip.Id, trip.DepartureStationId, trip.ArrivalStationId);
                                if (details != null)
                                {
                                    foreach(var section in details.Sections)
                                    {
                                        _logger.LogDebug("Section {SectionId} has {FreeSeats} free seats", section.Id, section.FreeSeatsCount);
                                        // retrieve free seats for the section
                                        var seatsResponse = await _rjApi.GetFreeSeats(section.Id, trip.DepartureStationId, trip.ArrivalStationId, "TRAIN_COUCHETTE_RELAX");
                                        if (seatsResponse != null && seatsResponse.Length > 0)
                                        {
                                            _logger.LogInformation("Section {SectionId} has {Count} vehicles", section.Id, seatsResponse[0].Vehicles);
                                            // retrieve indices of free seats for each vehicle
                                            // seatsResponse[0].Vehicles[i].Decks[0].FreeSeats[j].Index
                                            foreach (var vehicle in seatsResponse[0].Vehicles)
                                            {
                                                _logger.LogDebug("Vehicle {VehicleId} has {DecksCount} decks", vehicle.Id, vehicle.Decks.Count);
                                                foreach (var deck in vehicle.Decks)
                                                {
                                                    _logger.LogInformation("Deck {DeckName} has {FreeSeatsCount} free seats", deck.Name, deck.FreeSeats.Count);
                                                    foreach (var freeSeat in deck.FreeSeats)
                                                    {
                                                        _logger.LogInformation("Free seat index: {Index}", freeSeat.Index);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                                //{
                                //    if (desktop.MainWindow != null)
                                //    {
                                //        Dispatcher.UIThread.Invoke(() =>
                                //        {
                                //            desktop.MainWindow.Show();
                                //            desktop.MainWindow.WindowState = WindowState.Normal;
                                //        });
                                //    }
                                //}
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

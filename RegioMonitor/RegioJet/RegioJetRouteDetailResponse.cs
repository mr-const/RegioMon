using System;
using System.Collections.Generic;
using RegioMonitor.RegioJet.Models;

namespace RegioMon.RegioJet
{
    public class RegioJetRouteDetailResponse
    {
        public string Id { get; set; } = string.Empty;
        public long MainSectionId { get; set; }
        public long DepartureStationId { get; set; }
        public string DepartureStationName { get; set; } = string.Empty;
        public long DepartureCityId { get; set; }
        public string DepartureCityName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public long ArrivalStationId { get; set; }
        public string ArrivalStationName { get; set; } = string.Empty;
        public long ArrivalCityId { get; set; }
        public string ArrivalCityName { get; set; } = string.Empty;
        public DateTime ArrivalTime { get; set; }
        public int FreeSeatsCount { get; set; }
        public decimal PriceFrom { get; set; }
        public decimal PriceTo { get; set; }
        public decimal CreditPriceFrom { get; set; }
        public decimal CreditPriceTo { get; set; }
        public List<string> VehicleTypes { get; set; } = new List<string>();
        public List<PriceClass> PriceClasses { get; set; } = new List<PriceClass>();
        public object? Surcharge { get; set; } = null;
        public List<Section> Sections { get; set; } = new List<Section>();
        public bool Notices { get; set; }
        public object? TransfersInfo { get; set; } = null;
        public bool NationalTrip { get; set; }
        public bool Bookable { get; set; }
        public object? Delay { get; set; } = null;
        public string TravelTime { get; set; } = string.Empty;
        public CarbonOffset? CarbonOffset { get; set; }
    }

    public class PriceClass
    {
        public string SeatClassKey { get; set; } = string.Empty;
        public Conditions Conditions { get; set; } = new Conditions();
        public List<string> Services { get; set; } = new List<string>();
        public int FreeSeatsCount { get; set; }
        public decimal Price { get; set; }
        public decimal CreditPrice { get; set; }
        public string PriceSource { get; set; } = string.Empty;
        public List<object> CustomerNotifications { get; set; } = new List<object>();
        public ActionPrice? ActionPrice { get; set; }
        public List<string> Tariffs { get; set; } = new List<string>();
        public object? TariffNotifications { get; set; } = null;
        public bool Bookable { get; set; }
    }

    public class Conditions
    {
        public Descriptions Descriptions { get; set; } = new Descriptions();
        public object? Code { get; set; } = null;
        public bool RefundToOriginalSourcePossible { get; set; }
        public object? CancelCharge { get; set; } = null;
        public List<object> CancelCharges { get; set; } = new List<object>();
    }

    public class Descriptions
    {
        public string Cancel { get; set; } = string.Empty;
        public string CancelShort { get; set; } = string.Empty;
        public string Rebook { get; set; } = string.Empty;
        public string? RebookShort { get; set; } = null;
        public string Expiration { get; set; } = string.Empty;
    }

    public class ActionPrice
    {
        public long Id { get; set; }
        public string? Code { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool ShowIcon { get; set; }
    }

    public class Section
    {
        public long Id { get; set; }
        public string VehicleStandardKey { get; set; } = string.Empty;
        public bool Support { get; set; }
        public string SupportCode { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public bool FixedSeatReservation { get; set; }
        public Line Line { get; set; } = new Line();
        public long DepartureStationId { get; set; }
        public string DepartureStationName { get; set; } = string.Empty;
        public long DepartureCityId { get; set; }
        public string DepartureCityName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public string? DeparturePlatform { get; set; } = null;
        public long ArrivalStationId { get; set; }
        public string ArrivalStationName { get; set; } = string.Empty;
        public long ArrivalCityId { get; set; }
        public string ArrivalCityName { get; set; } = string.Empty;
        public DateTime ArrivalTime { get; set; }
        public string? ArrivalPlatform { get; set; } = null;
        public long CarrierId { get; set; }
        public int FreeSeatsCount { get; set; }
        public List<object> Notices { get; set; } = new List<object>();
        public List<object> Services { get; set; } = new List<object>();
        public object? Delay { get; set; } = null;
        public string TravelTime { get; set; } = string.Empty;
        public DateTime EstimatedArrivalTime { get; set; }
    }

    public class Line
    {
        public string LineGroupCode { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
    }

    public class CarbonOffset
    {
        public object Amount { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
    }

    public class Station
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
    }
}

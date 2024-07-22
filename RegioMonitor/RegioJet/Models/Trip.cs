using System;
using System.Collections.Generic;

namespace RegioMonitor.RegioJet.Models
{
    public class Trip
    {
        public string Id { get; set; } = string.Empty;
        public long DepartureStationId { get; set; }
        public DateTime DepartureTime { get; set; }
        public long ArrivalStationId { get; set; }
        public DateTime ArrivalTime { get; set; }
        public List<string> VehicleTypes { get; set; } = [];
        public int TransfersCount { get; set; }
        public int FreeSeatsCount { get; set; }
        public decimal PriceFrom { get; set; }
        public decimal PriceTo { get; set; }
        public decimal CreditPriceFrom { get; set; }
        public decimal CreditPriceTo { get; set; }
        public int PricesCount { get; set; }
        public object? ActionPrice { get; set; } = null; // Could be null, consider using a specific type if known
        public bool Surcharge { get; set; }
        public bool Notices { get; set; }
        public bool Support { get; set; }
        public bool NationalTrip { get; set; }
        public bool Bookable { get; set; }
        public object? Delay { get; set; } = null; // Could be null, consider using a specific type if known
        public string TravelTime { get; set; } = string.Empty;
        public List<string> VehicleStandards { get; set; } = new List<string>();

        // extended property
        public bool IsRequestedFound { get; private set; }

        public void SetIsRequestedFound(bool value)
        {
            IsRequestedFound = value;
        }
    }

    /*
     *     {
      "id": "7262757759",
      "departureStationId": 5990046024,
      "departureTime": "2024-08-07T22:10:00.000+02:00",
      "arrivalStationId": 372825000,
      "arrivalTime": "2024-08-08T06:37:00.000+02:00",
      "vehicleTypes": [
        "TRAIN"
      ],
      "transfersCount": 0,
      "freeSeatsCount": 0,
      "priceFrom": 0,
      "priceTo": 0,
      "creditPriceFrom": 0,
      "creditPriceTo": 0,
      "pricesCount": 0,
      "actionPrice": null,
      "surcharge": false,
      "notices": true,
      "support": false,
      "nationalTrip": false,
      "bookable": false,
      "delay": null,
      "travelTime": "08:27 h",
      "vehicleStandards": [
        "YELLOW"
      ]
    }
     */
}

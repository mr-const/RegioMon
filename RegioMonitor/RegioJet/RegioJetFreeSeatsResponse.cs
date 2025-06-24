using System.Collections.Generic;

namespace RegioMon.RegioJet
{
    public class RegioJetFreeSeatsResponse
    {
        public long SectionId { get; set; }
        public bool FixedSeatReservation { get; set; }
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public List<SelectedSeat> SelectedSeats { get; set; } = new List<SelectedSeat>();
    }

    public class Vehicle
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Standard { get; set; } = string.Empty;
        public int Number { get; set; }
        public List<SeatClass> SeatClasses { get; set; } = new List<SeatClass>();
        public List<object> Notifications { get; set; } = new List<object>();
        public bool CateringEnabled { get; set; }
        public List<Deck> Decks { get; set; } = new List<Deck>();
    }

    public class SeatClass
    {
        public string Name { get; set; } = string.Empty;
        public List<Service> Services { get; set; } = new List<Service>();
    }

    public class Service
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageCode { get; set; } = string.Empty;
    }

    public class Deck
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Seat> FreeSeats { get; set; } = new List<Seat>();
        public List<Seat> OccupiedSeats { get; set; } = new List<Seat>();
        public string LayoutURL { get; set; } = string.Empty;
        public string HorizontalLayoutURL { get; set; } = string.Empty;
    }

    public class Seat
    {
        public int Index { get; set; }
        public string SeatClass { get; set; } = string.Empty;
        public string? SeatConstraint { get; set; }
        public List<object> SeatNotes { get; set; } = new List<object>();
    }

    public class SelectedSeat
    {
        public long SectionId { get; set; }
        public int VehicleNumber { get; set; }
        public int VehicleDeckNumber { get; set; }
        public int SeatIndex { get; set; }
    }
}

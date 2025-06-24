using System.Collections.Generic;

namespace RegioMon.RegioJet
{
    public class RegioJetFreeSeatsRequest
    {
        public List<SectionRequest> Sections { get; set; } = new List<SectionRequest>();
        public string SeatClass { get; set; } = string.Empty;
        public List<string> Tariffs { get; set; } = new List<string>();
    }

    public class SectionRequest
    {
        public long SectionId { get; set; }
        public long FromStationId { get; set; }
        public long ToStationId { get; set; }
    }
}

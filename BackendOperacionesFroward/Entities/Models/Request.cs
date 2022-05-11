using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendOperacionesFroward.Entities.Models
{
    public class Request : Auth
    {

        [Key]
        public int? IdRequest { get; set; }

        public int NumRequest { get; set; }

        public Client? Client { get; set; }

        public Driver? Driver { get; set; }
        
        public Vehicle? Vehicle { get; set; }
        
        public string IndustrialShed { get; set; }

        public string DestinyDirection { get; set; }

        public DateTime ValidityStart { get; set; }

        public DateTime ValidityEnd { get; set; }

        public TimeSpan? ValidityStartHour { get; set; }

        public TimeSpan? ValidityEndHour { get; set; }

        public string Product { get; set; }

        public string Format { get; set; }

        public string NumAgreement { get; set; }

        public string OC { get; set; }

        public string NumAgris { get; set; }

        public string DestinyClient { get; set; }

        public bool Active { get; set; }

    }
}

using BackendOperacionesFroward.Entities.Models;
using System;
using System.Collections.Generic;

namespace BackendOperacionesFroward.Controllers.Models
{
    public class RequestData
    {
        public List<VehicleDriver>? VehicleDriver { get; set; }

        public bool? Active { get; set; }

        public DateTime? ValidityStart { get; set; }

        public DateTime? ValidityEnd { get; set; }

        public string ValidityStartHour { get; set; }

        public string ValidityEndHour { get; set; }

        public string Product { get; set; }

        public string Format { get; set; }

        public string NumAgreement { get; set; }

        public string OC { get; set; }

        public string NumAgris { get; set; }

        public int? NumRequest { get; set; }

        public string DestinyClient { get; set; }

        public string ClientName { get; set; }

        public string? DestinyDirection { get; set; }

        public string? DriverName { get; set; }

        public string? DriverLastName { get; set; }

        public string? DriverRUT { get; set; }

        public string? VehiclePatent { get; set; }

        public string IndustrialShed { get; set; }

        public List<VehicleDriver>? VehicleDriverModified { get; set; }

        public List<VehicleDriver>? VehicleDriverCreated{ get; set; }

    }
}

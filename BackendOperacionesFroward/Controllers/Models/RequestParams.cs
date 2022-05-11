using System;

namespace BackendOperacionesFroward.Controllers.Models
{
    public class RequestParams
    {
        public int? IdClient { get; set; }

        public int? IdUser { get; set; }

        public int? IdVehicle { get; set; }

        public int? IdDriver { get; set; }

        public int? IdHistory { get; set; }

        public int? IdSchedule { get; set; }

        public int? IdRequest { get; set; }

        private string _RUT;
        public string RUT
        {
            get { return _RUT; }
            set => _RUT = DataManagement.PrepareRUT(value);
        }

        public string? OT { get; set; }

        public string? Name { get; set; }

        public string? LastName { get; set; }

        public string? Giro { get; set; }

        public string? Direction { get; set; }

        public string? City { get; set; }

        public string? Telephone { get; set; }

        public string? Login { get; set; }

        public string? Email { get; set; }

        public string? DestinyDirection { get; set; }

        public string? Description { get; set; }

        public string? Permissions { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public TimeSpan? HourFrom { get; set; }

        public TimeSpan? HourTo { get; set; }

        public string? Action { get; set; }

        public string? Table { get; set; }

        public int? Object { get; set; }

        public bool? Active { get; set; }

        public int? ActiveCode { get; set; }

        public string? Patent { get; set; }

        public int? IdVehicleType { get; set; }

        public int? NumRequest { get; set; }

    }
}

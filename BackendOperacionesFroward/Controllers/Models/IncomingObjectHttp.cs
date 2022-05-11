using BackendOperacionesFroward.Entities.Models;

namespace BackendOperacionesFroward.Controllers.Models
{
    public class IncomingObjectHttp
    {
        public Client? Client { get; set; }

        public Driver? Driver { get; set; }

        public Schedule? Schedule { get; set; }

        public User? User { get; set; }

        public User[]? Users { get; set; }

        public Vehicle? Vehicle { get; set; }

        public string? HourToString {get; set;}

        public string? HourFromString { get; set; }
    }
}

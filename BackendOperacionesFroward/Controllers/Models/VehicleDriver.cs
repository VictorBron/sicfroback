using BackendOperacionesFroward.Entities.Models;

namespace BackendOperacionesFroward.Controllers.Models
{
    public class VehicleDriver
    {
        public int? IdRequest { get; set; }

        public Vehicle? Vehicle { get; set; }
        
        public Driver? Driver { get; set; }

        public bool? Active { get; set; } 
    }
}

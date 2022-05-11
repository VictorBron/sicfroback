using System.ComponentModel.DataAnnotations;

namespace BackendOperacionesFroward.Entities.Models
{

    public class VehicleType
    {
        [Key]
        public int? IdVehicleType { get; set; }

        public string Name { get; set; }

    }
}

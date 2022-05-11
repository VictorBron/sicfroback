using System.ComponentModel.DataAnnotations;

namespace BackendOperacionesFroward.Entities.Models
{

    public class Vehicle : Auth
    {

        [Key]
        public int? IdVehicle { get; set; }

        private string _Patent;
        public string? Patent
        {
            get { return _Patent; }
            set => _Patent = DataManagement.PreparePatent(value);
        }

        public VehicleType? VehicleType { get; set; }

        public string? Description { get; set; }

    }
}

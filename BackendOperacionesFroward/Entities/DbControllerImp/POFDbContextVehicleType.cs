using BackendOperacionesFroward.Entities.Models;
using System.Collections.Generic;
using System.Linq;

namespace BackendOperacionesFroward.Entities
{
    public class PofDbContextVehicleType : PofDbContext
    {

        public VehicleType CreateVehicleType(VehicleType vehicleNew, int userRequest)
        {
            if (vehicleNew.Name == null)
                return null;

            VehicleTypes.Add(vehicleNew);
            SaveChanges(userRequest);
            return vehicleNew;
        }

        public List<VehicleType> GetVehicleTypes()
            => VehicleTypes.ToList();

    }
}
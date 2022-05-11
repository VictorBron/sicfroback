using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Controllers.Utilities;
using BackendOperacionesFroward.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Controllers.Objects
{
    public class ORequest
    {
        public int IdRequest { get; set; }

        public int NumRequest { get; set; }

        public int IdClient { get; set; }

        public string ClientName { get; set; }

        public int IdDriver { get; set; }

        public string DriverName { get; set; }

        public string DriverLastName { get; set; }

        public string DriverRUT { get; set; }

        public int IdVehicle { get; set; }

        public string VehiclePatent { get; set; }

        public DateTime ValidityStart { get; set; }

        public DateTime ValidityEnd { get; set; }

        public string ValidityStartHour { get; set; }

        public string ValidityEndHour { get; set; }

        public string Product { get; set; }
        
        public string Format { get; set; }

        public string NumAgreement { get; set; }

        public string OC { get; set; }

        public string NumAgris { get; set; }

        public string DestinyClient { get; set; }

        public string DestinyDirection{ get; set; }

        public string IndustrialShed { get; set; }

        public bool Active { get; set; }

        public DateTime? Modified { get; set; }

        public string ModifiedBy { get; set; }

        public List<VehicleDriver> VehicleDriver { get; set; }

        public static List<ORequest> ParseRequest(List<Request> requests, User user, List<Request> requestWithVehicleDriver)
        {

            List<ORequest> requestsObjects = requests.Select(request => new ORequest
            {
                IdRequest = (int)request.IdRequest,
                NumRequest = request.NumRequest,
                IdClient = (int)request.Client.IdClient,
                ClientName = request.Client.Name,
                IdDriver = (int)request.Driver.IdDriver,
                DriverName = request.Driver.Name,
                DriverLastName = request.Driver.LastName,
                DriverRUT = request.Driver.RUT,
                IdVehicle = (int)request.Vehicle.IdVehicle,
                VehiclePatent = request.Vehicle.Patent,
                ValidityStart = request.ValidityStart,
                ValidityEnd = request.ValidityEnd,
                ValidityStartHour = AuxiliarMethods.GetHourString(request.ValidityStartHour.ToString()),
                ValidityEndHour = AuxiliarMethods.GetHourString(request.ValidityEndHour.ToString()),
                Product = request.Product,
                Format = request.Format,
                NumAgreement = request.NumAgreement,
                OC = request.OC,
                NumAgris = request.NumAgris,
                DestinyClient = request.DestinyClient,
                DestinyDirection = request.DestinyDirection,
                IndustrialShed = request.IndustrialShed,
                Modified = request.Modified,
                ModifiedBy = request.ModifiedBy == -1 ? "System" :user.Login,
                VehicleDriver = requestWithVehicleDriver.Select(requ => new VehicleDriver
                {
                    Driver = new Driver
                    {
                        IdDriver = requ.Driver.IdDriver,
                        Name = requ.Driver.Name,
                        LastName = requ.Driver.LastName,
                        RUT = requ.Driver.RUT,
                    },
                    Vehicle = new Vehicle
                    {
                        IdVehicle = requ.Vehicle.IdVehicle,
                        Patent = requ.Vehicle.Patent
                    },
                    Active = requ.Active
                }).ToList()
            }).ToList();

            requestsObjects.ForEach(req =>
            {
                if (req.VehicleDriver != null && req.VehicleDriver.Count > 0)
                    req.Active = !req.VehicleDriver.All(vehicleDriver => vehicleDriver.Active == false);
                else
                    req.Active = false;
            });

            return requestsObjects;
        }

    }
}

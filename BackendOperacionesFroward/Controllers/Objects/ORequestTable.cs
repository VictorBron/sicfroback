using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Controllers.Utilities;
using BackendOperacionesFroward.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Controllers.Objects
{
    public class ORequestTable
    {
        public int IdRequest { get; set; }

        public int NumRequest { get; set; }

        public string ClientName { get; set; }

        public string DriverName { get; set; }

        public string DriverRUT { get; set; }

        public string VehiclePatent { get; set; }

        public DateTime ValidityStart { get; set; }

        public DateTime ValidityEnd { get; set; }

        public string ValidityStartHour { get; set; }

        public string ValidityEndHour { get; set; }

        public bool Active { get; set; }

        public DateTime? Modified { get; set; }

        public string ModifiedBy { get; set; }


        public static ORequestTable ParseRequest(Request request, User user)
        {
            return new ORequestTable
            {
                IdRequest = (int)request.IdRequest,
                NumRequest = request.NumRequest,
                ClientName = request.Client.Name,
                DriverName = request.Driver.LastName + ", " + request.Driver.Name,
                DriverRUT = request.Driver.RUT,
                VehiclePatent = request.Vehicle.Patent,
                ValidityStart = request.ValidityStart,
                ValidityEnd = request.ValidityEnd,
                ValidityStartHour = AuxiliarMethods.GetHourString(request.ValidityStartHour.ToString()),
                ValidityEndHour = AuxiliarMethods.GetHourString(request.ValidityEndHour.ToString()),
                Active = request.Active,
                Modified = request.Modified,
                ModifiedBy = request.ModifiedBy == -1 ? "System" : user.Login,
            };
        }
    }

}
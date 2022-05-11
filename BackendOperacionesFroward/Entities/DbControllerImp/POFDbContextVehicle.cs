using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Entities.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BackendOperacionesFroward.Entities
{
    public class PofDbContextVehicle : PofDbContext
    {
        public Vehicle CreateVehicle(Vehicle vehicleNew, int userRequest)
        {
            if (vehicleNew.Patent == null)
                return null;

            if (vehicleNew.VehicleType == null)
                vehicleNew.VehicleType = VehicleTypes
                    .Where(type => type.Name == EnumObjects.GetVehicleType((int)EnumObjects.VEHICLE_TYPE.MAYOR))
                    .FirstOrDefault();

            Vehicle vehicle = GetVehicle(new RequestParams() { Patent = vehicleNew.Patent });

            if (vehicle != null)
                return vehicle;

            Attach(vehicleNew.VehicleType);
            Vehicles.Add(vehicleNew);
            SaveChanges(userRequest);
            return vehicleNew;
        }
        public int ExistVehicleWithPatent(string patent)
        {
            Vehicle tmp = Vehicles
                .Where(v =>
                    v.Patent==patent)
                .FirstOrDefault();

            return tmp != null ? (int)tmp.IdVehicle : -1;
        }

        public List<Vehicle> GetVehicles(RequestParams requestParams)
        {
            IQueryable<Vehicle> vehicle = Vehicles.Include(vehicle => vehicle.VehicleType);

            if (requestParams.IdVehicle != null)
                vehicle = vehicle.Where(WithIdVehicle((int)requestParams.IdVehicle));

            if (requestParams.Patent != null)
                vehicle = vehicle.Where(WithPatent(requestParams.Patent));

            if (requestParams.IdVehicleType != null)
                vehicle = vehicle.Where(WithType((int)requestParams.IdVehicleType));

            if (requestParams.Description != null)
                vehicle = vehicle.Where(WithDescription(requestParams.Description));

            return vehicle.ToList();
        }

        public List<Vehicle> GetVehicles() {
            return Vehicles.Include(vehicle => vehicle.VehicleType).ToList();
        }

        public Vehicle GetVehicle(RequestParams requestParams)
            => GetVehicles(requestParams).FirstOrDefault();

        public Vehicle UpdateVehicle(int idVehicle, Vehicle update, int idUser)
        {

            Vehicle vehicle = GetVehicles(new RequestParams() { IdVehicle = idVehicle }).FirstOrDefault();

            string[] peronalTratamentArr = {"VehicleType"};
            List<string> peronalTratament = peronalTratamentArr.ToList();
            peronalTratament.AddRange(invariableColumns);

            foreach (string property in vehicle.GetType().GetProperties().Select(prop => prop.Name))
                if (!peronalTratamentArr.Contains(property))
                {
                    PropertyInfo prop = typeof(Vehicle).GetProperty(property);
                    if (prop.GetValue(update) != null)
                        prop.SetValue(vehicle, prop.GetValue(update));
                }

            if (update.VehicleType != null)
                vehicle.VehicleType = VehicleTypes.Find(update.VehicleType.IdVehicleType);
            SaveChanges(idUser);

            return vehicle;
        }
        
        public bool ExistRelation(Vehicle vehicle)
            => Requests.Any(request => request.Vehicle == vehicle);
        

        public bool DeleteVehicle(Vehicle vehicle, int idUser)
        {
            Vehicles.Remove(vehicle);
            SaveChanges(idUser);
            return true;
        }

        /*
         *  FILTER METHODS
         */

        public Expression<Func<Vehicle, bool>> WithIdVehicle(int idVehicle)
            => prj => prj.IdVehicle == idVehicle;

        public Expression<Func<Vehicle, bool>> WithPatent(string patent)
            => prj => prj.Patent.Contains(patent);

        public Expression<Func<Vehicle, bool>> WithDescription(string description)
            => prj => prj.Description == description;

        public Expression<Func<Vehicle, bool>> WithType(int idType)
            => prj => prj.VehicleType.IdVehicleType == idType;
    }
}

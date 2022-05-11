using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Entities.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BackendOperacionesFroward.Entities
{
    public class PofDbContextDriver : PofDbContext
    {
        public Driver CreateDriver(Driver driverNew, int userRequest)
        {
            if (driverNew.RUT == null)
                return null;

            Driver idriver = GetDriver(new RequestParams() { RUT = driverNew.RUT });

            if (idriver != null)
                return idriver;

            Drivers.Add(driverNew);
            SaveChanges(userRequest);
            return driverNew;
        }

        public int ExistDriverWithRUT(string rut)
        {
            Driver tmp = Drivers
                .Where(d =>
                    d.RUT == rut)
                .FirstOrDefault();

            return tmp != null ? (int)tmp.IdDriver : -1;
        }

        public List<Driver> GetDrivers()
            => Drivers.ToList();
        

        public List<Driver> GetDrivers(RequestParams requestParams)
        {
            IQueryable<Driver> driver = Drivers;

            if (requestParams.IdDriver != null)
                driver = driver.Where(WithIdDriver((int)requestParams.IdDriver));

            if (requestParams.Name != null)
                driver = driver.Where(WithName(requestParams.Name));

            if (requestParams.LastName != null)
                driver = driver.Where(WithLastName(requestParams.LastName));

            if (requestParams.RUT != null)
                driver = driver.Where(WithRUT(requestParams.RUT));

            if (requestParams.Telephone != null)
                driver = driver.Where(WithTelephone(requestParams.Telephone));

            if (requestParams.Email != null)
                driver = driver.Where(WithEmail(requestParams.Email));

            return driver.ToList();
        }

        public Driver GetDriver(RequestParams requestParams)
            => GetDrivers(requestParams).FirstOrDefault();

        public Driver UpdateDriver(int idDriver, Driver update, int idUser)
        {

            Driver driver = GetDriver(new RequestParams() { IdDriver = idDriver });

            foreach (string property in driver.GetType().GetProperties().Select(prop => prop.Name))
                if (!invariableColumns.Contains(property))
                {
                    PropertyInfo prop = typeof(Driver).GetProperty(property);
                    if (prop.GetValue(update) != null)
                        prop.SetValue(driver, prop.GetValue(update));
                }

            SaveChanges(idUser);

            return driver;
        }

        public bool ExistRelation(Driver driver)
            => Requests.Any(request => request.Driver == driver);
        

        public bool DeleteDriver(Driver driver, int idUser)
        {
            Drivers.Remove(driver);
            SaveChanges(idUser);
            return true;
        }

        /*
         *  FILTER EXPRESSIONS
         */
        public Expression<Func<Driver, bool>> WithIdDriver(int idDriver)
                => prj => prj.IdDriver == idDriver;

        public Expression<Func<Driver, bool>> WithName(string name)
            => prj => prj.Name == name;

        public Expression<Func<Driver, bool>> WithLastName(string lastName)
            => prj => prj.LastName == lastName;

        public Expression<Func<Driver, bool>> WithRUT(string rUT)
            => prj => prj.RUT.Contains(rUT);

        public Expression<Func<Driver, bool>> WithTelephone(string telephone)
            => prj => prj.Telephone == telephone;

        public Expression<Func<Driver, bool>> WithEmail(string email)
            => prj => prj.Email == email;
    }
}

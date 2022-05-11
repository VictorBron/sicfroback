using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Entities.Models;
using BackendOperacionesFroward.Logger;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using static BackendOperacionesFroward.Entities.Models.EnumObjects;

namespace BackendOperacionesFroward.Entities
{
    public class PofDbContextRequest : PofDbContext
    {
        private static Semaphore _semaphore = new Semaphore(1, 1);

        public int GetNumRequest()
        {
            _semaphore.WaitOne();
            _logger.WriteLine(LOG_TYPES.INFO, $"GetNumRequest Requests: {Requests.Count()}");
            int numRequest = Requests.Any() ? Requests.Max(req => req.NumRequest) : 0;
            return numRequest + 1;
        }

        public void setNumRequestFree()
        => _semaphore.Release();


        public Request CreateRequest(Request request, int userRequest)
        {
            Requests.Add(request);
            SaveChanges(userRequest);
            return request;
        }

        public List<Request> GetRequests()
        {
            return Requests
                .Include(req => req.Driver)
                .Include(req => req.Vehicle)
                .Include(req => req.Client)
                .ToList();
        }

        public List<Request> GetRequests(RequestParams requestParams)
        {
            IQueryable<Request> request = Requests
                .Include(req => req.Driver)
                .Include(req => req.Vehicle)
                .Include(req => req.Client);

            if (requestParams.Patent != null)
                request = request.Where(WithPatent(requestParams.Patent));

            if (requestParams.RUT != null)
                request = request.Where(WithPatent(requestParams.RUT));

            if (requestParams.NumRequest != null)
                request = request.Where(WithNumRequest((int)requestParams.NumRequest));

            if (requestParams.IdClient != null)
                request = request.Where(WithIdClient((int)requestParams.IdClient));

            if (requestParams.ActiveCode != null)
                request = request.Where(WithActiveCode((int)requestParams.ActiveCode));

            if (requestParams.From != null && requestParams.To != null)
            {
                DateTime from = ((DateTime)requestParams.From);
                DateTime to = ((DateTime)requestParams.To);
                if (from == to)
                    request = request.Where(t =>
                        (t.ValidityStart == from && t.ValidityEnd == to));
                else
                    request = request.Where(t =>
                    // Have began but not finished [x, From]
                    (t.ValidityStart <= from && t.ValidityEnd >= from) ||
                    // Are going to live in the [From, To]
                    (t.ValidityStart >= from && t.ValidityEnd <= to) ||
                    // Are going to began and finish after [To, x]
                    t.ValidityStart <= to);

                if (to == DateTime.Today.AddHours(12))
                    request = request.Where(t => (t.ValidityEndHour >= DateTime.Now.TimeOfDay));
            }
            else
            {
                if (requestParams.From != null)
                    request = request.Where(WithValidityStart((DateTime)requestParams.From));

                if (requestParams.To != null)
                    request = request.Where(WithValidityEnd((DateTime)requestParams.To));
            }

            return request.OrderBy(req => req.ValidityStart).ThenBy(req => req.ValidityStartHour).ToList();
        }

        public Request GetRequest(RequestParams requestParams)
            => GetRequests(requestParams)
            .FirstOrDefault();

        public List<Request> GetVehicleDriver(int numRequest)
        {
            return Requests
                .Include(req => req.Driver)
                .Include(req => req.Vehicle)
                .Where(p => p.NumRequest == numRequest)
                .ToList();
        }

        public int GetNumRequestById(int requestId)
            => Requests.Find(requestId).NumRequest;


        public Request UpdateRequestState(int idRequest, int idUser)
        {
            Request query = Requests
            .Include(req => req.Client)
            .Include(req => req.Driver)
            .Include(req => req.Vehicle)
            .Where(req => req.IdRequest == idRequest)
            .FirstOrDefault();

            if (query == null)
                return new Request() { IdRequest = -1 };

            query.Active = !query.Active;
            SaveChanges(idUser);
            return query;
        }

        public List<Request> UpdateRequest(int idNumRequests, RequestData update, int idUser)
        {

            List<Request> requests = Requests
                .Include(req => req.Client)
                .Include(req => req.Driver)
                .Include(req => req.Vehicle)
                .Where(req => req.NumRequest == idNumRequests)
                .ToList();

            Request tmpRequestData = requests.FirstOrDefault();

            // direct string values
            IEnumerable<string> reqProperties = update.GetType().GetProperties().Select(prop => prop.Name);
            string[] peronalTratamentArr = {
                "VehicleDriverCreated",
                "VehicleDriverModified",
                "ValidityStart",
                "ValidityStartHour",
                "ValidityEnd",
                "ValidityEndHour"
            };
            List<string> peronalTratament = peronalTratamentArr.ToList();
            peronalTratament.AddRange(invariableColumns);

            PropertyInfo propGet, propSet;

            string[] timeValues;
            TimeSpan? startTime = null, endTime = null;
            if (update.ValidityStartHour != null)
            {
                timeValues = update.ValidityStartHour.Split(':');
                startTime = new TimeSpan(int.Parse(timeValues[0]), int.Parse(timeValues[1]), 0);
            }
            if (update.ValidityEndHour != null)
            {
                timeValues = update.ValidityEndHour.Split(':');
                endTime = new TimeSpan(int.Parse(timeValues[0]), int.Parse(timeValues[1]), 0);
            }

            // General objects: Not compounds as other Database objects
            foreach (Request request in requests)
            {
                foreach (string property in reqProperties)
                    if (!peronalTratament.Contains(property))
                    {
                        propGet = typeof(RequestData).GetProperty(property);
                        propSet = typeof(Request).GetProperty(property);
                        if (propGet.GetValue(update) != null)
                            propSet.SetValue(request, Caster.CastPropertyValue(propGet, propGet.GetValue(update)));
                    }
                if (update.ValidityStart != null)
                    request.ValidityStart = (DateTime)update.ValidityStart;

                if (update.ValidityEnd != null)
                    request.ValidityEnd = (DateTime)update.ValidityEnd;

                if (update.ValidityStartHour != null)
                    request.ValidityStartHour = startTime;

                if (update.ValidityEndHour != null)
                    request.ValidityEndHour = endTime;
            }

            // new/modification special treatment
            if (update.VehicleDriverCreated != null)
                update.VehicleDriverCreated.ForEach(vehicleDriver =>
                {
                    Request newRequest = new Request
                    {
                        NumRequest = idNumRequests,
                        ValidityStart = tmpRequestData.ValidityStart,
                        ValidityEnd = tmpRequestData.ValidityEnd,
                        ValidityStartHour = tmpRequestData.ValidityStartHour,
                        ValidityEndHour = tmpRequestData.ValidityEndHour,
                        Active = (bool)vehicleDriver.Active,
                        Product = tmpRequestData.Product,
                        Format = tmpRequestData.Format,
                        NumAgreement = tmpRequestData.NumAgreement,
                        OC = tmpRequestData.OC,
                        NumAgris = tmpRequestData.NumAgris,
                        DestinyClient = tmpRequestData.DestinyClient,
                        DestinyDirection = tmpRequestData.DestinyDirection,
                        IndustrialShed = tmpRequestData.IndustrialShed
                    };

                    Driver driver = Drivers.Find(vehicleDriver.Driver.IdDriver);
                    Attach(driver);
                    newRequest.Driver = driver;

                    Vehicle vehicle = Vehicles.Find(vehicleDriver.Vehicle.IdVehicle);
                    Attach(vehicle);
                    newRequest.Vehicle = vehicle;

                    Client Client = Clients.Find(tmpRequestData.Client.IdClient);
                    Attach(Client);
                    newRequest.Client = Client;

                    Requests.Add(newRequest);
                });

            if (update.VehicleDriverModified != null)
                update.VehicleDriverModified.ForEach(vehicleDriver =>
                {
                    tmpRequestData = requests.Find(request => request.IdRequest == vehicleDriver.IdRequest);
                    tmpRequestData.Active = (bool)vehicleDriver.Active;
                });

            SaveChanges(idUser);

            return Requests
                .Include(req => req.Driver)
                .Include(req => req.Vehicle)
                .Include(req => req.IndustrialShed)
                .Where(req => req.NumRequest == idNumRequests)
                .ToList();
        }

        /*
         *  FILTER METHODS
         */

        public Expression<Func<Request, bool>> WithNumRequest(int numRequest)
            => prj => prj.NumRequest == numRequest;

        public Expression<Func<Request, bool>> WithIdClient(int IdClient)
            => prj => prj.Client.IdClient == IdClient;

        public Expression<Func<Request, bool>> WithPatent(string patent)
            => prj => prj.Vehicle.Patent.Contains(patent);

        public Expression<Func<Request, bool>> WithRUT(string rut)
            => prj => prj.Driver.RUT.Contains(rut);

        public Expression<Func<Request, bool>> WithActive(bool active)
            => prj => prj.Active == active;

        public Expression<Func<Request, bool>> WithActiveCode(int activeCode)
        {
            if (activeCode == (int)REQUEST_STATE.VALID)
                return prj => prj.Active && prj.ValidityEnd >= DateTime.Today;

            if (activeCode == (int)REQUEST_STATE.DEFEATED)
                return prj => prj.Active && prj.ValidityEnd <= DateTime.Today;

            return prj => !prj.Active;
        }

        public Expression<Func<Request, bool>> WithValidityStart(DateTime ValidityStart)
            => prj => prj.ValidityStart >= ValidityStart || (prj.ValidityStart <= ValidityStart && prj.ValidityEnd >= ValidityStart);

        public Expression<Func<Request, bool>> WithValidityEnd(DateTime ValidityEnd)
            => prj => prj.ValidityEnd <= ValidityEnd;

        public Expression<Func<Request, bool>> WithFromHour(TimeSpan time)
            => prj => prj.ValidityStart.TimeOfDay >= time || (prj.ValidityStart.TimeOfDay <= time && prj.ValidityEnd.TimeOfDay >= time);

        public Expression<Func<Request, bool>> WithToHour(TimeSpan time)
            => prj => prj.ValidityEnd.TimeOfDay <= time;

    }
}

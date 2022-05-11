using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Controllers.Objects;
using BackendOperacionesFroward.Controllers.Utilities;
using BackendOperacionesFroward.Entities;
using BackendOperacionesFroward.Entities.Models;
using BackendOperacionesFroward.Logger;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : Controller
    {

        private readonly ILogger _logger;
        private readonly PofDbContextRequest _contextRequest;
        private readonly PofDbContextDriver _contextDriver;
        private readonly PofDbContextVehicle _contextVehicle;
        private readonly PofDbContextClient _contextClient;
        private readonly PofDbContextUser _contextUser;

        public RequestController(
            ILogger logger,
            PofDbContextRequest contextRequest,
            PofDbContextDriver contextDriver,
            PofDbContextVehicle contextVehicle,
            PofDbContextClient contextClient,
            PofDbContextUser contextUser
            )
        {
            _logger = logger.WithClass(MethodBase.GetCurrentMethod().DeclaringType.Name);
            _contextRequest = contextRequest;
            _contextDriver = contextDriver;
            _contextVehicle = contextVehicle;
            _contextClient = contextClient;
            _contextUser = contextUser;
        }

        [HttpGet]
        [Route("requests")]
        public async Task<ActionResult<string>> GetRequest()
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            RequestParams requestParams = AuxiliarMethods.FormatQuery(Request.Query);

            User userRequest = _contextRequest.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            if (userRequest.Permissions == EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.CLIENT))
                requestParams.IdClient = userRequest.Client.IdClient;

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.FROWARD_READER,
                (int)EnumObjects.USER_TYPES.FROWARD_SUPERVISOR,
                (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {
                var result = _contextRequest
                    .GetRequests(requestParams)
                    .Select(request => ORequestTable.ParseRequest(request, _contextUser.Users.Find(request.ModifiedBy)))
                    .ToList();
                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex.InnerException);
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpGet]
        [Route("request/{numRequest}")]
        public async Task<ActionResult<string>> GetRequest(int? numRequest)
        {

            string check;

            check = AuxiliarMethods.CheckIdParam(numRequest);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextRequest.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.FROWARD_READER,
                (int)EnumObjects.USER_TYPES.FROWARD_SUPERVISOR,
                (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {

                var request = _contextRequest
                    .GetRequests(new RequestParams { NumRequest = numRequest })
                    .FirstOrDefault();

                if (request == null)
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.ITEM_NOT_IN_DB));

                bool isFrowardTeam = AuxiliarMethods.CheckPermissions(userRequest,
                        (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                        (int)EnumObjects.USER_TYPES.FROWARD_READER,
                        (int)EnumObjects.USER_TYPES.FROWARD_SUPERVISOR) == null;

                bool isHisClientRequest = userRequest.Permissions == EnumObjects.GetUserType(
                        (int)EnumObjects.USER_TYPES.CLIENT) && request.Client.IdClient == userRequest.Client.IdClient;

                if (!isFrowardTeam &&
                    !isHisClientRequest)
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.ITEM_NOT_IN_DB));

                List<VehicleDriver> vehicleDriver = _contextRequest
                    .GetVehicleDriver((int)numRequest)
                    .Select(requ => new VehicleDriver
                    {
                        IdRequest = requ.IdRequest,
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
                    }).ToList();

                RequestData queryData = new RequestData
                {
                    NumRequest = request.NumRequest,
                    ClientName = request.Client.Name,
                    DriverName = request.Driver.Name,
                    DriverLastName = request.Driver.LastName,
                    DriverRUT = request.Driver.RUT,
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
                    VehicleDriver = vehicleDriver,
                    Active = !vehicleDriver.All(vehicleDriver => vehicleDriver.Active == false),
                };

                return AuxiliarMethods.JsonSerialicerObject(queryData);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex.InnerException);
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPost]
        [Route("request")]
        public async Task<ActionResult<string>> CreateRequest([FromBody] RequestData request)
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextDriver.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.CLIENT);
            _logger.WriteLine(LOG_TYPES.INFO, $"Check permissions: {check}");
            if (check != null)
                return check;

            try
            {
                List<int> listIds = new();

                if (request.VehicleDriver != null && request.VehicleDriver.Count > 0)
                {
                    _logger.WriteLine(LOG_TYPES.INFO, $"Vehicle driver count: {request.VehicleDriver.Count}");

                    int NumRequest = _contextRequest.GetNumRequest();
                    _logger.WriteLine(LOG_TYPES.INFO, $"NumRequest: {NumRequest}");

                    for (int i = 0; i < request.VehicleDriver.Count; i++)
                    {
                        AddRequest(request,
                            request.VehicleDriver[i],
                            (int)userRequest.IdUser,
                            (int)userRequest.Client.IdClient,
                            NumRequest);
                        listIds.Add(NumRequest);
                    }

                    _contextRequest.setNumRequestFree();
                }

                return AuxiliarMethods.JsonSerialicerObject(listIds);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex);
                _contextRequest.setNumRequestFree();
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPost]
        [Route("requests")]
        public async Task<ActionResult<string>> CreateRequests([FromBody] RequestData[] requests)
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextDriver.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;
            try
            {
                List<int> listIds = new();

                if (requests != null && requests.Length > 0)
                {
                    foreach (RequestData request in requests)
                        if (request.VehicleDriver != null && request.VehicleDriver.Count > 0)
                        {
                            int NumRequest = _contextRequest.GetNumRequest();

                            for (int i = 0; i < request.VehicleDriver.Count; i++)
                            {

                                AddRequest(request,
                                request.VehicleDriver[i],
                                (int)userRequest.IdUser,
                                (int)userRequest.Client.IdClient,
                                NumRequest);

                                listIds.Add(NumRequest);
                            }

                            _contextRequest.setNumRequestFree();
                        }
                }
                return AuxiliarMethods.JsonSerialicerObject(listIds);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex.InnerException);
                _contextRequest.setNumRequestFree();
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPut]
        [Route("request/{idRequest}")]
        public async Task<ActionResult<string>> UpdateRequest(int? idRequest)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(idRequest);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextDriver.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN, (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {
                Request after = _contextRequest.UpdateRequestState((int)idRequest, (int)userRequest.IdUser);
                if (after.IdRequest != -1)
                    return AuxiliarMethods.JsonSerialicerObject(ORequestTable.ParseRequest(after, userRequest));
                return AuxiliarMethods.JsonSerialicerObject(after);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex.InnerException);
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPut]
        [Route("requests/{numRequest}")]
        public async Task<ActionResult<string>> UpdateRequests(int? numRequest, [FromBody] RequestData requestData)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(numRequest);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextDriver.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;
            List<Request> requests = _contextRequest.UpdateRequest((int)numRequest, requestData, (int)userRequest.IdUser);

            List<ORequest> after = ORequest.ParseRequest(requests, userRequest, _contextRequest.GetVehicleDriver((int)numRequest));

            try
            {
                return AuxiliarMethods.JsonSerialicerObject(after);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex.InnerException);
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        private int AddRequest(
            RequestData req,
            VehicleDriver vehicleDriver,
            int userId,
            int clientId,
            int numRequest)
        {
            _logger.WriteLine(LOG_TYPES.INFO, $"vehicleDriver ID Driver: {vehicleDriver.Driver.IdDriver}, vehicleDriver ID Vehicle: {vehicleDriver.Vehicle.IdVehicle}, userId: {userId}, clientId: {clientId}, numRequest: {numRequest}");

            string[] timeValues;
            timeValues = req.ValidityStartHour.Split(':');
            TimeSpan startTime = new TimeSpan(int.Parse(timeValues[0]), int.Parse(timeValues[1]), 0);

            timeValues = req.ValidityEndHour.Split(':');
            TimeSpan endTime = new TimeSpan(int.Parse(timeValues[0]), int.Parse(timeValues[1]), 0);

            Request request = new()
            {
                NumRequest = numRequest,
                ValidityStart = new DateTime(((DateTime)req.ValidityStart).Year,
                        ((DateTime)req.ValidityStart).Month,
                        ((DateTime)req.ValidityStart).Day)
                    + new TimeSpan(0, 12, 0, 0, 0),
                ValidityEnd = new DateTime(((DateTime)req.ValidityEnd).Year,
                        ((DateTime)req.ValidityEnd).Month,
                        ((DateTime)req.ValidityEnd).Day)
                    + new TimeSpan(0, 12, 0, 0, 0),
                ValidityStartHour = startTime,
                ValidityEndHour = endTime,
                Active = (bool)vehicleDriver.Active,
                Product = req.Product,
                Format = req.Format,
                NumAgreement = req.NumAgreement,
                OC = req.OC,
                NumAgris = req.NumAgris,
                DestinyClient = req.DestinyClient,
                DestinyDirection = req.DestinyDirection,
                IndustrialShed = req.IndustrialShed
            };

            Driver driver = _contextDriver.Drivers.Find(vehicleDriver.Driver.IdDriver);
            _contextRequest.Attach(driver);
            request.Driver = driver;
            _logger.WriteLine(LOG_TYPES.INFO, $"Driver found: [ IdDriver: {driver.IdDriver}, RUT: {driver.RUT} Created: {driver.Created} CreatedBy: {driver.CreatedBy} ]");

            Vehicle vehicle = _contextVehicle.Vehicles.Find(vehicleDriver.Vehicle.IdVehicle);
            _contextRequest.Attach(vehicle);
            request.Vehicle = vehicle;
            _logger.WriteLine(LOG_TYPES.INFO, $"Vehicle found: [ IdVehicle: {vehicle.IdVehicle}, Patent: {vehicle.Patent} Created: {vehicle.Created} CreatedBy: {vehicle.CreatedBy} ]");

            Client Client = _contextClient.Clients.Find(clientId);
            _contextRequest.Attach(Client);
            request.Client = Client;
            _logger.WriteLine(LOG_TYPES.INFO, $"Client found: [ IdClient: {Client.IdClient}, Name: {Client.Name} RUT: {Client.RUT} CreatedBy: {Client.CreatedBy} ]");

            return (int)_contextRequest.CreateRequest(request, userId).IdRequest;
        }
    }
}
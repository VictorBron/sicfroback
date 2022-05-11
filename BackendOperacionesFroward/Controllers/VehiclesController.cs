using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Controllers.Utilities;
using BackendOperacionesFroward.Entities;
using BackendOperacionesFroward.Entities.Models;
using BackendOperacionesFroward.Logger;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : Controller
    {

        private readonly ILogger _logger;
        private readonly PofDbContextVehicle _contextVehicle;
        private readonly PofDbContextVehicleType _contextVehicleType;

        public VehicleController(ILogger logger,
            PofDbContextVehicle contextVehicle,
            PofDbContextVehicleType contextVehicleType)
        {
            _logger = logger.WithClass(MethodBase.GetCurrentMethod().DeclaringType.Name);
            _contextVehicle = contextVehicle;
            _contextVehicleType = contextVehicleType;
        }

        [HttpGet]
        [Route("vehicletypes")]
        public async Task<ActionResult<string>> GetVehicleTypes()
        {

            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextVehicle.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {
                return AuxiliarMethods.JsonSerialicerObject(_contextVehicleType.GetVehicleTypes());
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpGet]
        [Route("vehicles")]
        public async Task<ActionResult<string>> GetVehicles()
        {

            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            RequestParams requestParams = AuxiliarMethods.FormatQuery(Request.Query);

            User userRequest = _contextVehicle.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {
                return AuxiliarMethods.JsonSerialicerObject(_contextVehicle.GetVehicles(requestParams));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpGet]
        [Route("vehicle/{id}")]
        public async Task<ActionResult<string>> GetVehicle(int? id)
        {

            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextVehicle.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {
                Vehicle result = _contextVehicle.GetVehicle(new RequestParams { IdVehicle = id });
                if (result == null)
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.ITEM_NOT_IN_DB));
                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpGet]
        [Route("vehicles/patent")]
        public async Task<ActionResult<string>> GetVehiclesByPatent(string patents)
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextVehicle.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN, (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {
                string[] patentsArray = patents.Split(';');
                List<Vehicle> vehicles = new();
                for (int i = 0; i < patentsArray.Length; i++)
                    vehicles.Add(_contextVehicle.GetVehicle(new RequestParams { Patent = patentsArray[i] }));

                return AuxiliarMethods.JsonSerialicerObject(vehicles);
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPost]
        [Route("vehicle")]
        public async Task<ActionResult<string>> CreateVehicle([FromBody] IncomingObjectHttp incomingObjectHttp)
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextVehicle.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            int existPatent = _contextVehicle.ExistVehicleWithPatent(incomingObjectHttp.Vehicle.Patent);
            if (existPatent != -1)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.REPEATED_PATENT));
            }

            Vehicle result;
            try
            {
                result = _contextVehicle.CreateVehicle(incomingObjectHttp.Vehicle, (int)userRequest.IdUser);
                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception e)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPost]
        [Route("vehicles")]
        public async Task<ActionResult<string>> CreateVehicles([FromBody] Vehicle[] vehicles)
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextVehicle.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {
                List<Vehicle> result = new();

                if (vehicles != null && vehicles.Length > 0)
                    foreach (Vehicle vehicle in vehicles)
                        result.Add(_contextVehicle.CreateVehicle(vehicle, (int)userRequest.IdUser));

                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPut]
        [Route("vehicle/{id}")]
        public async Task<ActionResult<string>> UpdateVehicle(int? id, [FromBody] IncomingObjectHttp incomingObjectHttp)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextVehicle.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            int existPatent = _contextVehicle.ExistVehicleWithPatent(incomingObjectHttp.Vehicle.Patent);
            if (existPatent != -1)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.REPEATED_PATENT));
            }
            Vehicle after = _contextVehicle.UpdateVehicle((int)id, incomingObjectHttp.Vehicle, (int)userRequest.IdUser);

            try
            {
                return AuxiliarMethods.JsonSerialicerObject(after);
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpDelete]
        [Route("vehicle/{id}")]
        public async Task<ActionResult<string>> DeleteVehicle(int? id)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextVehicle.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            try
            {
                Vehicle vehicle = _contextVehicle.GetVehicle(new RequestParams { IdVehicle = id });

                if (vehicle == null)
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));

                if (_contextVehicle.ExistRelation(vehicle))
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.KEY_ALREADY_USED));

                return AuxiliarMethods.JsonSerialicerObject(_contextVehicle.DeleteVehicle(vehicle, (int)userRequest.IdUser));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }
    }
}

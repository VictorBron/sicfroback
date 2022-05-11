using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Controllers.Utilities;
using BackendOperacionesFroward.Entities;
using BackendOperacionesFroward.Entities.Models;
using BackendOperacionesFroward.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : Controller
    {

        private readonly ILogger _logger;
        private readonly PofDbContextDriver _contextDriver;

        public DriverController(ILogger logger, PofDbContextDriver contextDriver)
        {
            _logger = logger.WithClass(MethodBase.GetCurrentMethod().DeclaringType.Name);
            _contextDriver = contextDriver;
        }

        [HttpGet]
        [Route("drivers")]
        public async Task<ActionResult<string>> GetDrivers()
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            RequestParams requestParams = AuxiliarMethods.FormatQuery(Request.Query);

            User userRequest = _contextDriver.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {
                return AuxiliarMethods.JsonSerialicerObject(_contextDriver.GetDrivers(requestParams));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpGet]
        [Route("driver/{id}")]
        public async Task<ActionResult<string>> GetDriver(int? id)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextDriver.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            try
            {
                Driver result = _contextDriver.GetDriver(new RequestParams { IdDriver = id });
                if (result == null)
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.ITEM_NOT_IN_DB));
                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex.InnerException);
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpGet]
        [Route("drivers/RUT")]
        public async Task<ActionResult<string>> GetDriverByRut(string ruts)
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextDriver.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN, (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {
                string[] rutsArray = ruts.Split(';');
                List<Driver> drivers = new();
                for (int i = 0; i < rutsArray.Length; i++)
                    drivers.Add(_contextDriver.GetDriver(new RequestParams { RUT = rutsArray[i] }));

                return AuxiliarMethods.JsonSerialicerObject(drivers);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex.InnerException);
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPost]
        [Route("driver")]
        public async Task<ActionResult<string>> CreateDriver([FromBody] IncomingObjectHttp incomingObjectHttp)
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextDriver.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;
            int existRut = _contextDriver.ExistDriverWithRUT(incomingObjectHttp.Driver.RUT);
            if (existRut != -1)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.REPEATED_RUT));
            }
            Driver result;
            try
            {
                result = _contextDriver.CreateDriver(incomingObjectHttp.Driver, (int)userRequest.IdUser);
                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex.InnerException);
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPost]
        [Route("drivers")]
        public async Task<ActionResult<string>> CreateDrivers([FromBody] Driver[] drivers)
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextDriver.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {
                List<Driver> result = new();
                if (drivers != null && drivers.Length > 0)
                    foreach (Driver driver in drivers)
                        result.Add(_contextDriver.CreateDriver(driver, (int)userRequest.IdUser));

                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex.InnerException);
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPut]
        [Route("driver/{id}")]
        public async Task<ActionResult<string>> UpdateDriver(int? id, [FromBody] IncomingObjectHttp incomingObjectHttp)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextDriver.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;
            int existRut = _contextDriver.ExistDriverWithRUT(incomingObjectHttp.Driver.RUT);
            if (existRut != -1)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.REPEATED_RUT));
            }
            try
            {
                return AuxiliarMethods.JsonSerialicerObject(_contextDriver.UpdateDriver((int)id, incomingObjectHttp.Driver, (int)userRequest.IdUser));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpDelete]
        [Route("driver/{id}")]
        public async Task<ActionResult<string>> DeleteDriver(int? id)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextDriver.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            try
            {
                Driver driver = _contextDriver.GetDriver(new RequestParams { IdDriver = id });

                if (driver == null)
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));

                if (_contextDriver.ExistRelation(driver))
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.KEY_ALREADY_USED));

                return AuxiliarMethods.JsonSerialicerObject(_contextDriver.DeleteDriver(driver, (int)userRequest.IdUser));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

    }
}

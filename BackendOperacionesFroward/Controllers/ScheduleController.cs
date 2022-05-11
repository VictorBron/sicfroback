using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Controllers.Utilities;
using BackendOperacionesFroward.Entities;
using BackendOperacionesFroward.Entities.Models;
using BackendOperacionesFroward.Logger;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : Controller
    {

        private readonly ILogger _logger;
        private readonly PofDbContextSchedule _contextSchedule;

        public ScheduleController(ILogger logger, PofDbContextSchedule contextSchedule)
        {
            _logger = logger.WithClass(MethodBase.GetCurrentMethod().DeclaringType.Name);
            _contextSchedule = contextSchedule;
        }

        [HttpGet]
        [Route("schedules")]
        public async Task<ActionResult<string>> GetSchedules()
        {

            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            RequestParams requestParams = AuxiliarMethods.FormatQuery(Request.Query);

            User userRequest = _contextSchedule.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            if (userRequest.Permissions == EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.CLIENT))
                requestParams.IdClient = userRequest.Client.IdClient;

            check = AuxiliarMethods.CheckPermissions(
                userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.FROWARD_READER,
                (int)EnumObjects.USER_TYPES.FROWARD_SUPERVISOR,
                (int)EnumObjects.USER_TYPES.CLIENT);
            if (check != null)
                return check;

            try
            {
                return AuxiliarMethods.JsonSerialicerObject(_contextSchedule.GetSchedules(requestParams));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpGet]
        [Route("schedule/{id}")]
        public async Task<ActionResult<string>> GetSchedule(int? id)
        {

            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextSchedule.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(
                userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_SUPERVISOR);
            if (check != null)
                return check;

            try
            {
                Schedule result = _contextSchedule.GetSchedule(new RequestParams { IdSchedule = id });
                if (result == null)
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.ITEM_NOT_IN_DB));
                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPost]
        [Route("schedules")]
        public async Task<ActionResult<string>> PostSchedule([FromBody] IncomingObjectHttp incomingObjectHttp)
        {
            // just for the permissions
            string check;
            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextSchedule.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions( userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_SUPERVISOR);
            if (check != null)
                return check;

            Schedule result;
            try
            {
         
                result = _contextSchedule.CreateSchedule(incomingObjectHttp.Schedule, incomingObjectHttp.HourFromString, incomingObjectHttp.HourToString, (int)userRequest.IdUser);

                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPut]
        [Route("schedule/{id}")]
        public async Task<ActionResult<string>> UpdateSchedule(int? id, [FromBody] IncomingObjectHttp incomingObjectHttp)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextSchedule.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_SUPERVISOR);
            if (check != null)
                return check;

            Schedule after;

            try
            {
               after = _contextSchedule.UpdateSchedule((int)id, incomingObjectHttp.Schedule, incomingObjectHttp.HourFromString, incomingObjectHttp.HourToString, (int)userRequest.IdUser);
                return AuxiliarMethods.JsonSerialicerObject(after);
            }
            catch (Exception e)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }
    }
}

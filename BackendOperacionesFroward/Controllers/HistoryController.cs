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
    public class HistoryController : Controller
    {

        private readonly ILogger _logger;
        private readonly PofDbContextHistory _contextHistory;

        public HistoryController(ILogger logger, PofDbContextHistory contextHistory)
        {
            _logger = logger.WithClass(MethodBase.GetCurrentMethod().DeclaringType.Name);
            _contextHistory = contextHistory;
        }

        [HttpGet]
        [Route("histories")]
        public async Task<ActionResult<string>> GetHistory()
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextHistory.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            try
            {
                return AuxiliarMethods.JsonSerialicerObject(_contextHistory.GetHistories());
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpGet]
        [Route("histories/{id}")]
        public async Task<ActionResult<string>> GetHistory(int? id)
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextHistory.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            try
            {
                return AuxiliarMethods.JsonSerialicerObject(_contextHistory.GetHistory(new RequestParams { IdHistory = id }));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }

        }

        [HttpGet]
        [Route("histories")]
        public async Task<ActionResult<string>> GetHistories()
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            RequestParams requestParams = AuxiliarMethods.FormatQuery(Request.Query);

            User userRequest = _contextHistory.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;
            try
            {
                return AuxiliarMethods.JsonSerialicerObject(_contextHistory.GetHistories(requestParams));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }
    }
}

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
    public class ClientController : Controller
    {

        private readonly ILogger _logger;
        private readonly PofDbContextClient _contextClient;

        public ClientController(ILogger logger, PofDbContextClient contextClient)
        {
            _logger = logger.WithClass(MethodBase.GetCurrentMethod().DeclaringType.Name);
            _contextClient = contextClient;
        }

        [HttpGet]
        [Route("clients")]
        public async Task<ActionResult<string>> GetClient()
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            RequestParams requestParams = AuxiliarMethods.FormatQuery(Request.Query);

            User userRequest = _contextClient.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";
            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN,
                (int)EnumObjects.USER_TYPES.FROWARD_SUPERVISOR,
                (int)EnumObjects.USER_TYPES.FROWARD_READER);
            if (check != null)
                return check;
            List<Client> result;
            try
            {
                result = _contextClient.GetClients(requestParams);
                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpGet]
        [Route("client/{id}")]
        public async Task<ActionResult<string>> GetClients(int? id)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextClient.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            Client result;
            try
            {
                result = _contextClient.GetClient(new RequestParams { IdClient = id });
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
        [Route("clients")]
        public async Task<ActionResult<string>> CreateClient([FromBody] IncomingObjectHttp incomingObjectHttp)
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextClient.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest,
                (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;
            int existRut = _contextClient.ExistClientWithRUT(incomingObjectHttp.Client.RUT);
            if (existRut != -1)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.REPEATED_RUT));
            }
            Client result;
            try
            {
                result = _contextClient.CreateClient(incomingObjectHttp.Client, (int)userRequest.IdUser);
                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpPut]
        [Route("client/{id}")]
        public async Task<ActionResult<string>> UpdateClient(int? id, [FromBody] IncomingObjectHttp incomingObjectHttp)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextClient.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;
            int existRut = _contextClient.ExistClientWithRUT(incomingObjectHttp.Client.RUT);
            if (existRut != -1)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.REPEATED_RUT));
            }
            Client result;
            try
            {
                result = _contextClient.UpdateClient((int)id, incomingObjectHttp.Client, (int)userRequest.IdUser);
                return AuxiliarMethods.JsonSerialicerObject(result);
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
            return AuxiliarMethods.JsonSerialicerObject(result);
        }

        [HttpDelete]
        [Route("client/{id}")]
        public async Task<ActionResult<string>> DeleteClient(int? id)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextClient.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            try
            {
                Client client = _contextClient.GetClient(new RequestParams { IdClient = id });

                if (client == null)
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));

                if (_contextClient.ExistRelation(client))
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.KEY_ALREADY_USED));

                return AuxiliarMethods.JsonSerialicerObject(_contextClient.DeleteClient(client, (int)userRequest.IdUser));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }
    }
}
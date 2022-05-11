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
    public class UserController : Controller
    {

        private readonly ILogger _logger;
        private readonly PofDbContextUser _contextUser;
        private readonly PofDbContextRequest _contextRequest;

        public UserController(
            ILogger logger,
            PofDbContextUser contextUser,
            PofDbContextRequest contextRequest
           )
        {
            _logger = logger.WithClass(MethodBase.GetCurrentMethod().DeclaringType.Name);
            _contextUser = contextUser;
            _contextRequest = contextRequest;
        }


        [HttpGet]
        [Route("users")]
        public async Task<ActionResult<string>> GetUsers()
        {

            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextRequest.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            try
            {
                return AuxiliarMethods.JsonSerialicerObject(_contextUser.GetUsers());
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpGet]
        [Route("user")]
        public async Task<ActionResult<string>> GetUser()
        {

            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            RequestParams requestParams = AuxiliarMethods.FormatQuery(Request.Query);

            User userRequest = _contextRequest.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            try
            {
                return AuxiliarMethods.JsonSerialicerObject(_contextUser.GetUsers(requestParams));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }

        }

        [HttpGet]
        [Route("user/{id}")]
        public async Task<ActionResult<string>> GetUser(int? id)
        {

            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextRequest.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            try
            {
                User result = _contextUser.GetUser(new RequestParams { IdUser = id });
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
        [Route("users")]
        public async Task<ActionResult<string>> CreateUsers([FromBody] IncomingObjectHttp incomingObjectHttp)
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextRequest.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            List<int> listIds = new();
            try
            {
                if (incomingObjectHttp.Users != null)
                    foreach (User req in incomingObjectHttp.Users)
                    {
                        if (_contextUser.ExistUserWithSameRUT(req.RUT) || _contextUser.ExistUserWithSameLogin(req.Login))
                            listIds.Add((int)_contextUser.GetUser(new RequestParams { RUT = req.RUT }).IdUser);
                        else
                            listIds.Add(AddUser(req, (int)userRequest.IdUser));
                    }
                else
                {
                    if (_contextUser.ExistUserWithSameLogin(incomingObjectHttp.User.Login))
                        return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.REPEATED_LOGIN));

                    if (_contextUser.ExistUserWithSameRUT(incomingObjectHttp.User.RUT))
                       return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.REPEATED_RUT));

                    listIds.Add(AddUser(incomingObjectHttp.User, (int)userRequest.IdUser));
                }

                return AuxiliarMethods.JsonSerialicerObject(listIds);
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }

        }

        [HttpPut]
        [Route("user/{id}")]
        public async Task<ActionResult<string>> UpdateUser(int? id, [FromBody] IncomingObjectHttp incomingObjectHttp)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextRequest.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            string resultPermissions = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (resultPermissions != null)
                return resultPermissions;

            try
            {
                if (_contextUser.ExistUserWithSameLogin(incomingObjectHttp.User.Login))
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.REPEATED_LOGIN));

                if (_contextUser.ExistUserWithSameRUT(incomingObjectHttp.User.RUT))
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.REPEATED_RUT));

                return AuxiliarMethods.JsonSerialicerObject(_contextUser.UpdateUser((int)id, incomingObjectHttp.User, (int)userRequest.IdUser));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        [HttpDelete]
        [Route("user/{id}")]
        public async Task<ActionResult<string>> DeleteUser(int? id)
        {
            string check;

            check = AuxiliarMethods.CheckIdParam(id);
            if (check != null)
                return check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextUser.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return "";

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            try
            {
                User user = _contextUser.GetUser(new RequestParams { IdUser = id });

                if (user == null)
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));

                if (_contextUser.ExistRelation(user))
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.KEY_ALREADY_USED));

                return AuxiliarMethods.JsonSerialicerObject(_contextUser.DeleteUser(user, (int)userRequest.IdUser));
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        private int AddUser(User req, int userCreator)
        {
            return (int)_contextUser.CreateUserAsync(req, userCreator).IdUser;
        }
    }
}

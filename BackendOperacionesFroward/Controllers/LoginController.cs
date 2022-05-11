using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Controllers.Objects;
using BackendOperacionesFroward.Controllers.Utilities;
using BackendOperacionesFroward.Entities;
using BackendOperacionesFroward.Entities.Models;
using BackendOperacionesFroward.Logger;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {

        private readonly ILogger _logger;
        private readonly PofDbContextLogin _contextLogin;
        private readonly PofDbContextUser _contextUser;

        public LoginController(ILogger logger, PofDbContextLogin contextLogin, PofDbContextUser contextUser)
        {
            _logger = logger.WithClass(MethodBase.GetCurrentMethod().DeclaringType.Name);
            _contextLogin = contextLogin;
            _contextUser = contextUser;
        }

        [HttpGet]
        [Route("Auth")]
        public async Task<ActionResult<string>> GetToken()
        {

            string check;

            check = AuxiliarMethods.CheckAuthentication(Request, Constants.Authentication);
            if (check != null)
                return check;

            UserAuth userAuth;
            try
            {
                userAuth = AuxiliarMethods.CheckAuthenticationFormat(Request.Headers[Constants.Authentication]);
            }
            catch (FormatException)
            {
                return new ErrorHttpResponseException(StringError.AUTH_PASS_ERROR).ToString();
            }

            try
            {
                User result = _contextUser.ExistUser(userAuth.GetLogin(), userAuth.GetPassword());
                if (result != null && !(bool)result.Active)
                    return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.ACC_BLOQUED));
                return ReturnToken(result);
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }


        [HttpGet]
        [Route("tokens")]
        public async Task<ActionResult<string>> GetTokens()
        {

            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextLogin.UserFromTokenCode(Request.Headers[Constants.TokenCode]);

            check = AuxiliarMethods.CheckPermissions(userRequest, (int)EnumObjects.USER_TYPES.FROWARD_ADMIN);
            if (check != null)
                return check;

            try
            {
                return AuxiliarMethods.JsonSerialicerObject(_contextLogin.GetTokens());
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        private ActionResult<string> ReturnToken(User user)
        {
            if (user != null)
            {
                int userId = (int)user.IdUser;
                Token activeToken = _contextLogin.UserHasActiveToken(userId);
                if (activeToken != null)
                    return AuxiliarMethods.JsonSerialicerObject(ConvertTokenToOToken(activeToken, userId));

                Token tok = null;
                if (userId > -1)
                        tok = _contextLogin.CreateToken(userId);

                if (tok != null)
                    return AuxiliarMethods.JsonSerialicerObject(ConvertTokenToOToken(tok, userId));
            }

            try
            {
                return new ErrorHttpResponseException(StringError.AUTH_DEFAULT_TOKEN_ERROR).ToString();
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

        private OToken ConvertTokenToOToken(Token token, int idUser)
        {
            User user = _contextUser.Users.Find(idUser);
            return OToken.ParseToken(token, user); 
        }
    }
}

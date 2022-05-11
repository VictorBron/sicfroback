using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Controllers.Objects;
using BackendOperacionesFroward.Controllers.Utilities;
using BackendOperacionesFroward.Entities;
using BackendOperacionesFroward.Entities.Models;
using BackendOperacionesFroward.Logger;
using BackendOperacionesFroward.Shared.Messages;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecoverController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly PofDbContextLogin _contextLogin;
        private readonly PofDbContextUser _contextUser;

        public RecoverController(ILogger logger, PofDbContextLogin contextLogin, PofDbContextUser contextUser)
        {
            _logger = logger.WithClass(MethodBase.GetCurrentMethod().DeclaringType.Name);
            _contextLogin = contextLogin;
            _contextUser = contextUser;
        }

        [HttpGet]
        [Route("token")]
        public async Task<ActionResult<string>> GetToken()
        {
            string email = Request.Query[Constants.Email];
            if (email == null)
                return "";

            try
            {
                User user = _contextUser.GetUsers(new RequestParams { Email = email }).FirstOrDefault();

                if (user != null)
                {

                    Token token = _contextLogin.CreateToken((int)user.IdUser);
                    Sender.SendMessageRecoverPassword(user, token);
                }
            }
            catch (Exception)
            {
                // Return always true
            }
            return AuxiliarMethods.JsonSerialicerObject(true);
        }

        [HttpGet]
        [Route("validate")]
        public async Task<ActionResult<string>> GetChangePass()
        {
            string check;

            check = AuxiliarMethods.CheckTokenHeader(Request, Constants.TokenCode);
            if (check != null)
                return check;

            User userRequest = _contextLogin.UserFromTokenCode(Request.Headers[Constants.TokenCode]);
            if (userRequest == null)
                return new ErrorHttpResponseException(StringError.AUTH_PASS_ERROR).ToString();

            UserAuth userAuth;
            try
            {
                userAuth = AuxiliarMethods.CheckAuthenticationFormat(Request.Headers[Constants.Authentication]);
                return AuxiliarMethods.JsonSerialicerObject(_contextUser.UpdatePass(userAuth.GetEmail(), userAuth.GetPassword()));
            }
            catch (FormatException)
            {
                return new ErrorHttpResponseException(StringError.AUTH_PASS_ERROR).ToString();
            }

        }
    }
}

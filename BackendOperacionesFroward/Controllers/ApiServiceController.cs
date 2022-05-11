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
    [Route("[controller]")]
    [ApiController]
    public class ApiServiceController : Controller
    {

        private readonly ILogger _logger;
        private readonly PofDbContextRequest _contextRequest;
        private readonly PofDbContextUser _contextUser;

        public ApiServiceController(ILogger logger,
            PofDbContextRequest contextRequest,
            PofDbContextUser contextUser)
        {
            _logger = logger.WithClass(MethodBase.GetCurrentMethod().DeclaringType.Name);
            _contextRequest = contextRequest;
            _contextUser = contextUser;
        }

        [HttpGet]
        [Route("requests")]
        public async Task<ActionResult<string>> GetRequests()
        {
            string check;

            check = AuxiliarMethods.CheckUserPassHeader(Request, Constants.Username, Constants.Password, Constants.IdRequest);
            if (check != null)
                return check;

            UserAuth userAuth;
            try
            {
                userAuth = AuxiliarMethods.CheckAuthenticationFormat(Request.Headers[Constants.Username], Request.Headers[Constants.Password]);
            }
            catch (FormatException)
            {
                return new ErrorHttpResponseException(StringError.AUTH_PASS_ERROR).ToString();
            }

            User user = _contextRequest.UserFromUserPass(userAuth.GetLogin(), userAuth.GetPassword());
            if (user == null)
                return "";

            RequestParams requestParams = new RequestParams
            {
                ActiveCode = 1,
                IdRequest = int.Parse(Request.Headers[Constants.IdRequest])
            };

            try
            {
                var result = _contextRequest
                    .GetRequests(requestParams)
                    .Where(request => request.IdRequest > requestParams.IdRequest)
                    .Select(request => ORequestTable.ParseRequest(request, _contextUser.Users.Find(request.ModifiedBy)))
                    .ToList();
                return AuxiliarMethods.JsonSerialicerObject(new
                {
                    count = result.Count,
                    data = result
                });
            }
            catch (Exception)
            {
                return AuxiliarMethods.JsonSerialicerObject(new ErrorHttpResponseException(StringError.BAD_QUERY));
            }
        }

    }
}
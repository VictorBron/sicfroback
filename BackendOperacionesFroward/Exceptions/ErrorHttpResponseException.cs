using BackendOperacionesFroward.Controllers.Utilities;
using System;
using System.Net;

namespace BackendOperacionesFroward.Controllers.Models
{
    public enum StringError
    {
        HTTP_NUMBER_PARAMS,
        HTTP_NUMBER_HEADERS,
        AUTH_PERMISSION,
        AUTH_TOKEN_NOT_FOUND,
        AUTH_PASS_ERROR,
        AUTH_DEFAULT_TOKEN_ERROR,
        HTTP_PARAM_NOT_IMPLEMENTED,
        HTTP_PARAM_ID_EMPTY,
        BASE64_ERROR,
        BAD_QUERY,
        REPEATED_RUT,
        REPEATED_PATENT,
        KEY_ALREADY_USED,
        REPEATED_LOGIN,
        ITEM_NOT_IN_DB,
        ACC_BLOQUED
    }

    public class ErrorHttpResponseException : Exception
    {
        public string Error { get; set; }

        public int Code { get; set; }

        public ErrorHttpResponseException(StringError messageCode, string personalMSG = "")
        {
            Error = GetString(messageCode, personalMSG);
        }

        public ErrorHttpResponseException(String error, int code)
        {
            Error = error;
            Code = code;
        }

        public override string ToString()
            => AuxiliarMethods.JsonSerialicerObject(this);

        public string GetString(StringError value, string personalMSG = "")
        {
            switch (value)
            {
                case StringError.HTTP_NUMBER_PARAMS:
                    Code = (int)HttpStatusCode.BadRequest;
                    return "NUMBER OF HTTP PARAMS IS INCORRECT";

                case StringError.HTTP_NUMBER_HEADERS:
                    Code = (int)HttpStatusCode.BadRequest;
                    return "NUMBER OF HTTP HEADERS IS INCORRECT";

                case StringError.AUTH_PERMISSION:
                    Code = (int)HttpStatusCode.Forbidden;
                    return "NOT ENOUGHT PERMISSIONS";

                case StringError.AUTH_TOKEN_NOT_FOUND:
                    Code = (int)HttpStatusCode.BadRequest;
                    return "TOKEN NOT FOUND";

                case StringError.AUTH_PASS_ERROR:
                    Code = (int)HttpStatusCode.Unauthorized;
                    return "PASSWORD ERROR";

                case StringError.AUTH_DEFAULT_TOKEN_ERROR:
                    Code = (int)HttpStatusCode.Unauthorized;
                    return "ERROR GETTING TOKEN";

                case StringError.HTTP_PARAM_NOT_IMPLEMENTED:
                    Code = (int)HttpStatusCode.BadRequest;
                    return $"PARAM {personalMSG} SHOULD BE IMPLEMENTED AND HAVE A CORRECT VALUE";

                case StringError.HTTP_PARAM_ID_EMPTY:
                    Code = (int)HttpStatusCode.BadRequest;
                    return "GET CONTROLLER/ACTION/{Id} HAVE AN EMPTY ID";

                case StringError.BASE64_ERROR:
                    Code = (int)HttpStatusCode.BadRequest;
                    return "ERROR IN BASE64 ENCODE";

                case StringError.BAD_QUERY:
                    Code = (int)HttpStatusCode.BadRequest;
                    return "BAD_QUERY";

                case StringError.REPEATED_RUT:
                    Code = (int)HttpStatusCode.BadRequest;
                    return "REPEATED RUT";

                case StringError.REPEATED_PATENT:
                    Code = (int)HttpStatusCode.BadRequest;
                    return "REPEATED PATENT";

                case StringError.KEY_ALREADY_USED:
                    Code = (int)HttpStatusCode.BadRequest;
                    return "KEY ALREADY USED";

                case StringError.REPEATED_LOGIN:
                    Code = (int)HttpStatusCode.BadRequest;
                    return "REPEATED LOGIN";

                case StringError.ITEM_NOT_IN_DB:
                    Code = (int)HttpStatusCode.BadRequest;
                    return "ITEM NOT IN DB";

                case StringError.ACC_BLOQUED:
                    Code = (int)HttpStatusCode.Unauthorized;
                    return "ACC BLOQUED";
                    
                default:
                    return "NO VALUE GIVEN";
            }
        }
    }
}

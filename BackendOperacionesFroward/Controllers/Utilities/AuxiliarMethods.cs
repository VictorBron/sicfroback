using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Entities.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace BackendOperacionesFroward.Controllers.Utilities
{
    public static class AuxiliarMethods
    {
        public static void CheckHTTPParamsForm(IFormCollection request, params string[] lambda)
        {
            foreach (string param in lambda)
                if ((string)request[param] == null || (string)request[param] == "")
                    throw new ErrorHttpResponseException(StringError.HTTP_PARAM_NOT_IMPLEMENTED, param);

        }

        public static void CheckHTTPParamsHeader(IHeaderDictionary request, params string[] lambda)
        {
            foreach (string param in lambda)
                if ((string)request[param] == null || (string)request[param] == "" || (string)request[param] == "null")
                    throw new ErrorHttpResponseException(StringError.HTTP_PARAM_NOT_IMPLEMENTED, param);
        }

        public static UserAuth CheckAuthenticationFormat(string auth)
        {

            UserAuth userAuth = new();

            try
            {
                userAuth
                    .WithLogin(AuxiliarMethodsSecurity.DecodeFrom64(auth).Split('&')[0])
                    .WithPass(AuxiliarMethodsSecurity.GetHash256(AuxiliarMethodsSecurity.DecodeFrom64(auth).Split('&')[1]))
                    .WithEmail(AuxiliarMethodsSecurity.DecodeFrom64(auth).Split('&')[0]);
            }
            catch (FormatException)
            {
                new ErrorHttpResponseException(StringError.HTTP_PARAM_NOT_IMPLEMENTED, Constants.Authentication).ToString();
            }

            return userAuth;
        }

        public static UserAuth CheckAuthenticationFormat(string username, string password)
        {

            UserAuth userAuth = new();

            try
            {
                userAuth
                    .WithLogin(username)
                    .WithPass(AuxiliarMethodsSecurity.GetHash256(password));
            }
            catch (FormatException)
            {
                new ErrorHttpResponseException(StringError.HTTP_PARAM_NOT_IMPLEMENTED, Constants.Authentication).ToString();
            }

            return userAuth;
        }

        public static string CheckTokenForm(HttpRequest request, params string[] lambda)
        {

            try
            {
                if (request == null || request.Form.Count == 0)
                    return new ErrorHttpResponseException(StringError.HTTP_NUMBER_PARAMS).ToString();

                CheckHTTPParamsForm(request.Form, lambda);
            }
            catch (InvalidOperationException)
            {
                return new ErrorHttpResponseException(StringError.HTTP_NUMBER_PARAMS).ToString();
            }

            return null;
        }

        public static string CheckTokenHeader(HttpRequest request, params string[] lambda)
        {

            try
            {
                if (request == null || request.Headers.Count == 0)
                    return new ErrorHttpResponseException(StringError.HTTP_NUMBER_HEADERS).ToString();

                CheckHTTPParamsHeader(request.Headers, lambda);
            }
            catch (ErrorHttpResponseException exception)
            {
                return exception.ToString();
            }
            catch (InvalidOperationException)
            {
                return new ErrorHttpResponseException(StringError.HTTP_NUMBER_HEADERS).ToString();
            }

            return null;
        }

        public static string CheckUserPassHeader(HttpRequest request, params string[] lambda)
        {

            try
            {
                if (request == null || request.Headers.Count == 0)
                    return new ErrorHttpResponseException(StringError.HTTP_NUMBER_HEADERS).ToString();

                CheckHTTPParamsHeader(request.Headers, lambda);
            }
            catch (ErrorHttpResponseException exception)
            {
                return exception.ToString();
            }
            catch (InvalidOperationException)
            {
                return new ErrorHttpResponseException(StringError.HTTP_NUMBER_HEADERS).ToString();
            }

            return null;
        }

        public static string CheckPermissions(User userRequest, params int[] lambda)
        {
            if (userRequest == null)
                return new ErrorHttpResponseException(StringError.AUTH_TOKEN_NOT_FOUND).ToString();


            if (!lambda.Any(perm => perm == (int)Enum.Parse(typeof(EnumObjects.USER_TYPES), userRequest.Permissions)))
                return new ErrorHttpResponseException(StringError.AUTH_PERMISSION).ToString();

            return null;
        }

        public static string CheckAuthentication(HttpRequest request, params string[] lambda)
        {

            try
            {
                if (request == null || request.Headers.Count == 0)
                    throw new ErrorHttpResponseException(StringError.AUTH_PERMISSION);

                CheckHTTPParamsHeader(request.Headers, lambda);
                CheckAuthenticationFormat(request.Headers[Constants.Authentication]);
            }
            catch (ErrorHttpResponseException exception)
            {
                return exception.ToString();
            }
            catch (InvalidOperationException)
            {
                return new ErrorHttpResponseException(StringError.HTTP_NUMBER_PARAMS).ToString();
            }

            return null;
        }

        public static string JsonSerialicerObject(object obj)
        {
            if (obj is ErrorHttpResponseException exception)
                return JsonSerializer.Serialize(new ErrorHttpResponseException(exception.Error, exception.Code));

            return JsonSerializer.Serialize(obj);

        }

        public static string CheckIdParam(int? id)
        {
            if (id == null)
                return new ErrorHttpResponseException(StringError.HTTP_PARAM_ID_EMPTY).ToString();
            return null;
        }

        public static RequestParams FormatQuery(IQueryCollection query)
        {
            RequestParams requestParams = new RequestParams();
            var keysRequestParams = requestParams.GetType().GetProperties().Select(prop => prop.Name).ToList();
            ICollection<string> QueryKeys = query.Keys;
            foreach (var key in QueryKeys.Where(key => keysRequestParams.Contains(key)))
            {
                PropertyInfo prop = requestParams.GetType().GetProperty(key);
                prop.SetValue(requestParams, Caster.CastPropertyValue(prop, query[key]));
            }

            if (requestParams.From != null) requestParams.From = ((DateTime)requestParams.From).AddHours(12);
            if (requestParams.To != null) requestParams.To = ((DateTime)requestParams.To).AddHours(12);

            return requestParams;
        }

        public static string GetHourString(string oldHour)
        {
            string[] parsed = oldHour.Split(':');

            return $"{parsed[0]}:{parsed[1]}";
        }
    }
}

using BackendOperacionesFroward.Entities.Models;
using System;

namespace BackendOperacionesFroward.Controllers.Objects
{
    public class OToken
    {
        public DateTime? RequestedAt { get; set; }

        public int? TimeExpiration { get; set; }

        public string TokenCode { get; set; }

        public string PermissionLevel { get; set; }

        public string LoginName { get; set; }

        public static OToken ParseToken(Token token, User user)
        {
            return new OToken
            {
                TimeExpiration = (int)((DateTime)token.ExpiresAt - (DateTime)token.RequestedAt).TotalSeconds,
                RequestedAt = token.RequestedAt,
                TokenCode = token.TokenCode,
                PermissionLevel = user.Permissions,
                LoginName = user.Login
            };
        }
    }
}

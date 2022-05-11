using BackendOperacionesFroward.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BackendOperacionesFroward.Entities
{
    public class PofDbContextLogin : PofDbContext
    {
        public Token UserHasActiveToken(int userId)
        {

            Token tmp = Tokens
                .Where(t => t.IdUser == userId)
                .FirstOrDefault();

            if (!(tmp != null && tmp.TokenCode != null && tmp.ExpiresAt > DateTime.Now))
                return null;

            UpdateToken(DateTime.Now, userId);
            return tmp;
        }

        public Token CreateToken(int userId)
        {
            return base.CreateToken(userId: userId);
        }

        public List<Token> GetTokens()
            => Tokens.ToList();

    }
}

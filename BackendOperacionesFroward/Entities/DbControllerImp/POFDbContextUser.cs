using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Entities
{
    public class PofDbContextUser : PofDbContext
    {
        public User CreateUserAsync(User user, int userCreator)
        {
            if (user.Client != null)
                Attach(user.Client);
            string pass = "pfrwrd"+user.Login+DateTime.Now.Millisecond;
            user.Password = AuxiliarMethodsSecurity.GetHash256(pass);

            Users.Add(user);
            SaveChanges(userCreator);

            Token token = CreateToken(userId: (int)user.IdUser, userCreator: userCreator);
            _ = Shared.Messages.Sender.SendMessageRegistration(user, token);

            return user;
        }

        public User ExistUser(string userLog, string password)
        {
            User tmp = Users
                .Where(u =>
                    u.Login == userLog &&
                    u.Password == password)
                .FirstOrDefault();

            return tmp;
        }

        public bool ExistUserWithSameLogin(string login)
            => Users.Any(user => user.Login == login);
        
        public bool ExistUserWithSameRUT(string rut)
            => Users.Any(u => u.RUT == rut && u.Active == true);

        public List<User> GetUsers()
            => Users.Include(user => user.Client).ToList();
        

        public List<User> GetUsers(RequestParams requestParams)
        {
            IQueryable<User> user = Users.Include(user => user.Client);

            if (requestParams.IdUser != null)
                user = user.Where(WithIdUser((int)requestParams.IdUser));

            if (requestParams.IdClient != null)
                user = user.Where(WithIdClient((int)requestParams.IdClient));

            if (requestParams.Permissions != null)
                user = user.Where(WithPermissions(requestParams.Permissions));

            if (requestParams.Login != null)
                user = user.Where(WithLogin(requestParams.Login));

            if (requestParams.Name != null)
                user = user.Where(WithName(requestParams.Name));

            if (requestParams.LastName != null)
                user = user.Where(WithLastName(requestParams.LastName));

            if (requestParams.Telephone != null)
                user = user.Where(WithTelephone(requestParams.Telephone));

            if (requestParams.Email != null)
                user = user.Where(WithEmail(requestParams.Email));

            if (requestParams.From != null)
                user = user.Where(WithLastEntryFrom((DateTime)requestParams.From));

            if (requestParams.To != null)
                user = user.Where(WithLastEntryTo((DateTime)requestParams.To));

            if (requestParams.Active != null)
                user = user.Where(WithActiveStatus((bool)requestParams.Active));

            return user.ToList();
        }

        public User GetUser(RequestParams requestParams)
            => GetUsers(requestParams).FirstOrDefault();

        public User UpdateUser(int idUser, User update, int idUserModify)
        {

            User user = GetUser(new RequestParams() { IdUser = idUser });
            if (update.Client != null)
                Attach(update.Client);
            
            foreach (string property in user.GetType().GetProperties().Select(prop => prop.Name))
                if (!invariableColumns.Contains(property))
                {
                    PropertyInfo prop = typeof(User).GetProperty(property);
                    if (prop.GetValue(update) != null)
                        prop.SetValue(user, prop.GetValue(update));
                }

            SaveChanges(idUserModify);
            return user;
        }

        public bool UpdatePass(string email, string pass)
        {
            User user = Users.Where(user => user.Email == email).FirstOrDefault();
            if (user != null)
                user.Password = pass;
            SaveChanges();
            return true;
        }

        public bool ExistRelation(User user)
            => Histories.Any(history => history.IdUser == user.IdUser);
        
        public bool DeleteUser(User user, int idUser)
        {
            Users.Remove(user);
            SaveChanges(idUser);
            return true;
        }

        /*
         *  FILTER EXPRESSIONS
         */

        public Expression<Func<User, bool>> WithIdUser(int idUser)
            => prj => prj.IdUser == idUser;

        public Expression<Func<User, bool>> WithIdClient(int IdClient)
            => prj => prj.Client.IdClient == IdClient;

        public Expression<Func<User, bool>> WithPermissions(string permissions)
            => prj => prj.Permissions == permissions;

        public Expression<Func<User, bool>> WithLogin(string login)
            => prj => prj.Login == login;

        public Expression<Func<User, bool>> WithName(string name)
            => prj => prj.Name == name;

        public Expression<Func<User, bool>> WithLastName(string lastName)
            => prj => prj.LastName == lastName;

        public Expression<Func<User, bool>> WithTelephone(string telephone)
            => prj => prj.Telephone == telephone;

        public Expression<Func<User, bool>> WithEmail(string email)
            => prj => prj.Email.ToUpper() == email.ToUpper();

        public Expression<Func<User, bool>> WithLastEntryFrom(DateTime dateFrom)
            => prj => prj.LastEntry >= dateFrom;

        public Expression<Func<User, bool>> WithLastEntryTo(DateTime dateTo)
            => prj => prj.LastEntry >= dateTo;

        public Expression<Func<User, bool>> WithActiveStatus(bool state)
            => prj => prj.Active == state;
    }
}

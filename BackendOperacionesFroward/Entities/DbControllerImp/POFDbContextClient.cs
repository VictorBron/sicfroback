using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BackendOperacionesFroward.Entities
{
    public class PofDbContextClient : PofDbContext
    {
        public Client CreateClient(Client clientNew, int userRequest)
        {

            if (clientNew.RUT == null)
                return null;

            Client iclient = GetClient(new RequestParams() { RUT = clientNew.RUT });

            if (iclient != null)
                return null;

            Clients.Add(clientNew);
            SaveChanges(userRequest);
            return clientNew;
        }
        public int ExistClientWithRUT(string rut)
        {
            Client tmp = Clients
                .Where(c =>
                    c.RUT == rut)
                .FirstOrDefault();

            return tmp != null ? (int)tmp.IdClient : -1;
        }

        public List<Client> GetClients()
            => Clients.ToList();


        public List<Client> GetClients(RequestParams requestParams)
        {
            IQueryable<Client> client = Clients;

            if (requestParams.IdClient != null)
                client = client.Where(WithIdClient((int)requestParams.IdClient));

            if (requestParams.RUT != null)
                client = client.Where(WithRUT(requestParams.RUT));

            if (requestParams.OT != null)
                client = client.Where(WithOT(requestParams.OT));

            if (requestParams.Name != null)
                client = client.Where(WithName(requestParams.Name));

            if (requestParams.Giro != null)
                client = client.Where(WithGiro(requestParams.Giro));

            if (requestParams.Direction != null)
                client = client.Where(WithDirection(requestParams.Direction));

            if (requestParams.City != null)
                client = client.Where(WithCity(requestParams.City));

            if (requestParams.Telephone != null)
                client = client.Where(WithTelephone(requestParams.Telephone));

            if (requestParams.Email != null)
                client = client.Where(WithEmail(requestParams.Email));

            return client.ToList();
        }

        public Client GetClient(RequestParams requestParams)
            => GetClients(requestParams).FirstOrDefault();

        public Client GetClient(int idClient)
            => GetClient(new RequestParams() { IdClient = idClient });

        public Client UpdateClient(int idClient, Client update, int idUser)
        {

            Client client = GetClient(new RequestParams() { IdClient = idClient });

            foreach (string property in client.GetType().GetProperties().Select(prop => prop.Name))
                if (!invariableColumns.Contains(property))
                {
                    PropertyInfo prop = typeof(Client).GetProperty(property);
                    if (prop.GetValue(update) != null)
                        prop.SetValue(client, prop.GetValue(update));
                }

            SaveChanges(idUser);

            return client;
        }

        public bool ExistRelation(Client client)
            => Users.Any(user => user.Client == client) || Requests.Any(request => request.Client == client);
        

        public bool DeleteClient(Client client, int idUser)
        {
            Clients.Remove(client);
            SaveChanges(idUser);
            return true;
        }

        /*
         *  FILTER EXPRESSIONS
         */

        public Expression<Func<Client, bool>> WithIdClient(int idClient)
            => prj => prj.IdClient == idClient;

        public Expression<Func<Client, bool>> WithRUT(string rUT)
            => prj => prj.RUT == rUT;

        public Expression<Func<Client, bool>> WithOT(string oT)
            => prj => prj.OT == oT;

        public Expression<Func<Client, bool>> WithName(string name)
            => prj => prj.Name == name;

        public Expression<Func<Client, bool>> WithGiro(string Giro)
            => prj => prj.Giro == Giro;

        public Expression<Func<Client, bool>> WithDirection(string direction)
            => prj => prj.Direction == direction;

        public Expression<Func<Client, bool>> WithCity(string city)
            => prj => prj.City == city;

        public Expression<Func<Client, bool>> WithTelephone(string telephone)
            => prj => prj.Telephone == telephone;

        public Expression<Func<Client, bool>> WithEmail(string email)
            => prj => prj.Email == email;

    }
}

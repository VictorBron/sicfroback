using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BackendOperacionesFroward.Entities
{
    public class PofDbContextHistory : PofDbContext
    {
        public List<History> GetHistories()
        {
            return Histories.ToList();
        }

        public List<History> GetHistories(RequestParams requestParams)
        {
            IQueryable<History> history = Histories;

            if (requestParams.IdHistory != null)
                history = history.Where(WithIdHistory((int)requestParams.IdHistory));

            if (requestParams.IdUser != null)
                history = history.Where(WithIdUser((int)requestParams.IdUser));

            if (requestParams.From != null)
                history = history.Where(WithDateFrom((DateTime)requestParams.From));

            if (requestParams.To != null)
                history = history.Where(WithDateTo((DateTime)requestParams.To));

            if (requestParams.Action != null)
                history = history.Where(WithAction(requestParams.Action));

            if (requestParams.Table != null)
                history = history.Where(WithTable(requestParams.Table));

            if (requestParams.Object != null)
                history = history.Where(WithObject((int)requestParams.Object));

            return history.ToList();
        }

        public History GetHistory(RequestParams requestParams)
            => GetHistories(requestParams).FirstOrDefault();

        /*
         *  FILTER METHODS
         */

        public Expression<Func<History, bool>> WithIdHistory(int idHistory)
            => prj => prj.IdHistory == idHistory;

        public Expression<Func<History, bool>> WithIdUser(int idUser)
            => prj => prj.IdUser == idUser;

        public Expression<Func<History, bool>> WithDateFrom(DateTime dateFrom)
            => prj => prj.Date >= dateFrom;

        public Expression<Func<History, bool>> WithDateTo(DateTime dateTo)
            => prj => prj.Date >= dateTo;

        public Expression<Func<History, bool>> WithAction(string action)
            => prj => prj.Action == action;

        public Expression<Func<History, bool>> WithTable(string table)
            => prj => prj.Table == table;

        public Expression<Func<History, bool>> WithObject(int objectId)
            => prj => prj.Object == objectId;
    }
}

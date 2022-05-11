using BackendOperacionesFroward.Controllers.Models;
using BackendOperacionesFroward.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BackendOperacionesFroward.Entities
{
    public class PofDbContextSchedule : PofDbContext
    {


        public Schedule CreateSchedule(Schedule schedule, string hourFromString, string hourToString, int userRequest)
        {
            if (hourFromString != null)
                schedule.HourFrom = TimeSpan.Parse(hourFromString);

            if (hourToString != null)
                schedule.HourTo = TimeSpan.Parse(hourToString);

            Schedule ischedule = Schedules.Where(tmp =>
                    tmp.DayFrom == schedule.DayFrom &&
                    tmp.DayTo == schedule.DayTo &&
                    tmp.HourFrom == schedule.HourFrom &&
                    tmp.HourTo == schedule.HourTo
                    )
                .FirstOrDefault();

            if (ischedule != null)
                return ischedule;
            if (schedule.Client != null)
                Attach(schedule.Client);

            Schedules.Add(schedule);
            SaveChanges(userRequest);
            return schedule;
        }

        public List<Schedule> GetSchedules()
        {
            return Schedules.Include(schedule => schedule.Client).ToList();
        }

        public List<Schedule> GetSchedules(RequestParams requestParams)
        {
            IQueryable<Schedule> schedule = Schedules.Include(schedule => schedule.Client);

            if (requestParams.IdSchedule != null)
                schedule = schedule.Where(WithIdSchedule((int)requestParams.IdSchedule));

            if (requestParams.IdClient != null)
                schedule = schedule.Where(WithIdClient((int)requestParams.IdClient));

            schedule = schedule.Where(WithDateFrom(DateTime.Today));

            if (requestParams.To != null)
                schedule = schedule.Where(WithDateTo((DateTime)requestParams.To));

            if (requestParams.HourFrom != null)
                schedule = schedule.Where(WithHourFrom((TimeSpan)requestParams.HourFrom));

            if (requestParams.HourTo != null)
                schedule = schedule.Where(WithHourTo((TimeSpan)requestParams.HourTo));

            return schedule.ToList();
        }

        public Schedule GetSchedule(RequestParams requestParams)
            => GetSchedules(requestParams).FirstOrDefault();

        public Schedule UpdateSchedule(int idSchedule, Schedule update, string hourFromString, string hourToString, int idUser)
        {
            if (hourFromString != null)
                update.HourFrom = TimeSpan.Parse(hourFromString);

            if (hourToString != null)
                update.HourTo = TimeSpan.Parse(hourToString);

            Schedule schedule = GetSchedule(new RequestParams() { IdSchedule = idSchedule });
            if (update.Client != null)
                Attach(update.Client);


            foreach (string property in schedule.GetType().GetProperties().Select(prop => prop.Name))
                if (!invariableColumns.Contains(property))
                {
                    PropertyInfo prop = typeof(Schedule).GetProperty(property);
                    if (prop.GetValue(update) != null)
                        prop.SetValue(schedule, prop.GetValue(update));
                }

            SaveChanges(idUser);

            return schedule;
        }

        /*
         *  FILTER METHODS
         */

        public Expression<Func<Schedule, bool>> WithIdSchedule(int idSchedule)
            => prj => prj.IdSchedule == idSchedule;

        public Expression<Func<Schedule, bool>> WithIdClient(int idClient)
            => prj => prj.Client.IdClient == idClient;

        public Expression<Func<Schedule, bool>> WithDateFrom(DateTime dateFrom)
           => prj => prj.DayFrom >= dateFrom || (prj.DayFrom <= dateFrom && prj.DayTo >= dateFrom);

        public Expression<Func<Schedule, bool>> WithDateTo(DateTime dateTo)
            => prj => prj.DayTo >= dateTo;

        public Expression<Func<Schedule, bool>> WithHourFrom(TimeSpan dateFrom)
            => prj => prj.HourFrom >= dateFrom;

        public Expression<Func<Schedule, bool>> WithHourTo(TimeSpan dateTo)
            => prj => prj.HourTo >= dateTo;
    }
}

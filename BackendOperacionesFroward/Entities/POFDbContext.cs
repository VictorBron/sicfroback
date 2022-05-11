using BackendOperacionesFroward.Entities.Models;
using BackendOperacionesFroward.Logger;
using BackendOperacionesFroward.Settings.Objects;
using BackendOperacionesFroward.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BackendOperacionesFroward.Entities
{
    public partial class PofDbContext : DbContext
    {
        protected readonly List<string> invariableColumns = new Auth().GetType().GetProperties().Select(a => a.Name).ToList();

        public const int DEFAULT_TIME_EXPIRE_MIN = 50;
        public static int TIME_EXPIRE_CLIENT { get; set; }
        public static int TIME_EXPIRE_FROWARD_ADMIN { get; set; }
        public static int TIME_EXPIRE_FROWARD_READER { get; set; }
        public static int TIME_EXPIRE_FROWARD_SUPERVISOR { get; set; }

        /*******************************************/
        /********** Tables of the database  ********/
        /*******************************************/
        public DbSet<Client> Clients { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbContextOptionsBuilder<PofDbContext> DbContextOptionsBuilder { get; }

        protected readonly ILogger _logger = LoggerServiceConsole.CreateInstanceLoger();
        /*******************************************/
        /******** Constructors of context  *********/
        /*******************************************/

        // By default
        public PofDbContext()
        {
        }

        // By context options for the tests
        public PofDbContext(DbContextOptions<PofDbContext> options)
           : base(options)
        {
            SetIConfiguration();
        }


        // By context options builder to create in desing time (startup centralBackendOperacionesFroward)
        public PofDbContext(
            DbContextOptionsBuilder<PofDbContext> dbContextOptionsBuilder)
            => DbContextOptionsBuilder = dbContextOptionsBuilder;


        /*******************************************/
        /******** Basic data construction  *********/
        /*******************************************/

        public void InitialiceDB()
        {
            Clients = TestInitClass.Add(Clients);
            SaveChanges();

            Drivers = TestInitClass.Add(Drivers);
            SaveChanges();

            Users = TestInitClass.Add(Users, Clients);
            SaveChanges();

            Tokens = TestInitClass.Add(Tokens, Users, DEFAULT_TIME_EXPIRE_MIN);
            SaveChanges();

            VehicleTypes = TestInitClass.Add(VehicleTypes);
            SaveChanges();

            Vehicles = TestInitClass.Add(Vehicles, VehicleTypes);
            SaveChanges();

            Requests = TestInitClass.Add(Requests, Drivers, Clients, Vehicles);
            SaveChanges();

            Schedules = TestInitClass.Add(Schedules, Clients);
            SaveChanges();

        }

        // Migrates by console
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = AppSettings.GetAppSettings();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Db_Connection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            /*
                 * Table atributes
            */

            // Client properties
            modelBuilder.Entity<Client>(
                model => model.HasKey(client => client.IdClient));
            modelBuilder.Entity<Client>(
                model => model.Property(client => client.RUT).IsRequired());
            modelBuilder.Entity<Client>(
                model => model.Property(client => client.OT).IsRequired());
            modelBuilder.Entity<Client>(
                model => model.Property(client => client.Name).IsRequired());
            modelBuilder.Entity<Client>(
                model => model.Property(client => client.Giro).IsRequired());
            modelBuilder.Entity<Client>(
                model => model.Property(client => client.Direction).IsRequired());
            modelBuilder.Entity<Client>(
                model => model.Property(client => client.City).IsRequired());
            modelBuilder.Entity<Client>(
                model => model.Property(client => client.Telephone).IsRequired());
            modelBuilder.Entity<Client>(
                model => model.Property(client => client.Email).IsRequired());
            modelBuilder.Entity<Client>(
                model => model.ToTable("Client"));

            // Driver properties
            modelBuilder.Entity<Driver>(
                model => model.HasKey(driver => driver.IdDriver));
            modelBuilder.Entity<Driver>(
                model => model.Property(driver => driver.Name).IsRequired());
            modelBuilder.Entity<Driver>(
                model => model.Property(driver => driver.LastName).IsRequired());
            modelBuilder.Entity<Driver>(
                model => model.Property(driver => driver.RUT).IsRequired());
            modelBuilder.Entity<Driver>()
                .HasIndex(u => u.RUT).IsUnique();
            modelBuilder.Entity<Driver>(
                model => model.Property(driver => driver.Telephone).IsRequired());
            modelBuilder.Entity<Driver>(
                model => model.Property(driver => driver.Email));
            modelBuilder.Entity<Driver>(
                model => model.ToTable("Driver"));

            // History properties
            modelBuilder.Entity<History>(
                model => model.HasKey(history => history.IdHistory));
            modelBuilder.Entity<History>(
                model => model.Property(history => history.Date).IsRequired());
            modelBuilder.Entity<History>(
                model => model.Property(history => history.IdUser).IsRequired());
            modelBuilder.Entity<History>(
                model => model.Property(history => history.Action).IsRequired());
            modelBuilder.Entity<History>(
                model => model.Property(history => history.Table).IsRequired());
            modelBuilder.Entity<History>(
                model => model.Property(history => history.Object).IsRequired());
            modelBuilder.Entity<History>(
                model => model.ToTable("History"));

            // Request properties
            modelBuilder.Entity<Request>(
               model => model.HasKey(request => request.IdRequest));
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.NumRequest).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.Active).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.ValidityStart).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.ValidityEnd).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.ValidityStartHour).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.ValidityEndHour).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.Product).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.Format).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.NumAgreement).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.OC).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.NumAgris).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.DestinyClient).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.DestinyDirection).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.Property(request => request.IndustrialShed).IsRequired());
            modelBuilder.Entity<Request>(
                model => model.ToTable("Request"));

            // Schedule properties
            modelBuilder.Entity<Schedule>(
               model => model.HasKey(schedule => schedule.IdSchedule));
            modelBuilder.Entity<Schedule>(
                model => model.Property(schedule => schedule.DayFrom).IsRequired());
            modelBuilder.Entity<Schedule>(
                model => model.Property(schedule => schedule.DayTo).IsRequired());
            modelBuilder.Entity<Schedule>(
                model => model.Property(schedule => schedule.HourFrom).IsRequired());
            modelBuilder.Entity<Schedule>(
                model => model.Property(schedule => schedule.HourTo).IsRequired());
            modelBuilder.Entity<Schedule>(
              model => model.Property(schedule => schedule.Comment));
            modelBuilder.Entity<Schedule>(
                model => model.ToTable("Schedule"));

            // Token properties
            modelBuilder.Entity<Token>(
                model => model.HasKey(token => token.IdToken));
            modelBuilder.Entity<Token>(
                model => model.Property(token => token.IdUser).IsRequired());
            modelBuilder.Entity<Token>(
                model => model.Property(token => token.RequestedAt).IsRequired());
            modelBuilder.Entity<Token>(
                model => model.Property(token => token.ExpiresAt).IsRequired());
            modelBuilder.Entity<Token>(
                model => model.Property(token => token.TokenCode).IsRequired());
            modelBuilder.Entity<Token>(
                model => model.ToTable("Token"));

            // User properties
            modelBuilder.Entity<User>(
                model => model.HasKey(user => user.IdUser));
            modelBuilder.Entity<User>(
                model => model.Property(user => user.Permissions).IsRequired());
            modelBuilder.Entity<User>(
                model => model.Property(user => user.Login).IsRequired());
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login).IsUnique();
            modelBuilder.Entity<User>(
                model => model.Property(user => user.Password).IsRequired());
            modelBuilder.Entity<User>(
                model => model.Property(user => user.Name).IsRequired());
            modelBuilder.Entity<User>(
                model => model.Property(user => user.LastName).IsRequired());
            modelBuilder.Entity<User>(
              model => model.Property(user => user.RUT).IsRequired());
            modelBuilder.Entity<User>(
                model => model.Property(user => user.Telephone).IsRequired());
            modelBuilder.Entity<User>(
                model => model.Property(user => user.Email));
            modelBuilder.Entity<User>(
                model => model.Property(user => user.LastEntry).IsRequired());
            modelBuilder.Entity<User>(
                model => model.Property(user => user.Active).IsRequired());
            modelBuilder.Entity<User>(
                model => model.ToTable("User"));

            // Vehicletypes properties
            modelBuilder.Entity<VehicleType>(
                model => model.HasKey(vehicleType => vehicleType.IdVehicleType));
            modelBuilder.Entity<VehicleType>(
                model => model.Property(vehicleType => vehicleType.Name).IsRequired());

            // Vehicles properties
            modelBuilder.Entity<Vehicle>(
                model => model.HasKey(vehicle => vehicle.IdVehicle));
            modelBuilder.Entity<Vehicle>(
                model => model.Property(vehicle => vehicle.Patent).IsRequired());
            modelBuilder.Entity<Vehicle>()
                .HasIndex(u => u.Patent).IsUnique();
            modelBuilder.Entity<Vehicle>(
                model => model.Property(vehicle => vehicle.Description));
            modelBuilder.Entity<Vehicle>(
                model => model.ToTable("Vehicle"));

        }

        public void SetUpUserConfiguration()
        {
            ExpirationTimesConfiguration ExpirationTimes = AppSettings.GetConfigurationOptions().ExpirationTimes;

            TIME_EXPIRE_CLIENT = ExpirationTimes.CLIENT;
            TIME_EXPIRE_FROWARD_ADMIN = ExpirationTimes.FROWARD_ADMIN;
            TIME_EXPIRE_FROWARD_READER = ExpirationTimes.FROWARD_READER;
            TIME_EXPIRE_FROWARD_SUPERVISOR = ExpirationTimes.FROWARD_SUPERVISOR;

        }
        public int SaveChanges(int userId)
        {
            _logger.WriteLine(LOG_TYPES.INFO, $"Save Changes userId: {userId}");
            List<History> histories = new();
            List<EntityEntry> modifiedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .Where(e => e.Entity is not History)
                .ToList();

            // Appears in correct order in the db
            modifiedEntries.Reverse();
            _logger.WriteLine(LOG_TYPES.INFO, $"modifiedEntries: {modifiedEntries.Count}");

            foreach (var ent in modifiedEntries)
                foreach (History x in GetHistoryRecord(ent, userId))
                    histories.Add(x);

            UpdateUserTokenActivity(userId);

            int changes = base.SaveChanges();
            _logger.WriteLine(LOG_TYPES.INFO, $"changes: {changes}");

            // Record in History table
            string keyName;
            for (int i = 0; i < modifiedEntries.Count; i++)
            {
                keyName = modifiedEntries[i]
                    .Entity
                    .GetType()
                    .GetProperties()
                    .Single(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), false).Length > 0)
                    .Name;

                histories[i].Object = (int)modifiedEntries[i]
                    .CurrentValues
                    .GetValue<int?>(keyName);

                Update(histories[i]);
                SaveChanges();
            }

            return changes;
        }

        public override int SaveChanges()
        {

            List<EntityEntry> modifiedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Where(e => e.Entity is not History)
                .ToList();

            DateTime date = DateTime.Now;

            for (int i = 0; i < modifiedEntries.Count; i++)
            {
                if (modifiedEntries[i].Entity is Auth auth && modifiedEntries[i].State == EntityState.Added)
                {
                    if (auth.Created == null)
                        auth.Created = date;

                    if (auth.CreatedBy == null)
                        auth.CreatedBy = -1;

                    if (auth.Modified == null)
                        auth.Modified = date;

                    if (auth.ModifiedBy == null)
                        auth.ModifiedBy = -1;
                }

            }

            return base.SaveChanges();
        }

        public void UpdateUserTokenActivity(int userId)
        {
            DateTime dateNow = DateTime.Now;
            UpdateUser(dateNow, userId);
            UpdateToken(dateNow, userId);
        }

        public void UpdateUser(DateTime dateNow, int userId)
        {
            User user = Users.Where(user => user.IdUser == userId).FirstOrDefault();
            user.LastEntry = dateNow;
            SaveChanges();
        }

        public void UpdateToken(DateTime dateNow, int userId)
        {
            User user = Users.Find(userId);
            Token token = Tokens.Where(token => token.IdUser == userId).FirstOrDefault();
            token.RequestedAt = dateNow;
            token.ExpiresAt = dateNow.AddMinutes(GetAdditiveSessionTime(user));
            SaveChanges();
        }

        public int GetAdditiveSessionTime(User user)
        {
            if (user.Permissions == EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.CLIENT))
                return TIME_EXPIRE_CLIENT;
            else if (user.Permissions == EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.FROWARD_ADMIN))
                return TIME_EXPIRE_FROWARD_ADMIN;
            else if (user.Permissions == EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.FROWARD_READER))
                return TIME_EXPIRE_FROWARD_READER;
            else if (user.Permissions == EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.FROWARD_SUPERVISOR))
                return TIME_EXPIRE_FROWARD_SUPERVISOR;
            else
                return DEFAULT_TIME_EXPIRE_MIN;
        }

        private void SetIConfiguration() =>
            AppSettings.GetConfigurationSection();

        private static List<History> GetHistoryRecord(EntityEntry dbEntry, int IdUser)
        {
            List<History> result = new();

            DateTime changeTime = DateTime.UtcNow;

            string tableName = dbEntry
                .Entity
                .GetType()
                .GetCustomAttributes(typeof(TableAttribute), false)
                .SingleOrDefault() is TableAttribute tableAttr ? tableAttr.Name : dbEntry.Entity.GetType().Name;

            if (dbEntry.Entity is Auth auth)
            {
                if (dbEntry.State == EntityState.Added)
                {
                    auth.Created = changeTime;
                    auth.CreatedBy = IdUser;
                }
                auth.Modified = changeTime;
                auth.ModifiedBy = IdUser;
            }

            result.Add(new History()
            {
                IdUser = IdUser,
                Date = changeTime,
                Action = dbEntry.State.ToString(),
                Table = tableName,
            });

            return result;
        }

        public User UserFromTokenCode(string TokenCode)
        {

            Token token = Tokens
                .Where(tok =>
                    tok.TokenCode == TokenCode &&
                    tok.ExpiresAt > DateTime.Now)
                .FirstOrDefault();

            User tmp = null;

            if (token != null)
            {
                DateTime datetimeNow = DateTime.Now;
                tmp = Users.Where(p => p.IdUser == token.IdUser).Include(tmp => tmp.Client).FirstOrDefault();
                UpdateToken(datetimeNow, (int)token.IdUser);
                UpdateUserTokenActivity((int)token.IdUser);
                return tmp;
            }

            return tmp;
        }

        public User UserFromUserPass(string login, string pass)
        {
            return Users.Where(user => user.Login == login && user.Password == pass).FirstOrDefault();
        }

        public Token CreateToken(int userId, int userCreator = -1)
        {

            DateTime request = DateTime.Now;
            DateTime expires = DateTime.Now.AddMinutes(GetAdditiveSessionTime(Users.Find(userId)));
            string hash = AuxiliarMethodsSecurity.GetHash256(expires.ToString() + userId);

            Token actualToken = Tokens
                .Where(t => t.IdUser == userId)
                .FirstOrDefault();

            if (actualToken == null)
            {
                actualToken = new()
                {
                    IdUser = userId,
                    RequestedAt = request,
                    ExpiresAt = expires,
                    TokenCode = hash
                };
                Tokens.Add(actualToken);
            }
            else
            {
                actualToken.RequestedAt = request;
                actualToken.ExpiresAt = expires;
                actualToken.TokenCode = hash;
            }

            SaveChanges(userCreator != -1 ? userCreator : userId);
            return actualToken;
        }
    }
}
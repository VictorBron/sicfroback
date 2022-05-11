using System;
using Microsoft.EntityFrameworkCore;
namespace BackendOperacionesFroward.Entities.Models
{
    public static class TestInitClass
    {
        public static DbSet<Client> Add(DbSet<Client> clients)
        {
            clients.Add(new Client
            {
                RUT = "1422376-2",
                OT = "Froward",
                Name = "Froward",
                Giro = "12",
                Direction = "Direccion 1",
                City = "Ciudad 1",
                Telephone = "312786598",
                Email = "em@cli1"
            });
            clients.Add(new Client
            {
                RUT = "1422376-2",
                OT = "Cliente 1",
                Name = "client1",
                Giro = "3345",
                Direction = "Direccion 2",
                City = "Ciudad 2",
                Telephone = "78897987",
                Email = "em@cli2"
            });
            clients.Add(new Client
            {
                RUT = "12755616-4",
                OT = "Cliente 2",
                Name = "client2",
                Giro = "78",
                Direction = "Dirección 3",
                City = "Ciudad 3",
                Telephone = "312786598",
                Email = "em@cli3"
            });
            clients.Add(new Client
            {
                RUT = "12312312-3",
                OT = "Cliente 3",
                Name = "Client3",
                Giro = "71",
                Direction = "Dirección 3",
                City = "Ciudad 3",
                Telephone = "3123786598",
                Email = "em@cli3"
            });
            return clients;
        }
        public static DbSet<User> Add(DbSet<User> users, DbSet<Client> clients)
        {
            users.Add(new User
            {
                Client = clients.Find(1),
                Permissions = EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.FROWARD_ADMIN),
                Login = "admin",
                Password = "891487126187117422521931298924620411618024517185152621889179202245169193115202207197",
                Name = "Admin",
                LastName = "Nimda",
                RUT = "26026378-1",
                Telephone = "78945321",
                Email = "admin@admin",
                LastEntry = DateTime.Now,
                Active = true
            });
            users.Add(new User
            {
                Client = clients.Find(1),
                Permissions = EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.FROWARD_SUPERVISOR),
                Login = "Supervisor",
                Password = "891487126187117422521931298924620411618024517185152621889179202245169193115202207197",
                Name = "Supervisor",
                LastName = "Rosivrepus",
                RUT = "26026378-1",
                Telephone = "78945321",
                Email = "supervisor@supervisor",
                LastEntry = DateTime.Now,
                Active = true
            });
            users.Add(new User
            {
                Client = clients.Find(1),
                Permissions = EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.FROWARD_READER),
                Login = "Lector",
                Password = "891487126187117422521931298924620411618024517185152621889179202245169193115202207197",
                Name = "Lector",
                LastName = "Rotcel",
                RUT = "21904187-K",
                Telephone = "987656897",
                Email = "lector@lector",
                LastEntry = DateTime.Now,
                Active = true
            });
            users.Add(new User
            {
                Client = clients.Find(1),
                Permissions = EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.FROWARD_READER),
                Login = "ApiUser",
                Password = "891487126187117422521931298924620411618024517185152621889179202245169193115202207197",
                Name = "",
                LastName = "",
                RUT = "",
                Telephone = "",
                Email = "",
                LastEntry = DateTime.Now,
                Active = true
            });
            users.Add(new User
            {
                Client = clients.Find(2),
                Permissions = EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.CLIENT),
                Login = "Client1",
                Password = "891487126187117422521931298924620411618024517185152621889179202245169193115202207197",
                Name = "Cliente 1",
                LastName = "Cliente",
                RUT = "15044475-6",
                Telephone = "678956785367",
                Email = "client1@client1",
                LastEntry = DateTime.Now,
                Active = true
            });
            users.Add(new User
            {
                Client = clients.Find(3),
                Permissions = EnumObjects.GetUserType((int)EnumObjects.USER_TYPES.CLIENT),
                Login = "Client2",
                Password = "891487126187117422521931298924620411618024517185152621889179202245169193115202207197",
                Name = "Cliente 2",
                LastName = "Cliente",
                RUT = "15044475-6",
                Telephone = "678956785367",
                Email = "client2@client2",
                LastEntry = DateTime.Now,
                Active = true
            });
            return users;
        }
        public static DbSet<Token> Add(DbSet<Token> tokens, DbSet<User> users, int time)
        {
            DateTime initTime = Convert.ToDateTime(DateTime.Now); tokens.Add(new Token
            {
                IdUser = 3,
                RequestedAt = initTime,
                ExpiresAt = initTime.AddMinutes(time),
                TokenCode = "testCode - Still active FROWARD_SUPERVISOR"
            });
            tokens.Add(new Token
            {
                IdUser = 2,
                RequestedAt = initTime,
                ExpiresAt = initTime.AddMinutes(time),
                TokenCode = "testCode - Still active FROWARD_READER"
            });
            tokens.Add(new Token
            {
                IdUser = 4,
                RequestedAt = initTime.AddMinutes(300),
                ExpiresAt = initTime.AddMinutes(300),
                TokenCode = "testCode - Still active FROWARD_ADMIN"
            });
            tokens.Add(new Token
            {
                IdUser = 1,
                RequestedAt = initTime.AddMinutes(300),
                ExpiresAt = initTime.AddMinutes(300),
                TokenCode = "testCode - Still active CLIENT1"
            });
            tokens.Add(new Token
            {
                IdUser = 5,
                RequestedAt = initTime.AddMinutes(300),
                ExpiresAt = initTime.AddMinutes(300),
                TokenCode = "testCode - Still active CLIENT2"
            });
            return tokens;
        }
        public static DbSet<Driver> Add(DbSet<Driver> drivers)
        {
            drivers.Add(new Driver
            {
                Name = "driver1",
                LastName = "driver1",
                RUT = "35185391-3",
                Telephone = "4567897",
                Email = "driv@el1"
            });
            drivers.Add(new Driver
            {
                Name = "driver2",
                LastName = "driver2",
                RUT = "8109414-4",
                Telephone = "545567897",
                Email = "driv@el2"
            });
            drivers.Add(new Driver
            {
                Name = "driver3",
                LastName = "driver3",
                RUT = "10063987-4",
                Telephone = "1237",
                Email = "driv@el3"
            });
            return drivers;
        }
        public static DbSet<VehicleType> Add(DbSet<VehicleType> vehiclesTypes)
        {
            vehiclesTypes.Add(new VehicleType()
            {
                Name = EnumObjects.GetVehicleType((int)EnumObjects.VEHICLE_TYPE.MINOR)
            }); vehiclesTypes.Add(new VehicleType()
            {
                Name = EnumObjects.GetVehicleType((int)EnumObjects.VEHICLE_TYPE.MAYOR)
            });
            return vehiclesTypes;
        }
        public static DbSet<Vehicle> Add(DbSet<Vehicle> vehicles, DbSet<VehicleType> vehicleTypes)
        {
            vehicles.Add(new Vehicle
            {
                Patent = "PATE42",
                VehicleType = vehicleTypes.Find(1),
            });
            vehicles.Add(new Vehicle
            {
                Patent = "PATE48",
                VehicleType = vehicleTypes.Find(1),
            });
            vehicles.Add(new Vehicle
            {
                Patent = "PATE49",
                VehicleType = vehicleTypes.Find(2),
            });
            return vehicles;
        }

        public static DbSet<Request> Add(
          DbSet<Request> request,
          DbSet<Driver> drivers,
          DbSet<Client> clients,
          DbSet<Vehicle> vehicles
          )
        {
            DateTime dateTimeNow = DateTime.Now;

            request.Add(new Request
            {
                NumRequest = 1,
                Client = clients.Find(2),
                Driver = drivers.Find(2),
                Vehicle = vehicles.Find(2),
                IndustrialShed = "Ind 1",
                DestinyDirection = "destiny direction 1",
                ValidityStart = DateTime.Today.AddDays(4).AddHours(12),
                ValidityEnd = DateTime.Today.AddDays(5).AddHours(12),
                ValidityStartHour = DateTime.Today.TimeOfDay.Add(new TimeSpan(3, 0, 0)),
                ValidityEndHour = DateTime.Today.TimeOfDay.Add(new TimeSpan(10, 0, 0)),
                Product = "Product NumRequest 1",
                Format = "Format 1",
                NumAgreement = "123456",
                OC = "11357",
                NumAgris = "123456",
                DestinyClient = "Destiny client 1",
                Active = true,
            });
            request.Add(new Request
            {
                NumRequest = 2,
                Client = clients.Find(2),
                Driver = drivers.Find(1),
                Vehicle = vehicles.Find(1),
                IndustrialShed = "Ind 2",
                DestinyDirection = "destiny direction 2",
                ValidityStart = DateTime.Today.AddHours(12),
                ValidityEnd = DateTime.Today.AddDays(5).AddHours(12),
                ValidityStartHour = DateTime.Today.TimeOfDay.Add(new TimeSpan(1, 0, 0)),
                ValidityEndHour = DateTime.Today.TimeOfDay.Add(new TimeSpan(9, 0, 0)),
                Product = "Product NumRequest 2",
                Format = "Format 2",
                NumAgreement = "223456",
                OC = "21357",
                NumAgris = "223456",
                DestinyClient = "Destiny client 2",
                Active = true
            });
            request.Add(new Request
            {
                NumRequest = 3,
                Client = clients.Find(3),
                Driver = drivers.Find(1),
                Vehicle = vehicles.Find(2),
                IndustrialShed = "Ind 3",
                DestinyDirection = "destiny direction 3",
                ValidityStart = DateTime.Today.AddDays(-2).AddHours(12),
                ValidityEnd = DateTime.Today.AddDays(-1).AddHours(12),
                ValidityStartHour = DateTime.Today.TimeOfDay.Add(new TimeSpan(10, 0, 0)),
                ValidityEndHour = DateTime.Today.TimeOfDay.Add(new TimeSpan(15, 0, 0)),
                Product = "Product NumRequest 3",
                Format = "Format 3",
                NumAgreement = "323456",
                OC = "31357",
                NumAgris = "323456",
                DestinyClient = "Destiny client 3",
                Active = true
            });
            request.Add(new Request
            {
                NumRequest = 10,
                Client = clients.Find(2),
                Driver = drivers.Find(3),
                Vehicle = vehicles.Find(2),
                IndustrialShed = "Ind 10",
                DestinyDirection = "destiny direction 10",
                ValidityStart = DateTime.Today.AddDays(-5).AddHours(12),
                ValidityEnd = DateTime.Today.AddHours(12),
                ValidityStartHour = DateTime.Today.TimeOfDay.Add(new TimeSpan(1, 0, 0)),
                ValidityEndHour = DateTime.Today.TimeOfDay.Add(new TimeSpan(9, 0, 0)),
                Product = "Product NumRequest 10",
                Format = "Format 2",
                NumAgreement = "1023456",
                OC = "101357",
                NumAgris = "1023456",
                DestinyClient = "Destiny client 10",
                Active = false,
            });
            // Time Test
            request.Add(new Request
            {
                NumRequest = 4,
                Client = clients.Find(2),
                Driver = drivers.Find(1),
                Vehicle = vehicles.Find(2),
                IndustrialShed = "Ind 4",
                DestinyDirection = "destiny direction 4",
                ValidityStart = DateTime.Today.AddHours(12),
                ValidityEnd = DateTime.Today.AddHours(12),
                ValidityStartHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours - 2, 0, 0),
                ValidityEndHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours + 2, 0, 0),
                Product = "test times 2h left",
                Format = "Format 4",
                NumAgreement = "423456",
                OC = "41357",
                NumAgris = "423456",
                DestinyClient = "Destiny client 4",
                Active = true
            });

            request.Add(new Request
            {
                NumRequest = 5,
                Client = clients.Find(2),
                Driver = drivers.Find(1),
                Vehicle = vehicles.Find(2),
                IndustrialShed = "Ind 5",
                DestinyDirection = "destiny direction 5",
                ValidityStart = DateTime.Today.AddHours(12),
                ValidityEnd = DateTime.Today.AddHours(12),
                ValidityStartHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours - 4, 0, 0),
                ValidityEndHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours - 2, 0, 0),
                Product = "test times not appearing",
                Format = "Format 5",
                NumAgreement = "523456",
                OC = "51357",
                NumAgris = "423456",
                DestinyClient = "Destiny client 5",
                Active = true
            });

            request.Add(new Request
            {
                NumRequest = 6,
                Client = clients.Find(4),
                Driver = drivers.Find(1),
                Vehicle = vehicles.Find(2),
                IndustrialShed = "Ind 6",
                DestinyDirection = "destiny direction 6",
                ValidityStart = DateTime.Today.AddDays(-4).AddHours(12),
                ValidityEnd = DateTime.Today.AddDays(-2).AddHours(12),
                ValidityStartHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours - 4, 0, 0),
                ValidityEndHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours - 2, 0, 0),
                Product = "X",
                Format = "Format 6",
                NumAgreement = "623456",
                OC = "61357",
                NumAgris = "623456",
                DestinyClient = "Destiny client 6",
                Active = true
            });
            request.Add(new Request
            {
                NumRequest = 7,
                Client = clients.Find(4),
                Driver = drivers.Find(1),
                Vehicle = vehicles.Find(2),
                IndustrialShed = "Ind 7",
                DestinyDirection = "destiny direction 7",
                ValidityStart = DateTime.Today.AddDays(-3).AddHours(12),
                ValidityEnd = DateTime.Today.AddDays(2).AddHours(12),
                ValidityStartHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours - 4, 0, 0),
                ValidityEndHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours - 2, 0, 0),
                Product = "T",
                Format = "Format 7",
                NumAgreement = "723456",
                OC = "71357",
                NumAgris = "723456",
                DestinyClient = "Destiny client 7",
                Active = true
            });

            request.Add(new Request
            {
                NumRequest = 8,
                Client = clients.Find(4),
                Driver = drivers.Find(1),
                Vehicle = vehicles.Find(2),
                IndustrialShed = "Ind 8",
                DestinyDirection = "destiny direction 8",
                ValidityStart = DateTime.Today.AddDays(-1).AddHours(12),
                ValidityEnd = DateTime.Today.AddDays(1).AddHours(12),
                ValidityStartHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours - 4, 0, 0),
                ValidityEndHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours - 2, 0, 0),
                Product = "Y",
                Format = "Format 8",
                NumAgreement = "823456",
                OC = "81357",
                NumAgris = "823456",
                DestinyClient = "Destiny client 8",
                Active = true
            });

            request.Add(new Request
            {
                NumRequest = 9,
                Client = clients.Find(4),
                Driver = drivers.Find(1),
                Vehicle = vehicles.Find(2),
                IndustrialShed = "Ind 9",
                DestinyDirection = "destiny direction 9",
                ValidityStart = DateTime.Today.AddDays(2).AddHours(12),
                ValidityEnd = DateTime.Today.AddDays(4).AddHours(12),
                ValidityStartHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours - 4, 0, 0),
                ValidityEndHour = new TimeSpan(dateTimeNow.TimeOfDay.Hours - 2, 0, 0),
                Product = "Y",
                Format = "Format 9",
                NumAgreement = "923456",
                OC = "91357",
                NumAgris = "923456",
                DestinyClient = "Destiny client 9",
                Active = true
            });

            return request;
        }
        public static DbSet<Schedule> Add(DbSet<Schedule> schedule, DbSet<Client> clients)
        {
            schedule.Add(new Schedule
            {
                Client = clients.Find(2),
                DayFrom = DateTime.Today,
                DayTo = DateTime.Today,
                HourFrom = DateTime.Now.TimeOfDay,
                HourTo = DateTime.Now.AddHours(6).TimeOfDay,
                Comment = "Comentario 1"
            });
            schedule.Add(new Schedule
            {
                Client = clients.Find(2),
                DayFrom = DateTime.Today.AddDays(1),
                DayTo = DateTime.Today.AddDays(2),
                HourFrom = DateTime.Now.TimeOfDay,
                HourTo = DateTime.Now.AddHours(6).TimeOfDay,
                Comment = "Comentario 2"
            });
            schedule.Add(new Schedule
            {
                Client = clients.Find(3),
                DayFrom = DateTime.Today.AddDays(2),
                DayTo = DateTime.Today.AddDays(3),
                HourFrom = DateTime.Now.TimeOfDay,
                HourTo = DateTime.Now.AddHours(6).TimeOfDay,
                Comment = "Comentario 3"
            });
            schedule.Add(new Schedule
            {
                Client = clients.Find(3),
                DayFrom = DateTime.Today.AddDays(3),
                DayTo = DateTime.Today.AddDays(4),
                HourFrom = DateTime.Now.TimeOfDay,
                HourTo = DateTime.Now.AddHours(6).TimeOfDay,
                Comment = "Comentario 4"
            });
            schedule.Add(new Schedule
            {
                Client = clients.Find(2),
                DayFrom = DateTime.Today.AddDays(-3),
                DayTo = DateTime.Today.AddDays(-4),
                HourFrom = DateTime.Now.TimeOfDay,
                HourTo = DateTime.Now.AddHours(6).TimeOfDay,
                Comment = "Comentario 5"
            }); return schedule;
        }
    }
}


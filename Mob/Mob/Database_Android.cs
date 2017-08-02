using Mob;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.IO;
using Xamarin.Forms;
using System.Linq;
using Mob.Dto;

[assembly: Dependency(typeof(Database_Android))]
namespace Mob
{
    public interface IDatabase
    {
        SQLiteConnection DBConnect();
    }


    public class Database_Android : IDatabase
    {
        public Database_Android()
        {
        }
        public SQLiteConnection DBConnect()
        {
            try
            {
                var filename = "rent.db";
                string folder =
                Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var path = Path.Combine(folder, filename);
                var t = System.IO.File.Exists(path);
                var connection = new SQLiteConnection(path);
                return connection;
            }
            catch(Exception ex)
            {
                App.Toast("Ошибка БД!!!");
            }
            return null;
        }
    }

    public class Repository
    {
        protected static object locker = new object();
        protected SQLiteConnection database;
        public Repository()
        {
            try
            {
                database = DependencyService.Get<IDatabase>().DBConnect();
                database.CreateTable<PriceInfo>();
                database.CreateTable<Rent>();
                database.CreateTable<UserSettings>();
                database.CreateTable<Error>();
                CheckTable("");
            }
            catch(Exception)
            {
                App.Toast("Ошибка БД!");
            }
        }
        private void CheckTable(string tableName)
        {
            if ((from i in database.Table<PriceInfo>() select i).Count() == 0)
            {
                database.Insert(new PriceInfo { Name = "10 минут", Price = 150, Time = new TimeSpan(0, 0, 10, 0), Vehicle = "G" });
                database.Insert(new PriceInfo { Name = "30 минут", Price = 400, Time = new TimeSpan(0, 0, 30, 0), Vehicle = "G" });
                database.Insert(new PriceInfo { Name = "60 минут", Price = 700, Time = new TimeSpan(0, 1, 0, 0), Vehicle = "G" });
                database.Insert(new PriceInfo { Name = "1 час", Price = 130, Time = new TimeSpan(0, 1, 0, 0), Vehicle = "C" });
                database.Insert(new PriceInfo { Name = "2 часа", Price = 130 * 2, Time = new TimeSpan(0, 2, 0, 0), Vehicle = "C" });
                database.Insert(new PriceInfo { Name = "3 часа", Price = 130 * 3, Time = new TimeSpan(0, 3, 0, 0), Vehicle = "C" });
                database.Insert(new PriceInfo { Name = "4 часа", Price = 130 * 4, Time = new TimeSpan(0, 4, 0, 0), Vehicle = "C" });
                database.Insert(new PriceInfo { Name = "Полдня", Price = 400, Time = new TimeSpan(0, 12, 0, 0), Vehicle = "C" });
                database.Insert(new PriceInfo { Name = "Весь день", Price = 700, Time = new TimeSpan(1, 0, 0, 0), Vehicle = "C" });
            }
        }
        #region Rent
        public List<Rent> GetRents(DateTime dateTime)
        {
            lock (locker)
            {
                return (from i in database.Table<Rent>() where i.Date == dateTime select i).OrderBy(x=>x.Time).ToList();
            }
        }       
        public int SaveRent(Rent item)
        {
            lock (locker)
            {
                if (item.Id != 0)
                {
                    database.Update(item);
                    return item.Id;
                }
                else
                {
                    return database.Insert(item);
                }
            }
        }
        #endregion
        #region UserSettings
        public string GetUserInfo()
        {
            lock (locker)
            {
                var name = GetSettingsByName("UserName")?.Vlaue;
                var location = GetSettingsByName("Location")?.Vlaue;
                return name == null || location == null ? null : $"{name} {location}";
            }
        }
        public int SaveUserSettings(UserSettings item)
        {
            lock (locker)
            {
                if (item.Id != 0)
                {
                    database.Update(item);
                    return item.Id;
                }
                else
                {
                    return database.Insert(item);
                }
            }
        }
        public UserSettings GetSettingsByName(string Name)
        {
            lock (locker)
            {
                return (from i in database.Table<UserSettings>() where i.Name == Name select i).SingleOrDefault();
            }
        }
        #endregion
        #region Price
        public List<PriceInfo> GetPrices()
        {
            lock (locker)
            {
                return (from i in database.Table<PriceInfo>() select i).ToList();
            }
        }
        public List<PriceInfo> GetPrices(string Type)
        {
            lock (locker)
            {
                return (from i in database.Table<PriceInfo>() where i.Vehicle == Type select i).ToList();
            }
        }
        public int SavePrice(PriceInfo item)
        {
            lock (locker)
            {
                if (item.Id != 0)
                {
                    database.Update(item);
                    return item.Id;
                }
                else
                {
                    return database.Insert(item);
                }
            }
        }
        #endregion
        #region Error
        public List<Error> GetErrors()
        {
            lock (locker)
            {
                return (from i in database.Table<Error>() select i).ToList();
            }
        }
        public void DeleteAllErrors()
        {
            lock (locker)
            {
                database.DeleteAll<Error>();
            }
        }
        public int SaveError(Error item)
        {
            lock (locker)
            {
                if (item.Id != 0)
                {
                    database.Update(item);
                    return item.Id;
                }
                else
                {
                    return database.Insert(item);
                }
            }
        }
        public int ErrorCount()
        {
            lock (locker)
            {
                return (from i in database.Table<Error>() select i).Count();
            }
        }
        #endregion

    }
}

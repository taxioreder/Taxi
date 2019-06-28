using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DBAplication;
using DBAplication.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiMobaileTaxi.Service
{
    public class SqlCoommandTaxiApi
    {
        private Context context = null;

        public SqlCoommandTaxiApi()
        {
            context = new Context();
        }

        public List<Driver> CheckOrderForDriver()
        {
            context.Drivers.Load();
            List<Driver> drivers = null;
            List<Order> orders = context.Orders.ToList().Where(o => o.Driver != null && (o.CurrentStatus == "Assigned" || o.CurrentStatus == "Picked up")).ToList();
            if (orders == null || orders.Count == 0)
            {
                drivers = context.Drivers.Where(d => d.IsWork).ToList();
            }
            else
            {
                drivers = context.Drivers.Where(d => orders.FirstOrDefault(o => o.Driver == d) == null && d.IsWork).ToList();
            }
            return drivers;
        }

        public async Task AddDriversInOrder(string idOrder, string idDriver)
        {
            Order order = context.Orders.FirstOrDefault(s => s.ID == Convert.ToInt32(idOrder));
            Driver driver = context.Drivers.FirstOrDefault(d => d.ID == Convert.ToInt32(idDriver));
            order.Driver = driver;
            order.CurrentStatus = "Assigned";
            await context.SaveChangesAsync();
        }

        public async void SetWorkDrive()
        {
            List<Driver> drivers = context.Drivers.ToList();
            foreach (var driver in drivers)
            {
                driver.IsWork = true;
            }
            await context.SaveChangesAsync();
        }

        internal bool CheckEmailAndPsw(string email, string password)
        {
            return context.Drivers.FirstOrDefault(d => d.EmailAddress == email && d.Password == password) != null ? true : false;
        }

        internal async void SaveToken(string email, string password, string token)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.EmailAddress == email && d.Password == password);
            driver.Token = token;
            await context.SaveChangesAsync();
        }

        internal bool CheckToken(string token)
        {
            return context.Drivers.FirstOrDefault(d => d.Token == token) != null ? true : false;
        }

        internal List<Order> GetOrdersForToken(string token)
        {
            List<Order> orders1 = new List<Order>();
            Driver driver = context.Drivers.FirstOrDefault(d => d.Token == token);
            List<Order> orders = context.Orders.ToList().FindAll(s => s.Driver != null && s.Driver.ID == driver.ID);
            if (orders == null)
            {
                return new List<Order>();
            }
            orders1.AddRange(orders.FindAll(s => s.CurrentStatus == "Assigned" || s.CurrentStatus == "Picked up"));
            return orders1;
        }

        internal async void SaveGPSLocationData(string token, Geolocations geolocations)
        {
            Driver driver = context.Drivers.Where(d => d.Token == token)
                .Include(d => d.geolocations)
                .FirstOrDefault();
            driver.geolocations = geolocations;
            await context.SaveChangesAsync();
        }

        public async void RecurentOrderDriveDB(string status, int idorder)
        {
            Order order = await context.Orders.FirstOrDefaultAsync(o => o.ID == idorder);
            order.CurrentOrder = status;
            if(status == "DriveFrome")
            {
                order.CurrentStatus = "Picked up";
                order.isAccept = true;
                order.IsVisableAccept = false;
            }
            await context.SaveChangesAsync();
        }

        public async Task<Order> GetAddressToOrderDB(int idorder)
        {
            Order order = await context.Orders
                .Include(o => o.Driver)
                .FirstOrDefaultAsync(o => o.ID == idorder);
            return order;
        }

        public List<Order> GetOrders()
        {
            return context.Orders.ToList().Where(o => o.CurrentStatus == "NewLoad" && (DateTime.Parse($"{GetDFormat(o.Date)} {o.TimeOfPickup}").AddMinutes(20) > DateTime.Now && DateTime.Now > DateTime.Parse($"{GetDFormat(o.Date)} {o.TimeOfPickup}").AddHours(-3))).ToList();
        }

        private string GetDFormat(string data)
        {
            DateTime date;
            if (DateTime.TryParseExact(data, "MM.dd.yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "dd.MM.yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if(DateTime.TryParseExact(data, "yyyy.MM.dd", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "MM-dd-yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "dd-MM-yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "yyyy-MM-dd", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "MM/dd/yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "dd/MM/yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "yyyy/MM/dd", null, DateTimeStyles.None, out date))
            {
            }
            return date.ToShortDateString();
        }

        public void AsiignedNext(int idOrder, int idDriverCurent)
        {
            File.WriteAllText("123.txt", "222");
            Driver driver = context.Drivers.FirstOrDefault(d => d.ID == idDriverCurent);
            Order order =  context.Orders.FirstOrDefault(o => o.ID == idOrder);
            order.CurrentOrder = "Next";
            order.CurrentStatus = "Assigned";
            order.Driver = driver;
            context.SaveChanges();
        }

        public async Task<Order> RecurentTwoOrder(string token, int idOrder)
        {
            Order order = await context.Orders.FirstOrDefaultAsync(o => o.ID == idOrder);
            Order order1 = GetOrdersForToken(token).FirstOrDefault(o => o.CurrentOrder == "Next");
            order.CurrentOrder = "None";
            order.CurrentStatus = "Delivered";
            if (order1 != null)
            {
                order1.CurrentOrder = "NewNext";
                order1.CurrentStatus = "Assigned";
                order1.isAccept = false;
            }
            await context.SaveChangesAsync();
            return order1;
        }

        public async void RecurentCancelOrder(int idOrder)
        {
            Order order = await context.Orders.FirstOrDefaultAsync(o => o.ID == idOrder);
            order.CurrentOrder = "New";
            order.CurrentStatus = "NewLoad";
            await context.SaveChangesAsync();
        }

        public Order GetOrderDb(int idDOrder)
        {
            return context.Orders.Include(o => o.Driver).FirstOrDefault(d => d.ID == idDOrder);
        }

        public async void SetAcceptVisable(int idDOrder)
        {
            Order order = context.Orders.FirstOrDefault(d => d.ID == idDOrder);
            order.IsVisableAccept = true;
            await context.SaveChangesAsync();
        }
    }
}
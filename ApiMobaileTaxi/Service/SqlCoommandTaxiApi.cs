using System;
using System.Collections.Generic;
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
            return context.Orders.Where(o => o.CurrentStatus == "NewLoad" && (Convert.ToDateTime($"{o.Date} {o.TimeOfPickup}") > DateTime.Now && DateTime.Now > Convert.ToDateTime($"{o.Date} {o.TimeOfPickup}").AddHours(-3))).ToList();
        }

        public async void AsiignedNext(int idOrder, int idDriverCurent)
        {
            Driver driver = await context.Drivers.FirstOrDefaultAsync(d => d.ID == idDriverCurent);
            Order order = await context.Orders.FirstOrDefaultAsync(o => o.ID == idOrder);
            order.CurrentOrder = "Next";
            order.CurrentStatus = "Assigned";
            order.Driver = driver;
            await context.SaveChangesAsync();
        }

        public async void RecurentTwoOrder(string token, int idOrder)
        {
            Order order = await context.Orders.FirstOrDefaultAsync(o => o.ID == idOrder);
            Order order1 = GetOrdersForToken(token).FirstOrDefault(o => o.CurrentOrder == "Next");
            order.CurrentOrder = "None";
            order.CurrentStatus = "Delivered";
            if (order1 != null)
            {
                order1.CurrentOrder = "NewNext";
                order1.CurrentStatus = "Assigned";
            }
            await context.SaveChangesAsync();
        }

        public async void RecurentCancelOrder(int idOrder)
        {
            Order order = await context.Orders.FirstOrDefaultAsync(o => o.ID == idOrder);
            order.CurrentOrder = "New";
            order.CurrentStatus = "NewLoad";
            await context.SaveChangesAsync();
        }
    }
}
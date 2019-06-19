using DBAplication;
using DBAplication.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceManager
{
    public class SqlCommanManager
    {
        private Context context = null;

        public SqlCommanManager()
        {
            context = new Context();
        }

        public List<Driver> CheckOrderForDriver()
        {
            context.Drivers.Load();
            List<Driver> drivers = null;
             List<Order> orders = context.Orders.ToList().Where(o => (o.Driver != null && o.Driver.IsWork && o.CurrentStatus == "Assigned" || o.CurrentStatus == "Picked up")).ToList();
            if(orders == null || orders.Count == 0)
            {
                drivers = context.Drivers.ToList();
            }
            else
            {
                drivers = context.Drivers.Where(d => orders.FirstOrDefault(o => o.Driver == d) == null).ToList();
            }
            return drivers;
        }

        public List<Order> GetOrders()
        {
            return context.Orders.Where(o => o.CurrentStatus == "NewLoad" && (Convert.ToDateTime($"{o.Date} {o.TimeOfPickup}") > DateTime.Now && DateTime.Now > Convert.ToDateTime($"{o.Date} {o.TimeOfPickup}").AddHours(-3))).ToList();
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
            foreach(var driver in drivers)
            {
                driver.IsWork = true;
            }
            await context.SaveChangesAsync();
        }
    }
}
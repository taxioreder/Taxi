using DBAplication;
using DBAplication.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTaxi.Service
{
    public class SqlCommand
    {
        private Context context = null;

        public SqlCommand()
        {
            context = new Context();
            InitUserOne();
        }

        private async void InitUserOne()
        {
            if (context.Admins.Count() == 0)
            {
                Admin admin = new Admin();
                admin.Login = "1";
                admin.Password = "1";
                await context.Admins.AddAsync(admin);
                await context.SaveChangesAsync();
            }
        }

        public bool ExistsDataUser(string login, string password)
        {
            return context.Admins.FirstOrDefault(u => u.Login == login && u.Password == password) != null;
        }

        public async void SaveKeyDatabays(string login, string password, int key)
        {
            try
            {
                Admin users = context.Admins.FirstOrDefault(u => u.Login == login && u.Password == password);
                users.KeyAuthorized = key.ToString();
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }

        public List<Order> GetShippings(string status, int page)
        {
            List<Order> orders = null;
            List<Order> ordersTmp = new List<Order>();
            orders = context.Orders.ToList().FindAll(o => o.CurrentStatus == status);
            if (page != 0)
            {
                try
                {
                    orders = orders.GetRange((20 * page) - 25, 25);
                }
                catch (Exception)
                {
                    orders = orders.GetRange((20 * page) - 25, orders.Count % 25);
                }
            }
            else
            {
                try
                {
                    orders = orders.GetRange(0, 25);
                }
                catch (Exception)
                {
                    orders = orders.GetRange(0, orders.Count % 25);
                }
            }
            foreach (var order in orders)
            {
                int orderIndex = ordersTmp.FindIndex(o => (o.FromZip == order.FromZip && o.ToZip == order.ToZip) || (o.FromZip == order.FromZip || o.ToZip == order.ToZip));
                if (orderIndex == -1)
                {
                    ordersTmp.Add(order);
                }
                else
                {
                    ordersTmp.Insert(orderIndex + 1, order);
                }
            }
            return ordersTmp;
        }

        public bool CheckKeyDb(string key)
        {
            return context.Admins.FirstOrDefault(u => u.KeyAuthorized == key) != null;
        }

        public int GetCountPageInDb(string status)
        {
            int countPage = 0;
            List<Order> orders = context.Orders.ToList().FindAll(s => s.CurrentStatus == status);
            countPage = orders.Count / 25;
            int remainderPage = orders.Count % 25;
            countPage = remainderPage > 0 ? countPage + 1 : countPage;
            return countPage;
        }

        public void SaveOrder(Order order)
        {
            List<Order> orders = context.Orders.ToList();
            Order orderDb = orders.FirstOrDefault(o => o.Comment == order.Comment && o.Date == order.Date && o.FromAddress == order.FromAddress && o.Milisse == order.Milisse && o.NameCustomer == order.NameCustomer
         && o.NoName == order.NoName && o.NoName1 == order.NoName1 && o.NoName2 == order.NoName2 && o.NoName3 == order.NoName3 && o.NoName4 == order.NoName4 && o.NoName5 == order.NoName5 && o.NoName6 == order.NoName6
         && o.Phone == order.Phone && o.Price == order.Price && o.TimeOfAppointment == order.TimeOfAppointment && o.TimeOfPickup == order.TimeOfPickup && o.ToAddress == order.ToAddress);
            if (orderDb != null)
            {
                return;
            }
            int orderIndex = orders.FindIndex(o => (o.FromZip == order.FromZip && o.ToZip == order.ToZip) || (o.FromZip == order.FromZip || o.ToZip == order.ToZip));
            //if(orderIndex == -1)
            //{
                context.Orders.Add(order);
            //}
            //else
            //{
            //    orders.Insert(orderIndex + 1, order);
            //    context.UpdateRange(orders);
            //}
            context.SaveChanges();
        }

        public Order GetShipping(string id)
        {
            return context.Orders.FirstOrDefault(o => o.ID == Convert.ToInt32(id));
        }

        public async void UpdateorderInDb(string idLoad, string nameCustomer, string phone, string fromAddress, string toAddress, string noName, string noName1,
           string noName2, string status, string date, string timeOfPickup, string timeOfAppointment, string milisse, string price, string noName3, string noNama4, string noName5, string noName6)
        {
            Order order = context.Orders.FirstOrDefault(s => s.ID == Convert.ToInt32(idLoad));
            order.NameCustomer = nameCustomer != null ? nameCustomer : order.NameCustomer;
            order.CurrentStatus = status != null ? status : order.CurrentStatus;
            order.Phone = phone != null ? phone : order.Phone;
            order.FromAddress = fromAddress != null ? fromAddress : order.FromAddress;
            order.ToAddress = toAddress != null ? toAddress : order.ToAddress;
            order.NoName = noName != null ? noName : order.NoName;
            order.NoName1 = noName1 != null ? noName1 : order.NoName1;
            order.NoName2 = noName2 != null ? noName2 : order.NoName2;
            order.Date = date != null ? date : order.Date;
            order.TimeOfPickup = timeOfPickup != null ? timeOfPickup : order.TimeOfPickup;
            order.TimeOfAppointment = timeOfAppointment != null ? timeOfAppointment : order.TimeOfAppointment;
            order.Milisse = milisse != null ? milisse : order.Milisse;
            order.Price = price != null ? price : order.Price;
            order.NoName3 = noName3 != null ? noName3 : order.NoName3;
            order.NoName4 = noNama4 != null ? noNama4 : order.NoName4;
            order.NoName5 = noName5 != null ? noName5 : order.NoName5;
            order.NoName6 = noName6 != null ? noName6 : order.NoName6;
            await context.SaveChangesAsync();
        }

        public async void RecurentOnArchived(string id)
        {
            context.Orders.Load();
            Order order = await context.Orders.FirstOrDefaultAsync(s => s.ID == Convert.ToInt32(id));
            if (order != null)
            {
                order.CurrentStatus = "Archived";
                await context.SaveChangesAsync();
            }
        }

        public async void RecurentOnDeleted(string id)
        {
            context.Orders.Load();
            Order shipping = await context.Orders.FirstOrDefaultAsync(s => s.ID == Convert.ToInt32(id));
            if (shipping != null)
            {
                shipping.CurrentStatus = "Deleted";
                await context.SaveChangesAsync();
            }
        }

        public async Task<Order> CreateShipping()
        {
            Order order = new Order();
            order.CurrentStatus = "NewLoad";
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            Order order1 = context.Orders.FirstOrDefault(s => s.ID == order.ID);
            return order1;
        }

        public List<Driver> GetDrivers(int page)
        {
            List<Driver> drivers = null;
            drivers = context.Drivers.ToList();

            if (page == -1)
            {
            }
            else if (page != 0)
            {
                try
                {
                    drivers = drivers.GetRange((20 * page) - 20, 20);
                }
                catch (Exception)
                {
                    drivers = drivers.GetRange((20 * page) - 20, drivers.Count % 20);
                }
            }
            else
            {
                try
                {
                    drivers = drivers.GetRange(0, 20);
                }
                catch (Exception)
                {
                    drivers = drivers.GetRange(0, drivers.Count % 20);
                }
            }
            return drivers;
        }

        public async void AddDriver(Driver driver)
        {
            await context.Drivers.AddAsync(driver);
            await context.SaveChangesAsync();
        }

        public async void RemoveDriveInDb(int id)
        {
            context.Drivers.Remove(context.Drivers.FirstOrDefault(d => d.ID == id));
            await context.SaveChangesAsync();
        }

        public List<Driver> GetDriversInDb()
        {
            context.Geolocations.Load();
            return context.Drivers.ToList();
        }

        public async void AddDriversInOrder(string idOrder, string idDriver)
        {
            context.Drivers.Load();
            Order order = context.Orders.FirstOrDefault<Order>(s => s.ID == Convert.ToInt32(idOrder));
            Driver driver = context.Drivers.FirstOrDefault<Driver>(d => d.ID == Convert.ToInt32(idDriver));
            order.Driver = driver;
            order.CurrentStatus = "Assigned";
            await context.SaveChangesAsync();
        }

        public async void RemoveDriversInOrder(string idOrder)
        {
            context.Drivers.Load();
            Order order = context.Orders.FirstOrDefault<Order>(s => s.ID == Convert.ToInt32(idOrder));
            order.Driver = null;
            order.CurrentStatus = "NewLoad";
            await context.SaveChangesAsync();
        }
    }
}
using DBAplication;
using DBAplication.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            //InitUserOne();
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
                //Admin admin1 = new Admin();
                //admin1.Login = "2";
                //admin1.Password = "2";
                //await context.Admins.AddAsync(admin);
                //await context.SaveChangesAsync();
                //Admin admin2 = new Admin();
                //admin2.Login = "3";
                //admin2.Password = "3";
                //await context.Admins.AddAsync(admin);
                //await context.SaveChangesAsync();
            }
        }

        public async Task SetFedBack(int idOrder, string feedBack, bool isValid)
        {
            Order order = context.Orders.First(o => o.ID == idOrder);
            order.isValid = isValid;
            order.FB = feedBack;
            await context.SaveChangesAsync();
        }

        public async void AssigneOrderMobile(OrderMobile orderMobile)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.ID.ToString() == orderMobile.IdDriver);
            orderMobile.Status = "New";
            foreach (Order order in orderMobile.Orders)
            {
                Order order1 = context.Orders.FirstOrDefault(o => o.ID == order.ID);
                order.CurrentStatus = "Assigned";
                order1.Driver = driver;
            }
            context.OrderMobiles.Add(orderMobile);
            await context.SaveChangesAsync();
        }

        public List<Order> GetOrders()
        {
            return context.Orders.Where(o => o.CurrentStatus == "NewLoad" && o.isValid && (DateTime.Parse($"{GetDFormat(o.Date)} {o.TimeOfPickup}").AddMinutes(20) > DateTime.Now && DateTime.Now > DateTime.Parse($"{GetDFormat(o.Date)} {o.TimeOfPickup}").AddHours(-1))).ToList();
        }

        public List<Order> GetFullOrders()
        {
            return context.Orders.ToList();
        }

        public Driver GetDriver(string idDriver)
        {
            return context.Drivers.FirstOrDefault(d => d.ID.ToString() == idDriver);
        }

        public async void ReCheckWorkInDb(int idDriver, bool checkedDriver)
        {
            Driver driver = await context.Drivers.FirstOrDefaultAsync(d => d.ID == idDriver);
            driver.IsWork = checkedDriver;
            await context.SaveChangesAsync();
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
            orders = context.Orders.Include(d => d.Driver).Where(o => o.CurrentStatus == status).ToList();
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
            if (page != 0)
            {
                try
                {
                    ordersTmp = ordersTmp.GetRange((20 * page) - 25, 25);
                }
                catch (Exception)
                {
                    ordersTmp = ordersTmp.GetRange((20 * page) - 25, orders.Count % 25);
                }
            }
            else
            {
                try
                {
                    ordersTmp = ordersTmp.GetRange(0, 25);
                }
                catch (Exception)
                {
                    ordersTmp = ordersTmp.GetRange(0, orders.Count % 25);
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
            Order orderDb = context.Orders.FirstOrDefault(o => o.Comment == order.Comment && o.Date == order.Date && o.FromAddress == order.FromAddress && o.Milisse == order.Milisse && o.NameCustomer == order.NameCustomer
         && o.NoName == order.NoName && o.NoName1 == order.NoName1 && o.NoName2 == order.NoName2 && o.NoName3 == order.NoName3 && o.NoName4 == order.NoName4 && o.NoName5 == order.NoName5 && o.NoName6 == order.NoName6
         && o.Phone == order.Phone && o.Price == order.Price && o.TimeOfAppointment == order.TimeOfAppointment && o.TimeOfPickup == order.TimeOfPickup && o.ToAddress == order.ToAddress);
            if (orderDb != null)
            {
                return;
            }
            order.FB = "Wait a minute we check the order";
            order.isValid = false;
            context.Orders.Add(order);
            context.SaveChanges();
        }

        public Order GetShipping(string id)
        {
            return context.Orders.Include(o => o.Driver).FirstOrDefault(o => o.ID.ToString() == id);
        }

        public async void UpdateorderInDb(string idLoad, string nameCustomer, string phone, string fromAddress, string toAddress, string noName, string noName1, string noName2, string status,
            string date, string timeOfPickup, string timeOfAppointment, string milisse, string price, string noName3, string noNama4, string noName5, string noName6, int countCustomer)
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
            if (order.CurrentStatus == "NewLoad")
            {
                order.FB = "Wait a minute we check the order";
                order.isValid = false;
            }
            order.NoName3 = noName3 != null ? noName3 : order.NoName3;
            order.NoName4 = noNama4 != null ? noNama4 : order.NoName4;
            order.NoName5 = noName5 != null ? noName5 : order.NoName5;
            order.NoName6 = noName6 != null ? noName6 : order.NoName6;
            order.NoName6 = noName6 != null ? noName6 : order.NoName6;
            order.CountCustomer = countCustomer == 0 ? 1 : countCustomer > 4 ? 4 : countCustomer;
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
            order.CurrentOrder = "New";
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

        public async void UpdateDriver(Driver driver)
        {
            context.Drivers.Update(driver);
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

        private string GetDFormat(string data)
        {
            DateTime date;
            if (DateTime.TryParseExact(data, "MM.dd.yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "dd.MM.yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "yyyy.MM.dd", null, DateTimeStyles.None, out date))
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
    }
}
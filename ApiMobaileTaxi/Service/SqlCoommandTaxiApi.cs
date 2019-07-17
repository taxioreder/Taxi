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

        public async Task SetStatusMobileOrderStart(int idOrderMobile)
        {
            OrderMobile orderMobile = context.OrderMobiles
                .Include(om => om.Orders)
                .Include(om => om.OnePointForAddressOrders)
                .FirstOrDefault(om => om.ID == idOrderMobile);
            Order order = context.Orders.FirstOrDefault(o => o.ID == orderMobile.OnePointForAddressOrders[0].IDorder);
            orderMobile.Status = "InWork";
            orderMobile.OnePointForAddressOrders[0].Status = "DriveFromPoint";
            order.CurrentStatus = "Picked up";
            await context.SaveChangesAsync();
        }

        public async Task SetStatusCompletPoint(int idOrderMobile)
        {
            OrderMobile orderMobile = context.OrderMobiles
                .Include(om => om.Orders)
                .Include(om => om.OnePointForAddressOrders)
                .FirstOrDefault(om => om.ID == idOrderMobile);
            OnePointForAddressOrder onePointForAddressOrder = orderMobile.OnePointForAddressOrders.FirstOrDefault(om => om.Status == "DriveFromPoint");
            Order order = context.Orders.FirstOrDefault(o => o.ID == onePointForAddressOrder.IDorder);
            if (onePointForAddressOrder != null)
            {
                int index = orderMobile.OnePointForAddressOrders.IndexOf(onePointForAddressOrder);
                orderMobile.OnePointForAddressOrders.First(om => om.Status == "DriveFromPoint").Status = "CompletePoint";
                orderMobile.OnePointForAddressOrders[index + 1].Status = "DriveFromPoint";
            }
            if(onePointForAddressOrder.Type == "Start")
            {
                order.CurrentStatus = "Picked up";
            }
            else if (onePointForAddressOrder.Type == "End")
            {
                order.CurrentStatus = "Delivered";
            }
            await context.SaveChangesAsync();
        }

        public async Task SetStatusMobileOrderEnd(int idOrderMobile, string token)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.Token == token);
            OrderMobile orderMobile1 = context.OrderMobiles.ToList().FirstOrDefault(om => om.IdDriver == driver.ID.ToString() && om.Status == "NewNext");
            OrderMobile orderMobile = context.OrderMobiles
                .Include(om => om.Orders)
                .Include(om => om.OnePointForAddressOrders)
                .FirstOrDefault(om => om.ID == idOrderMobile);
            if (orderMobile1 != null)
            {
                orderMobile1.Status = "New";
            }
            Order order = context.Orders.FirstOrDefault(o => o.ID == orderMobile.OnePointForAddressOrders[orderMobile.OnePointForAddressOrders.Count - 1].IDorder);
            orderMobile.Status = "EndWork";
            orderMobile.OnePointForAddressOrders[0].Status = "CompletePoint";
            order.CurrentStatus = "Delivered";
            await context.SaveChangesAsync();
        }

        public List<Driver> CheckOrderForDriver()
        {
            File.WriteAllText("1.txt", "CheckOrderForDriver");
            context.Drivers.Load();
            List<Driver> drivers = null;
            List<Order> orders = context.Orders.ToList().Where(o => o.Driver != null && (o.CurrentStatus == "Assigned" || o.CurrentStatus == "Picked up")).ToList();
            if (orders == null || orders.Count == 0)
            {
                drivers = context.Drivers.Where(d => d.IsWork).ToList();
            }
            else
            {
                drivers = context.Drivers.ToList().Where(d => orders.FirstOrDefault(o => o.Driver.ID == d.ID) == null && d.IsWork).ToList();
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

        public async Task<BackgroundService.DriverManager.location> GetAddressToOrderDB(int idMobileOrder)
        {
            OrderMobile orderMobile = await context.OrderMobiles
                .Include(o => o.OnePointForAddressOrders)
                .FirstOrDefaultAsync(o => o.ID == idMobileOrder);
            OnePointForAddressOrder onePointForAddressOrder = orderMobile.OnePointForAddressOrders[orderMobile.OnePointForAddressOrders.Count - 1];
            return new ApiMobaileTaxi.BackgroundService.DriverManager.location(onePointForAddressOrder.Lat, onePointForAddressOrder.Lng)
            {
                ID = orderMobile.IdDriver,
                ApiniTime = onePointForAddressOrder.PTime,
                Date = onePointForAddressOrder.Date
            };
        }
         
        public List<Order> GetOrders()
        {
            return context.Orders.ToList().Where(o => o.CurrentStatus == "NewLoad" && (DateTime.Parse($"{GetDFormat(o.Date)} {o.TimeOfPickup}").AddMinutes(20) > DateTime.Now && DateTime.Now > DateTime.Parse($"{GetDFormat(o.Date)} {o.TimeOfPickup}").AddHours(-3))).ToList();
        }

        public List<Order> GetOrders(string date)
        {
            return context.Orders.ToList().Where(o => o.CurrentStatus == "NewLoad" && (DateTime.Parse($"{GetDFormat(o.Date)} {o.TimeOfPickup}").AddMinutes(20) < DateTime.Parse(GetDFormat(date)) && DateTime.Parse(GetDFormat(date)) > DateTime.Parse($"{GetDFormat(o.Date)} {o.TimeOfPickup}").AddHours(-3))).ToList();
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

        public async void SetAcceptVisableDb(int idDOrder)
        {
            Order order = context.Orders.FirstOrDefault(d => d.ID == idDOrder);
            order.IsVisableAccept = true;
            await context.SaveChangesAsync();
        }

        public async Task SetOrederMobile(OrderMobile orderMobile, string idDrigver, bool isNew)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.ID.ToString() == idDrigver);
            orderMobile.Status = isNew ? "New" : "NewNext";
            foreach (Order order in orderMobile.Orders)
            {
                Order order1 = context.Orders.FirstOrDefault(o => o.ID == order.ID);
                order.CurrentStatus = "Assigned";
                order1.Driver = driver;
            }
            context.OrderMobiles.Add(orderMobile);
                await context.SaveChangesAsync();
        }

        public OrderMobile GetOrderMobile(string token)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.Token == token);
            return context.OrderMobiles
                .Include(o => o.Orders)
                .Include(o => o.OnePointForAddressOrders)
                .FirstOrDefault(o => o.IdDriver == driver.ID.ToString() && o.Status == "New" || o.Status == "InWork");
        }
    }
}
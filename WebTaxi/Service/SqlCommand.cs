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
            orders = context.Orders.ToList().FindAll(o => o.CurrentStatus == status);

            if (page != 0)
            {
                try
                {
                    orders = orders.GetRange((20 * page) - 20, 20);
                }
                catch (Exception)
                {
                    orders = orders.GetRange((20 * page) - 20, orders.Count % 20);
                }
            }
            else
            {
                try
                {
                    orders = orders.GetRange(0, 20);
                }
                catch (Exception)
                {
                    orders = orders.GetRange(0, orders.Count % 20);
                }
            }
            return orders;
        }

        public bool CheckKeyDb(string key)
        {
            return context.Admins.FirstOrDefault(u => u.KeyAuthorized == key) != null;
        }

        public int GetCountPageInDb(string status)
        {
            int countPage = 0;
            List<Order> orders = context.Orders.ToList().FindAll(s => s.CurrentStatus == status);
            countPage = orders.Count / 20;
            int remainderPage = orders.Count % 20;
            countPage = remainderPage > 0 ? countPage + 1 : countPage;
            return countPage;
        }

        public async Task SaveOrder(Order order)
        {
            Order orderDb = await context.Orders.FirstOrDefaultAsync(o => o.Comment == order.Comment && o.Date == order.Date && o.FromAddress == order.FromAddress && o.Milisse == order.Milisse && o.NameCustomer == order.NameCustomer
            && o.NoName == order.NoName && o.NoName1 == order.NoName1 && o.NoName2 == order.NoName2 && o.NoName3 == order.NoName3 && o.NoName4 == order.NoName4 && o.NoName5 == order.NoName5 && o.NoName6 == order.NoName6
            && o.Phone == order.Phone && o.Price == order.Price && o.TimeOfAppointment == order.TimeOfAppointment && o.TimeOfPickup == order.TimeOfPickup && o.ToAddress == order.ToAddress);
            if(orderDb != null)
            {
                return;
            }
            await context.AddAsync(order);
            await context.SaveChangesAsync();
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
    }
}
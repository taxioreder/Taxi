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
    }
}
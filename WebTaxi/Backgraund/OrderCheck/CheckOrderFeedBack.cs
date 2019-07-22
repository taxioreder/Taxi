using DBAplication.Model;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using WebTaxi.Service;

namespace WebTaxi.Backgraund.OrderCheck
{
    public class CheckOrderFeedBack : IJob
    {
        private SqlCommand sqlCommand = null;

        public void Execute()
        {
            sqlCommand = new SqlCommand();
            Task.Run(() => Work());
        }

        private async void Work()
        {
            List<Order> orders = sqlCommand.GetFullOrders();
        }

        private async Task CheckSucceeding(List<Order> orders)
        {
            foreach (Order order in orders)
            {
                if (DateTime.Parse($"{GetDFormat(order.Date)} {order.TimeOfPickup}") < DateTime.Now)
                {
                    await sqlCommand.SetFedBack(order.ID, "This order is not given to anyone and is no longer valid!", false);
                }
                else if (DateTime.Parse($"{GetDFormat(order.Date)} {order.TimeOfPickup}") < DateTime.Now.AddMinutes(-20))
                {
                    await sqlCommand.SetFedBack(order.ID, "This order is not given to anyone, and after 20 minutes it will not be valid!", false);
                }
                else
                {
                    await sqlCommand.SetFedBack(order.ID, "OK", true);
                }
            }
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

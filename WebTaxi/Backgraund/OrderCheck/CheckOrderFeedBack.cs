using DBAplication.Model;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebTaxi.Service;

namespace WebTaxi.Backgraund.OrderCheck
{
    public class CheckOrderFeedBack : IJob
    {
        private SqlCommand sqlCommand = null;
        private ConnectorApiMaps connectorApiMaps = null;

        public void Execute()
        {
            sqlCommand = new SqlCommand();
            connectorApiMaps = new ConnectorApiMaps();
            Task.Run(() => Work());
        }

        private async void Work()
        {
            List<Order> orders = sqlCommand.GetFullOrders();
            await CheckSucceeding(orders.Where(o => o.CurrentStatus == "NewLoad").ToList());
        }

        private async Task CheckSucceeding(List<Order> orders)
        {
            foreach (Order order in orders)
            {
                if ((order.Date != null && order.Date == "") || order.Date == null)
                {
                    await sqlCommand.SetFedBack(order.ID, "Date is not filled!", false);
                }
                else if (!IsFormat(order.Date))
                {
                    await sqlCommand.SetFedBack(order.ID, "Date is not valid!", false);
                }
                else if (DateTime.Parse($"{GetDFormat(order.Date)} {order.TimeOfPickup}") < DateTime.Now)
                {
                    await sqlCommand.SetFedBack(order.ID, "This order is not given to anyone and is no longer valid!", false);
                }
                else if (DateTime.Parse($"{GetDFormat(order.Date)} {order.TimeOfPickup}") < DateTime.Now.AddMinutes(-20))
                {
                    await sqlCommand.SetFedBack(order.ID, "This order is not given to anyone, and after 20 minutes it will not be valid!", true);
                }
                else if((order.TimeOfPickup != null && order.TimeOfPickup == "") || order.TimeOfPickup == null)
                {
                    await sqlCommand.SetFedBack(order.ID, "Time Of Pickup is not filled!", false);
                }
                else if ((order.TimeOfAppointment != null && order.TimeOfAppointment == "") || order.TimeOfAppointment == null)
                {
                    await sqlCommand.SetFedBack(order.ID, "Time Of Appointment is not filled!", false);
                }
                else if((order.FromAddress != null && order.FromAddress == "") || order.FromAddress == null)
                {
                    await sqlCommand.SetFedBack(order.ID, "From Address is not filled!", false);
                }
                else if (!connectorApiMaps.IsValidAddress(order.FromAddress))
                {
                    await sqlCommand.SetFedBack(order.ID, "From Address is not valid!", false);
                }
                else if ((order.ToAddress != null && order.ToAddress == "") || order.ToAddress == null)
                {
                    await sqlCommand.SetFedBack(order.ID, "To Address is not filled!", false);
                }
                else if (!connectorApiMaps.IsValidAddress(order.ToAddress))
                {
                    await sqlCommand.SetFedBack(order.ID, "To Address is not valid!", false);
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

        private bool IsFormat(string data)
        {
            bool isDate = false;
            DateTime dateTime;
            if (DateTime.TryParseExact(data, "MM.dd.yyyy", null, DateTimeStyles.None, out dateTime))
            {
                isDate = true;
            }
            else if (DateTime.TryParseExact(data, "dd.MM.yyyy", null, DateTimeStyles.None, out dateTime))
            {
                isDate = true;
            }
            else if (DateTime.TryParseExact(data, "yyyy.MM.dd", null, DateTimeStyles.None, out dateTime))
            {
                isDate = true;
            }
            else if (DateTime.TryParseExact(data, "MM-dd-yyyy", null, DateTimeStyles.None, out dateTime))
            {
                isDate = true;
            }
            else if (DateTime.TryParseExact(data, "dd-MM-yyyy", null, DateTimeStyles.None, out dateTime))
            {
                isDate = true;
            }
            else if (DateTime.TryParseExact(data, "yyyy-MM-dd", null, DateTimeStyles.None, out dateTime))
            {
                isDate = true;
            }
            else if (DateTime.TryParseExact(data, "MM/dd/yyyy", null, DateTimeStyles.None, out dateTime))
            {
                isDate = true;
            }
            else if (DateTime.TryParseExact(data, "dd/MM/yyyy", null, DateTimeStyles.None, out dateTime))
            {
                isDate = true;
            }
            else if (DateTime.TryParseExact(data, "yyyy/MM/dd", null, DateTimeStyles.None, out dateTime))
            {
                isDate = true;
            }
            return isDate;
        }
    }
}

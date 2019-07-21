using DBAplication.Model;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace WebTaxi.Service
{
    public class OrderMobileWorke
    {
        public SqlCommand sqlCommand = null;
        private ConnectorApiMaps connectorApiMaps = null;

        public OrderMobileWorke(SqlCommand sqlCommand)
        {
            this.sqlCommand = sqlCommand;
            connectorApiMaps= new ConnectorApiMaps();
        }

        public List<Order> SuitableOrders(string idDriver)
        {
            List<Model.Location> locationsOrder = new List<Model.Location>();
            Model.Location locationsDriver = null;
            Driver driver = sqlCommand.GetDriver(idDriver);
            List<Order> orders = sqlCommand.GetOrders();
            Model.Location locationDriveZip = connectorApiMaps.GetGetLonAndLanToAddress(driver.ZipCod.ToString());
            if (locationDriveZip != null)
            {
                locationDriveZip.ID = driver.ID.ToString();
                locationsDriver = locationDriveZip;
            }
            foreach (var order in orders)
            {
                Model.Location locationOrder = connectorApiMaps.GetGetLonAndLanToAddress(order.FromAddress.ToString());
                if (locationOrder != null)
                {
                    locationOrder.ID = order.ID.ToString();
                    locationOrder.Date = order.Date;
                    locationOrder.PickuoTime = order.TimeOfPickup;
                    locationOrder.ApiniTime = order.TimeOfAppointment;
                    locationOrder.CountCusstomer = order.CountCustomer;
                    locationsOrder.Add(locationOrder);
                }
            }
            if((locationsOrder != null && locationsOrder.Count != 0) && locationsDriver != null)
            {
                return GetSuitableOrders(locationsOrder, locationsDriver, orders);
            }
            return new List<Order>();
        }

        private List<Order> GetSuitableOrders(List<Model.Location> locationsOrder, Model.Location locationDriver, List<Order> orders)
        {
            List<Order> orders1 = new List<Order>();
            foreach(Model.Location locationOrder in locationsOrder)
            {
                if(DistanceTo(locationDriver.lat, locationDriver.lng, locationsOrder[0].lat, locationsOrder[0].lng) <= 2)
                {
                    int duration = connectorApiMaps.GetGetDuration($"{ConvertTOString(locationDriver.lat)},{ConvertTOString(locationDriver.lng)}", $"{ConvertTOString(locationOrder.lat)},{ConvertTOString(locationOrder.lng)}");
                    if(DateTime.Now.AddSeconds(duration) < DateTime.Parse($"{GetDFormat(locationOrder.Date)} {locationOrder.PickuoTime}"))
                    {
                        orders1.Add(orders.Find(o => o.ID.ToString() == locationOrder.ID));
                    }
                }
            }
            return orders1;
        }

        private double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'M')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }
            return dist;
        }

        private string ConvertTOString(double value)
        {
            return Convert.ToString(value).Replace(',', '.');
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

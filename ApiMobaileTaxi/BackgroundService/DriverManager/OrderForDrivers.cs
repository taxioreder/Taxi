using ApiMobaileTaxi.BackgroundService.Model;
using ApiMobaileTaxi.Service;
using DBAplication.Model;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ApiMobaileTaxi.BackgroundService.DriverManager
{
    public class OrderForDrivers : IJob
    {
        SqlCoommandTaxiApi sqlCoommandTaxiApi = null;
        ConnectorApiMaps connectorApiMaps = null;

        public void Execute()
        {
            sqlCoommandTaxiApi = new SqlCoommandTaxiApi();
            connectorApiMaps = new ConnectorApiMaps();
            Task.Run(() => SetOrderForDrivers());
        }

        private async void SetOrderForDrivers()
        {
            int iteration = 0;
            List<location> locationsDriver = null;
            List<location> locationsOrder = null;
            List<Driver> drivers = sqlCoommandTaxiApi.CheckOrderForDriver();
            List<Order> orders = sqlCoommandTaxiApi.GetOrders();
            locationsDriver = new List<location>();
            locationsOrder = new List<location>();
            foreach (var driver in drivers)
            {
                location locationDriveZip = connectorApiMaps.GetGetLonAndLanToAddress(driver.ZipCod.ToString());
                if (locationDriveZip != null)
                {
                    locationDriveZip.ID = driver.ID.ToString();
                    locationsDriver.Add(locationDriveZip);
                }
            }
            foreach (var order in orders)
            {
                location locationOrder = connectorApiMaps.GetGetLonAndLanToAddress(order.FromAddress.ToString());
                if (locationOrder != null)
                {
                    locationOrder.ID = order.ID.ToString();
                    locationOrder.Date = order.Date;
                    locationOrder.PickuoTime = order.TimeOfPickup;
                    locationOrder.ApiniTime = order.TimeOfAppointment;
                    locationsOrder.Add(locationOrder);
                }
            }
            iteration = locationsDriver.Count;
            for (int i = 0; i < iteration; i++)
            {
                if (locationsOrder.Count == 0)
                {
                    break;
                }
                List<location> locationsAcceptOrder = SerchMinDistance(locationsDriver, locationsOrder);
                Order order = orders.Find(o => o.ID.ToString() == locationsAcceptOrder[1].ID);
                location locationEndLoc = connectorApiMaps.GetGetLonAndLanToAddress(order.ToAddress);
                locationEndLoc.ApiniTime = order.TimeOfAppointment;
                locationEndLoc.Date = order.Date;
                locationEndLoc.PickuoTime = order.TimeOfPickup;
                order = null;
                locationsOrder.Remove(locationsAcceptOrder[1]);
                locationsDriver.Remove(locationsAcceptOrder[0]);
                OrderOnTheWay(locationsOrder, locationsAcceptOrder, locationEndLoc, orders);
            }
        }

        private void OrderOnTheWay(List<location> locationsOrder, List<location> locationsAcceptOrder, location locationEndLoc, List<Order> orders, OrderMobile orderMobile = null, List<OnePointForAddressOrder> onePointForAddressOrders = null)
        {
            orderMobile = new OrderMobile();
            onePointForAddressOrders = new List<OnePointForAddressOrder>();
            List<Steps> steps = connectorApiMaps.GetGetDirections($"{locationsAcceptOrder[1].lat.ToString()},{locationsAcceptOrder[1].lng.ToString()}", $"{locationEndLoc.lat.ToString()},{locationEndLoc.lng.ToString()}");
            if (orderMobile.Orders == null)
            {
                orderMobile.Orders = new List<Order>();
            }
            foreach (location location in locationsOrder)
            {
                bool isOnTheWayStart = false;
                bool isOnTheWayEnd = false;
                double lat = 0;
                double lng = 0;
                Order order = orders.Find(o => o.ID.ToString() == location.ID);
                location locationEndCurent = connectorApiMaps.GetGetLonAndLanToAddress(order.ToAddress);
                locationEndCurent.ApiniTime = order.TimeOfAppointment;
                locationEndCurent.Date = order.Date;
                locationEndCurent.PickuoTime = order.TimeOfPickup;
                order = null;
                double latF = Convert.ToDouble(location.lat.Replace('.', ','));
                double lngF = Convert.ToDouble(location.lng.Replace('.', ','));
                foreach (Steps step in steps)
                {
                    double tmpLoc = Convert.ToDouble(step.end_location.lat.Replace('.', ',')) - Convert.ToDouble(step.start_location.lat.Replace('.', ','));
                    lat = Convert.ToDouble(step.start_location.lat.Replace('.', ',')) + tmpLoc;
                    tmpLoc = Convert.ToDouble(step.end_location.lng.Replace('.', ',')) - Convert.ToDouble(step.start_location.lng.Replace('.', ','));
                    lng = Convert.ToDouble(step.start_location.lng.Replace('.', ',')) + tmpLoc;
                    if ((latF - 0.013 < lat && lat + 0.013 > latF)
                    && (lngF - 0.013 < lng && lng + 0.013 > lngF) && !isOnTheWayStart)
                    {
                        int durationSO1ToSO2 = connectorApiMaps.GetGetDuration($"{location.lat.ToString()},{location.lng.ToString()}", $"{locationsAcceptOrder[1].lat.ToString()},{locationsAcceptOrder[1].lng.ToString()}");
                        if(DateTime.Parse($"{GetDFormat(locationsAcceptOrder[1].Date)} {locationsAcceptOrder[1].PickuoTime}").AddSeconds(durationSO1ToSO2).AddMinutes(-10) < DateTime.Parse($"{GetDFormat(location.Date)} {location.PickuoTime}") 
                            && DateTime.Parse($"{GetDFormat(location.Date)} {location.PickuoTime}") < DateTime.Parse($"{GetDFormat(locationsAcceptOrder[1].Date)} {locationsAcceptOrder[1].PickuoTime}").AddSeconds(durationSO1ToSO2).AddMinutes(10))
                        {
                            isOnTheWayStart = true;
                        }
                    }
                    else if ((Convert.ToDouble(locationEndCurent.lat.Replace('.', ',')) - 0.013 < lat && lat + 0.013 > Convert.ToDouble(locationEndCurent.lat.Replace('.', ',')))
                   && (Convert.ToDouble(locationEndCurent.lng.Replace('.', ',')) - 0.013 < lng && lng + 0.013 > Convert.ToDouble(locationEndCurent.lng.Replace('.', ','))) && isOnTheWayStart)
                    {
                        isOnTheWayEnd = true;
                    }
                }
                if(isOnTheWayStart && isOnTheWayEnd)
                {
                    int durationEnd = connectorApiMaps.GetGetDuration($"{locationEndLoc.lat.ToString()},{locationEndLoc.lng.ToString()}", $"{locationEndCurent.lat.ToString()},{locationEndCurent.lng.ToString()}");
                    DateTime dateTime1 =  locationEndCurent.ApiniTime == null ? DateTime.Parse($"{GetDFormat(locationEndCurent.Date)} {locationEndCurent.PickuoTime}").AddMinutes(90) : DateTime.Parse($"{locationEndCurent.Date} {locationEndCurent.ApiniTime}");
                    DateTime dateTime2 =  locationEndLoc.ApiniTime == null ? DateTime.Parse($"{GetDFormat(locationEndLoc.Date)} {locationEndLoc.PickuoTime}").AddMinutes(90) : DateTime.Parse($"{locationEndLoc.Date} {locationEndLoc.ApiniTime}");
                    if (dateTime1.AddSeconds(durationEnd) < dateTime2)
                    {
                        orderMobile.IdDriver = locationsAcceptOrder[0].ID;
                        orderMobile.Orders.Add(orders.Find(o => o.ID.ToString() == location.ID));
                        orderMobile.Orders.Add(orders.Find(o => o.ID.ToString() == locationsAcceptOrder[1].ID));
                        break;
                    }
                }
                else if(isOnTheWayStart)
                {
                    int durationEnd = connectorApiMaps.GetGetDuration($"{locationEndCurent.lat.ToString()},{locationEndCurent.lng.ToString()}", $"{locationEndLoc.lat.ToString()},{locationEndLoc.lng.ToString()}");
                    DateTime dateTime1 = locationEndCurent.ApiniTime == null ? DateTime.Parse($"{GetDFormat(locationEndCurent.Date)} {locationEndCurent.PickuoTime}").AddMinutes(90) : DateTime.Parse($"{locationEndCurent.Date} {locationEndCurent.ApiniTime}");
                    DateTime dateTime2 = locationEndLoc.ApiniTime == null ? DateTime.Parse($"{GetDFormat(locationEndLoc.Date)} {locationEndLoc.PickuoTime}").AddMinutes(90) : DateTime.Parse($"{locationEndLoc.Date} {locationEndLoc.ApiniTime}");
                    if (dateTime1 > dateTime2.AddSeconds(durationEnd))
                    {
                        orderMobile.IdDriver = locationsAcceptOrder[0].ID;
                        orderMobile.Orders.Add(orders.Find(o => o.ID.ToString() == location.ID));
                        orderMobile.Orders.Add(orders.Find(o => o.ID.ToString() == locationsAcceptOrder[1].ID));
                        break;
                    }
                }
            }
            if(orderMobile.Orders.Count == 0)
            {
                orderMobile.IdDriver = locationsAcceptOrder[0].ID;
                orderMobile.Orders.Add(orders.Find(o => o.ID.ToString() == locationsAcceptOrder[1].ID));
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


        private int FullDuration(location locationDriver, location locationOrder, List<location> locationsOrder = null)
        {
            int timeDuration = 0;
            if (locationsOrder == null)
            {
                 timeDuration = connectorApiMaps.GetGetDuration($"{locationDriver.lat.ToString()},{locationDriver.lng.ToString()}", $"{locationOrder.lat.ToString()},{locationOrder.lng.ToString()}");
            }
            else
            {
                timeDuration += connectorApiMaps.GetGetDuration($"{locationsOrder[0].lat.ToString()},{locationsOrder[0].lng.ToString()}", $"{locationsOrder[1].lat.ToString()},{locationsOrder[1].lng.ToString()}");
                timeDuration += connectorApiMaps.GetGetDuration($"{locationsOrder[1].lat.ToString()},{locationsOrder[1].lng.ToString()}", $"{locationsOrder[2].lat.ToString()},{locationsOrder[2].lng.ToString()}");
            }
            return timeDuration;
        }

        private List<location> SerchMinDistance(List<location> locationsDriver, List<location> locationsOrder)
        {
            List<location> locations = new List<location>();
            location tmpOrder = locationsOrder[0];
            location tmpDriver = locationsDriver[0];
            double distance = DistanceTo(Convert.ToDouble(locationsDriver[0].lat.Replace('.', ',')), Convert.ToDouble(locationsDriver[0].lng.Replace('.', ',')), Convert.ToDouble(locationsOrder[0].lat.Replace('.', ',')), Convert.ToDouble(locationsOrder[0].lng.Replace('.', ',')));
            foreach (var locationDriver in locationsDriver)
            {
                foreach (var locationOrder in locationsOrder)
                {
                    double tempDistance = DistanceTo(Convert.ToDouble(locationDriver.lat.Replace('.', ',')), Convert.ToDouble(locationDriver.lng.Replace('.', ',')), Convert.ToDouble(locationOrder.lat.Replace('.', ',')), Convert.ToDouble(locationOrder.lng.Replace('.', ',')));
                    if (tempDistance < distance)
                    {
                        int timeDuration = FullDuration(locationOrder, locationDriver);
                        if (DateTime.Now.AddSeconds(timeDuration) < DateTime.Parse($"{GetDFormat(locationOrder.Date)} {locationOrder.PickuoTime}").AddMinutes(20))
                        {
                            distance = tempDistance;
                            tmpOrder = locationOrder;
                            tmpDriver = locationDriver;
                        }
                    }
                }
            }
            locations.Add(tmpDriver);
            locations.Add(tmpOrder);

            return locations;
        }

        private double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
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
    }
}
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
                location location1 = connectorApiMaps.GetGetLonAndLanToAddress(order.ToAddress.ToString());
                if (locationOrder != null)
                {
                    locationOrder.ID = order.ID.ToString();
                    locationOrder.Date = order.Date;
                    locationOrder.PickuoTime = order.TimeOfPickup;
                    locationOrder.ApiniTime = order.TimeOfAppointment;
                    locationOrder.latE = location1.lat;
                    locationOrder.lngE = location1.lng;
                    locationOrder.CountCusstomer = order.CountCustomer;
                    locationsOrder.Add(locationOrder);
                }
            }
            if ((drivers != null && drivers.Count != 0) && (orders != null && orders.Count != 0))
            {
                iteration = locationsDriver.Count;
                for (int i = 0; i < iteration; i++)
                {
                    if (locationsOrder.Count == 0)
                    {
                        break;
                    }
                    List<location> locationsAcceptOrder = SerchMinDistance(locationsDriver, locationsOrder);
                    locationsOrder.Remove(locationsAcceptOrder[1]);
                    locationsDriver.Remove(locationsAcceptOrder[0]);
                    await OrderOnTheWay(locationsOrder, locationsAcceptOrder, orders);
                }
            }
        }

        public async Task OrderOnTheWay(List<location> locationsOrder, List<location> locationsAcceptOrder, List<Order> orders, bool isNew = true, OrderMobile orderMobile = null)
        {
            int numberOfSeats = 4;
            orderMobile = new OrderMobile();
            orderMobile.OnePointForAddressOrders = new List<OnePointForAddressOrder>();
            orderMobile.Status = "New";
            Order order = orders.Find(o => o.ID.ToString() == locationsAcceptOrder[1].ID);
            orderMobile.OnePointForAddressOrders.Add(new OnePointForAddressOrder(order.ID, Convert.ToDouble(locationsAcceptOrder[1].lat.Replace('.', ',')), Convert.ToDouble(locationsAcceptOrder[1].lng.Replace('.', ',')), order.TimeOfPickup, order.Date, "Start", order.FromAddress));
            orderMobile.OnePointForAddressOrders.Add(new OnePointForAddressOrder(order.ID, Convert.ToDouble(locationsAcceptOrder[1].latE.Replace('.', ',')), Convert.ToDouble(locationsAcceptOrder[1].lngE.Replace('.', ',')), order.TimeOfAppointment, order.Date, "End", order.ToAddress));
            orderMobile.Orders = new List<Order>();
            orderMobile.IdDriver = locationsAcceptOrder[0].ID;
            orderMobile.Orders.Add(order);
            numberOfSeats -= orders.Find(o => o.ID.ToString() == locationsAcceptOrder[1].ID).CountCustomer;
            List<Steps> steps = connectorApiMaps.GetGetDirections($"{locationsAcceptOrder[1].lat.ToString()},{locationsAcceptOrder[1].lng.ToString()}", $"{locationsAcceptOrder[1].latE.ToString()},{locationsAcceptOrder[1].lngE.ToString()}");
            if (orderMobile.Orders == null)
            {
                orderMobile.Orders = new List<Order>();
            }
            if (numberOfSeats > 0)
            {
                for (int i = 0; i < locationsOrder.Count; i++)
                {
                    bool isAddOrder = true;
                    int positionS = 0;
                    int positionE = 0;
                    if (locationsOrder[i].CountCusstomer > numberOfSeats)
                    {
                        continue;
                    }
                    bool isOnTheWayStart = false;
                    double lat = 0;
                    double lng = 0;
                    double latF = Convert.ToDouble(locationsOrder[i].lat.Replace('.', ','));
                    double lngF = Convert.ToDouble(locationsOrder[i].lng.Replace('.', ','));
                    foreach (Steps step in steps)
                    {
                        if (locationsOrder[i].CountCusstomer > numberOfSeats)
                        {
                            break;
                        }
                        List<OnePointForAddressOrder> onePointForAddressOrders = new List<OnePointForAddressOrder>();
                        onePointForAddressOrders.AddRange(orderMobile.OnePointForAddressOrders);
                        double tmpLoc = Convert.ToDouble(step.end_location.lat.Replace('.', ',')) - Convert.ToDouble(step.start_location.lat.Replace('.', ','));
                        lat = Convert.ToDouble(step.start_location.lat.Replace('.', ',')) + tmpLoc;
                        tmpLoc = Convert.ToDouble(step.end_location.lng.Replace('.', ',')) - Convert.ToDouble(step.start_location.lng.Replace('.', ','));
                        lng = Convert.ToDouble(step.start_location.lng.Replace('.', ',')) + tmpLoc;
                        if ((latF - 0.013 < lat && lat + 0.013 > latF)
                        && (lngF - 0.013 < lng && lng + 0.013 > lngF) && !isOnTheWayStart)
                        {
                            location locationNewS = new location(locationsOrder[i].lat, locationsOrder[i].lng);
                            location locationNewE = new location(locationsOrder[i].latE, locationsOrder[i].lngE);
                            GetPositionLocation(onePointForAddressOrders, locationNewS, locationsAcceptOrder[0], ref positionS);
                            Order order1 = orders.Find(o => o.ID.ToString() == locationsOrder[i].ID);
                            onePointForAddressOrders.Insert(positionS, new OnePointForAddressOrder(order1.ID, Convert.ToDouble(locationsOrder[i].lat.Replace('.', ',')), Convert.ToDouble(locationsOrder[i].lng.Replace('.', ',')), order1.TimeOfPickup, order1.Date, "Start", order1.FromAddress));
                            if (GetEndOrderOnTheWay(steps.GetRange(steps.IndexOf(step), (steps.Count - 1) - steps.IndexOf(step)), locationsOrder[i]))
                            {
                                GetPositionLocation(onePointForAddressOrders, locationNewE, locationsAcceptOrder[0], ref positionE);
                                onePointForAddressOrders.Insert(positionE, new OnePointForAddressOrder(order1.ID, Convert.ToDouble(locationsOrder[i].latE.Replace('.', ',')), Convert.ToDouble(locationsOrder[i].lngE.Replace('.', ',')), order1.TimeOfAppointment, order1.Date, "End", order1.ToAddress));
                            }
                            else
                            {
                                onePointForAddressOrders.Add(new OnePointForAddressOrder(order1.ID, Convert.ToDouble(locationsOrder[i].latE.Replace('.', ',')), Convert.ToDouble(locationsOrder[i].lngE.Replace('.', ',')), order1.TimeOfAppointment, order1.Date, "End", order1.ToAddress));
                            }
                            int duration = connectorApiMaps.GetGetDuration($"{locationsAcceptOrder[0].lat},{locationsAcceptOrder[0].lng}", $"{onePointForAddressOrders[0].Lat.ToString().Replace(',', '.')},{onePointForAddressOrders[0].Lng.ToString().Replace(',', '.')}");
                            if (DateTime.Now.AddSeconds(duration).AddMinutes(-10) < DateTime.Parse($"{GetDFormat(onePointForAddressOrders[0].Date)} {onePointForAddressOrders[0].PTime}"))
                            {
                                for (int j = 1; j < onePointForAddressOrders.Count; j++)
                                {
                                    DateTime dateTime = new DateTime();
                                    DateTime dateTime1 = new DateTime();
                                    int duration1 = connectorApiMaps.GetGetDuration($"{onePointForAddressOrders[j - 1].Lat.ToString().Replace(',', '.')},{onePointForAddressOrders[j - 1].Lng.ToString().Replace(',', '.')}", $"{onePointForAddressOrders[j].Lat.ToString().Replace(',', '.')},{onePointForAddressOrders[j].Lng.ToString().Replace(',', '.')}");
                                    if (onePointForAddressOrders[j - 1].Type == "Start")
                                    {
                                        dateTime = DateTime.Parse($"{GetDFormat(onePointForAddressOrders[j - 1].Date)} {onePointForAddressOrders[j - 1].PTime}");
                                    }
                                    else if (onePointForAddressOrders[j - 1].Type == "End")
                                    {
                                        var date = orders.Find(o => o.ID == onePointForAddressOrders[j - 1].IDorder);
                                        string date1 = date.TimeOfAppointment != null ? date.TimeOfAppointment : DateTime.Parse($"{GetDFormat(date.Date)} {date.TimeOfPickup}").AddMinutes(90).ToShortTimeString();
                                        dateTime = DateTime.Parse($"{GetDFormat(onePointForAddressOrders[j - 1].Date)} {date1}");
                                        onePointForAddressOrders[j - 1].PTime = DateTime.Parse(date.TimeOfPickup).AddMinutes(90).ToShortTimeString();
                                    }
                                    if (onePointForAddressOrders[j].Type == "Start")
                                    {
                                        dateTime1 = DateTime.Parse($"{GetDFormat(onePointForAddressOrders[j].Date)} {onePointForAddressOrders[j].PTime}");
                                    }
                                    else if (onePointForAddressOrders[j].Type == "End")
                                    {
                                        var date = orders.Find(o => o.ID == onePointForAddressOrders[j].IDorder);
                                        string date1 = date.TimeOfAppointment != null ? date.TimeOfAppointment : DateTime.Parse($"{GetDFormat(date.Date)} {date.TimeOfPickup}").AddMinutes(90).ToShortTimeString();
                                        dateTime1 = DateTime.Parse($"{GetDFormat(onePointForAddressOrders[j].Date)} {date1}");
                                        onePointForAddressOrders[j].PTime = DateTime.Parse(date.TimeOfPickup).AddMinutes(90).ToShortTimeString();
                                    }
                                    if (onePointForAddressOrders[j].Type == "Start"
                                        && !(dateTime.AddSeconds(duration1) < dateTime1.AddMinutes(10)
                                        && dateTime1.AddMinutes(-10) < dateTime.AddSeconds(duration1)))
                                    {
                                        isAddOrder = false;
                                        break;
                                    }
                                    else if (onePointForAddressOrders[j].Type == "End"
                                        && !(dateTime.AddSeconds(duration1).AddMinutes(-90) < dateTime1 && dateTime1 < dateTime.AddSeconds(duration1).AddMinutes(95)))
                                    {
                                        isAddOrder = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    if (isAddOrder)
                    {
                        Order order1 = orders.Find(o => o.ID.ToString() == locationsOrder[i].ID);
                        if (order1.CountCustomer <= numberOfSeats)
                        {
                            orderMobile.OnePointForAddressOrders.Insert(positionS, new OnePointForAddressOrder(order1.ID, Convert.ToDouble(locationsOrder[i].lat.Replace('.', ',')), Convert.ToDouble(locationsOrder[i].lng.Replace('.', ',')), order1.TimeOfPickup, order1.Date, "Start", order1.FromAddress));
                            orderMobile.OnePointForAddressOrders.Insert(positionE, new OnePointForAddressOrder(order1.ID, Convert.ToDouble(locationsOrder[i].latE.Replace('.', ',')), Convert.ToDouble(locationsOrder[i].lngE.Replace('.', ',')), order1.TimeOfAppointment, order1.Date, "End", order1.ToAddress));
                            orderMobile.Orders.Add(order1);
                            locationsOrder.Remove(locationsOrder.Find(l => l.ID == order1.ID.ToString()));
                            numberOfSeats -= order1.CountCustomer;
                        }
                    }
                }
                sqlCoommandTaxiApi.SetOrederMobile(orderMobile, locationsAcceptOrder[0].ID, isNew);
            }
        }

        private bool GetEndOrderOnTheWay(List<Steps> steps, location location)
        {
            bool isEndOrderOnTheWay = false;
            double lat = 0;
            double lng = 0;
            foreach (Steps step in steps)
            {
                double tmpLoc = Convert.ToDouble(step.end_location.lat.Replace('.', ',')) - Convert.ToDouble(step.start_location.lat.Replace('.', ','));
                lat = Convert.ToDouble(step.start_location.lat.Replace('.', ',')) + tmpLoc;
                tmpLoc = Convert.ToDouble(step.end_location.lng.Replace('.', ',')) - Convert.ToDouble(step.start_location.lng.Replace('.', ','));
                lng = Convert.ToDouble(step.start_location.lng.Replace('.', ',')) + tmpLoc;
                if ((Convert.ToDouble(location.latE.Replace('.', ',')) - 0.013 < lat && lat + 0.013 > Convert.ToDouble(location.latE.Replace('.', ',')))
                       && (Convert.ToDouble(location.lngE.Replace('.', ',')) - 0.013 < lng && lng + 0.013 > Convert.ToDouble(location.lngE.Replace('.', ','))))
                {
                    isEndOrderOnTheWay = true;
                    break;
                }
            }
            return isEndOrderOnTheWay;
        }

        private void GetPositionLocation(List<OnePointForAddressOrder> onePointForAddressOrders, location locationNewOrder, location locationDriver, ref int positon)
        {
            positon = 0;
            int durationNewOrder = connectorApiMaps.GetGetDuration($"{locationDriver.lat.ToString()},{locationDriver.lng.ToString()}", $"{locationNewOrder.lat.ToString()},{locationNewOrder.lng.ToString()}");
            List<double> durations = new List<double>(); 
            foreach (OnePointForAddressOrder onePointForAddressOrder in onePointForAddressOrders)
            {
                int duration = connectorApiMaps.GetGetDuration($"{locationDriver.lat},{locationDriver.lng}", $"{onePointForAddressOrder.Lat.ToString().Replace(',', '.')},{onePointForAddressOrder.Lng.ToString().Replace(',', '.')}");
                if (duration < durationNewOrder)
                {
                    positon++;
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


        private int FullDuration(location locationDriver, location locationOrder, List<location> locationsOrder = null)
        {
            int timeDuration = 0;
            if (locationsOrder == null)
            {
                 timeDuration = connectorApiMaps.GetGetDuration($"{locationDriver.lat.ToString()},{locationDriver.lng.ToString()}", $"{locationOrder.lat.ToString()},{locationOrder.lng.ToString()}");
            }
            else
            {
                for(int i = 0; i <= locationsOrder.Count; i++)
                {
                    timeDuration += connectorApiMaps.GetGetDuration($"{locationsOrder[i].lat.ToString()},{locationsOrder[i].lng.ToString()}", $"{locationsOrder[i+1].lat.ToString()},{locationsOrder[i+1].lng.ToString()}");
                }
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
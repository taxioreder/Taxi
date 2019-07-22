using ApiMobaileTaxi.BackgroundService.Model;
using ApiMobaileTaxi.Service;
using DBAplication.Model;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
                    OrderOnTheWay(locationsOrder, locationsAcceptOrder, orders, sqlCoommandTaxiApi);
                }
            }
        }

        public async void OrderOnTheWay(List<location> locationsOrder, List<location> locationsAcceptOrder, List<Order> orders, SqlCoommandTaxiApi sqlCoommandTaxiApi, bool isNew = true, OrderMobile orderMobile = null)
        {
            if (connectorApiMaps == null)
            {
                connectorApiMaps = new ConnectorApiMaps();
                this.sqlCoommandTaxiApi = sqlCoommandTaxiApi;
            }
            int numberOfSeats = 4;
            orderMobile = new OrderMobile();
            orderMobile.OnePointForAddressOrders = new List<OnePointForAddressOrder>();
            orderMobile.Status = "New";
            Order order = orders.Find(o => o.ID.ToString() == locationsAcceptOrder[1].ID);
            orderMobile.OnePointForAddressOrders.Add(new OnePointForAddressOrder(order.ID, locationsAcceptOrder[1].lat, locationsAcceptOrder[1].lng, order.TimeOfPickup, order.Date, "Start", order.FromAddress));
            orderMobile.OnePointForAddressOrders.Add(new OnePointForAddressOrder(order.ID, locationsAcceptOrder[1].latE, locationsAcceptOrder[1].lngE, order.TimeOfAppointment, order.Date, "End", order.ToAddress));
            orderMobile.Orders = new List<Order>();
            orderMobile.IdDriver = locationsAcceptOrder[0].ID;
            orderMobile.Orders.Add(order);
            numberOfSeats -= orders.Find(o => o.ID.ToString() == locationsAcceptOrder[1].ID).CountCustomer;
            List<Steps> steps = connectorApiMaps.GetGetDirections($"{ConvertTOString(locationsAcceptOrder[1].lat)},{ConvertTOString(locationsAcceptOrder[1].lng)}", $"{ConvertTOString(locationsAcceptOrder[1].latE)},{ConvertTOString(locationsAcceptOrder[1].lngE)}");
            if (orderMobile.Orders == null)
            {
                orderMobile.Orders = new List<Order>();
            }
            if (numberOfSeats > 0)
            {
                for (int i = 0; i < locationsOrder.Count; i++)
                {
                    bool isAddOrder = true;
                    bool isOnTheWay = false;
                    int positionS = 0;
                    int positionE = 0;
                    if (locationsOrder[i].CountCusstomer > numberOfSeats)
                    {
                        continue;
                    }
                    bool isOnTheWayStart = false;
                    double lat = 0;
                    double lng = 0;
                    double latF = locationsOrder[i].lat;
                    double lngF = locationsOrder[i].lng;
                    foreach (Steps step in steps)
                    {
                        if (locationsOrder[i].CountCusstomer > numberOfSeats)
                        {
                            break;
                        }
                        List<OnePointForAddressOrder> onePointForAddressOrders = new List<OnePointForAddressOrder>();
                        onePointForAddressOrders.AddRange(orderMobile.OnePointForAddressOrders);
                        double tmpLoc = Convert.ToDouble(step.end_location.lat - step.start_location.lat);
                        lat = step.start_location.lat + tmpLoc;
                        tmpLoc = step.end_location.lng - step.start_location.lng;
                        lng = step.start_location.lng + tmpLoc;
                        if ((latF - 0.013 < lat && lat + 0.013 > latF)
                        && (lngF - 0.013 < lng && lng + 0.013 > lngF) && !isOnTheWayStart)
                        {
                            location locationNewS = new location(locationsOrder[i].lat, locationsOrder[i].lng);
                            location locationNewE = new location(locationsOrder[i].latE, locationsOrder[i].lngE);
                            GetPositionLocation(onePointForAddressOrders, locationNewS, locationsAcceptOrder[0], ref positionS);
                            Order order1 = orders.Find(o => o.ID.ToString() == locationsOrder[i].ID);
                            onePointForAddressOrders.Insert(positionS, new OnePointForAddressOrder(order1.ID, locationsOrder[i].lat, locationsOrder[i].lng, order1.TimeOfPickup, order1.Date, "Start", order1.FromAddress));
                            if (GetEndOrderOnTheWay(steps.GetRange(steps.IndexOf(step), (steps.Count - 1) - steps.IndexOf(step)), locationsOrder[i]))
                            {
                                GetPositionLocation(onePointForAddressOrders, locationNewE, locationsAcceptOrder[0], ref positionE);
                                onePointForAddressOrders.Insert(positionE, new OnePointForAddressOrder(order1.ID, locationsOrder[i].latE, locationsOrder[i].lngE, order1.TimeOfAppointment, order1.Date, "End", order1.ToAddress));
                            }
                            else
                            {
                                onePointForAddressOrders.Add(new OnePointForAddressOrder(order1.ID, locationsOrder[i].latE, locationsOrder[i].lngE, order1.TimeOfAppointment, order1.Date, "End", order1.ToAddress));
                                isOnTheWay = true;
                            }
                            int duration = connectorApiMaps.GetGetDuration($"{ConvertTOString(locationsAcceptOrder[0].lat)},{ConvertTOString(locationsAcceptOrder[0].lng)}", $"{ConvertTOString(onePointForAddressOrders[0].Lat)},{ConvertTOString(onePointForAddressOrders[0].Lng)}");
                            if (DateTime.Now.AddSeconds(duration).AddMinutes(-20) < DateTime.Parse($"{GetDFormat(onePointForAddressOrders[0].Date)} {onePointForAddressOrders[0].PTime}"))
                            {
                                for (int j = 1; j < onePointForAddressOrders.Count; j++)
                                {
                                    DateTime dateTime = new DateTime();
                                    DateTime dateTime1 = new DateTime();
                                    int duration1 = connectorApiMaps.GetGetDuration($"{ConvertTOString(onePointForAddressOrders[j - 1].Lat)},{ConvertTOString(onePointForAddressOrders[j - 1].Lng)}", $"{ConvertTOString(onePointForAddressOrders[j].Lat)},{ConvertTOString(onePointForAddressOrders[j].Lng)}");
                                    if (onePointForAddressOrders[j - 1].Type == "Start")
                                    {
                                        dateTime = DateTime.Parse($"{GetDFormat(onePointForAddressOrders[j - 1].Date)} {onePointForAddressOrders[j - 1].PTime}");
                                    }
                                    else if (onePointForAddressOrders[j - 1].Type == "End")
                                    {
                                        var date = orders.Find(o => o.ID == onePointForAddressOrders[j - 1].IDorder);
                                        string date1 = date.TimeOfAppointment != null ? date.TimeOfAppointment : DateTime.Parse($"{GetDFormat(date.Date)} {date.TimeOfPickup}").AddSeconds(duration1).AddMinutes(30).ToShortTimeString();
                                        dateTime = DateTime.Parse($"{GetDFormat(onePointForAddressOrders[j - 1].Date)} {date1}");
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
                                        onePointForAddressOrders[j].PTime = date1;
                                    }
                                    if (onePointForAddressOrders[j].Type == "Start"
                                        && !(dateTime.AddSeconds(duration1) < dateTime1.AddMinutes(20)
                                        && dateTime1.AddMinutes(-20) < dateTime.AddSeconds(duration1)))
                                    {
                                        isAddOrder = false;
                                        break;
                                    }
                                    else if (onePointForAddressOrders[j].Type == "End"
                                        && !(dateTime.AddSeconds(duration1) < dateTime1))
                                    {
                                        isAddOrder = false;
                                        break;
                                    }
                                }
                                if(isAddOrder)
                                {
                                    break;
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
                            orderMobile.OnePointForAddressOrders.Insert(positionS, new OnePointForAddressOrder(order1.ID, locationsOrder[i].lat, locationsOrder[i].lng, order1.TimeOfPickup, order1.Date, "Start", order1.FromAddress));
                            if(isOnTheWay)
                            {

                                orderMobile.OnePointForAddressOrders.Add(new OnePointForAddressOrder(order1.ID, locationsOrder[i].latE, locationsOrder[i].lngE, order1.TimeOfAppointment, order1.Date, "End", order1.ToAddress));
                            }
                            else
                            {

                                orderMobile.OnePointForAddressOrders.Insert(positionE, new OnePointForAddressOrder(order1.ID, locationsOrder[i].latE, locationsOrder[i].lngE, order1.TimeOfAppointment, order1.Date, "End", order1.ToAddress));
                            }
                            orderMobile.Orders.Add(order1);
                            locationsOrder.Remove(locationsOrder.Find(l => l.ID == order1.ID.ToString()));
                            numberOfSeats -= order1.CountCustomer;
                        }
                    }
                }
                await sqlCoommandTaxiApi.SetOrederMobile(orderMobile, locationsAcceptOrder[0].ID, isNew);
                
            }
        }

        private string ConvertTOString(double value)
        {
            return Convert.ToString(value).Replace(',', '.');
        }

        private bool GetEndOrderOnTheWay(List<Steps> steps, location location)
        {
            bool isEndOrderOnTheWay = false;
            double lat = 0;
            double lng = 0;
            foreach (Steps step in steps)
            {
                double tmpLoc = step.end_location.lat - step.start_location.lat;
                lat = step.start_location.lat + tmpLoc;
                tmpLoc = step.end_location.lng - step.start_location.lng;
                lng = step.start_location.lng + tmpLoc;
                if ((location.latE - 0.013 < lat && lat + 0.013 > location.latE)
                       && (location.lngE - 0.013 < lng && lng + 0.013 >location.lngE))
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
            int durationNewOrder = connectorApiMaps.GetGetDuration($"{ConvertTOString(locationDriver.lat)},{ConvertTOString(locationDriver.lng)}", $"{ConvertTOString(locationNewOrder.lat)},{ConvertTOString(locationNewOrder.lng)}");
            List<double> durations = new List<double>(); 
            foreach (OnePointForAddressOrder onePointForAddressOrder in onePointForAddressOrders)
            {
                int duration = connectorApiMaps.GetGetDuration($"{ConvertTOString(locationDriver.lat)},{ConvertTOString(locationDriver.lng)}", $"{ConvertTOString(onePointForAddressOrder.Lat)},{ConvertTOString(onePointForAddressOrder.Lng)}");
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
            double distance = DistanceTo(locationsDriver[0].lat, locationsDriver[0].lng, locationsOrder[0].lat, locationsOrder[0].lng);

            foreach (var locationDriver in locationsDriver)
            {
                foreach (var locationOrder in locationsOrder)
                {
                    double tempDistance = DistanceTo(locationDriver.lat, locationDriver.lng, locationOrder.lat, locationOrder.lng);
                    if (tempDistance < distance)
                    {
                        try
                        {
                            int timeDuration = FullDuration(locationOrder, locationDriver);
                            if (DateTime.Now.AddSeconds(timeDuration) < DateTime.Parse($"{GetDFormat(locationOrder.Date)} {locationOrder.PickuoTime}").AddMinutes(20))
                            {
                                distance = tempDistance;
                                tmpOrder = locationOrder;
                                tmpDriver = locationDriver;
                            }
                        }
                        catch(Exception e)
                        {

                            File.WriteAllText("2.txt", e.Message);
                        }
                    }
                }
            }

            File.WriteAllText("1.txt", "13");
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
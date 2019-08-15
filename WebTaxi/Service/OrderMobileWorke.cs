using DBAplication.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebTaxi.Service.Model;

namespace WebTaxi.Service
{
    public class OrderMobileWorke
    {
        public SqlCommand sqlCommand = null;
        private ConnectorApiMaps connectorApiMaps = null;

        public OrderMobileWorke()
        {
            sqlCommand = new SqlCommand();
            connectorApiMaps = new ConnectorApiMaps();
        }

        public List<Order> SuitableOrders(string idDriver, OrderMobile orderMobile = null, Order order1 = null)
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
                Model.Location location1 = connectorApiMaps.GetGetLonAndLanToAddress(order.ToAddress.ToString());
                if (locationOrder != null)
                {
                    locationOrder.ID = order.ID.ToString();
                    locationOrder.Date = order.Date;
                    locationOrder.PickuoTime = order.TimeOfPickup;
                    locationOrder.ApiniTime = order.TimeOfAppointment;
                    locationOrder.ApiniTime = order.TimeOfAppointment;
                    locationOrder.latE = location1.lat;
                    locationOrder.lngE = location1.lng;
                    locationOrder.CountCusstomer = order.CountCustomer;
                    locationsOrder.Add(locationOrder);
                }
            }
            if((locationsOrder != null && locationsOrder.Count != 0) && locationsDriver != null)
            {
                if (orderMobile == null && order1 == null)
                {
                    return GetSuitableOrders(locationsOrder, locationsDriver, orders);
                }
                else
                {
                    return InsertOrderAndPointAddres(orderMobile, order1, locationsOrder, locationsDriver, orders);
                }
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

        private List<Order> InsertOrderAndPointAddres(OrderMobile orderMobile, Order order, List<Model.Location> locationsOrder, Model.Location locationDriver, List<Order> orders)
        {
            int positionS = 0;
            int positionE = 0;
            int numberOfSeats = 4;
            orderMobile.Status = "New";
            if (orderMobile.OnePointForAddressOrders == null || orderMobile.OnePointForAddressOrders.Count == 0)
            {
                orderMobile.OnePointForAddressOrders = new List<OnePointForAddressOrder>();
                Model.Location location = locationsOrder.Find(l => l.ID == order.ID.ToString());
                orderMobile.OnePointForAddressOrders.Add(new OnePointForAddressOrder(order.ID, location.lat, location.lng, order.TimeOfPickup, order.Date, "Start", order.FromAddress));
                orderMobile.OnePointForAddressOrders.Add(new OnePointForAddressOrder(order.ID, location.latE, location.lngE, order.TimeOfAppointment, order.Date, "End", order.ToAddress));
            }
            else
            {
                Model.Location location = locationsOrder.Find(l => l.ID == order.ID.ToString());
                Model.Location location1 = new Model.Location(location.latE, location.lngE); 
                GetPositionLocation(orderMobile.OnePointForAddressOrders, location, locationDriver, ref positionS);
                orderMobile.OnePointForAddressOrders.Insert(positionS, new OnePointForAddressOrder(order.ID, location.lat, location.lng, order.TimeOfPickup, order.Date, "Start", order.FromAddress));
                GetPositionLocation(orderMobile.OnePointForAddressOrders, location1, locationDriver, ref positionE);
                orderMobile.OnePointForAddressOrders.Insert(positionE, new OnePointForAddressOrder(order.ID, location.latE, location.lngE, order.TimeOfAppointment, order.Date, "End", order.ToAddress));
                orderMobile.OnePointForAddressOrders.Sort((b1, b2) => DateTime.Compare(DateTime.Parse(b1.PTime), DateTime.Parse(b2.PTime)));
            }
            if (orderMobile.Orders == null)
            {
                orderMobile.Orders = new List<Order>();
            }
            orderMobile.Orders.Add(order);
            foreach(Order order1 in orderMobile.Orders)
            {
                numberOfSeats -= order1.CountCustomer;
                if(locationsOrder.FirstOrDefault(l => l.ID == order1.ID.ToString()) != null)
                {
                    locationsOrder.Remove(locationsOrder.Find(l => l.ID == order1.ID.ToString()));
                }
            }
            if(locationsOrder != null && locationsOrder.Count != 0 && numberOfSeats > 0)
            {
                return OrderOnTheWay(locationsOrder, locationDriver, orderMobile, orders, numberOfSeats);
            }
            return new List<Order>();
        }

        private List<Order> OrderOnTheWay(List<Model.Location> locationsOrder, Model.Location locationDriver, OrderMobile orderMobile, List<Order> orders, int numberOfSeats)
        {
            List<Order> orders1 = new List<Order>();
            List<Steps> steps = new List<Steps>();
            for(int i = 1; i < orderMobile.OnePointForAddressOrders.Count; i++)
            {
                steps.AddRange(connectorApiMaps.GetGetDirections($"{ConvertTOString(orderMobile.OnePointForAddressOrders[i-1].Lat)},{ConvertTOString(orderMobile.OnePointForAddressOrders[i - 1].Lng)}", $"{ConvertTOString(orderMobile.OnePointForAddressOrders[i].Lat)},{ConvertTOString(orderMobile.OnePointForAddressOrders[i].Lng)}"));
            }
            for (int i = 0; i < locationsOrder.Count; i++)
            {
                if (locationsOrder[i].CountCusstomer > numberOfSeats)
                {
                    continue;
                }
                bool isAddOrder = true;
                bool isOnTheWay = false;
                int positionS = 0;
                int positionE = 0;
                bool isOnTheWayStart = false;
                double lat = 0;
                double lng = 0;
                double latF = locationsOrder[i].lat;
                double lngF = locationsOrder[i].lng;
                foreach (Steps step in steps)
                {
                    List<OnePointForAddressOrder> onePointForAddressOrders = new List<OnePointForAddressOrder>();
                    onePointForAddressOrders.AddRange(orderMobile.OnePointForAddressOrders);
                    double tmpLoc = Convert.ToDouble(step.end_location.lat - step.start_location.lat);
                    lat = step.start_location.lat + tmpLoc;
                    tmpLoc = step.end_location.lng - step.start_location.lng;
                    lng = step.start_location.lng + tmpLoc;
                    lng = step.start_location.lng + tmpLoc;
                    if ((latF - 0.013 < lat && lat + 0.013 > latF)
                    && (lngF - 0.013 < lng && lng + 0.013 > lngF) && !isOnTheWayStart)
                    {
                        Model.Location locationNewS = new Model.Location(locationsOrder[i].lat, locationsOrder[i].lng);
                        Model.Location locationNewE = new Model.Location(locationsOrder[i].latE, locationsOrder[i].lngE);
                        GetPositionLocation(onePointForAddressOrders, locationNewS, locationDriver, ref positionS);
                        Order order1 = orders.Find(o => o.ID.ToString() == locationsOrder[i].ID);
                        onePointForAddressOrders.Insert(positionS, new OnePointForAddressOrder(order1.ID, locationsOrder[i].lat, locationsOrder[i].lng, order1.TimeOfPickup, order1.Date, "Start", order1.FromAddress));
                        if (GetEndOrderOnTheWay(steps.GetRange(steps.IndexOf(step), (steps.Count - 1) - steps.IndexOf(step)), locationsOrder[i]))
                        {
                            GetPositionLocation(onePointForAddressOrders, locationNewE, locationDriver, ref positionE);
                            onePointForAddressOrders.Insert(positionE, new OnePointForAddressOrder(order1.ID, locationsOrder[i].latE, locationsOrder[i].lngE, order1.TimeOfAppointment, order1.Date, "End", order1.ToAddress));
                        }
                        else
                        {
                            onePointForAddressOrders.Add(new OnePointForAddressOrder(order1.ID, locationsOrder[i].latE, locationsOrder[i].lngE, order1.TimeOfAppointment, order1.Date, "End", order1.ToAddress));
                            isOnTheWay = true;
                        }
                        for (int j = 1; j < onePointForAddressOrders.Count; j++)
                        {
                            DateTime dateTime = new DateTime();
                            DateTime dateTime1 = new DateTime();
                            int duration1 = connectorApiMaps.GetGetDuration($"{ConvertTOString(onePointForAddressOrders[j - 1].Lat)},{ConvertTOString(onePointForAddressOrders[j - 1].Lng)}", $"{ConvertTOString(onePointForAddressOrders[j].Lat)},{ConvertTOString(onePointForAddressOrders[j].Lng)}");
                            if(duration1 < 60 * 10)
                            {
                                continue;
                            }
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
                                && !(dateTime.AddSeconds(duration1) < dateTime1.AddMinutes(10)
                                && dateTime1.AddMinutes(-10) < dateTime.AddSeconds(duration1)))
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
                        if (isAddOrder)
                        {
                            break;
                        }
                    }
                }
                if (isAddOrder)
                {
                    Order order1 = orders.Find(o => o.ID.ToString() == locationsOrder[i].ID);
                    if (order1.CountCustomer <= numberOfSeats)
                    {
                        orders1.Add(order1);
                        numberOfSeats -= order1.CountCustomer;
                        orders.Remove(order1);
                    }
                }
            }
            return orders1;
        }

        private bool GetEndOrderOnTheWay(List<Steps> steps, Model.Location location)
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
                       && (location.lngE - 0.013 < lng && lng + 0.013 > location.lngE))
                {
                    isEndOrderOnTheWay = true;
                    break;
                }
            }
            return isEndOrderOnTheWay;
        }

        private void GetPositionLocation(List<OnePointForAddressOrder> onePointForAddressOrders, Model.Location locationNewOrder, Model.Location locationDriver, ref int positon)
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

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
                location locationEndLoc = connectorApiMaps.GetGetLonAndLanToAddress(orders.Find(o => o.ID.ToString() == locationsAcceptOrder[1].ID).ToAddress);
                locationsOrder.Remove(locationsAcceptOrder[1]);
                locationsDriver.Remove(locationsAcceptOrder[0]);
                OrderOnTheWay(locationsOrder, locationsAcceptOrder, locationEndLoc, orders);
                //await sqlCoommandTaxiApi.AddDriversInOrder(locationsAcceptOrder[1].ID, locationsAcceptOrder[0].ID);
            }
        }

        private void OrderOnTheWay(List<location> locationsOrder, List<location> locationsAcceptOrder, location locationEndLoc, List<Order> orders)
        {
            List<Steps> steps = connectorApiMaps.GetGetDirections($"{locationsAcceptOrder[1].lat.ToString()},{locationsAcceptOrder[1].lng.ToString()}", $"{locationEndLoc.lat.ToString()},{locationEndLoc.lng.ToString()}");
            foreach (location location in locationsOrder)
            {
                bool isOnTheWayStart = false;
                //bool isOnTheWayEnd = true;
                double lat = 0;
                double lng = 0;
                double latF = Convert.ToDouble(location.lat.Replace('.', ','));
                double lngF = Convert.ToDouble(location.lng.Replace('.', ','));
                foreach (Steps step in steps)
                {
                    //location locationEndCurent = null;
                    double tmpLoc = Convert.ToDouble(step.end_location.lat.Replace('.', ',')) - Convert.ToDouble(step.start_location.lat.Replace('.', ','));
                    lat = Convert.ToDouble(step.start_location.lat.Replace('.', ',')) + tmpLoc;
                    tmpLoc = Convert.ToDouble(step.end_location.lng.Replace('.', ',')) - Convert.ToDouble(step.start_location.lng.Replace('.', ','));
                    lng = Convert.ToDouble(step.start_location.lng.Replace('.', ',')) + tmpLoc;
                    if ((latF - 0.013 < lat && lat + 0.013 > latF)
                    && (lngF - 0.013 < lng && lng + 0.013 > lngF) && !isOnTheWayStart/* && isOnTheWayEnd*/)
                    {
                        //locationEndCurent = connectorApiMaps.GetGetLonAndLanToAddress(orders.Find(o => o.ID.ToString() == location.ID).ToAddress);
                        int durationSO1ToSO2 = connectorApiMaps.GetGetDuration($"{location.lat.ToString()},{location.lng.ToString()}", $"{locationsAcceptOrder[1].lat.ToString()},{locationsAcceptOrder[1].lng.ToString()}");
                        //int durationTimeStatick = DateTime.Parse($"{GetDFormat(location.Date)} {location.PickuoTime}").Second - DateTime.Parse($"{GetDFormat(locationsAcceptOrder[1].Date)} {locationsAcceptOrder[1].PickuoTime}").Second;
                        if(DateTime.Parse($"{GetDFormat(locationsAcceptOrder[1].Date)} {locationsAcceptOrder[1].PickuoTime}").AddSeconds(durationSO1ToSO2).AddMinutes(-10) < DateTime.Parse($"{GetDFormat(location.Date)} {location.PickuoTime}") && DateTime.Parse($"{GetDFormat(location.Date)} {location.PickuoTime}") < DateTime.Parse($"{GetDFormat(locationsAcceptOrder[1].Date)} {locationsAcceptOrder[1].PickuoTime}").AddSeconds(durationSO1ToSO2).AddMinutes(10))
                        {
                            isOnTheWayStart = true;
                            locationsAcceptOrder.Add(location);
                            break;
                        }


                        //1)
                        //List<location> locationstmp = new List<location>();
                        //locationstmp.Add(locationsAcceptOrder[0]);
                        //locationstmp.Add(locationsAcceptOrder[1]);
                        //locationstmp.Add(location);
                        //FullDuration(null, null, locationstmp);
                        //if(DateTime.Parse($"{GetDFormat(location.Date}) {lo}"))
                        //{

                        //}

                        //2)
                        //if (DistanceTo(Convert.ToDouble(locationsAcceptOrder[0].lat.Replace('.', ',')), Convert.ToDouble(locationsAcceptOrder[0].lng.Replace('.', ',')), Convert.ToDouble(location.lat.Replace('.', ',')), Convert.ToDouble(location.lng.Replace('.', ','))) 
                        //> DistanceTo(Convert.ToDouble(locationsAcceptOrder[0].lat.Replace('.', ',')), Convert.ToDouble(locationsAcceptOrder[0].lng.Replace('.', ',')), Convert.ToDouble(locationsAcceptOrder[1].lat.Replace('.', ',')), Convert.ToDouble(locationsAcceptOrder[1].lng.Replace('.', ','))))
                        //{
                        //    locationstmp.Add(locationsAcceptOrder[0]);
                        //    locationstmp.Add(location);
                        //    locationstmp.Add(locationsAcceptOrder[1]);
                        //}
                        //else
                        //{
                        //    locationstmp.Add(locationsAcceptOrder[0]);
                        //    locationstmp.Add(locationsAcceptOrder[1]);
                        //    locationstmp.Add(location);
                        //}
                        //isOnTheWayEnd = false;
                    }
                   // if ((Convert.ToDouble(locationEndCurent.lat.Replace('.', ',')) - 0.013 < lat && lat + 0.013 > Convert.ToDouble(locationEndCurent.lat.Replace('.', ',')))
                   //&& (Convert.ToDouble(locationEndCurent.lng.Replace('.', ',')) - 0.013 < lng && lng + 0.013 > Convert.ToDouble(locationEndCurent.lng.Replace('.', ','))) && isOnTheWayStart && !isOnTheWayEnd)
                   // {
                   //     isOnTheWayEnd = true;
                   //     break;
                   // }
                }
                if (isOnTheWayStart/* && isOnTheWayEnd*/)
                {
                    break;
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
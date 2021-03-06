﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DBAplication.Model;

namespace ApiMobaileTaxi.Service
{
    public class ManagerTaxiApi
    {
        SqlCoommandTaxiApi sqlCoommandTaxiApi = null;

        public ManagerTaxiApi()
        {
            sqlCoommandTaxiApi = new SqlCoommandTaxiApi();
        }

        internal string Avtorization(string email, string password)
        {
            string token = "";
            if (sqlCoommandTaxiApi.CheckEmailAndPsw(email, password))
            {
                token = CreateToken(email, password);
                sqlCoommandTaxiApi.SaveToken(email, password, token);
            }
            return token;
        }

        public async void RecurentOrderDrive(int idOrderMobile, string statusOrderMobil, string token)
        {
            if(statusOrderMobil == "StartOrder")
            {
                await sqlCoommandTaxiApi.SetStatusMobileOrderStart(idOrderMobile);
            }
            else if (statusOrderMobil == "NewOrder")
            {
                await WorkNewOrder(idOrderMobile, statusOrderMobil, token, false, false);
            }
            else if (statusOrderMobil == "CompletePoint")
            {
                await sqlCoommandTaxiApi.SetStatusCompletPoint(idOrderMobile);
            }
            else if (statusOrderMobil == "EndOrder")
            {
                await sqlCoommandTaxiApi.SetStatusMobileOrderEnd(idOrderMobile, token);
            }
            else if (statusOrderMobil == "NewOrderAndEndOrder")
            {
                await WorkNewOrder(idOrderMobile, statusOrderMobil, token, false, true);
            }
        }

        internal void SaveTokenStore(string token, string tokenStore)
        {
            sqlCoommandTaxiApi.SaveTokenStoreinDb(token, tokenStore);
        }

        private async Task WorkNewOrder(int idOrderMobile, string statusOrderMobil, string token, bool isNew, bool isNewOrderAndEndOrder)
        {
            List<BackgroundService.DriverManager.location> locationsOrder = new List<BackgroundService.DriverManager.location>();
            ConnectorApiMaps connectorApiMaps = new ConnectorApiMaps();
            BackgroundService.DriverManager.location locationOrderEnd = await sqlCoommandTaxiApi.GetAddressToOrderDB(idOrderMobile);
            List<Order> orders = sqlCoommandTaxiApi.GetOrders($"{locationOrderEnd.Date} {locationOrderEnd.ApiniTime}");
            foreach (var order1 in orders)
            {
                BackgroundService.DriverManager.location locationOrder1 = connectorApiMaps.GetGetLonAndLanToAddress(order1.FromAddress);
                BackgroundService.DriverManager.location location1 = connectorApiMaps.GetGetLonAndLanToAddress(order1.ToAddress.ToString());
                if (locationOrder1 != null)
                {
                    locationOrder1.ID = order1.ID.ToString();
                    locationOrder1.Date = order1.Date;
                    locationOrder1.PickuoTime = order1.TimeOfPickup;
                    locationOrder1.ApiniTime = order1.TimeOfAppointment;
                    locationOrder1.latE = location1.lat;
                    locationOrder1.lngE = location1.lng;
                    locationOrder1.ID = order1.ID.ToString();
                    locationsOrder.Add(locationOrder1);
                }
            }
            if (locationsOrder.Count > 0)
            {
                BackgroundService.DriverManager.OrderForDrivers orderForDrivers = new BackgroundService.DriverManager.OrderForDrivers();
                BackgroundService.DriverManager.location locations = SerchMinDistance(locationOrderEnd, locationsOrder);
                List<BackgroundService.DriverManager.location> locationsAcceptOrder = new List<BackgroundService.DriverManager.location>();
                locationsAcceptOrder.Add(locationOrderEnd);
                locationsAcceptOrder.Add(locations);
                locationsOrder.Remove(locationsAcceptOrder[1]);
                orderForDrivers.OrderOnTheWay(locationsOrder, locationsAcceptOrder, orders, sqlCoommandTaxiApi, isNew);
            }
            if (isNewOrderAndEndOrder)
            {
                await sqlCoommandTaxiApi.SetStatusMobileOrderEnd(idOrderMobile, token);
            }
        }

        private void CheckAccept(object state)
        {
            Order order = (Order)state;
            order =  sqlCoommandTaxiApi.GetOrderDb(order.ID);
            if(order.Driver == null || (order.Driver.IsWork && !order.isAccept))
            {
                sqlCoommandTaxiApi.SetAcceptVisableDb(order.ID);
            }
        }

        private BackgroundService.DriverManager.location SerchMinDistance(BackgroundService.DriverManager.location locationsEndAddress, List<BackgroundService.DriverManager.location> locationsOrder)
        {
            BackgroundService.DriverManager.location tmpOrder = locationsOrder[0];
            double distance = DistanceTo(locationsEndAddress.lat, Convert.ToDouble(locationsEndAddress.lng), locationsOrder[0].lat, locationsOrder[0].lng);
            foreach (var locationOrder in locationsOrder)
            {
                double tempDistance = DistanceTo(locationsEndAddress.lat, locationsEndAddress.lng, locationOrder.lat, locationOrder.lng);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    tmpOrder = locationOrder;
                }
            }
            return tmpOrder;
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

        private string CreateToken(string email, string password)
        {
            string token = "";
            for (int i = 0; i < email.Length; i++)
            {
                token += i * new Random().Next(1, 1000) + email[i];
            }
            for (int i = 0; i < password.Length; i++)
            {
                token += i * new Random().Next(1, 1000) + password[i];
            }
            return token;
        }

        internal bool CheckToken(string token)
        {
            return sqlCoommandTaxiApi.CheckToken(token);
        }

        internal bool GetOrdersForToken(string token, ref List<Order> orders)
        {
            bool isToken = sqlCoommandTaxiApi.CheckToken(token);
            if (isToken)
            {
                orders = sqlCoommandTaxiApi.GetOrdersForToken(token);
            }
            return isToken;
        }
        
        public void SaveGPSLocationData(string token, string longitude, string latitude)
        {
            Geolocations geolocations = new Geolocations();
            geolocations.Latitude = latitude;
            geolocations.Longitude = longitude;
            sqlCoommandTaxiApi.SaveGPSLocationData(token, geolocations);
        }

        public OrderMobile GetOrderMobile(string token)
        {
            return sqlCoommandTaxiApi.GetOrderMobile(token);
        }
    }
}
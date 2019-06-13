using System;
using System.Collections.Generic;
using ApiMobaileTaxi.Model;
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

        public async void RecurentOrderDrive(string token, string status, int idorder)
        {
            if (status == "DriveFrome")
            {
                sqlCoommandTaxiApi.RecurentOrderDriveDB(status, idorder);
            }
            else if(status == "DriveTo")
            {
                sqlCoommandTaxiApi.RecurentOrderDriveDB(status, idorder);
            }
            else if (status == "Next")
            {
                List<location> locationsOrder = new List<location>();
                location locationOrder = null;
                ConnectorApiMaps connectorApiMaps = new ConnectorApiMaps();
                Order order = await sqlCoommandTaxiApi.GetAddressToOrderDB(idorder);
                List<Order> orders = sqlCoommandTaxiApi.GetOrders();
                foreach (var order1 in orders)
                {
                    location locationOrder1 = connectorApiMaps.GetGetLonAndLanToAddress(order1.FromAddress.ToString());
                    if (locationOrder1 != null)
                    {
                        locationOrder1.ID = order1.ID.ToString();
                        locationsOrder.Add(locationOrder1);
                    }
                }
                if (locationsOrder.Count > 0)
                {
                    locationOrder = connectorApiMaps.GetGetLonAndLanToAddress(order.ToAddress.ToString());
                    location locations = SerchMinDistance(locationOrder, locationsOrder);
                    sqlCoommandTaxiApi.AsiignedNext(Convert.ToInt32(locations.ID), order.Driver.ID);
                }
            }
            else if (status == "NewNext")
            {
                sqlCoommandTaxiApi.RecurentTwoOrder(token, idorder);
            }
        }

        private location SerchMinDistance(location locationsEndAddress, List<location> locationsOrder)
        {
            location tmpOrder = locationsOrder[0];
            double distance = DistanceTo(Convert.ToDouble(locationsEndAddress.lat.Replace('.', ',')), Convert.ToDouble(locationsEndAddress.lng.Replace('.', ',')), Convert.ToDouble(locationsOrder[0].lat.Replace('.', ',')), Convert.ToDouble(locationsOrder[0].lng.Replace('.', ',')));
            foreach (var locationOrder in locationsOrder)
            {
                double tempDistance = DistanceTo(Convert.ToDouble(locationsEndAddress.lat.Replace('.', ',')), Convert.ToDouble(locationsEndAddress.lng.Replace('.', ',')), Convert.ToDouble(locationOrder.lat.Replace('.', ',')), Convert.ToDouble(locationOrder.lng.Replace('.', ',')));
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    tmpOrder = locationOrder;
                }
            }
            return tmpOrder;
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

    }
}
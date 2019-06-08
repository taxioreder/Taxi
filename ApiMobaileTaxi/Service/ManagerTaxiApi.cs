﻿using System;
using System.Collections.Generic;
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
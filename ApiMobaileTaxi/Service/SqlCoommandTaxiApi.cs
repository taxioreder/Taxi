using System;
using System.Collections.Generic;
using System.Linq;
using DBAplication;
using DBAplication.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiMobaileTaxi.Service
{
    public class SqlCoommandTaxiApi
    {
        private Context context = null;

        public SqlCoommandTaxiApi()
        {
            context = new Context();
        }

        internal bool CheckEmailAndPsw(string email, string password)
        {
            return context.Drivers.FirstOrDefault(d => d.EmailAddress == email && d.Password == password) != null ? true : false;
        }

        internal async void SaveToken(string email, string password, string token)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.EmailAddress == email && d.Password == password);
            driver.Token = token;
            await context.SaveChangesAsync();
        }

        internal bool CheckToken(string token)
        {
            return context.Drivers.FirstOrDefault(d => d.Token == token) != null ? true : false;
        }

        internal List<Order> GetOrdersForToken(string token)
        {
            List<Order> orders1 = new List<Order>();
            Driver driver = context.Drivers.FirstOrDefault(d => d.Token == token);
            List<Order> orders = context.Orders.ToList().FindAll(s => s.Driver != null && s.Driver.ID == driver.ID);
            if (orders == null)
            {
                return new List<Order>();
            }
            orders1.AddRange(orders.FindAll(s => s.CurrentStatus == "Assigned" || s.CurrentStatus == "Picked up"));
            return orders1;
        }

        internal async void SaveGPSLocationData(string token, Geolocations geolocations)
        {
            Driver driver = context.Drivers.Where(d => d.Token == token)
                .Include(d => d.geolocations)
                .FirstOrDefault();
            driver.geolocations = geolocations;
            await context.SaveChangesAsync();
        }
    }
}
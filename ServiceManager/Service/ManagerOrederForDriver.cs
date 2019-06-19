
using DBAplication.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceManager.Service
{
    public class ManagerOrederForDriver
    {
        private SqlCommanManager sqlCommanManager = null;
        private ConnectorApiMaps connectorApiMaps = null;

        public ManagerOrederForDriver()
        {
            sqlCommanManager = new SqlCommanManager();
            connectorApiMaps = new ConnectorApiMaps();
            Work();
        }

        private void Work()
        {
            int period = 60000 * 2;
            Task.Run(() =>
            {
                while (true)
                {
                    SetOrderForDrivers();
                    if(CheckTime())
                    {
                        sqlCommanManager.SetWorkDrive();
                    }
                    Thread.Sleep(period);
                }
            });
        }

        private bool CheckTime()
        {
            if(7 <= DateTime.Now.Hour && 8 > DateTime.Now.Hour)
            {
                if(30 <= DateTime.Now.Minute && 34 > DateTime.Now.Minute)
                {
                    return true;
                }
            }
            return false;
        }

        private async void SetOrderForDrivers()
        {
            int iteration = 0;
            List<location> locationsDriver = null; 
            List<location> locationsOrder = null;
            List<Driver> drivers = sqlCommanManager.CheckOrderForDriver();
            if (drivers != null)
            {
                locationsDriver = new List<location>();
                locationsOrder = new List<location>();
                List<Order> orders = sqlCommanManager.GetOrders();
                foreach (var driver in drivers)
                {
                    location locationDriveZip = connectorApiMaps.GetGetLonAndLanToAddress(driver.ZipCod.ToString());
                    if(locationDriveZip != null)
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
                        locationsOrder.Add(locationOrder);
                    }
                }
                iteration = locationsDriver.Count;
                for (int i = 0; i < iteration; i++)
                {
                    if(locationsOrder.Count == 0)
                    {
                        break;
                    }
                    List<location> locations = SerchMinDistance(locationsDriver, locationsOrder);
                    locationsOrder.Remove(locations[0]);
                    locationsDriver.Remove(locations[1]);
                    await sqlCommanManager.AddDriversInOrder(locations[0].ID, locations[1].ID);
                }
            }
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
                        distance = tempDistance;
                        tmpOrder = locationOrder;
                        tmpDriver = locationDriver;
                    }
                }
            }
            locations.Add(tmpOrder);
            locations.Add(tmpDriver);
            return locations;
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
    }
}
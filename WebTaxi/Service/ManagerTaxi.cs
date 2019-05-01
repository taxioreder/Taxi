using DBAplication.Model;
using System;
using System.Collections.Generic;

namespace WebTaxi.Service
{
    public class ManagerTaxi
    {
        public SqlCommand sqlCommand = null;

        public ManagerTaxi()
        {
            sqlCommand = new SqlCommand();
        }

        public bool Avthorization(string login, string password)
        {
            return sqlCommand.ExistsDataUser(login, password);
        }

        public int Createkey(string login, string password)
        {
            Random random = new Random();
            int key = random.Next(1000, 1000000000);
            sqlCommand.SaveKeyDatabays(login, password, key);
            return key;
        }

        public List<Order> GetOrders(string status, int page)
        {
            return sqlCommand.GetShippings(status, page);
        }

        public bool CheckKey(string key)
        {
            return sqlCommand.CheckKeyDb(key);
        }

        //public List<Driver> GetDrivers()
        //{
        //    return _sqlEntityFramworke.GetDriversInDb();
        //}

        public int GetCountPage(string status)
        {
            return sqlCommand.GetCountPageInDb(status);
        }


    }
}
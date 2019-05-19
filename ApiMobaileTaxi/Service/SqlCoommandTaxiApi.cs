using System.Linq;
using DBAplication;
using DBAplication.Model;

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
    }
}
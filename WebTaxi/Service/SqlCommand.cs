using DBAplication;
using DBAplication.Model;
using System;
using System.Linq;

namespace WebTaxi.Service
{
    public class SqlCommand
    {
        private Context context = null;

        public SqlCommand()
        {
            context = new Context();
            InitUserOne();
        }

        private async void InitUserOne()
        {
            if (context.Admins.Count() == 0)
            {
                Admin admin = new Admin();
                admin.Login = "1";
                admin.Password = "1";
                await context.Admins.AddAsync(admin);
                await context.SaveChangesAsync();
            }
        }

        public bool ExistsDataUser(string login, string password)
        {
            return context.Admins.FirstOrDefault(u => u.Login == login && u.Password == password) != null;
        }

        public async void SaveKeyDatabays(string login, string password, int key)
        {
            try
            {
                Admin users = context.Admins.FirstOrDefault(u => u.Login == login && u.Password == password);
                users.KeyAuthorized = key.ToString();
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }
    }
}

using DBAplication.Model;
using Microsoft.EntityFrameworkCore;

namespace DBAplication
{
    public class Context : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Order> Orders { get; set; }
        public Context()
        {
            //Database.Migrate();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Taxi;Trusted_Connection=True;");
                //optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=WebDispatchDB;Integrated Security=False;User ID=WebDispatch;Password=WebDispatch;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
            }
        }
    }
}

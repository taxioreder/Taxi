using DBAplication.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace DBAplication
{
    public class Context : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderMobile> OrderMobiles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Geolocations> Geolocations { get; set; }

        public Context()
        {
            try
            {
                Database.Migrate();
                //Database.EnsureCreated();
            }
            catch (Exception e)
            {
                File.WriteAllText("1.txt", e.Message);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.EnableSensitiveDataLogging(true);
                    //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Taxi;Trusted_Connection=True;");
                    optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=WebTaxi;Integrated Security=False;User ID=123;Password=123;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
                }
            }
            catch (Exception e)
            {
                File.WriteAllText("2.txt", e.Message);
            }
        }
    }
}
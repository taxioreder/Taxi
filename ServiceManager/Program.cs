using ServiceManager.Service;
using System;
using System.Threading.Tasks;

namespace ServiceManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => new ManagerOrederForDriver()).Wait();
            Console.ReadKey();
        }
    }
}
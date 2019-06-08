using Plugin.Connectivity;
using System.Collections.Generic;
using TaxiApp.Models;

namespace TaxiApp.Service
{
    public class ManagerTaxi
    {
        private A_R a_R = null;
        private OrderGet orderGet = null;
        private Api_Map_Google api_Map_Google = null;

        internal int A_RWork(string typeR_A, string username, string password, ref string description, ref string token)
        {
            a_R = new A_R();
            int stateA_R = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeR_A == "authorisation")
                {
                    stateA_R = a_R.Avthorization(username, password, ref description, ref token);
                }
            }
            a_R = null;
            return stateA_R;
        }

        public int OrderWork(string typeOrder, string token, ref string description, ref List<Order> orders)
        {
            orderGet = new OrderGet();
            int stateOrder = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeOrder == "OrderGet")
                {
                    stateOrder = orderGet.ActiveOreder(token, ref description, ref orders);
                }
            }
            orderGet = null;
            return stateOrder;
        }

        public bool ApiGoogleMapsWork(string typeOrder, string address, ref location locationt)
        {
            api_Map_Google = new Api_Map_Google();
            if (typeOrder == "GetGetLonAndLanToAddress")
            {
                return api_Map_Google.GetGetLonAndLanToAddress(address, ref locationt);
            }
            api_Map_Google = null;
            return false;
        }
    }
}
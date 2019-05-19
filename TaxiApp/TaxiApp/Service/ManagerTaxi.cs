using Plugin.Connectivity;
using System;

namespace TaxiApp.Service
{
    public class ManagerTaxi
    {
        private A_R a_R = null;

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
    }
}

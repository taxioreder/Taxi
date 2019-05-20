using ApiMobaileTaxi.Model;
using ApiMobaileTaxi.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiMobaileTaxi.Controllers
{
    [Route("Api.Mobile")]
    public class A_RController : ControllerBase
    {
        ManagerTaxiApi managerTaxi = new ManagerTaxiApi();

        [HttpPost]
        [Route("Avtorization")]
        public string Avtorization(string email, string password)
        {
            string token = null;
            string respons = null;
            if ((email != "" || email != null) && (password != "" || password != null))
            {
                token = managerTaxi.Avtorization(email, password);
                if (token != null && token != "")
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", token));
                }
                else
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Password or mail was not correct", ""));
                }
            }
            else
            {
                if (email != "" || email != null)
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Email field is empty", ""));
                }
                else if (password != "" || password != null)
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Password field is empty", ""));
                }
            }
            return respons;
        }
    }
}
using System;
using ApiMobaileTaxi.Model;
using ApiMobaileTaxi.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiMobaileTaxi.Controllers
{
    [Route("Api.Mobile")]
    public class StoreController : ControllerBase
    {

        private ManagerTaxiApi managerTaxiApi = new ManagerTaxiApi();

        [HttpPost]
        [Route("tokenStore/Save")]
        public string TokenStoreSave(string token, string tokenStore)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("failed", "1", null));
            }
            try
            {
                bool isToken = managerTaxiApi.CheckToken(token);
                if (isToken)
                {
                    managerTaxiApi.SaveTokenStore(token, tokenStore);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
                }
                else
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "2", null));
                }
            }
            catch (Exception)
            {
                respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Technical work on the service", null));
            }
            return respons;
        }
    }
}
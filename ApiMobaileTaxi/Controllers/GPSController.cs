using ApiMobaileTaxi.Model;
using ApiMobaileTaxi.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ApiMobaileTaxi.Controllers
{
    [Route("Api.Mobile")]
    public class GPSController : ControllerBase
    {
        private ManagerTaxiApi managerTaxi = new ManagerTaxiApi();

        [HttpPost]
        [Route("GPS/Save")]
        public string GPSSave(string token, string longitude, string latitude)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("failed", "1", null));
            }
            try
            {
                bool isToken = managerTaxi.CheckToken(token);
                if (isToken)
                {

                    Task.Run(() =>
                    {
                        managerTaxi.SaveGPSLocationData(token, longitude, latitude);
                    });
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
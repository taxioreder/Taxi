using ApiMobaileTaxi.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace ApiMobaileTaxi.Controllers
{
    [Route("Api.Mobile")]
    [ApiController]
    public class NetController : ControllerBase
    {
        [HttpGet]
        [Route("Net")]
        public string CheckNet()
        {
            string respons = null;
            try
            {
                respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", true));
            }
            catch (Exception)
            {
                respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Technical work on the service", null));
            }
            return respons;
        }
    }
}
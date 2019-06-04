using Microsoft.AspNetCore.Mvc;
using System;
using WebTaxi.Service;

namespace WebTaxi.Controellers
{
    [Route("Geolcation")]
    public class GeolcationController : Controller
    {
        ManagerTaxi managerTaxi = new ManagerTaxi();

        [Route("Map")]
        public IActionResult GeolocationPageGet()
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    ViewBag.Driver = managerTaxi.GetDrivers();
                    actionResult = View("MapsGeoDriver");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvthoTaxi"))
                    {
                        Response.Cookies.Delete("KeyAvthoTaxi");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }
    }
}
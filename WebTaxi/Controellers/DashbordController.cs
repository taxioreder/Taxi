using Microsoft.AspNetCore.Mvc;
using System;
using WebTaxi.Service;

namespace WebTaxi.Controellers
{
    public class DashbordController : Controller
    {
        ManagerTaxi managerTaxi = new ManagerTaxi();

        [Route("Dashbord/Order/NewLoad")]
        public IActionResult NewLoad(int page)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    ViewBag.Orders = managerTaxi.GetOrders("NewLoad", page);
                    //ViewBag.Drivers = managerTaxi.GetDrivers();
                    ViewBag.count = managerTaxi.GetCountPage("NewLoad");
                    actionResult = View("NewLoad");
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
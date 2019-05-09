using Microsoft.AspNetCore.Mvc;
using System;
using WebTaxi.Service;

namespace WebTaxi.Controellers
{
    public class DriverController : Controller
    {
        ManagerTaxi managerTaxi = new ManagerTaxi();

        [Route("Driver/Drivers")]
        public IActionResult Drivers(int page)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    ViewBag.Drivers = managerTaxi.GetDrivers(page);
                    actionResult = View("FullAllDrivers");
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

        [HttpGet]
        [Route("Driver/Drivers/CreateDriver")]
        public IActionResult CreateDriver()
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    actionResult = View("CreateDriver");
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

        [HttpPost]
        [Route("Driver/Drivers/CreateDriver")]
        public IActionResult CreateDriver(string fullName, string emailAddress, string password, string phoneNumbe)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    if ((fullName != null && fullName != "") && (emailAddress != null && emailAddress != "") && (emailAddress != null && emailAddress != "")
                        && (password != null && password != "") && (fullName != null && fullName != ""))
                    {
                        managerTaxi.CreateDriver(fullName, emailAddress, password, phoneNumbe);
                        actionResult = Redirect($"{Config.BaseReqvesteUrl}/Driver/Drivers");
                    }
                    else
                    {
                        actionResult = View("CreateDriver");
                    }
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

        [HttpGet]
        [Route("Driver/Drivers/Remove")]
        public IActionResult RemoveDriver(int id)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    managerTaxi.RemoveDrive(id);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Driver/Drivers");
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
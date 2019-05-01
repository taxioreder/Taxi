using Microsoft.AspNetCore.Mvc;
using System;
using WebTaxi.Service;

namespace WebTaxi.Controellers
{
    public class RAController : Controller
    {
        ManagerTaxi managerTaxi = new ManagerTaxi();

        public IActionResult Index()
        {
            IActionResult actionResult = null;
            ViewData["hidden"] = "hidden";
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            if (Request.Cookies.ContainsKey("KeyAvthoTaxi"))
            {
                actionResult = Redirect("/Dashbord/Order/NewLoad");
            }
            else
            {
                actionResult = View("Avthorization");
            }
            return actionResult;
        }

        [HttpPost]
        public IActionResult Avthorization(string Email, string Password)
        {
            IActionResult actionResult = null;
            try
            {
                if (Email == null || Password == null)
                    throw new Exception();
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                if (managerTaxi.Avthorization(Email, Password))
                {
                    ViewData["hidden"] = "";
                    actionResult = Redirect("/Dashbord/Order/NewLoad");
                    int key = managerTaxi.Createkey(Email, Password);
                    Response.Cookies.Append("KeyAvthoTaxi", key.ToString());
                }
                else
                {
                    ViewData["hidden"] = "hidden";
                    ViewData["TextError"] = "Password or mail have been entered incorrectly";
                    actionResult = View("Avthorization");
                }

            }
            catch (Exception)
            {
                ViewData["hidden"] = "hidden";
                ViewData["TextError"] = "Password or mail have been entered incorrectly";
                actionResult = View("Avthorization");
            }
            return actionResult;
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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

        [HttpPost]
        [Route("Dashbord/SaveFile")]
        public string AddFile(IFormFile uploadedFile)
        {
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    if (uploadedFile != null)
                    {
                        string path = "Load/"+uploadedFile.FileName;
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            uploadedFile.CopyTo(fileStream);
                        }
                        managerTaxi.ParseExel(uploadedFile.FileName);
                    }
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvthoTaxi"))
                    {
                        Response.Cookies.Delete("KeyAvthoTaxi");
                    }
                }
            }
            catch (Exception e)
            {

            }
            return "true";
        }

        [Route("Dashbord/Order/Edit")]
        public IActionResult EditOrder(string id)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    if (id != "" && id != null)
                    {
                        ViewBag.Order = managerTaxi.GetOrder(id);
                        actionResult = View("EditOrder");
                    }
                    else
                    {
                        actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad");
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

        [Route("Dashbord/Order/SavaOrder")]
        public IActionResult SaveOrder(string idLoad, string nameCustomer, string phone, string fromAddress, string toAddress, string noName, string noName1,
           string noName2, string status, string date, string timeOfPickup, string timeOfAppointment, string milisse, string price, string noName3, string noName4, string noName5, string noName6)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerTaxi.CheckKey(key))
                {
                    managerTaxi.Updateorder(idLoad, nameCustomer, phone, fromAddress, toAddress, noName, noName1, noName2, status, date, timeOfPickup,
                        timeOfAppointment, milisse, price, noName3, noName4, noName5, noName6);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
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
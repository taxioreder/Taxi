using DBAplication.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
                    ViewBag.Drivers = managerTaxi.GetDrivers();
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

        [Route("Dashbord/Order/Archived")]
        public IActionResult Archived(int page)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    ViewBag.Orders = managerTaxi.GetOrders("Archived", page);
                    ViewBag.Drivers = managerTaxi.GetDrivers();
                    ViewBag.count = managerTaxi.GetCountPage("Archived");
                    actionResult = View("Archived");
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

        [Route("Dashbord/Order/Assigned")]
        public IActionResult Assigned(int page)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    ViewBag.Orders = managerTaxi.GetOrders("Assigned", page);
                    ViewBag.Drivers = managerTaxi.GetDrivers();
                    ViewBag.count = managerTaxi.GetCountPage("Assigned");
                    actionResult = View("Assigned");
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

        [Route("Dashbord/Order/Deleted")]
        public IActionResult Deleted(int page)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    ViewBag.Orders = managerTaxi.GetOrders("Deleted", page);
                    ViewBag.Drivers = managerTaxi.GetDrivers();
                    ViewBag.count = managerTaxi.GetCountPage("Deleted");
                    actionResult = View("Deleted");
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

        [Route("Dashbord/Order/Delivered")]
        public IActionResult Delivered(int page)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    ViewBag.Orders = managerTaxi.GetOrders("Delivered", page);
                    ViewBag.Drivers = managerTaxi.GetDrivers();
                    ViewBag.count = managerTaxi.GetCountPage("Delivered");
                    actionResult = View("Delivered");
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

        [Route("Dashbord/Order/Pickedup")]
        public IActionResult Pickedup(int page)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    ViewBag.Orders = managerTaxi.GetOrders("Picked up", page);
                    ViewBag.Drivers = managerTaxi.GetDrivers();
                    ViewBag.count = managerTaxi.GetCountPage("Pickedup");
                    actionResult = View("Pickedup");
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
           string noName2, string status, string date, string timeOfPickup, string timeOfAppointment, string milisse, string price, string noName3, string noName4, string noName5, string noName6, int countCustomer)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    managerTaxi.Updateorder(idLoad, nameCustomer, phone, fromAddress, toAddress, noName, noName1, noName2, status, date, timeOfPickup,
                        timeOfAppointment, milisse, price, noName3, noName4, noName5, noName6, countCustomer);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad");
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

        [Route("Dashbord/Order/ArchivedOrder")]
        public IActionResult ArchivedOrder(string id)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    managerTaxi.ArchvedOrder(id);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad");
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

        [Route("Dashbord/Order/DeletedOrder")]
        public IActionResult DeletedOrder(string id)
            {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    managerTaxi.DeletedOrder(id);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad");
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

        [Route("Dashbord/Order/Creat")]
        public async Task<IActionResult> CreatOrderpage()
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    Order order = await managerTaxi.CreateShiping();
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/Edit?id={order.ID}");
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
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Assign")]
        [HttpPost]
        public string DriverSelect(string idOrder, string idDriver)
        {
            bool actionResult = false;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                string key = null;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    if ((idDriver != null && idDriver != "") && (idOrder != null && idOrder != ""))
                    {
                        managerTaxi.Assign(idOrder, idDriver);
                        actionResult = true;
                    }
                    else
                    {
                        actionResult = false;
                    }

                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvthoTaxi"))
                    {
                        Response.Cookies.Delete("KeyAvthoTaxi");
                    }
                    actionResult = false;
                }
            }
            catch (Exception)
            {

            }
            return actionResult.ToString();
        }

        [Route("Dashbord/Unassign")]
        [HttpPost]
        public string DriverUnSelect(string idOrder)
        {
            bool actionResult = false;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    if (idOrder != null && idOrder != "")
                    {
                        managerTaxi.Unassign(idOrder);
                        actionResult = true;
                    }
                    else
                    {
                        actionResult = false;
                    }

                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvthoTaxi"))
                    {
                        Response.Cookies.Delete("KeyAvthoTaxi");
                    }
                    actionResult = false;
                }
            }
            catch (Exception)
            {

            }
            return actionResult.ToString();
        }


        private static OrderMobile OrderMobile = null; 
        [Route("Dashbord/Orders/Assigne")]
        [HttpGet]
        public IActionResult OrderAssigne(string idDriver)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key))
                {
                    OrderMobile = new OrderMobile();
                    Driver driver = managerTaxi.GetDriver(idDriver);
                    OrderMobile.IdDriver = driver.ID.ToString();
                    ViewBag.Driver = driver;
                    actionResult = View("AssignePage");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvthoTaxi"))
                    {
                        Response.Cookies.Delete("KeyAvthoTaxi");
                    }
                    OrderMobile = null;
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {
                OrderMobile = null;
            }
            return actionResult;
        }

        [Route("Dashbord/Orders/Init")]
        [HttpPost]
        public string OrderInit(string idDriver)
        {
            string actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key) && OrderMobile != null)
                {
                   List<Order> orders = managerTaxi.GetOrdersSuitable(idDriver);
                   actionResult = JsonConvert.SerializeObject(orders);
                }
                else
                {
                    actionResult = JsonConvert.SerializeObject(new List<Order>());
                }
            }
            catch (Exception)
            {
                OrderMobile = null;
            }
            return actionResult;
        }

        [Route("Dashbord/Orders/InitInsert")]
        [HttpPost]
        public string OrderInitAndInsert(string idDriver, string idOrder)
        {
            string actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key) && OrderMobile != null)
                {
                    Order order = managerTaxi.GetOrder(idOrder);
                    List<Order> orders = managerTaxi.GetOrdersSuitableAndInsert(idDriver, OrderMobile, order);
                    actionResult = JsonConvert.SerializeObject(orders);
                }
                else
                {
                    actionResult = JsonConvert.SerializeObject(new List<Order>());
                }
            }
            catch (Exception)
            {
                OrderMobile = null;
            }
            return actionResult;
        }

        [Route("Dashbord/Orders/GetOrderMobile")]
        [HttpPost]
        public string GetOrderMobile(string idDriver, string idOrder)
        {
            string actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                if (managerTaxi.CheckKey(key) && OrderMobile != null)
                {
                    actionResult = JsonConvert.SerializeObject(OrderMobile);
                }
                else
                {
                    actionResult = JsonConvert.SerializeObject(new List<Order>());
                }
            }
            catch (Exception)
            {
                OrderMobile = null;
            }
            return actionResult;
        }
    }
}
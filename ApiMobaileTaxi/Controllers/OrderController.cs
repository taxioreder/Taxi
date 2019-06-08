﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiMobaileTaxi.Model;
using ApiMobaileTaxi.Service;
using DBAplication.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiMobaileTaxi.Controllers
{
    [Route("Api.Mobile")]
    public class OrderController : ControllerBase
    {
        ManagerTaxiApi ManagerMobileApi = new ManagerTaxiApi();

        [HttpPost]
        [Route("Order")]
        public string GetActiveOrder(string token)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("failed", "1", null));
            }
            try
            {
                List<Order> orders = null;
                bool isToken = ManagerMobileApi.GetOrdersForToken(token, ref orders);
                if (isToken)
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", null, orders));
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
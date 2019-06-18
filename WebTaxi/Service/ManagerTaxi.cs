using DBAplication.Model;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTaxi.Service
{
    public class ManagerTaxi
    {
        public SqlCommand sqlCommand = null;

        public ManagerTaxi()
        {
            sqlCommand = new SqlCommand();
        }

        public void ReCheckWork(int idDriver, bool checkedDriver)
        {
            sqlCommand.ReCheckWorkInDb(idDriver, checkedDriver);
        }

        public bool Avthorization(string login, string password)
        {
            return sqlCommand.ExistsDataUser(login, password);
        }

        public int Createkey(string login, string password)
        {
            Random random = new Random();
            int key = random.Next(1000, 1000000000);
            sqlCommand.SaveKeyDatabays(login, password, key);
            return key;
        }

        public List<Order> GetOrders(string status, int page)
        {
            return sqlCommand.GetShippings(status, page);
        }

        public bool CheckKey(string key)
        {
            return sqlCommand.CheckKeyDb(key);
        }

        public void ParseExel(string nameFile)
        {
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open($"Load/{nameFile}", true))
            {
                SharedStringTable sharedStringTable = spreadSheet.WorkbookPart.SharedStringTablePart.SharedStringTable;
                foreach (WorksheetPart worksheetPart in spreadSheet.WorkbookPart.WorksheetParts)
                {
                    foreach (SheetData sheetData in worksheetPart.Worksheet.Elements<SheetData>())
                    {
                        if (sheetData.HasChildren)
                        {
                            foreach (Row row in sheetData.Elements<Row>())
                            {
                                Order order = new Order();
                                if (row.ToList().Count > 18)
                                {
                                    try
                                    {
                                        string tmp = null;
                                        var Cells = row.Elements<Cell>().ToList();
                                        order.CurrentStatus = "NewLoad";
                                        order.NoName = GetData(Cells[0], sharedStringTable);
                                        order.NoName1 = GetData(Cells[1], sharedStringTable);
                                        order.NameCustomer = GetData(Cells[2], sharedStringTable);
                                        order.Phone = GetData(Cells[3], sharedStringTable);
                                        tmp = GetData(Cells[4], sharedStringTable);
                                        order.Date = DateTime.FromOADate(Convert.ToDouble(tmp)).ToShortDateString();
                                        order.NoName2 = GetData(Cells[5], sharedStringTable);
                                        tmp = GetData(Cells[6], sharedStringTable);
                                        order.TimeOfPickup = tmp.Remove(2);
                                        order.TimeOfPickup += $":{tmp.Remove(0, 2)}M";
                                        tmp = GetData(Cells[7], sharedStringTable);
                                        order.TimeOfAppointment = tmp.Remove(2);
                                        order.TimeOfAppointment += $":{tmp.Remove(0, 2)}M";
                                        order.FromAddress = GetData(Cells[9], sharedStringTable);
                                        order.FromAddress += GetData(Cells[8], sharedStringTable).Replace(",,", ",");
                                        order.FromAddress = order.FromAddress.ToLower();
                                        order.FromZip = Convert.ToInt32(order.FromAddress.Split(',').Last());
                                        order.ToAddress = GetData(Cells[11], sharedStringTable);
                                        order.ToAddress += GetData(Cells[10], sharedStringTable).Replace(",,", ",");
                                        order.ToAddress = order.ToAddress.ToLower();
                                        order.ToZip = Convert.ToInt32(order.ToAddress.Split(',').Last());
                                        order.Milisse = GetData(Cells[12], sharedStringTable);
                                        tmp = GetData(Cells[13], sharedStringTable);
                                        if(tmp == "B7708_1R" || tmp == "B7708_1")
                                        {
                                            order.Price = "0";
                                        }
                                        else
                                        {
                                            order.Price = GetData(Cells[13], sharedStringTable);
                                        }
                                        order.NoName3 = GetData(Cells[14], sharedStringTable);
                                        order.NoName4 = GetData(Cells[15], sharedStringTable);
                                        order.NoName5 = GetData(Cells[16], sharedStringTable);
                                        order.NoName6 = GetData(Cells[17], sharedStringTable);
                                        order.Comment = GetData(Cells[18], sharedStringTable);
                                        sqlCommand.SaveOrder(order);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
                spreadSheet.Close();
            }

        }

        private string GetData(Cell cell, SharedStringTable sharedStringTable)
        {
            string data = null;
            if (cell.DataType != null)
            {
                if (cell.DataType == CellValues.SharedString)
                {
                    data = sharedStringTable.ElementAt(Int32.Parse(cell.InnerText)).InnerText;
                }
                else
                {
                    data = cell.InnerText;
                }
            }
            return data;
        }

        public List<Driver> GetDrivers()
        {
            return sqlCommand.GetDriversInDb();
        }

        public int GetCountPage(string status)
        {
            return sqlCommand.GetCountPageInDb(status);
        }

        public Order GetOrder(string id)
        {
            return sqlCommand.GetShipping(id);
        }

        public void Updateorder(string idLoad, string nameCustomer, string phone, string fromAddress, string toAddress, string noName, string noName1,
           string noName2, string status, string date, string timeOfPickup, string timeOfAppointment, string milisse, string price, string noName3, string noNam4, string noNam5, string noNam6)
        {
            sqlCommand.UpdateorderInDb(idLoad, nameCustomer, phone, fromAddress, toAddress, noName, noName1, noName2, status, date, timeOfPickup,
                        timeOfAppointment, milisse, price, noName3, noNam4, noNam5, noNam6);
        }

        public void ArchvedOrder(string id)
        {
            sqlCommand.RecurentOnArchived(id);
        }

        public void DeletedOrder(string id)
        {
            sqlCommand.RecurentOnDeleted(id);
        }

        public async Task<Order> CreateShiping()
        {
            return await sqlCommand.CreateShipping();
        }

        public List<Driver> GetDrivers(int pag)
        {
            return sqlCommand.GetDrivers(pag);
        }

        public void CreateDriver(string fullName, string emailAddress, string password, string phoneNumbe, string zipCod)
        {
            Driver driver = new Driver();
            driver.FullName = fullName;
            driver.EmailAddress = emailAddress;
            driver.Password = password;
            driver.PhoneNumber = phoneNumbe;
            driver.ZipCod = zipCod;
            sqlCommand.AddDriver(driver);
        }

        public void RemoveDrive(int id)
        {
            sqlCommand.RemoveDriveInDb(id);
        }

        public void Assign(string idOrder, string idDriver)
        {
            //ManagerNotifyWeb managerNotify = new ManagerNotifyWeb();
            //bool isDriverAssign = sqlCommand.CheckDriverOnShipping(idOrder);
            ////string tokenShope = null;
            //if (isDriverAssign)
            //{
            //    tokenShope = sqlCommand.GerShopTokenForShipping(idOrder);
            //}
            sqlCommand.AddDriversInOrder(idOrder, idDriver);
            //Task.Run(() =>
            //{
            //    string tokenShope1 = _sqlEntityFramworke.GerShopTokenForShipping(idOrder);
            //    if (!isDriverAssign)
            //    {
            //        managerNotify.SendNotyfyAssign(idOrder, tokenShope1, vehiclwInformations);
            //    }
            //    else
            //    {
            //        managerNotify.SendNotyfyAssign(idOrder, tokenShope1, vehiclwInformations);
            //        managerNotify.SendNotyfyUnassign(idOrder, tokenShope, vehiclwInformations);
            //    }
            //});
        }

        public void Unassign(string idOrder)
        {
            //ManagerNotifyWeb managerNotify = new ManagerNotifyWeb();
            //string tokenShope = _sqlEntityFramworke.GerShopTokenForShipping(idOrder);
            //List<VehiclwInformation> vehiclwInformations = await _sqlEntityFramworke.RemoveDriversInOrder(idOrder);
            sqlCommand.RemoveDriversInOrder(idOrder);
            //Task.Run(() =>
            //{
            //    managerNotify.SendNotyfyUnassign(idOrder, tokenShope, vehiclwInformations);
            //});
        }
    }
}
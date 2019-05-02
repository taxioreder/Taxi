using DBAplication.Model;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebTaxi.Service
{
    public class ManagerTaxi
    {
        public SqlCommand sqlCommand = null;

        public ManagerTaxi()
        {
            sqlCommand = new SqlCommand();
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
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open("Load/test.xlsx", true))
            {
                SharedStringTable sharedStringTable = spreadSheet.WorkbookPart.SharedStringTablePart.SharedStringTable;
                string cellValue = null;

                foreach (WorksheetPart worksheetPart in spreadSheet.WorkbookPart.WorksheetParts)
                {
                    foreach (SheetData sheetData in worksheetPart.Worksheet.Elements<SheetData>())
                    {
                        if (sheetData.HasChildren)
                        {
                            foreach (Row row in sheetData.Elements<Row>())
                            {
                                foreach (Cell cell in row.Elements<Cell>())
                                {
                                    cellValue = cell.InnerText;
                                    if (cell.DataType != null)
                                    {
                                        if (cell.DataType == CellValues.SharedString)
                                        {
                                            Console.WriteLine("cell val: " + sharedStringTable.ElementAt(Int32.Parse(cellValue)).InnerText);
                                        }
                                        else
                                        {
                                            Console.WriteLine("cell val: " + cellValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                spreadSheet.Close();
            }

        }

        //public List<Driver> GetDrivers()
        //{
        //    return _sqlEntityFramworke.GetDriversInDb();
        //}

        public int GetCountPage(string status)
        {
            return sqlCommand.GetCountPageInDb(status);
        }


    }
}
using CsvHelper;
using QBFC14Lib;
using SemaforoPorLotes.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace SemaforoPorLotes.Models
{
    public class ReporteRecalculo
    {
        public static string GenerateReportForRecalcInformation(DateTime reportDataTime)
        {
            bool sessionBegun = false;
            bool connectionOpen = false;
            QBSessionManager sessionManager = null;

            try
            {
                sessionManager = new QBSessionManager();

                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 14, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                GenerateReportQueryRq(requestMsgSet, reportDataTime);

                sessionManager.OpenConnection("", "Semaforización por Lotes");
                connectionOpen = true;
                sessionManager.BeginSession("", ENOpenMode.omDontCare);
                sessionBegun = true;
                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                sessionManager.EndSession();
                sessionBegun = false;
                sessionManager.CloseConnection();
                connectionOpen = false;
                string responseStr = responseMsgSet.ToXMLString();
                File.WriteAllText("recalc.xml", responseStr);
                return responseStr;

            }
            catch (Exception e)
            {
                MessageBox.Show("Verifica que Quickbooks esté abierto y reinicia el programa \n" + e.Message, "Error al sincronizar datos", MessageBoxButton.OK, MessageBoxImage.Error);
                if (sessionBegun)
                {
                    sessionManager.EndSession();
                }
                if (connectionOpen)
                {
                    sessionManager.CloseConnection();
                }
                return null;
            }            
        }

        private static void GenerateReportQueryRq(IMsgSetRequest requestMsgSet, DateTime reportDataTime)
        {
            IGeneralDetailReportQuery generalDetailReportQueryRq = requestMsgSet.AppendGeneralDetailReportQueryRq();
            generalDetailReportQueryRq.GeneralDetailReportType.SetValue(ENGeneralDetailReportType.gdrtInventoryValuationDetail);
            generalDetailReportQueryRq.ORReportPeriod.ReportPeriod.FromReportDate.SetValue(reportDataTime);
            generalDetailReportQueryRq.ReportItemFilter.ORReportItemFilter.ItemTypeFilter.SetValue(ENItemTypeFilter.itfInventoryAndAssembly);
            generalDetailReportQueryRq.IncludeColumnList.Add(ENIncludeColumn.icItem);            
            generalDetailReportQueryRq.IncludeColumnList.Add(ENIncludeColumn.icQuantityOnHand);
            generalDetailReportQueryRq.IncludeColumnList.Add(ENIncludeColumn.icSerialOrLotNumber);    
        }

        public static IEnumerable<RowData> getAllRowDataFromXmlResponse(string responseInXml)
        {
            try
            {
                XDocument responseDocument = XDocument.Parse(responseInXml);
                if (responseDocument != null)
                {
                    IEnumerable<RowData> dataFromXml = from item in responseDocument.Descendants("DataRow")
                                                       where item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "4") != null                                                       
                                                       select new RowData(
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "2")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "3")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "4")?.Attribute("value")?.Value                                                           
                                                           );
                    using (var writer = new StreamWriter("./data.csv"))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(dataFromXml);
                    }
                    return dataFromXml;
                }
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return null;
        }

        public static void ProcessAllRowData(IEnumerable<RowData> rowDatas)
        {
            if (rowDatas != null)
            {
                ItemRepositoryImpl itemRepositoryImpl = new ItemRepositoryImpl();
                LotNumberRepositoryImpl lotNumberRepositoryImpl = new LotNumberRepositoryImpl();
                foreach (RowData rowData in rowDatas)
                {
                    int quantityOnHand = Convert.ToInt32(float.Parse(rowData.Quantity));
                    string itemName = rowData.ItemName;
                    string lotNumber = rowData.LotNumber;

                    int itemId = itemRepositoryImpl.GetItemId(itemName);
                    if (itemId != -1)
                    {
                        int lotNumberId = lotNumberRepositoryImpl.GetLotNumberId(itemId, lotNumber);
                        if (lotNumberId != -1)
                        {
                            lotNumberRepositoryImpl.UpdateLotNumberQuantity(lotNumberId, quantityOnHand, rowData.Date);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                    
                }
            }
        }
    }
}

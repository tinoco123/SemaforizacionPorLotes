using QBFC14Lib;
using SemaforizacionPorLotes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SemaforoPorLotes.Models
{
    public class InventoryAdjustmentQuery
    {

        private static QBSessionManager sessionManager;
        public static bool sessionBegun = false;
        public static bool connectionOpen = false;

        public static void OpenConnection() 
        {
            try
            {
                sessionManager = new QBSessionManager();
                sessionManager.OpenConnection("", "Semaforizacion por lotes");
                connectionOpen = true;
                sessionManager.BeginSession("", ENOpenMode.omDontCare);
                sessionBegun = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "Error");
                if (sessionBegun)
                {
                    sessionManager.EndSession();
                }
                if (connectionOpen)
                {
                    sessionManager.CloseConnection();
                }
            }
        }

        public static void CloseConnection()
        {
            if (connectionOpen && sessionBegun)
            {
                sessionManager.EndSession();
                sessionBegun = false;
                sessionManager.CloseConnection();
                connectionOpen = false;
            }
        }

        private static string DoInventoryAdjustmentQuery(DateTime initialDate, DateTime finalDate)
        {            
            string responseInXml = "";
            try
            {                
                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 14, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                BuildInventoryAdjustmentQueryRq(requestMsgSet, initialDate, finalDate);                

                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                responseInXml = responseMsgSet.ToXMLString();
                File.WriteAllText("./data.xml", responseInXml);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "Error");
                if (sessionBegun)
                {
                    sessionManager.EndSession();
                }
                if (connectionOpen)
                {
                    sessionManager.CloseConnection();
                }
            }
            return responseInXml;
        }
        private static void BuildInventoryAdjustmentQueryRq(IMsgSetRequest requestMsgSet, DateTime initialDate, DateTime finalDate)
        {
            IInventoryAdjustmentQuery InventoryAdjustmentQueryRq = requestMsgSet.AppendInventoryAdjustmentQueryRq();
            InventoryAdjustmentQueryRq.ORInventoryAdjustmentQuery.TxnFilterWithItemFilter.ORDateRangeFilter.TxnDateRangeFilter.ORTxnDateRangeFilter.TxnDateFilter.FromTxnDate.SetValue(initialDate);
            InventoryAdjustmentQueryRq.ORInventoryAdjustmentQuery.TxnFilterWithItemFilter.ORDateRangeFilter.TxnDateRangeFilter.ORTxnDateRangeFilter.TxnDateFilter.ToTxnDate.SetValue(finalDate);
            InventoryAdjustmentQueryRq.IncludeLineItems.SetValue(true);
        }
        public static IEnumerable<InventoryAdjustmentLineRet> GetLotNumberInventoryAdjustment(DateTime initialDate, DateTime finalDate)
        {

            OpenConnection();
            string responseInXml = DoInventoryAdjustmentQuery(initialDate, finalDate);

            try
            {
                XDocument responseDocument = XDocument.Parse(responseInXml);
                var filteredData = from lineRet in responseDocument.Descendants("InventoryAdjustmentLineRet")
                                   where lineRet?.Element("LotNumber")?.Value != "" && lineRet?.Element("QuantityDifference")?.Value == "0"
                                   select new InventoryAdjustmentLineRet(
                                       lineRet?.Element("ItemRef")?.Element("FullName")?.Value,
                                       lineRet?.Element("LotNumber")?.Value
                                       );
                return filteredData;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);                
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                CloseConnection();
            }    
            
            return null;
        }
    }
}

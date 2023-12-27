using QBFC14Lib;
using System;
using System.IO;
using System.Windows;

namespace SemaforoPorLotes.Models
{
    public class InventoryValuationDetailReport
    {
        public static string GenerateInventoryValuationDetailResponseXml(DateTime reportDataTime)
        {
            bool sessionBegun = false;
            bool connectionOpen = false;
            QBSessionManager sessionManager = null;

            try
            {
                sessionManager =  new QBSessionManager();

                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 14, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                GeneralInventoryValuationDetailReportQueryRq(requestMsgSet, reportDataTime);

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
                File.WriteAllText("data.xml", responseStr);
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
            return null;
        }

        private static void GeneralInventoryValuationDetailReportQueryRq(IMsgSetRequest requestMsgSet, DateTime reportDataTime)
        {
            IGeneralDetailReportQuery generalDetailReportQueryRq = requestMsgSet.AppendGeneralDetailReportQueryRq();
            generalDetailReportQueryRq.GeneralDetailReportType.SetValue(ENGeneralDetailReportType.gdrtInventoryValuationDetail);            
            generalDetailReportQueryRq.ORReportPeriod.ReportPeriod.FromReportDate.SetValue(reportDataTime);            
            generalDetailReportQueryRq.ReportItemFilter.ORReportItemFilter.ItemTypeFilter.SetValue(ENItemTypeFilter.itfInventoryAndAssembly);
            generalDetailReportQueryRq.IncludeColumnList.Add(ENIncludeColumn.icItem);
            generalDetailReportQueryRq.IncludeColumnList.Add(ENIncludeColumn.icItemDesc);
            generalDetailReportQueryRq.IncludeColumnList.Add(ENIncludeColumn.icQuantity);
            generalDetailReportQueryRq.IncludeColumnList.Add(ENIncludeColumn.icSerialOrLotNumber);
            generalDetailReportQueryRq.IncludeColumnList.Add(ENIncludeColumn.icTxnType);            
            generalDetailReportQueryRq.IncludeColumnList.Add(ENIncludeColumn.icName);            
            generalDetailReportQueryRq.IncludeColumnList.Add(ENIncludeColumn.icTxnID);
            generalDetailReportQueryRq.IncludeColumnList.Add(ENIncludeColumn.icDate);            
        }
    }
}

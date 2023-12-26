using QBFC14Lib;
using SemaforoPorLotes.Models;
using SemaforoPorLotes.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace SemaforoPorLotes.Utils
{
    public class LoadData
    {
        private static ItemRepositoryImpl itemRepositoryImpl = new ItemRepositoryImpl();
        private static VendorRepositoryImpl vendorRepositoryImpl = new VendorRepositoryImpl();
        private static LotNumberRepositoryImpl lotNumberRepositoryImpl = new LotNumberRepositoryImpl();
        public static IEnumerable<RowData> getAllRowDataFromXmlResponse(string responseInXml)
        {
            try
            {
                XDocument responseDocument = XDocument.Parse(responseInXml);
                if (responseDocument != null)
                {
                    IEnumerable<RowData> dataFromXml = from item in responseDocument.Descendants("DataRow")
                                                       where item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "5") != null
                                                       orderby item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "8")?.Attribute("value")?.Value
                                                       select new RowData(
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "2")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "3")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "4")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "5")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "6")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "7")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "8")?.Attribute("value")?.Value
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

        public static void processAllDatFromXmlResponse(IEnumerable<RowData> dataFromXml)
        {

            // Problema alterno, quickbooks solo me sigue retornando aunque yo le indique un datetime todas las transacciones de ese dia
            if (dataFromXml != null)
            {
                int rowDataCount = dataFromXml.Count();
                if (rowDataCount == 1)
                {                    
                    RowData rowData = dataFromXml.First();
                    TxnRepositoryImpl txnRepositoryImpl = new TxnRepositoryImpl();                    
                    bool exists = txnRepositoryImpl.GetTxnId(rowData.TxnId);
                    if (!exists)
                    {
                        txnRepositoryImpl.InsertTxn(rowData.TxnId);
                        int rowDataQuantity = Convert.ToInt32(double.Parse(rowData.Quantity));
                        if (rowDataQuantity != 0)
                        {
                            processRowData(rowData, rowDataQuantity, rowData.Vendor);
                        }
                    }
                    
                }
                else if (rowDataCount > 0)
                {                   
                    RowData lastRowData = dataFromXml.ElementAt(0);
                    RowData lastRowDataInList = dataFromXml.Last();
                    string vendor = "";
                    int quantity = 0;
                    string currentTxnId = "";
                    string lastTxnId = lastRowData.TxnId;
                    
                    TxnRepositoryImpl txnRepositoryImpl = new TxnRepositoryImpl();                    
                    foreach (RowData rowData in dataFromXml)
                    {
                        // Checar si la transaccion existe
                        // Si existe, saltar el flujo
                        // si no existe insertarla en la bd y continuar con el flujo
                        bool exists = txnRepositoryImpl.GetTxnId(rowData.TxnId);//3f
                        if (exists)
                        {
                            lastTxnId = rowData.TxnId;
                            continue;
                        }
                        else
                        {
                            quantity = Convert.ToInt32(float.Parse(rowData.Quantity));                            
                            currentTxnId = rowData.TxnId;//3f
                            if (currentTxnId == lastTxnId)
                            {                                
                                
                                if (rowData.Type == "Bill" || rowData.Type == "Item Receipt")
                                {
                                    vendor = rowData.Vendor;
                                }
                                lastRowData = rowData;
                                
                                if (quantity != 0)
                                {
                                    processRowData(rowData, quantity, vendor);
                                }
                                if (rowData == lastRowDataInList)
                                {
                                    txnRepositoryImpl.InsertTxn(rowData.TxnId);
                                }
                                
                            }
                            else
                            {
                                exists = txnRepositoryImpl.GetTxnId(lastTxnId);

                                //3f
                                
                                if (exists)
                                {
                                    lastTxnId = currentTxnId;
                                    if (rowData == lastRowDataInList)
                                    {
                                        txnRepositoryImpl.InsertTxn(rowData.TxnId);
                                        processRowData(rowData, quantity, vendor);
                                    }
                                    continue;
                                }
                                else if (quantity != 0)
                                {
                                    lastRowData = rowData;
                                    if (rowData.Type == "Bill" || rowData.Type == "Item Receipt")
                                    {
                                        vendor = rowData.Vendor;
                                    }
                                    else
                                    {
                                        vendor = "";
                                    }
                                    processRowData(lastRowData, quantity, vendor);  // Se inserta en BD valores de ese item y lot number
                                    
                                }

                                // Se reinician todos los valores a los actuales
                                                               
                                lastTxnId = currentTxnId;                 
                            }
                        }                             
                    }   
                    
                }
            }
            
        }

        public static void processRowData(RowData rowData, int quantity, string vendor)
        {
            // Validate item
            int itemId = -1;
            if (rowData.ItemName != "")
            {
                
                itemId = itemRepositoryImpl.GetItemId(rowData.ItemName);
                if (itemId == -1)
                {
                    Item item = new Item(rowData.ItemName, rowData.ItemDesc);
                    itemRepositoryImpl.InsertItem(item);
                    itemId = itemRepositoryImpl.GetItemId(rowData.ItemName);
                }
            }


            // Validate vendor
            int vendorId = -1;
            if (vendor != null && vendor != "")
            {                
                vendorId = vendorRepositoryImpl.GetVendorId(vendor);
                if (vendorId == -1)
                {
                    Vendor newVendor = new Vendor(vendor);
                    vendorRepositoryImpl.InsertVendor(newVendor);
                    vendorId = vendorRepositoryImpl.GetVendorId(vendor);
                }
            }

            string expirationDate = getExpirationDateFromLotNumber(rowData.LotNumber);
            
            if (itemId != -1)
            {
                int lotNumberId = lotNumberRepositoryImpl.GetLotNumberId(itemId, rowData.LotNumber);
                //Insertar nuevo lot number 
                if (lotNumberId == -1)
                {
                    if (vendorId == -1)
                    {
                        LotNumber lotNumber = new LotNumber(rowData.LotNumber, quantity, itemId, expirationDate);
                        lotNumberRepositoryImpl.SaveLotNumber(lotNumber);
                    }
                    else
                    {
                        LotNumber lotNumber = new LotNumber(rowData.LotNumber, quantity, itemId, vendorId, expirationDate);
                        lotNumberRepositoryImpl.SaveLotNumber(lotNumber);
                    }
                }
                else // O Actualizar quantity  y vendor del lotnumber
                {
                    int oldQuantity = lotNumberRepositoryImpl.GetLotNumberQuantity(lotNumberId);
                    lotNumberRepositoryImpl.UpdateLotNumberQuantity(lotNumberId, quantity + oldQuantity);
                }
            }
        }

        public static string getExpirationDateFromLotNumber(string lotNumber)
        {

            if (lotNumber.Contains("SC"))
            {
                return "Sin caducidad";
            }
            Match match = Regex.Match(lotNumber, "(\\W\\d{2}\\W\\d{2}\\W\\d{4}\\.?$|\\W\\d{4}\\W\\d{2}\\W\\d{2}\\.?$)");
            if (match.Success)
            {
                string date = match.Value;
                if (date.Length > 1)
                {
                    date = date.Substring(1);
                }
                else
                {
                    date = string.Empty; // O puedes manejarlo de otra manera según tus necesidades
                }
                if (date.EndsWith("."))
                {
                    date = date.Remove(date.Length - 1);
                }
                string expirationDate = Regex.Replace(date, "\\D", "-");
                if (Regex.IsMatch(expirationDate, "\\d{2}\\W\\d{2}\\W\\d{4}"))
                {
                    try
                    {
                        DateTime fechaFormato = DateTime.ParseExact(expirationDate, "dd-MM-yyyy", null);
                        expirationDate = fechaFormato.ToString("yyyy-MM-dd");
                    }
                    catch (FormatException e)
                    {
                        return "No definido";
                    }
                }
                return expirationDate;
            }
            else
            {
                return "No definido";
            }

        }

        public static DateTime getLastSyncDate()
        {
            SyncRepositoryImpl syncRepositoryImpl = new SyncRepositoryImpl();
            return syncRepositoryImpl.getLastSync();
        }


    }
}

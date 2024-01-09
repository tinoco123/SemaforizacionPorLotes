using CsvHelper;
using SemaforizacionPorLotes.Models;
using SemaforizacionPorLotes.Repository;
using SemaforoPorLotes.Models;
using SemaforoPorLotes.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace SemaforoPorLotes.Utils
{
    public class LoadData
    {
        private static ItemRepositoryImpl itemRepositoryImpl = new ItemRepositoryImpl();
        private static VendorRepositoryImpl vendorRepositoryImpl = new VendorRepositoryImpl();
        private static LotNumberRepositoryImpl lotNumberRepositoryImpl = new LotNumberRepositoryImpl();
        private static TxnRepositoryImpl txnRepositoryImpl = new TxnRepositoryImpl();
        private static TxnHistoryRepositoryImpl txnHistoryRepositoryImpl = new TxnHistoryRepositoryImpl();

        public static IEnumerable<RowData> getAllRowDataFromXmlResponse(string responseInXml)
        {
            try
            {
                XDocument responseDocument = XDocument.Parse(responseInXml);
                if (responseDocument != null)
                {
                    IEnumerable<RowData> dataFromXml = from item in responseDocument.Descendants("DataRow")
                                                       where item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "5") != null
                                                       orderby item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "9")?.Attribute("value")?.Value
                                                       select new RowData(
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "2")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "3")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "4")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "5")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "6")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "7")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "8")?.Attribute("value")?.Value,
                                                           item.Elements("ColData").FirstOrDefault(col => col?.Attribute("colID")?.Value == "9")?.Attribute("value")?.Value
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
            if (dataFromXml != null && dataFromXml.Count() >= 1)
            {
                var enumerator = dataFromXml.GetEnumerator();
                string lastTxnId = dataFromXml.First().TxnId;
                RowData lastRowData = dataFromXml.Last();
                while (enumerator.MoveNext())
                {
                    var currentRowData = enumerator.Current;
                    bool exists = txnRepositoryImpl.GetTxnId(currentRowData.TxnId);
                    if (exists)
                    {
                        continue;
                    }
                    else
                    {
                        if (currentRowData.TxnId != lastTxnId)
                        {
                            bool lastTxnIdExists = txnRepositoryImpl.GetTxnId(lastTxnId);
                            if (!lastTxnIdExists)
                            {
                                txnRepositoryImpl.InsertTxn(lastTxnId);
                            }
                        }

                        int quantity = Convert.ToInt32(float.Parse(currentRowData.Quantity));
                        string vendor = "";
                        if (currentRowData.Type == "Bill" || currentRowData.Type == "Item Receipt")
                        {
                            vendor = currentRowData.Vendor;
                        }
                        processRowData(currentRowData, quantity, vendor);
                        lastTxnId = currentRowData.TxnId;
                        if (currentRowData == lastRowData)
                        {
                            txnRepositoryImpl.InsertTxn(lastTxnId);
                        }
                    }
                }
            }

        }

        public static void processRowData(RowData rowData, int quantity, string vendor, bool quantityOnHand = false)
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
                        LotNumber lotNumber = new LotNumber(rowData.LotNumber, quantity, itemId, expirationDate, rowData.Date);
                        lotNumberRepositoryImpl.SaveLotNumber(lotNumber);
                    }
                    else
                    {
                        LotNumber lotNumber = new LotNumber(rowData.LotNumber, quantity, itemId, vendorId, expirationDate, rowData.Date);
                        lotNumberRepositoryImpl.SaveLotNumber(lotNumber);
                        lotNumberId = lotNumberRepositoryImpl.GetLotNumberId(itemId, rowData.LotNumber);
                    }
                }
                else // O Actualizar quantity  y vendor del lotnumber
                {
                    if (quantityOnHand)
                    {
                        lotNumberRepositoryImpl.UpdateLotNumberQuantity(lotNumberId, quantity, rowData.Date);
                    }
                    else
                    {
                        int oldQuantity = lotNumberRepositoryImpl.GetLotNumberQuantity(lotNumberId);
                        lotNumberRepositoryImpl.UpdateLotNumberQuantity(lotNumberId, quantity + oldQuantity, rowData.Date);
                    }
                }
                txnHistoryRepositoryImpl.InsertTxnHistory(new TxnHistory(lotNumberId, quantity, rowData.Date));
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

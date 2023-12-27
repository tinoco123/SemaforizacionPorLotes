using System.Data.SQLite;
using SemaforoPorLotes.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using SemaforoPorLotes.Utils;

namespace SemaforoPorLotes.Repository
{
    public class LotNumbersTableData
    {
        public static ObservableCollection<LotNumbersView> GetLotNumbers(Hashtable parameters)
        {
            ObservableCollection<LotNumbersView> lotNumbersList = new ObservableCollection<LotNumbersView> ();
            try
            {
                string query = @"SELECT i.item_name, i.item_desc, l.lot_number, l.quantity, l.expiration_date, julianday(l.expiration_date) - julianday(date()) AS days_to_expire, v.vendor_name FROM lot_numbers l INNER JOIN items i ON i.id = l.item_id LEFT JOIN vendors v ON v.id = l.vendor_id WHERE l.quantity != 0 AND 1 = 1";
                if (parameters != null)
                {
                    if (parameters.ContainsKey("@vendor_id"))
                    {
                        query += " AND v.id = @vendor_id";
                    }
                    if (parameters.ContainsKey("@item_id"))
                    {
                        query += " AND i.id = @item_id";
                    }
                    if (parameters.ContainsKey("@expiration_date"))
                    {
                        query += " AND l.expiration_date = @expiration_date";
                    }
                    if (parameters.ContainsKey("@days_to_expire"))
                    {
                        int days_to_expire = (int)parameters["@days_to_expire"];
                        if (days_to_expire == 30)
                        {
                            query += " AND days_to_expire <= @days_to_expire";
                        } else if (days_to_expire == 90)
                        {
                            query += " AND days_to_expire > 30 AND days_to_expire <= @days_to_expire";
                        } else if (days_to_expire == 91)
                        {
                            query += " AND days_to_expire >= @days_to_expire";
                        }
                    }
                    if (parameters.ContainsKey("initial_date"))
                    {
                        query += " AND datetime(l.expiration_date) >= datetime(@initial_date)";
                    }
                    if (parameters.ContainsKey("final_date"))
                    {
                        query += " AND datetime(l.expiration_date) <= datetime(@final_date)";
                    }
                }
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                //Assign parameters

                if (parameters != null)
                {
                    if (parameters.ContainsKey("@vendor_id"))
                    {
                        command.Parameters.AddWithValue("@vendor_id", parameters["@vendor_id"]);
                    }
                    if (parameters.ContainsKey("@item_id"))
                    {
                        command.Parameters.AddWithValue("@item_id", parameters["@item_id"]);
                    }
                    if (parameters.ContainsKey("@expiration_date"))
                    {
                        command.Parameters.AddWithValue("@expiration_date", parameters["@expiration_date"]);
                    }
                    if (parameters.ContainsKey("@days_to_expire"))
                    {
                        command.Parameters.AddWithValue("@days_to_expire", parameters["@days_to_expire"]);
                    }
                    if (parameters.ContainsKey("initial_date"))
                    {
                        command.Parameters.AddWithValue("@initial_date", parameters["initial_date"]);
                    }
                    if (parameters.ContainsKey("final_date"))
                    {
                        command.Parameters.AddWithValue("@final_date", parameters["final_date"]);
                    }
                }

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LotNumbersView lotNumberView = new LotNumbersView();
                        string itemName = reader.GetString(0);
                        string itemDesc = reader.GetString(1);
                        string lotNumber = reader.GetString(2);
                        int quantity = reader.GetInt32(3);
                        string expirationDate = reader.GetString(4);
                        if (expirationDate == "Sin caducidad")
                        {
                            lotNumberView.Color = "Blanco";
                        }
                        else if (expirationDate == "No definido")
                        {
                            lotNumberView.Color = "Morado";
                        }
                        if (!reader.IsDBNull(5))
                        {
                            int daysToExpire = Convert.ToInt32(reader.GetDouble(5));
                            lotNumberView.DaysToExpire = daysToExpire;
                            if (daysToExpire <= 30)
                            {
                                lotNumberView.Color = "Rojo";
                            }
                            else if (daysToExpire <= 90)
                            {
                                lotNumberView.Color = "Tomate";
                            }
                            else if (daysToExpire > 90)
                            {
                                lotNumberView.Color = "Verde";
                            }
                        }
                        if (!reader.IsDBNull(6))  // Vendor
                        {
                            lotNumberView.Vendor = reader.GetString(6);
                        }
                        lotNumberView.ExpirationDate = expirationDate;
                        lotNumberView.ItemName = itemName;
                        lotNumberView.ItemDesc = itemDesc;
                        lotNumberView.LotNumber = lotNumber;
                        lotNumberView.Quantity = quantity;
                        lotNumbersList.Add(lotNumberView);
                    }
                }

            }
            catch(SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error al cargar datos de la tabla", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return lotNumbersList;
        }        

        public static List<Vendor> GetVendors()
        {
            List<Vendor> vendorList = new List<Vendor>();
            vendorList.Add(new Vendor(0, "Seleccionar Proveedor"));
            try
            {
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT id, vendor_name FROM vendors";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string vendorName = reader.GetString(1);
                        vendorList.Add(new Vendor(id, vendorName));
                    }
                }
            }
            catch(SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error al obtener la lista de proveedores", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return vendorList;
        }

        public static List<Item> GetItems()
        {
            List<Item> itemsList = new List<Item>();
            itemsList.Add(new Item(0, "Seleccionar Item"));
            try
            {
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT id, item_name FROM items";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string itemName = reader.GetString(1);
                        itemsList.Add(new Item(id, itemName));
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error al obtener la lista de items", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return itemsList;
        }
    }    
}

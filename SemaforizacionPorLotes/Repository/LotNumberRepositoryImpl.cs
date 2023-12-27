using System.Data.SQLite;
using SemaforoPorLotes.Models;
using System.Windows.Forms;

namespace SemaforoPorLotes.Repository
{
    public class LotNumberRepositoryImpl : ILotNumberRepository
    {
        public int GetLotNumberId(int itemId, string lotNumber)
        {
            int lotNumberId = -1;

            try
            {
                string query = @"SELECT id FROM lot_numbers WHERE lot_number = @lot_number and item_id = @item_id";
                using(SQLiteConnection connection = new SQLiteConnection("Data Source=./semaforo.db"))
                {
                    SQLiteCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@lot_number", lotNumber);
                    command.Parameters.AddWithValue("@item_id", itemId);
                    connection.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lotNumberId = reader.GetInt32(0);
                        }
                    }
                }
            } catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            
            return lotNumberId;
        }

        public int GetLotNumberQuantity(int lotNumberId)
        {
            int quantity = 0;
            try
            {
                string query = @"SELECT quantity from lot_numbers WHERE id = @id";
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=./semaforo.db"))
                {
                    SQLiteCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@id", lotNumberId);
                    connection.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quantity = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch(SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return quantity;
        }

        public bool UpdateLotNumberQuantity(int lotNumberId, int quantity)
        {
            bool result = false;
            try
            {
                string query = @"UPDATE lot_numbers SET quantity = @quantity WHERE id = @id";
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=./semaforo.db"))
                {
                    connection.Open();
                    SQLiteCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.Parameters.AddWithValue("@id", lotNumberId);
                    command.ExecuteNonQuery();
                }
            }catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return result;
        }       

        public bool SaveLotNumber(LotNumber lotNumber)
        {
            bool result = false;
            try
            {
                string query = "";                
                
                if (lotNumber.VendorId > 0)
                {
                    query = @"INSERT INTO lot_numbers (lot_number, quantity, item_id, vendor_id, expiration_date) VALUES (@lot_number, @quantity, @item_id, @vendor_id, @expiration_date)";
                }
                else
                {
                    query = @"INSERT INTO lot_numbers (lot_number, quantity, item_id, expiration_date) VALUES (@lot_number, @quantity, @item_id, @expiration_date)";
                }
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=./semaforo.db"))
                {
                    SQLiteCommand command = connection.CreateCommand();
                    command.CommandText = query;                    
                    if (lotNumber.VendorId > 0)
                    {
                        command.Parameters.AddWithValue("@vendor_id", lotNumber.VendorId);
                    }
                    command.Parameters.AddWithValue("@quantity", lotNumber.Quantity);
                    command.Parameters.AddWithValue("@lot_number", lotNumber.LotNumberName);                        
                    command.Parameters.AddWithValue("@item_id", lotNumber.ItemId);
                    command.Parameters.AddWithValue("@expiration_date", lotNumber.ExpirationDate);                        
                     
                    connection.Open();
                    command.ExecuteNonQuery();
                    result = true;
                }
            } catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return result;
        }
    }
}

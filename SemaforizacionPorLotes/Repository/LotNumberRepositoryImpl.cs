using System.Data.SQLite;
using SemaforoPorLotes.Models;
using System.Windows.Forms;
using SemaforoPorLotes.Utils;

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
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@lot_number", lotNumber);
                command.Parameters.AddWithValue("@item_id", itemId);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lotNumberId = reader.GetInt32(0);
                    }
                }
            }
            catch (SQLiteException ex)
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
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@id", lotNumberId);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        quantity = reader.GetInt32(0);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return quantity;
        }

        public bool UpdateLotNumberQuantity(int lotNumberId, int quantity, string date)
        {
            bool result = false;
            string query = "";
            try
            {
                if (date != "")
                {
                    query = @"UPDATE lot_numbers SET quantity = @quantity, last_update = @last_update WHERE id = @id";
                }
                else
                {
                    query = @"UPDATE lot_numbers SET quantity = @quantity WHERE id = @id";
                }

                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@quantity", quantity);
                if (date != "")
                {
                    command.Parameters.AddWithValue("@last_update", date);
                }
                command.Parameters.AddWithValue("@id", lotNumberId);
                command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
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
                    query = @"INSERT INTO lot_numbers (lot_number, item_id, vendor_id, expiration_date) VALUES (@lot_number, @item_id, @vendor_id, @expiration_date)";
                }
                else
                {
                    query = @"INSERT INTO lot_numbers (lot_number, item_id, expiration_date) VALUES (@lot_number, @item_id, @expiration_date)";
                }
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                if (lotNumber.VendorId > 0)
                {
                    command.Parameters.AddWithValue("@vendor_id", lotNumber.VendorId);
                }
                command.Parameters.AddWithValue("@lot_number", lotNumber.LotNumberName);
                command.Parameters.AddWithValue("@item_id", lotNumber.ItemId);
                command.Parameters.AddWithValue("@expiration_date", lotNumber.ExpirationDate);

                command.ExecuteNonQuery();
                result = true;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return result;
        }

        public bool DeleteDataFromInitialDate(string initialDate)
        {
            bool result = false;
            try
            {
                string query = "DELETE FROM lot_numbers where date(@initial_date) <= date(last_update);";
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@initial_date", initialDate);
                command.ExecuteNonQuery();
                result = true;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return result;
        }

        public bool UpdateVendor(int lotNumberId, int vendor_id)
        {
            bool result = false;
            try
            {
                string query = "UPDATE lot_numbers SET vendor_id = @vendor_id WHERE id = @lot_number_id";
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@vendor_id", vendor_id);
                command.Parameters.AddWithValue("@lot_number_id", lotNumberId);
                command.ExecuteNonQuery();
                result = true;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return false;
        }
    }
}

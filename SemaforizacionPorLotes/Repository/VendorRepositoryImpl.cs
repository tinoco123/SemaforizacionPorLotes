using System.Data.SQLite;
using SemaforoPorLotes.Models;
using System.Windows.Forms;

namespace SemaforoPorLotes.Repository
{
    public class VendorRepositoryImpl : IVendorRepository
    {
        public int GetVendorId(string vendorName)
        {
            int vendorId = -1;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=./semaforo.db"))
                {
                    string query = @"SELECT id FROM vendors WHERE vendor_name = @vendor_name";
                    SQLiteCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@vendor_name", vendorName);
                    connection.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vendorId = reader.GetInt32(0);
                        }
                    }
                }
            }catch (SQLiteException ex)
            {
                MessageBox.Show($"{ex.ErrorCode}\n{ex.Message}", "Error en la base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return vendorId;
        }

        public bool InsertVendor(Vendor vendor)
        {
            bool result = false;
            try
            {
                string query = @"INSERT INTO vendors(vendor_name) VALUES(@vendor_name)";
                using(SQLiteConnection connection = new SQLiteConnection("Data Source=./semaforo.db"))
                {
                    SQLiteCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@vendor_name", vendor.VendorName);
                    connection.Open();
                    command.ExecuteNonQuery();
                    result = true;
                }
            }catch (SQLiteException ex)
            {
                MessageBox.Show($"{ex.Message}", "Error en la base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
    }
}

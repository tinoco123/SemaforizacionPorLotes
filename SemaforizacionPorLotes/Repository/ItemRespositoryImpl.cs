using System.Data.SQLite;
using SemaforoPorLotes.Models;
using System.Windows.Forms;

namespace SemaforoPorLotes.Repository
{
    public class ItemRepositoryImpl : IItemRepository
    {
        public int GetItemId(string itemName)
        {
            int itemId = -1;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=./semaforo.db"))
                {
                    string query = @"SELECT id FROM items WHERE item_name = @item_name";
                    SQLiteCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@item_name", itemName);
                    connection.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            itemId = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch(SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return itemId;
        }

        public bool InsertItem(Item item)
        {
            bool result = false;
            try
            {
                string query = @"INSERT INTO items(item_name, item_desc) VALUES(@item_name, @item_desc)";
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=./semaforo.db"))
                {
                    SQLiteCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@item_name", item.ItemName);
                    command.Parameters.AddWithValue("@item_desc", item.ItemDesc);
                    connection.Open();
                    command.ExecuteNonQuery();
                    result = true;
                }
            }
            catch(SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return result;
        }
    }
}

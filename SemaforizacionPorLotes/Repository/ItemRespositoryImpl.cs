using System.Data.SQLite;
using SemaforoPorLotes.Models;
using SemaforoPorLotes.Utils;
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
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                string query = @"SELECT id FROM items WHERE item_name = @item_name";
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@item_name", itemName);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        itemId = reader.GetInt32(0);
                    }
                }
            }
            catch (SQLiteException ex)
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
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@item_name", item.ItemName);
                command.Parameters.AddWithValue("@item_desc", item.ItemDesc);
                command.ExecuteNonQuery();
                result = true;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return result;
        }
    }
}

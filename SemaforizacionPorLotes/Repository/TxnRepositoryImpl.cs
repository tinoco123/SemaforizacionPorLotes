using System.Data.SQLite;
using System.Windows.Forms;
using SemaforoPorLotes.Models;
using SemaforoPorLotes.Utils;
namespace SemaforoPorLotes.Repository
{
    public class TxnRepositoryImpl : ITxnRepository
    {
        public bool GetTxnId(string txnId)
        {
            bool exists = false;
            try
            {
                string query = @"SELECT 1 FROM txns WHERE txn_id = @txn_id";
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@txn_id", txnId);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        exists = true;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"{ex.ErrorCode}\n{ex.Message}", "Error en la base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return exists;
        }

        public bool InsertTxn(string txnId)
        {
            bool result = false;
            try
            {
                string query = @"INSERT INTO txns(txn_id) VALUES(@txn_id)";
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@txn_id", txnId);
                command.ExecuteNonQuery();
                result = true;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"{ex.Message}", "Error en la base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
    }
}

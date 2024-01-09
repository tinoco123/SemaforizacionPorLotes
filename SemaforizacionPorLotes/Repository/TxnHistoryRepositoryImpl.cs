using SemaforizacionPorLotes.Models;
using SemaforoPorLotes.Utils;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SemaforizacionPorLotes.Repository
{
    public class TxnHistoryRepositoryImpl : ITxnHistoryRepository
    {
        public bool DeleteHistoryFromInitialDate(string initial_date)
        {
            bool result = false;
            try
            {
                string query = "DELETE FROM txn_history where date(@initial_date) <= date(date);";
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@initial_date", initial_date);
                command.ExecuteNonQuery();
                result = true;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return result;
        }

        public bool InsertTxnHistory(TxnHistory txnHistory)
        {
            bool result = false;
            try
            {
                string query = "INSERT INTO txn_history(lot_number_id, quantity, date) VALUES (@lot_number_id, @quantity, @date)";

                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;

                command.Parameters.AddWithValue("@lot_number_id", txnHistory.LotNumberId);
                command.Parameters.AddWithValue("@quantity", txnHistory.Quantity);
                command.Parameters.AddWithValue("@date", txnHistory.Date);
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

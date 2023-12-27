using System.Data.SQLite;
using SemaforoPorLotes.Models;
using SemaforoPorLotes.Utils;
using System;

namespace SemaforoPorLotes.Repository
{
    public class SyncRepositoryImpl : ISyncRepository
    {
        public DateTime getLastSync()
        {
            DateTime last_sync = DateTime.Parse("01/01/1970");
            try
            {
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT datetime(last_sync) FROM syncs where id = @id";
                command.Parameters.AddWithValue("@id", 1);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        last_sync = DateTime.ParseExact(reader.GetString(0), "yyyy-MM-dd HH:mm:ss", null);
                    }
                    else
                    {
                        Sync currentSync = new Sync(DateTime.Now);
                        saveSync(currentSync);
                    }
                }                               
            } catch (SQLiteException ex) {
                Console.WriteLine(ex.Message);                
            }
            return last_sync;
        }

        public bool saveSync(Sync sync)
        {
            string query;
            if (sync.Id > 0)
            {
                query = @"UPDATE syncs SET last_sync = @last_sync WHERE id = @id";

            }
            else
            {
                query = @"INSERT INTO syncs(last_sync) VALUES(@last_sync)";
            }
            try
            {                
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@last_sync", sync.LastSync.ToString("yyyy-MM-dd HH:mm:ss"));
                if (sync.Id > 0)
                {
                    command.Parameters.AddWithValue("@id", sync.Id);
                }
                command.ExecuteNonQuery();
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}

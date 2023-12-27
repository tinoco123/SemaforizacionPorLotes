using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemaforoPorLotes.Utils
{
    public sealed class DbConnection
    {
        private SQLiteConnection connection;
        private static DbConnection instance;
        
        
        public static DbConnection Instance { 
            get 
            {
                if (instance == null)
                {
                    instance = new DbConnection();
                }
                return instance;
            }
        }

        
        private DbConnection()
        {
            string connectionString = "Data Source=./semaforo.db; Version=3;";
            connection = new SQLiteConnection(connectionString);
            connection.Open();
        }

        public SQLiteConnection GetConnection()
        {
            return connection;
        }

        public void CloseConnection()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}

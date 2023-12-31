﻿using System.Data.SQLite;
using SemaforoPorLotes.Models;
using SemaforoPorLotes.Utils;
using System.Windows;

namespace SemaforoPorLotes.Repository
{
    public class UserRepositoryImpl : IUserRepository
    {
        public bool login(User user)
        {
            bool access = false;
            try
            {
                SQLiteConnection connection = DbConnection.Instance.GetConnection();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM users WHERE username = @username AND password = @password";
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@password", user.Password);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        access = true;
                    }
                }
            }
            catch (SQLiteException ex)
            {                
                access = false;
                MessageBox.Show($"Ocurrio un problema en la base de datos\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return access;
        }
    }
}

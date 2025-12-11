// * This Project Created by Mostafa Desha
// * Copyright MostafaDesha © 2025


using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AccServer.Database
{
    using MYSQLCOMMAND = MySql.Data.MySqlClient.MySqlCommand;
    using MYSQLREADER = MySql.Data.MySqlClient.MySqlDataReader;
    using MYSQLCONNECTION = MySql.Data.MySqlClient.MySqlConnection;

    public unsafe static class DataHolder
    {
        public static string ConnectionString;

        private static string MySqlUsername;
        private static string MySqlPassword;
        private static string MySqlDatabase;
        private static string MySqlHost;
        public static void CreateConnection()
        {
            MySqlUsername = Program.DBuser;
            MySqlPassword = Program.DBPass;
            MySqlDatabase = Program.DBName;
            MySqlHost = Program.DBhost;

            if (string.IsNullOrEmpty(MySqlUsername) || string.IsNullOrEmpty(MySqlPassword) ||
                string.IsNullOrEmpty(MySqlDatabase) || string.IsNullOrEmpty(MySqlHost))
            {
                Console.WriteLine("[MySQL] Invalid database configuration.");
                Environment.Exit(1);
            }

            ConnectionString = $"Server={MySqlHost};" +
                               $"Database={MySqlDatabase};" +
                               $"Uid={MySqlUsername};" +
                               $"Pwd={MySqlPassword};" +
                               $"Charset=utf8mb4;" +
                               $"SslMode=None;" +
                               $"Pooling=true;Max Pool Size=300;Min Pool Size=5;";
        }
        public static MYSQLCONNECTION   MySqlConnection
        {
            get
            {
                MYSQLCONNECTION conn = new MYSQLCONNECTION();
                conn.ConnectionString = ConnectionString;
                return conn;
            }
        }
    }
}
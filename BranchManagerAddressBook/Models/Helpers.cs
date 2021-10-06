using System;
using System.Configuration;
using System.IO;
using Microsoft.Data.Sqlite;

namespace BranchManagerAddressBook.Models
{
    public class Helpers
    {
        // private const string ConnectionString = "Data Source=App_Data/products.db";
        private static string ConnectionString = ConfigurationManager.AppSettings["dbConnectionString"].ToString();

        public static SqliteConnection NewConnection()
        {
            return new SqliteConnection(ConnectionString);
            // return new SqliteConnection(@"Data Source=E:\Source\BranchManagerAddressBook\BranchManagerAddressBook\App_Data\products.db");
        }
    }
}
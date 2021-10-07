using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Data.Sqlite;

namespace BranchManagerAddressBook.Models
{
    public class Helpers
    {
        // private const string ConnectionString = "Data Source=App_Data/products.db";
        private static string ConnectionString = ConfigurationManager.AppSettings["dbConnectionString"].ToString();

        public static SqliteConnection NewConnection()
        {
            // var workingDirectory = Environment.CurrentDirectory;
            // var myType = typeof(Helpers);
            // var n = myType.Namespace.Split('.')[0];
            // var filePath = $"{workingDirectory}\\{n}.dll.config";
            // // Configuration config2 = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);//当前应用程序的配置文件
            // // ConnectionStringSettings conn = config2.ConnectionStrings.ConnectionStrings["dbConnectionString"];
            // var path = filePath;
            // var doc = new XmlDocument();
            // doc.Load(path);
            // var dbConnectionString = doc.DocumentElement.SelectSingleNode("/configuration/appSettings/add[@key='dbConnectionString']").Attributes["value"].Value;
            // // var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // return new SqliteConnection(ConnectionString);
            return new SqliteConnection(@"Data Source=E:\Source\BranchManagerAddressBook\BranchManagerAddressBook\App_Data\products.db");
        }
    }
}
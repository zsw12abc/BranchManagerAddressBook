using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace RefactorThis.Models
{
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
    public class Customers
    {
        public List<Customer> Items { get; private set; }

        public Customers()
        {
            LoadCustomers(null);
        }

        public Customers(string name)
        {
            LoadCustomers($"where lower(name) like '%{name.ToLower()}%'");
        }

        public Customers(List<Customer> items)
        {
            this.Items = items;
        }

        private void LoadCustomers(string where)
        {
            Items = new List<Customer>();

            using var conn = Helpers.NewConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = $"select id from Customer {@where}";
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var id = Guid.Parse(rdr.GetString(0));
                    Items.Add(new Customer(id));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
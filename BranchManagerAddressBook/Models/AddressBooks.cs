using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace RefactorThis.Models
{
    public class AddressBooks
    {
        public List<AddressBook> Items { get; private set; }

        public AddressBooks()
        {
            LoadAddressBooks(null);
        }

        public AddressBooks(Guid customerId)
        {
            LoadAddressBooks($"where customerId = '{customerId}' collate nocase");
        }

        public AddressBooks(List<AddressBook> items)
        {
            this.Items = items;
        }

        private void LoadAddressBooks(string where)
        {
            Items = new List<AddressBook>();
            using var conn = Helpers.NewConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = $"select id from AddressBook {@where}";

                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var id = Guid.Parse(rdr.GetString(0));
                    Items.Add(new AddressBook(id));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace RefactorThis.Models
{
    public class AddressBooks
    {
        public List<AddressBook> Address { get; private set; }

        public AddressBooks()
        {
            LoadAddressBooks(null);
        }

        public AddressBooks(Guid customerId)
        {
            LoadAddressBooks($"where customerId = '{customerId}' collate nocase");
        }

        public AddressBooks(List<AddressBook> address)
        {
            this.Address = address;
        }

        private void LoadAddressBooks(string where)
        {
            Address = new List<AddressBook>();
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
                    Address.Add(new AddressBook(id));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
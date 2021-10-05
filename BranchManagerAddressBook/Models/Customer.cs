using System;
using Newtonsoft.Json;

namespace RefactorThis.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PhoneNumber { get; set; }
        [JsonIgnore] public bool IsNew { get; }

        public Customer()
        {
            IsNew = true;
        }

        public Customer(Guid id)
        {
            IsNew = true;
            using var conn = Helpers.NewConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            try
            {
                cmd.CommandText = $"select * from Customer where id = '{id}' collate nocase";

                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    IsNew = false;
                    Id = Guid.Parse(rdr["Id"].ToString());
                    Name = rdr["Name"].ToString();
                    PhoneNumber = int.Parse(rdr["PhoneNumber"].ToString());
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
    }
}
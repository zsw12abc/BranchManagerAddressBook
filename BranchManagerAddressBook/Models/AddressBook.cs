using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace BranchManagerAddressBook.Models
{
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
    public class AddressBook
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        [JsonIgnore] public bool IsNew { get; }

        public AddressBook()
        {
            Id = Guid.NewGuid();
            IsNew = true;
        }

        public AddressBook(Guid id)
        {
            IsNew = true;
            using var conn = Helpers.NewConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            try
            {
                // cmd.CommandText = $"select * from AddressBook where id = '{id}' collate nocase";
                cmd.CommandText = $"select a.Id, a.CustomerId, c.Name, c.PhoneNumber, a.Address from AddressBook a " +
                                  $"inner join Customer c on a.CustomerId = c.Id where a.Id = @Id collate nocase";

                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "@Id",
                    Value = id
                });

                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    IsNew = false;
                    Id = Guid.Parse(rdr["Id"].ToString());
                    CustomerId = Guid.Parse(rdr["CustomerId"].ToString());
                    Name = rdr["Name"].ToString();
                    PhoneNumber = int.Parse(rdr["PhoneNumber"].ToString());
                    Address = rdr["Address"].ToString();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int Save()
        {
            using var conn = Helpers.NewConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tran;
            var rowsAffected = 0;
            try
            {
                cmd.CommandText = IsNew
                    ? $"insert into AddressBook (id, customerId, address) values (@Id, @CustomerId, @Address)"
                    : $"update AddressBook set address = @Address where id = @Id collate nocase";

                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "@Id",
                    Value = Id
                });
                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "@CustomerId",
                    Value = CustomerId
                });
                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "@Address",
                    Value = Address,
                    SqliteType = SqliteType.Text
                });


                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                tran.Commit();
            }

            return rowsAffected;
        }

        public int Delete()
        {
            using var conn = Helpers.NewConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tran;
            var rowsAffected = 0;
            try
            {
                cmd.CommandText = $"delete from AddressBook where id = @Id collate nocase";
                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "@Id",
                    Value = Id
                });
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                tran.Commit();
            }

            return rowsAffected;
        }
    }
}
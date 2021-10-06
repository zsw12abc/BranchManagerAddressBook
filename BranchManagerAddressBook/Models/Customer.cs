using System;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace BranchManagerAddressBook.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PhoneNumber { get; set; }
        [JsonIgnore] public bool IsNew { get; }

        public Customer()
        {
            Id = Guid.NewGuid();
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
                cmd.CommandText = $"select * from Customer where id = @id collate nocase";
                
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
                    Name = rdr["Name"].ToString();
                    PhoneNumber = int.Parse(rdr["PhoneNumber"].ToString());
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
                    ? $"insert into Customer (id, name, phonenumber) values (@Id, @Name, @PhoneNumber)"
                    : $"update Customer set name = @Name, phonenumber = @PhoneNumber where id = @Id collate nocase";
                
                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "@Id",
                    Value = Id
                });
                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "@Name",
                    Value = Name,
                    SqliteType = SqliteType.Text
                });
                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "@PhoneNumber",
                    Value = PhoneNumber,
                    SqliteType = SqliteType.Integer
                });

                // conn.Open();
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
            foreach (var address in new AddressBooks(Id).Address)
                address.Delete();

            using var conn = Helpers.NewConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tran;
            var rowsAffected = 0;
            try
            {
                cmd.CommandText = $"delete from Customer where id = @Id collate nocase";
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
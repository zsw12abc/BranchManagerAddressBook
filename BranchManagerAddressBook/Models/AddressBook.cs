using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RefactorThis.Models
{
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
    public class AddressBook
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }
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
                cmd.CommandText = $"select * from AddressBook where id = '{id}' collate nocase";

                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    IsNew = false;
                    Id = Guid.Parse(rdr["Id"].ToString());
                    CustomerId = Guid.Parse(rdr["CustomerId"].ToString());
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
                    ? $"insert into AddressBook (id, customerId, address) values ('{Id}', '{CustomerId}', '{Address}')"
                    : $"update AddressBook set address = '{Address}' where id = '{Id}' collate nocase";

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
                cmd.CommandText = $"delete from AddressBook where id = '{Id}' collate nocase";
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
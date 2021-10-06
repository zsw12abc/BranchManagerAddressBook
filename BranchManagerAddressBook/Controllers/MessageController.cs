using System;
using BranchManagerAddressBook.Models;

namespace BranchManagerAddressBook.Controllers
{
    public class MessageController
    {
        private const bool IsLocal = true;

        public void EventLog(LogType logType, string message)
        {
            if (IsLocal)
            {
                Console.WriteLine($"{logType}", $"{message} at {DateTime.Now}");
            }
            else
            {
                using var conn = Helpers.NewConnection();
                conn.Open();
                using var tran = conn.BeginTransaction();
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tran;
                try
                {
                    cmd.CommandText =
                        $"insert into Eventlog (LogType, Message, CreateDate) values ({logType},{message}, getdate())";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    throw;
                }
                finally
                {
                    tran.Commit();
                }
            }
        }

        public void LogError(string ClassName, Exception exception)
        {
            if (IsLocal)
            {
                Console.WriteLine($"Exception happened in {ClassName}", exception.Message,
                    exception.StackTrace);
            }
            else
            {
                using var conn = Helpers.NewConnection();
                conn.Open();
                using var tran = conn.BeginTransaction();
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tran;
                try
                {
                    cmd.CommandText =
                        $"insert into ExceptionLog (ClassName, StrackTrace, CreateDate) values ({ClassName}, {exception.Message},{exception.StackTrace}, getdate())";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    throw;
                }
                finally
                {
                    tran.Commit();
                }
            }
        }
    }
}
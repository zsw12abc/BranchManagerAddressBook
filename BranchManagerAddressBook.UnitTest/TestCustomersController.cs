using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BranchManagerAddressBook.Models;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace BranchManagerAddressBook.UnitTest
{
    [TestClass]
    public class TestCustomersController
    {
        private Customer GetNewCustomer()
        {
            return new Customer()
            {
                Id = Guid.NewGuid(),
                Name = "Adam",
                PhoneNumber = 0444444444
            };
        }

        [TestMethod]
        public void TestSave()
        {
            var customer = GetNewCustomer();
            try
            {
                var rowsAffected = customer.Save();
                Assert.AreEqual(rowsAffected, 1);
            }
            catch (Exception e)
            {
                Assert.Fail("[Product Save] Expected no exception, but got: " + e.Message);
                throw;
            }

            var customerFetch = new Customer(customer.Id);
            Assert.AreEqual(customer.Id, customerFetch.Id);
            Assert.AreEqual(customer.Name, customerFetch.Name);
            Assert.AreEqual(customer.PhoneNumber, customerFetch.PhoneNumber);
            Assert.AreNotEqual(customer.IsNew, customerFetch.IsNew);
        }
    }
}
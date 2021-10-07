using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BranchManagerAddressBook.Models;
using NUnit.Framework;
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
                Assert.Fail("[Customer Save] Expected no exception, but got: " + e.Message);
                throw;
            }

            var customerFetch = new Customer(customer.Id);
            Assert.AreEqual(customer.Id, customerFetch.Id);
            Assert.AreEqual(customer.Name, customerFetch.Name);
            Assert.AreEqual(customer.PhoneNumber, customerFetch.PhoneNumber);
            Assert.AreNotEqual(customer.IsNew, customerFetch.IsNew);
        }
        
        [TestMethod]
        public void TestUpdate()
        {
            var customer = GetNewCustomer();
            try
            {
                customer.Save();
                const int updatedPhoneNumber = 0499999999;
                var customerNew = new Customer(customer.Id)
                {
                    PhoneNumber = updatedPhoneNumber
                };
                var rowsAffected = customerNew.Save();
                Assert.AreEqual(rowsAffected, 1);
                var productCheck = new Customer(customerNew.Id);
                Assert.AreNotEqual(customer.PhoneNumber, productCheck.PhoneNumber);
                Assert.AreEqual(customer.Id, productCheck.Id);
                Assert.AreEqual(customer.Name, productCheck.Name);
            }
            catch (Exception e)
            {
                Assert.Fail("[Customer Update] Expected no exception, but got: " + e.Message);
                throw;
            }

            var productFetch = new Customer(customer.Id);
        }

        [TestMethod]
        public void TestDelete()
        {
            try
            {
                var customer = GetNewCustomer();
                customer.Save();
                var rowsAffected = customer.Delete();
                Assert.AreEqual(rowsAffected, 1);
            }
            catch (Exception e)
            {
                Assert.Fail("[Customer Delete] Expected no exception, but got: " + e.Message);
                throw;
            }
        }
    }
}
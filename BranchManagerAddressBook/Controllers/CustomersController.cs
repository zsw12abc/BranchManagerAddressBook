using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using BranchManagerAddressBook.Auth;
using BranchManagerAddressBook.Models;

namespace BranchManagerAddressBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class CustomersController : ControllerBase
    {
        private IMemoryCache _cache;
        private MessageController _messageController = new MessageController();

        public CustomersController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        [ResponseCache(Duration = 3600)]
        [HttpGet]
        // [Route("GetAllCustomers")]
        public Customers Get(string name = "")
        {
            if (name != "")
            {
                return GetAllCustomersByName(name);
            }
            else
            {
                var found = _cache.TryGetValue("Customers", out Customers customers);
                if (found)
                {
                    return customers;
                }

                customers = new Customers();
                _cache.Set("Customers", customers, new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)));
                return customers;
            }
        }

        public Customers GetAllCustomersByName(string name)
        {
            var found = _cache.TryGetValue("Customers", out Customers customers);
            if (found)
            {
                var items = customers.Contact.Where(x => x.Name.ToLower().Contains(name.ToLower()));
                if (items.Any())
                {
                    return new Customers(items.ToList());
                }
            }

            customers = new Customers(name);
            if (customers.Contact.Any(customer => customer.IsNew))
            {
                throw new Exception($"Customers named {name} should not be New");
            }

            return customers;
        }

        [ResponseCache(Duration = 3600)]
        [HttpGet("{id}")]
        public Customer Get(Guid id)
        {
            var found = _cache.TryGetValue("Customers", out Customers customers);
            if (found)
            {
                var item = customers.Contact.Find(x => x.Id == id);
                if (item != null)
                {
                    return item;
                }
            }

            var customer = new Customer(id);
            if (customer.IsNew)
                throw new Exception($"Customers with id: {id.ToString()} should not be New");

            return customer;
        }

        [HttpPost]
        public IActionResult Post(Customer customer)
        {
            var found = _cache.TryGetValue("Customers", out Customers customers);
            if (found)
            {
                customers.Contact.Add(customer);
                _cache.Set("Customers", customers, new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)));
            }

            var rowsAffected = 0;
            try
            {
                rowsAffected = customer.Save();
            }
            catch (Exception e)
            {
                _messageController.LogError("Customers Post", e);
                throw;
            }

            if (rowsAffected == 1)
            {
                return Ok($"Customer {customer.Name} with id = {customer.Id} has been added.");
            }
            else
            {
                return Problem("Try to add Customer {customer.Name} with id = {customer.Id} but failed");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Customer customer)
        {
            var orig = new Customer(id)
            {
                Name = customer.Name,
                PhoneNumber = customer.PhoneNumber,
            };
            var rowsAffected = 0;

            if (!orig.IsNew)
            {
                try
                {
                    rowsAffected = orig.Save();
                }
                catch (Exception e)
                {
                    _messageController.LogError("Customers Update", e);
                    throw;
                }
            }

            if (rowsAffected > 0)
            {
                var found = _cache.TryGetValue("Customers", out Customers customers);
                if (found)
                {
                    var customerUpdated = customers.Contact.Find(customer => customer.Id == id);
                    if (customerUpdated != null)
                    {
                        customerUpdated.Name = customer.Name;
                        customerUpdated.PhoneNumber = customer.PhoneNumber;
                        var removed = customers.Contact.Remove(new Customer(id));
                        if (removed)
                        {
                            customers.Contact.Add(customerUpdated);
                            _cache.Set("Customers", customers, new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromMinutes(5)));
                        }
                    }
                }

                return Ok($"Customer {customer.Name} with id = {customer.Id} has been Updated.");
            }
            else
            {
                return Problem($"Try to update Customer {customer.Name} with id = {customer.Id}, but failed");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var customer = new Customer(id);
            var rowsAffected = 0;
            try
            {
                rowsAffected = customer.Delete();
            }
            catch (Exception e)
            {
                _messageController.LogError("Customers Delete", e);
                throw;
            }

            var found = _cache.TryGetValue("Customers", out Customers customers);
            if (found)
            {
                var customerUpdated = customers.Contact.Find(customer => customer.Id == id);
                if (customerUpdated != null)
                {
                    customers.Contact.Remove(new Customer(id));
                    _cache.Set("Customers", customers, new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)));
                }
            }

            if (rowsAffected > 0)
            {
                return Ok($"Customer {customer.Name} with id = {customer.Id} has been deleted.");
            }
            else
            {
                return Ok($"Customer {customer.Name} with id = {id} never exist");
            }
        }

        [ResponseCache(Duration = 3600)]
        [HttpGet("{customerId}/addressbooks")]
        public AddressBooks GetAddressBooks(Guid customerId)
        {
            var found = _cache.TryGetValue("AddressBooks", out AddressBooks addressBooks);
            if (found)
            {
                var result = addressBooks.Address.Where(item => item.CustomerId == customerId);
                if (result.Any())
                {
                    return new AddressBooks(result.ToList());
                }
            }
            else
            {
                _cache.Set("AddressBooks", new AddressBooks(), new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)));
            }

            return new AddressBooks(customerId);
        }


        [ResponseCache(Duration = 3600)]
        [HttpGet("addressbooks")]
        public AddressBooks GetAllAddressBooks()
        {
            var found = _cache.TryGetValue("AddressBooks", out AddressBooks addressBooks);
            if (found)
            {
                return addressBooks;
            }

            _cache.Set("AddressBooks", new AddressBooks(), new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5)));

            return new AddressBooks();
        }

        [ResponseCache(Duration = 3600)]
        [HttpPut("addressbooks")]
        public IActionResult Update(AddressBook[]  addressBooks)
        {
            var rowsAffected = 0;
            var found = _cache.TryGetValue("AddressBooks", out AddressBooks books);
            foreach (var addressBook in addressBooks)
            {
                var orig = new AddressBook(addressBook.Id)
                {
                    CustomerId = addressBook.CustomerId,
                    Name = addressBook.Name,
                    PhoneNumber = addressBook.PhoneNumber,
                    Address = addressBook.Address
                };

                if (!orig.IsNew)
                {
                    try
                    {
                        rowsAffected += orig.Save();
                    }
                    catch (Exception e)
                    {
                        _messageController.LogError("AddressBooks Update", e);
                        throw;
                    }
                }
                if (found)
                {
                    var addressUpdated = books.Address.Find(address => address.Id == addressBook.Id);
                    if (addressUpdated != null)
                    {
                        addressUpdated.Address = addressBook.Address;
                        addressUpdated.Name = addressBook.Name;
                        addressUpdated.PhoneNumber = addressBook.PhoneNumber;
                        addressUpdated.CustomerId = addressBook.CustomerId;
                        var removed = books.Address.Remove(new AddressBook(addressBook.Id));
                        if (removed)
                        {
                            books.Address.Add(addressUpdated);
                            _cache.Set("AddressBooks", books, new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromMinutes(5)));
                        }
                    }
                }
            }

            if (rowsAffected > 0)
            {
                return Ok($"{addressBooks.Length} AddressBooks have been Updated.");
            }
            else
            {
                return Problem($"Try to update {addressBooks.Length} AddressBooks, but failed");
            }
            
        }
    }
}
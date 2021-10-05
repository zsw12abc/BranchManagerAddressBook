using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RefactorThis.Auth;
using RefactorThis.Models;

namespace RefactorThis.Controllers
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
                var items = customers.Items.Where(x => x.Name.ToLower().Contains(name.ToLower()));
                if (items.Any())
                {
                    return new Customers(items.ToList());
                }
            }

            customers = new Customers(name);
            if (customers.Items.Any(customer => customer.IsNew))
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
                var item = customers.Items.Find(x => x.Id == id);
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
                customers.Items.Add(customer);
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
    }
}
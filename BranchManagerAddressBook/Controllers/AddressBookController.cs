using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RefactorThis.Auth;
using RefactorThis.Models;

namespace RefactorThis.Controllers
{
    [Route("api/AddressBook")]
    [ApiController]
    [ApiKey]
    public class AddressBookController : ControllerBase
    {
        private IMemoryCache _cache;
        private MessageController _messageController = new MessageController();

        public AddressBookController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        [ResponseCache(Duration = 3600)]
        [HttpGet]
        public Customers Get(string name = "")
        {
            if (name != "")
            {
                return GetAllCustomersByName(name);
            }
            else
            {
                var found = _cache.TryGetValue("Products", out Customers customers);
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
            var found = _cache.TryGetValue("Products", out Customers customers);
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
    }
}
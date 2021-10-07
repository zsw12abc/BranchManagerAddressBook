## Function contains in This App
- All API Endpoints available
- Auth checked before hit endpoint
- Cache available in API
- Prevent SQL Inject attack
- Error Log stored in DB in `MessageController`
- Response meanful


## Getting started for applicants

All the API should have auth header
    ```
    ApiKey : Za4mbupdiXoWdjedXwj6Kn
    ```


There should be these endpoints:

1. `GET /Customers` - gets all Customers.
   ```
   [GET]
   http://localhost:5000/api/Customers/
   ```
2. `POST /Customers` - creates a new Customers.
    ```
    [POST]
    http://localhost:5000/api/Customers/
    [body]
    {
        "Id": "01234567-89ab-cdef-0123-456789abcdef",
        "Name": "Alice",
        "PhoneNumber": "0422222222"
    }
    ```
3. `DELETE /Customers/{id}` - deletes a Customer and his address.
   ```
   [DELETE]
   http://localhost:5000/api/Customers/01234567-89ab-cdef-0123-456789abcdef
   ```
4. `GET /Customers/{id}/addressbooks` - finds all addressbooks for a specified Customers.
   ```
   [GET]
   http://localhost:5000/api/Customers/c71122df-18e4-4a78-a446-fbf7b8f2969b/addressbooks
   ```
5. `GET /Customers/addressbooks` - finds all addressbooks for All specified Customers.
   ```
   [GET]
   http://localhost:5000/api/Customers/addressbooks
   ```
6. `Put /Customers/addressbooks` - Update addressbooks with the Address Details in Body based on Id 
    ```
    [PUT]
    http://localhost:5000/api/Customers/addressbooks
    [Body]
    [{
    "Id": "0c705951-5510-48f5-850d-6d424e752f60",
    "CustomerId": "c71122df-18e4-4a78-a446-fbf7b8f2969b",
    "Name": "Harry",
    "PhoneNumber": "0422222222",
    "Address":"king street"
    },
    {
    "Id": "2449931a-51ed-404f-8f55-7bb902f30a6a",
    "CustomerId": "c71122df-18e4-4a78-a446-fbf7b8f2969b",
    "Name": "Harry",
    "PhoneNumber": "433073333",
    "Address":"collins street"
    }]
    ```


## Unit Test
3 unit tests are available for 
- Create Customer
- Update Customer 
- Delete Customer
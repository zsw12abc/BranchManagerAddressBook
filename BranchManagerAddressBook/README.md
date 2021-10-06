## Getting started for applicants

There should be these endpoints:

1. `GET /Customers` - gets all Customers.
2. `POST /Customers` - creates a new Customers.
3. `DELETE /Customers/{id}` - deletes a Customer and his address.
4. `GET /Customers/{id}/addressbooks` - finds all addressbooks for a specified Customers.
5. `GET /Customers/addressbooks` - finds all addressbooks for All specified Customers.
6. `Put /Customers/addressbooks` - Update addressbooks with the Address Details in Body based on Id 

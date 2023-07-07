using Laundry.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Laundry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateCustomer : ControllerBase

    {
        private readonly ITwilioRestClient _client;
        private readonly LaundryContext _context;

        public CreateCustomer(LaundryContext context, ITwilioRestClient client)
        {
            _context = context;
            _client = client;
        }
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(customer.Password);
            customer.Password = hash;
            _context.Customers.Add(customer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(customer.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            /*return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);*/

            var sms = new SmsMessage();
            sms.To = "+84966876014";
            sms.From = "+15855359643";
            sms.Messsage = "Account successfully created";
            var message = MessageResource.Create(
            to: new PhoneNumber(sms.To),
            from: new PhoneNumber(sms.From),
            body: sms.Messsage,
            client: _client);
           
            return Ok();
        }
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}

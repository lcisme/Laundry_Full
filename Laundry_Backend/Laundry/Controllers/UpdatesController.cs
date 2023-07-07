using Laundry.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Laundry.Controllers
{
    /*[Authorize]*/
    [Route("api/[controller]")]
    [ApiController]
    public class UpdatesController : ControllerBase
    {

        private readonly LaundryContext _context;

        /* public UpdatesController(LaundryContext context)
         {
             _context = context;
         }

         /*  private readonly ITwilioRestClient _client;
           public SmsController(ITwilioRestClient client)
           {
               _client = client;
           }*/
        private readonly ITwilioRestClient _client;
        public UpdatesController(LaundryContext context, ITwilioRestClient client)
        {
            _context = context;
            _client = client;
        }

        // cap nhat trang thai
        [HttpPut]
        [Route("status/{id:int}")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, ParaSearch paraSearch)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null) return NotFound();

            invoice.Status = paraSearch.Status;

            // 3 trang thai Ready, Pending, Delivered

            if (invoice.Status == "Delivered")
            {
              invoice.EndDate = DateTime.Now;
                var customer = await _context.Customers.FindAsync(invoice.CustomerId);

                _context.Entry(invoice).State = EntityState.Modified;
                var sms = new SmsMessage();
                sms.To = "+84966876014";
                sms.From = "+15855359643";
                sms.Messsage = customer.Name + "'s bill " + invoice.Id + " has been delivered";
                var message = MessageResource.Create(
                to: new PhoneNumber(sms.To),
                from: new PhoneNumber(sms.From),
                body: sms.Messsage,
                client: _client);

            }

            if (invoice.Status == "Ready")
            {
                invoice.EndDate = DateTime.Now;
                var customer = await _context.Customers.FindAsync(invoice.CustomerId);

                _context.Entry(invoice).State = EntityState.Modified;
                var sms = new SmsMessage();
                sms.To = "+84966876014";
                sms.From = "+15855359643";
                sms.Messsage = customer.Name + "'s bill " + invoice.Id + " has been ready";
                var message = MessageResource.Create(
                to: new PhoneNumber(sms.To),
                from: new PhoneNumber(sms.From),
                body: sms.Messsage,
                client: _client);

            }
            _context.Entry(invoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(invoice);

        }

        // cap nhat thanh toan

        [HttpPut]
        [Route("isPay/{id:int}")]
        public async Task<IActionResult> UpdateisPay([FromRoute] int id, ParaSearch paraSearch)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null) return NotFound();

            invoice.IsPay = paraSearch.isPay;

            var customer = await _context.Customers.FindAsync(invoice.CustomerId);

            _context.Entry(invoice).State = EntityState.Modified;

            

            if (paraSearch.isPay == true)
            {
                var sms = new SmsMessage();
                sms.To = "+84966876014";
                sms.From = "+15855359643";
                sms.Messsage = customer.Name + "'s bill " + invoice.Id + " has been paid";
                var message = MessageResource.Create(
                to: new PhoneNumber(sms.To),
                from: new PhoneNumber(sms.From),
                body: sms.Messsage,
                client: _client);
            }
            

            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(invoice);

        }


        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }
    }
}



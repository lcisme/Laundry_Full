using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laundry.Models;
using Microsoft.AspNetCore.Authorization;

using System.Text;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using Twilio.Clients;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

namespace Laundry.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly LaundryContext _context;
        private readonly ITwilioRestClient _client;

        public InvoicesController(LaundryContext context, ITwilioRestClient client)
        {
            _context = context;
            _client = client;
        }


        // GET: api/Invoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            return await _context.Invoices.ToListAsync();
        }

        // GET: api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        // PUT: api/Invoices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return BadRequest();
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



        // id membership

        private async Task<Membership> GetMembership(string startdate)
        {
            return await _context.Memberships.FirstOrDefaultAsync(u => u.StartDate.ToString() == startdate);
        }



        // POST: api/Invoices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
        {
            // set datime now
            invoice.StartDate = DateTime.Now;
            invoice.Status = "Pending";
            invoice.IsPay = false;
            // tim customer can dang ky
            var customer = await _context.Customers.FindAsync(invoice.CustomerId);


            if (customer != null)
            {
                if (customer.IsMember)
                {
                    invoice.Type = 4;
                }

                if (invoice.Type == 1)
                {
                    invoice.Price = 1 * invoice.NumberOfItems;
                }
                else if (invoice.Type == 2)
                {
                    invoice.Price = 5 * invoice.WeightOfItems;
                }
                else if (invoice.Type == 3)
                {
                    var membership = new Membership();
                    membership.CustomerId = invoice.CustomerId;
                    membership.StartDate = DateTime.Now;
                    membership.EndDate = DateTime.Now.AddDays(30);

                    _context.Memberships.Add(membership);

                    customer.IsMember = true;
                    invoice.Price = 75;
                }
                else if (invoice.Type == 4)
                {
                    invoice.Price = 0;
                    invoice.IsPay = true;
                }
                else
                {
                    invoice.Price = null;
                }

                _context.Invoices.Add(invoice);
               

            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (InvoiceExists(invoice.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var sms = new SmsMessage();
            sms.To = "+84966876014";
            sms.From = "+15855359643";
            sms.Messsage = "Invoice creation successful";
            var message = MessageResource.Create(
            to: new PhoneNumber(sms.To),
            from: new PhoneNumber(sms.From),
            body: sms.Messsage,
            client: _client);
            QrModel qrModel = new QrModel();
            qrModel.Link = "http://localhost:4500/dashboard/" + invoice.Id;
         


            return Ok(qrModel);

      
            
        }

        // DELETE: api/Invoices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }
    }
}

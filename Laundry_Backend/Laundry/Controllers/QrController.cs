using Laundry.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laundry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QrController : ControllerBase
    {
        private readonly LaundryContext _context;

        public QrController(LaundryContext context)
        {
            _context = context;
        }

        // xem 1 hóa đơn qua qr
        [HttpGet("/invoce/{id}")]
        public async Task<ActionResult<IEnumerable<Invoice>>> InfoOneInvoice(int id)
        {

             
            var result = _context.Invoices.Join(
                _context.Customers,
                i => i.CustomerId,
                c => c.Id,
                (i, c) => new
                {
                    ID = i.Id,
                    Phone = c.Phone,
                    Name = c.Name,
                    Price = i.Price,
                    Status = i.Status,
                    Pay = i.IsPay,
                    Type = i.Type,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate
                })
                .Where(oo => oo.ID == id);
            return Ok(result);
        }
    }
}

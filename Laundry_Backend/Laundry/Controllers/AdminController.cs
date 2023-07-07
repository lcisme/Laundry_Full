using Laundry.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Laundry.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly LaundryContext _context;

        public AdminController(LaundryContext context)
        {
            _context = context;
        }
       

        // admin xem hoa don 
        [HttpPost]
        [Route("admin/invoice")]
        public async Task<ActionResult<IEnumerable<Invoice>>> InfoInvoiceForAdmin(ParaSearch paraSearch)
        {
            //search default phone "" , status Pending, name = "" , type 1 , isPay false, startDate năm trc , endate de năm sau
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
                .Where(oo => oo.Phone.Contains(paraSearch.Phone))
                /* .Where(oo => oo.Status == paraSearch.Status)*/
                /* .Where(oo => oo.Name.Contains(paraSearch.Name))*/
                /*.Where(oo => oo.Type == paraSearch.Type)*/
                .Where(oo => oo.Pay == paraSearch.isPay)
                .Where(oo => oo.Status == paraSearch.Status);
              /*  .Where(oo => oo.StartDate >= Convert.ToDateTime(paraSearch.StartDate));*/
                /*.Where(oo => oo.EndDate <= Convert.ToDateTime(paraSearch.EndDate));*/
            return Ok(result);
        }

        [HttpGet]
        [Route("admin/invoice/all")]
        public async Task<ActionResult<IEnumerable<Invoice>>> InfoInvoiceForAdminall()
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
                }).ToList();
              
            return Ok(result);
        }




        //cap nhat the thanh vien membership, trang thai member

        [HttpGet]
        [Route("admin/updatemembership")]
        public async Task<ActionResult<Membership>> UpdateMemberShip()
        {

            List<Membership> list_member_card = _context.Memberships.Where(oo => oo.EndDate <= DateTime.Now).ToList();
            

            for ( int i = 0; i< list_member_card.Count; i++)
                {
                var customer = await _context.Customers.FindAsync(list_member_card[i].CustomerId);
                customer.IsMember = false;
                _context.Entry(customer).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        [Route("admin/total")]
        public async Task<ActionResult<Membership>> totalInvoice(ParaSearch paraSearch)
        {
            var invoices = _context.Invoices
                .Where(oo => oo.IsPay == true)
                .Where(oo => oo.EndDate >= Convert.ToDateTime(paraSearch.StartDate))
                .Where(oo => oo.EndDate <= Convert.ToDateTime(paraSearch.EndDate)).ToList();
            var total = new Total();
            total.sum = 0;
          
            foreach(var invoice in invoices)
            {
                total.sum = total.sum + invoice.Price;
            }
            return Ok(total);
        }

        // hiện all danh sách khách hàng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }
        // cập nhật thông tin khách hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }
            //customer.IsMember = false;



            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(customer);
        }


        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }


        //tim kiếm khách hàng
        [HttpPost]
        [Route("admin/customer")]
        public async Task<ActionResult<IEnumerable<Invoice>>> InfoCustomerForAdmin(ParaSearch paraSearch)
        {
            var result = _context.Customers
                .Where(oo => oo.Phone.Contains(paraSearch.Phone))
                .Where(oo => oo.Mail.Contains(paraSearch.Mail));
            return Ok(result);
        }


    }
}

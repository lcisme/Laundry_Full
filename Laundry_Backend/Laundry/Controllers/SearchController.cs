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
    public class SearchController : ControllerBase
    {

        private readonly LaundryContext _context;

        public SearchController(LaundryContext context)
        {
            _context = context;
        }


        // nguoi dung xem hoa don 
        [HttpPost]
        [Route("customer/invoice")]
        public async Task<ActionResult<IEnumerable<Invoice>>> InfoInvoiceForCustomer(ParaSearch paraSearch)
        {

            var result = _context.Invoices.Join(
                _context.Customers,
                i => i.CustomerId,
                c => c.Id,
                (i, c) => new
                {
                    ID = i.Id,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate,
                    Price = i.Price,
                    Status = i.Status,
                    Phone = c.Phone,
                    Name = c.Name
                })
                .Where(oo => oo.Phone == paraSearch.Phone)
                .Where(oo => oo.Status.Contains(paraSearch.Status));
             /*   .Where(oo => oo.StartDate >= Convert.ToDateTime(paraSearch.StartDate));*/
                /*.Where(oo => oo.EndDate <= Convert.ToDateTime(paraSearch.EndDate));*/
            return Ok(result);
        }



        // admin xem danh sach khach hang la thanh vien
        [HttpPost]
        [Route("admin/memberlist")]
        public async Task<ActionResult<IEnumerable<Invoice>>> InfoMemberForAdmin(ParaSearch paraSearch)
        {

            var result = _context.Customers.Join(
                _context.Memberships,
                c => c.Id,
                m => m.CustomerId,
                
                (c, m) => new
                {
                    Name = c.Name,
                    Phone = c.Phone,
                    Mail = c.Mail,
                    Address = c.Address,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate

                })
                .Where(oo => oo.Phone.Contains(paraSearch.Phone))
                .Where(oo => oo.Name.Contains(paraSearch.Name))
                .Where(oo => oo.Mail.Contains(paraSearch.Mail))
                .Where(oo => oo.Address.Contains(paraSearch.Address))
                .Where(oo => oo.StartDate >= Convert.ToDateTime(paraSearch.StartDate))
                .Where(oo => oo.EndDate <= Convert.ToDateTime(paraSearch.EndDate));
            return Ok(result);
        }


      

        // xem thong tin member cua khach hang
        [HttpGet("/member/{id}")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInfoMember(int id)
        {

            var result = _context.Memberships.Join(
                _context.Customers,
                m => m.CustomerId,
                c => c.Id,
                (m, c) => new
                {
                    ID = c.Id,
                    Name = c.Name,
                    StartDate = m.EndDate,
                    EndDate = m.EndDate
                })
                .Where(oo => oo.ID == id);
            return Ok(result);
        }

       

    }
}

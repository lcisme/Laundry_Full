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
    public class MembershipsController : ControllerBase
    {
        
            private readonly LaundryContext _context;

            public MembershipsController(LaundryContext context)
            {
                _context = context;
            }

            // GET: api/Memberships
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Membership>>> GetMemberships()
            {
                return await _context.Memberships.ToListAsync();
            }

            // GET: api/Memberships/id
            [HttpGet("{id}")]
            public async Task<ActionResult<Membership>> GetMembership(int id)
            {
                var membership = await _context.Memberships.FindAsync(id);

                if (membership == null)
                {
                    return NotFound();
                }

                return membership;
            }

            // PUT: api/Memberships/id
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPut("{id}")]
            public async Task<IActionResult> PutMembership(int id, Membership membership)
            {
                if (id != membership.Id)
                {
                    return BadRequest();
                }

                _context.Entry(membership).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok(membership);
            }

            // POST: api/Memberships
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPost]
            public async Task<ActionResult<Membership>> PostInvoice(Membership membership)
            {
                _context.Memberships.Add(membership);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (MembershipExists(membership.Id))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetMembership", new { id = membership.Id }, membership);
            }

            // DELETE: api/Memberships/id
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteMembership(int id)
            {
                var membership = await _context.Memberships.FindAsync(id);
                if (membership == null)
                {
                    return NotFound();
                }

                _context.Memberships.Remove(membership);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            private bool MembershipExists(int id)
            {
                return _context.Memberships.Any(e => e.Id == id);
        }
    }
}

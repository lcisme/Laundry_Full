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
using Vonage.Request;
using Vonage;

namespace Laundry.Controllers   
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly LaundryContext _context;

        public PackagesController(LaundryContext context)
        {
            _context = context;
        }

        // GET: api/Packages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Package>>> GetPackages()
        {

          /*  var credentials = Credentials.FromApiKeyAndSecret(
             "f6b7ff97",
            "aCyWTbOmmNaC3gdJ"
            );

            var client = new VonageClient(credentials);
            var response = client.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest()
            {
                To = "84963298031",
                From = "Duy",
                Text = "A text message sent using the Vonage SMS API aaaaaaaaaaaaaa"
            });*/

            return await _context.Packages.ToListAsync();
        }

        // GET: api/Packages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Package>> GetPackage(int id)
        {
            var package = await _context.Packages.FindAsync(id);

            if (package == null)
            {
                return NotFound();
            }

            return package;
        }

        // PUT: api/Packages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPackage(int id, Package package)
        {
            if (id != package.Id)
            {
                return BadRequest();
            }

            _context.Entry(package).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


           
            return Ok(package);
        }

        // POST: api/Packages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Package>> PostPackage(Package package)
        {
            _context.Packages.Add(package);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PackageExists(package.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPackage", new { id = package.Id }, package);
        }

        // DELETE: api/Packages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }

            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PackageExists(int id)
        {
            return _context.Packages.Any(e => e.Id == id);
        }
    }
}

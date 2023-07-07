using Laundry.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Laundry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly LaundryContext _context;

        public TokenController(IConfiguration config, LaundryContext context)
        {
            _configuration = config;
            _context = context;
        }
        private Customer GetCustomer(string phone)
        {
            return _context.Customers.FirstOrDefault(u => u.Phone == phone);
        }



        [HttpPost]
        public IActionResult Post(Customer _customerData)
        {

            if (_customerData != null && _customerData.Phone != null && _customerData.Password != null)
            {
                var customer = GetCustomer(_customerData.Phone);

                if (customer == null)
                {
                    return BadRequest("Invalid credentials");
                }

                bool verified = BCrypt.Net.BCrypt.Verify(_customerData.Password, customer.Password);

                if (customer != null && verified )
                {
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("ID", customer.Id.ToString()),
                    new Claim("Name", customer.Name.ToString()),
                    new Claim("Mail", customer.Mail.ToString()),
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"],
                        claims, expires: DateTime.Now.AddSeconds(3600), signingCredentials: signIn);
                    
                    var tokenresult = new Token();
                    tokenresult.token = new JwtSecurityTokenHandler().WriteToken(token);
                   

                    if (customer.Phone == "123456789")
                    {
                        tokenresult.Id = 1;
                        tokenresult.IdUser = customer.Id;
                        return Ok(tokenresult);
                    }else
                    {
                        tokenresult.Id = 0;
                        tokenresult.IdUser = customer.Id;
                        return Ok(tokenresult);
                    }
                    
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }

        }
    }
}

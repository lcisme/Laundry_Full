using Laundry.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Laundry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly ITwilioRestClient _client;
        public SmsController(ITwilioRestClient client)
        {
            _client = client;
        }
        [HttpPost]
        public IActionResult Sms(SmsMessage model)
        {
            var message = MessageResource.Create(
                to: new PhoneNumber(model.To),
                from: new PhoneNumber(model.From),
                body: model.Messsage,
                client: _client);

            return Ok();
        }
    }
}

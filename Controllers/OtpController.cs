using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TOTPTester.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    { 

        // GET api/<OtpController>/5
        [HttpGet]
        [Route("totpuri")] 
        public string Get(
            [FromQuery(Name = "key")] string key,
            [FromQuery(Name = "user")] string user,
            [FromQuery(Name = "issuer")] string issuer)
        {   
            string totpUrl = TOTP.GetUri(key, user, issuer);       
            return totpUrl;
        }

        // GET api/<OtpController>
        [HttpGet]
        [Route("ValidateTotp/{code}")]
        public bool ValidateTotp(string code)
        {
            string key = "ARISMANTES";
            return TOTP.ValidateTotp(key, code);
        }
 
    }
}

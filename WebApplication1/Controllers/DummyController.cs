using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DummyController : ControllerBase
    {       
        private readonly ILogger<DummyController> _logger;
        private readonly IConfiguration _configuration;
        private readonly MySettings _mysettings;

        public DummyController(
            ILogger<DummyController> logger, 
            IOptionsSnapshot<MySettings> mysettings, 
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _mysettings = mysettings.Value;
        }

        [HttpGet]
        public object Get()
        {
            return new 
            { 
                Url = _mysettings.Url,
                Key = _mysettings.Key,
                AllowedHosts = _configuration["AllowedHosts"]
            };
        }
    }
}

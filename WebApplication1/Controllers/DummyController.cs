using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DummyController : ControllerBase
    {       
        private readonly ILogger<DummyController> _logger;
        private readonly IConfiguration _configuration;        

        public DummyController(
            ILogger<DummyController> logger,             
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;            
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_configuration.AsEnumerable());
        }
    }
}

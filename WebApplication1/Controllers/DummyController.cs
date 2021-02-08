using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DummyController : ControllerBase
    {       
        private readonly IOptionsSnapshot<MySettings> _mySettings;

        public DummyController(IOptionsSnapshot<MySettings> mySettings)
        {            
            _mySettings = mySettings;      
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_mySettings);
        }
    }
}

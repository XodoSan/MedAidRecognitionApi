using Microsoft.AspNetCore.Mvc;

namespace RecognitionApi
{
    [Controller]
    [Route("api/[controller]")]
    public class RecognitionController : ControllerBase
    {
        public IActionResult GetResult()
        {
            return Ok("Порез");
        }
    }
}
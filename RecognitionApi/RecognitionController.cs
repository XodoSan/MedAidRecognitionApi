using Microsoft.AspNetCore.Mvc;

namespace RecognitionApi
{
    [Controller]
    [Route("api/[controller]")]
    public class RecognitionController : ControllerBase
    {
        public IActionResult GetResult()
        {
            List<string> injuries = new List<string> { "Порез", "Ушиб" };
            Random random = new Random();
            int index = random.Next(injuries.Count);
            string randomInjury = injuries[index];
            return Ok(randomInjury);
        }
    }
}
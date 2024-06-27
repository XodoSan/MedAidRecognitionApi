using Microsoft.AspNetCore.Mvc;

namespace RecognitionApi
{
    [Controller]
    [Route("api/[controller]")]
    public class RecognitionController : ControllerBase
    {
        [HttpPost]
        public IActionResult GetResult([FromBody] string[] links)
        {
            List<MockModel> mockModels = new();
            List<string> injuries = new List<string> { "Порез", "Ушиб" };
            Random random = new Random();

            for (int i = 0; i < 3; i++)
            {
                int index = random.Next(injuries.Count);
                string randomInjury = injuries[index];
                mockModels.Add(new MockModel { FileName = Guid.NewGuid().ToString() + ".jpg", InjuryType = randomInjury});
            }

            return Ok(mockModels);
        }

        public class MockModel
        {
            public string FileName { get; set; }
            public string InjuryType { get; set; }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RecognitionApi
{
    [Controller]
    [Route("api/[controller]")]
    public class RecognitionController : ControllerBase
    {
        static readonly HttpClient client = new HttpClient();
        static readonly string detectPath = Path.Combine(Directory.GetCurrentDirectory(), "runs", "detect");
        static readonly string pythonScriptPath = @"C:\Users\Andrew\Desktop\python\testScript.py";
        static readonly string _pythonInterpretatorPath = @"C:\Users\Andrew\AppData\Local\Programs\Python\Python312\python.exe";

        [HttpPost]
        public async Task<IActionResult> GetResult([FromBody] string[] urls)
        {
            string inputFolderPath = @"C:\Users\Andrew\Desktop\InputPictures";
            string outputFolderPath = @"C:\Users\Andrew\Desktop\OutputPictures";
            DirectoryInfo directoryInfo = new DirectoryInfo(inputFolderPath);
            directoryInfo.Delete(true);
            Directory.CreateDirectory(inputFolderPath);
            List<MockModel> mockModels = new();

            foreach (string url in urls)
            {
                string fileName = Path.GetFileName(new Uri(url).AbsolutePath);
                string newFileName = Guid.NewGuid().ToString() + fileName + ".jpg";
                string filePath = Path.Combine(inputFolderPath, newFileName);

                byte[] imageBytes = await client.GetByteArrayAsync(url);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
            }

            var output = ExecuteScript(pythonScriptPath);
            // Регулярное выражение для извлечения имени файла и класса
            Regex regex = new Regex(@"image 1/1 (.*): .* (\d+ \w+),");
            Regex regexNoDetections = new Regex(@"image 1/1 (.*): .* \(no detections\),");

            // Ищем все совпадения в выводе Python
            MatchCollection matches = regex.Matches(output);
            MatchCollection matchesNoDetections = regexNoDetections.Matches(output);

            // Выводим результаты
            foreach (Match match in matches)
            {
                mockModels.Add(new MockModel { FileName = Path.GetFileName(match.Groups[1].Value), InjuryType = match.Groups[2].Value });
            }

            // Выводим результаты для изображений без обнаружений
            foreach (Match match in matchesNoDetections)
            {
                mockModels.Add(new MockModel { FileName = Path.GetFileName(match.Groups[1].Value), InjuryType = "No detections" });
            }

            DirectoryInfo ultralyticsInfo = new(detectPath);
            var currentDirectory = ultralyticsInfo
                .GetDirectories()
                .OrderByDescending(x => x.CreationTime)
                .FirstOrDefault();

            foreach (var file in currentDirectory.GetFiles())
            {
                string outputFilePath = Path.Combine(outputFolderPath, file.Name);
                System.IO.File.Move(Path.Combine(currentDirectory.FullName, file.Name), outputFilePath);                
            }

            return Ok(mockModels);
        }

        public void RegexReplacePath(string[] picturesPath)
        {
            string pattern = @"(directory\s*=\s*)";
            string newPythonCode = Regex.Replace(pythonScriptPath, pattern, "$1" + picturesPath);
        }

        public string ExecuteScript(string scriptPath)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(_pythonInterpretatorPath, scriptPath);
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }

        public class MockModel
        {
            public string FileName { get; set; }
            public string InjuryType { get; set; }
        }
    }
}
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GlobalAzureBootcamp
{
    public static class GlobalAzureHash
    {
        [FunctionName("GlobalAzureHash")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("###### Functions called");

            string toHash = req.Query["toHash"];
            if(string.IsNullOrWhiteSpace(toHash))
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                toHash = data?.toHash;
            }

            if(string.IsNullOrWhiteSpace(toHash))
            {
                log.LogError("###### No content to hash");
                new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }
            log.LogInformation($"###### toHash: '{toHash}'");

            string hash = GetStringSha256Hash(toHash);

            string response = $"'{toHash}' - '{hash}'";
            log.LogInformation($"###### Response: {response}");

            return new OkObjectResult(response);
        }

        internal static string GetStringSha256Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}

using Newtonsoft.Json;
using SampleJsonGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonExampleSenarios
{
    public class JsonPopulateDemo
    {
        public static void Show()
        {
            Console.WriteLine("Json PopulateObject Example ");
            var userLogs = Generator.GetUserInteractions(10000);

            string jsonReviewed = @"{
                'reviewed': true,
                'processedBy': ['ReviewerProcess'],
                'reviewedDate': '" + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK") + @"' 
            }";

            foreach(var log in userLogs)
            {
                JsonConvert.PopulateObject(jsonReviewed, log);
            }

            Console.WriteLine($"reviewed : {userLogs[1].reviewed} ," +
                $"  processedBy: {string.Join(" | " , userLogs[1].processedBy)} ,  " +
                $"reviewedDate : {userLogs[1].reviewedDate}");

        }
    }
}

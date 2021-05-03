using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SampleJsonGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonExampleSenarios
{
    public class JsonFragments
    {
        public static void Show()
        {
            Console.WriteLine("Json Fragments Example");
            var logView = Generator.GetUserInteractions(10000);
            var jsonLogViews = JsonConvert.SerializeObject(logView);

            DeserialiseWithJsonConvert(jsonLogViews);
            DeserialiseWithFragments(jsonLogViews);
        }

        private static void DeserialiseWithJsonConvert(string bigLog)
        {
            Console.WriteLine("-Deserilize the entire Json Text");
            var totalSecondsWatched = 0;
            try
            {
                var logViews = JsonConvert.DeserializeObject<List<UserInteraction>>(bigLog);
                foreach(var log in logViews)
                {
                    totalSecondsWatched +=  log.courseView.secondsWatched;
                }

                Console.WriteLine("Total watched: " + totalSecondsWatched + " ms");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void  DeserialiseWithFragments(string bigLog)
        {
            var jsonLogArray = JArray.Parse(bigLog);
            var totalTimeWatched = 0;
            foreach(var jsonLogObject in jsonLogArray)
            {
                var secondsWatchde = jsonLogObject["courseView"]["secondsWatched"].Value<int>();
                totalTimeWatched += secondsWatchde;
            }
            Console.WriteLine($"Total Time Watched : {totalTimeWatched} ms");
        }
    }
}

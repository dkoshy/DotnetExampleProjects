using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SampleJsonGenerator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonExampleSenarios
{
    public class ReadAndQueryJson
    {
        public static void ShowJsonRead()
        {
            Console.WriteLine("Reading Json using LINQ to JSON");

            var dataJson = Generator.GetCourseViewString(100);

            var parsedJsonData = JObject.Parse(dataJson);

            var jArrayView = parsedJsonData["views"].Value<JArray>();

            Console.WriteLine("_ First Course view");
            Console.WriteLine(jArrayView[0].ToString());

            Console.WriteLine("_ Get properties of first view ");
            Console.WriteLine(jArrayView[0]["userId"]);
            Console.WriteLine(jArrayView[0]["user"].Value<string>());

            int seconds = (int)jArrayView[0]["secondsWatched"];
            int otherSeconds = jArrayView[0].Value<int>("secondsWatched");

            Console.WriteLine($" Seconds : {seconds}  ,  OtherSeconds : {otherSeconds} ");

            DateTime date = (DateTime)jArrayView[0]["watchedDate"];
            Console.WriteLine($"date : {date}");

            Console.WriteLine($"Total Views : + { ((jArrayView?.Count == null) ? 0 : jArrayView.Count ) } " );

            Console.WriteLine("Next: " + jArrayView[0].Next["user"]);
            Console.WriteLine("Last: " +  jArrayView.Last["user"] );
        }

        public static void ShowJsonQuery()
        {
            Console.WriteLine("Quering JSON data using LINQ");

            string courseViews = Generator.GetCourseViewString(100);

            var jAllView = JObject.Parse(courseViews).Value<JArray>("views");

            var SecondsWatchedList = jAllView.Select(v => v.Value<int>("secondsWatched"));
            var totalseconds = 0;

            SecondsWatchedList.ToList().ForEach(s => totalseconds += s);
            Console.WriteLine("Total Seconds Watched : " + totalseconds);

            Console.WriteLine("Total Seconds Watched : " + jAllView.Sum(v=> v.Value<int>("secondsWatched")));
            Console.WriteLine("Total Seconds Watched :" + jAllView.Average(v => v.Value<int>("secondsWatched")));

            //Group by cluse 
            var watchedBy = jAllView.GroupBy(v => v["course"], v => v)
                                    .Select(vg => new
                                    {
                                        course = vg.Key,
                                        count = vg.Count(),
                                        totalWatchedhours = vg.Sum(v => v["secondsWatched"].Value<int>()),
                                        minWatchedDuration = vg.Min(v=> v["secondsWatched"].Value<int>()),
                                        maxWatchedDuration = vg.Max(v=> v.Value<int>("secondsWatched"))
                                    });

            watchedBy.ToList().ForEach(d =>
            {
                Console.WriteLine($"{d.course} -  {d.count} - {d.totalWatchedhours} - " +
                    $"{d.minWatchedDuration} - " +
                    $"{d.maxWatchedDuration}");
            });

            //select only "solar corse"

            var solarCourseView = new JArray() {
                jAllView.Where(v => v["course"].Value<string>() == "Solar")
                                           .Select(sv => new JObject
                                           {
                                               {
                                                   "course" , sv.Value<string>("course")
                                               },
                                               {
                                                   "WatchedDuration" , sv.Value<string>("secondsWatched")
                                               },
                                               {
                                                   "date", sv.Value<DateTime>("watchedDate")
                                               }
                                           }).Take(10) };

            Console.WriteLine(solarCourseView.ToString());
        }

        public static void SelectTokenDemo()
        {
            Console.WriteLine("***Select Token Example *****");

            string authorJson = Generator.SingleJson();

            string logViews = Generator.GetCourseViewString(100);

            var jauthorObject = JObject.Parse(authorJson);

            Console.WriteLine($"{jauthorObject.SelectToken("name")}");
            Console.WriteLine($"{jauthorObject.SelectToken("courses")[0]}");

            //Iterating through Courses

            foreach(var c in jauthorObject.SelectToken("courses").Select(c => c.ToString() ))
            {
                Console.WriteLine(c);
            }

            //using JSON Path Syntax
            Console.WriteLine();
            foreach(var c in jauthorObject.SelectToken("$.courses"))
            {
                Console.WriteLine(c);
            }

            var jAlllogView = JObject.Parse(logViews).SelectToken("views");

            Console.WriteLine(jAlllogView[0]);

            Console.WriteLine(jAlllogView.SelectToken("$.[0].user"));

            jAlllogView.SelectTokens("$.[?(@.course == 'Solar')]").Take(10).ToList().ForEach(c =>
            {

                Console.WriteLine(c);

            });

            Console.WriteLine(jAlllogView.SelectTokens("$.[*].secondsWatched").Sum(t => t.Value<int>()));

            //only solar course

            Console.WriteLine(jAlllogView.SelectTokens("$.[?(@.course == 'Solar')].secondsWatched").Sum(t => t.Value<int>()));

            //group by 
            var groupedData = jAlllogView.SelectTokens("$.[?(@.secondsWatched > 600)]")
                                          .GroupBy(vg => vg["course"], vg => vg)
                                          .Select(g => new
                                          {
                                              Course = g.Key,
                                              Count = g.Count(),
                                              logData = new JArray { g.ToList() }.ToObject<List<CourseView>>()
                                          });

            groupedData.ToList().ForEach(d => {

                Console.WriteLine($"{d.Course} -  {d.Count} -  {d.logData?[0].user} -  {d.logData.Sum(k=> k.secondsWatched)}");
            });

            var parsedData = jAlllogView.SelectTokens("$.[?(@.secondsWatched > 600 && @.course == 'Solar')].secondsWatched");

            Console.WriteLine($"{parsedData.Count()} {parsedData.Sum(j=>j.Value<int>())}");

            //creating new json

            var newJobject = new JObject();
            var newJGropedArray = new JArray();

            groupedData.ToList().ForEach(j =>
            {
                newJGropedArray.Add(new JObject
                {
                    {"courseName" , j.Course },
                    {"totalCount" , j.Count },
                    {"totalWatchedduration" , j.logData.Sum(d => d.secondsWatched)}
                });
               
            });

            newJobject.Add(new JProperty("gropedViews", newJGropedArray));


            Console.WriteLine(JsonConvert.SerializeObject(newJobject, Formatting.Indented));
            Console.WriteLine(newJobject);
        }
    }
}

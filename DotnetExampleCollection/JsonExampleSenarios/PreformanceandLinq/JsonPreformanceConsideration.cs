using Newtonsoft.Json;
using SampleJsonGenerator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonExampleSenarios
{
    public class JsonPreformanceConsideration
    {
        public static void Show()
        {
            //Serialisation

            Console.WriteLine("Default creation of Json Course view");

            var courseviews = Generator.GetCourseViews(1000);
            Console.WriteLine($"Total Number of Courses : {courseviews.Count}");
          
            var jsoncourseView1 = JsonDefaultSerilisation(courseviews);

            Console.WriteLine("Manual creation of Json Course view");

            var josnCorseview2 = JsonManaulSerilisation(courseviews);

            //Deserialisation 

            Console.WriteLine("Default Deserilisation");

            JsonDefaultDeserialisation(jsoncourseView1);

            Console.WriteLine("Manual Deserilisation");

            JsonManualDeserilisationToFindSolar(jsoncourseView1);

            JsonManaulSerilisation(josnCorseview2);

        }

        private static void JsonManaulSerilisation(string josnCorseview2)
        {
            //Mnual Serilisation of Json courseView

            var courseViewList = new CourseViewList();
            CourseView courseview = null;

            var watch = new Stopwatch();
            watch.Start();

            using (var jsonReader = new JsonTextReader(new StringReader(josnCorseview2)))
            {
                var arraystarted = false;

                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.StartArray)
                    {
                        arraystarted = true;
                    }

                    if (arraystarted == true && jsonReader.TokenType == JsonToken.StartObject)
                    {
                        courseview = new CourseView();

                        while (jsonReader.Read())
                        {
                            var propertyName = string.Empty;

                            if (jsonReader.TokenType == JsonToken.PropertyName)
                            {
                                propertyName = jsonReader.Value.ToString();
                            }
                            else
                            {
                                switch (propertyName)
                                {
                                    case "userId":
                                        courseview.userId = int.Parse(jsonReader.Value?.ToString());
                                        break;
                                    case "user":
                                        courseview.user = jsonReader.Value?.ToString();
                                        break;
                                    case "course":
                                        courseview.course = jsonReader.Value?.ToString();
                                        break;
                                    case "watchedDate":
                                        courseview.watchedDate = DateTime.Parse(jsonReader.Value?.ToString());
                                        break;
                                    case "secondsWatched":
                                        courseview.secondsWatched = int.Parse(jsonReader.Value?.ToString());
                                        break;

                                }
                            }

                            if (jsonReader.TokenType == JsonToken.EndObject)
                            {
                                courseViewList.courseViews.Add(courseview);
                                break;
                            }

                        }

                    }

                }
            }

            watch.Stop();


            Console.WriteLine($"Total number of Course View {courseViewList.courseViews.Count}, Elapsed Time {watch.ElapsedMilliseconds} ms");
        }

        private static void JsonManualDeserilisationToFindSolar(string jsoncourseView)
        {
            var totalwatchedTimeForsolar = 0;
            var currentCourse = string.Empty;
            var watch = new Stopwatch();
            watch.Start();
            using (var jsonTextReader = new JsonTextReader(new StringReader(jsoncourseView)))
            {
                while (jsonTextReader.Read())
                {
                    if (jsonTextReader.TokenType == JsonToken.String
                        && jsonTextReader.Value?.ToString() == "Solar")
                    {
                        currentCourse = "Solar";
                    }


                    if (currentCourse == "Solar" && jsonTextReader.Value?.ToString() == "secondsWatched")
                    {
                        jsonTextReader.Read();
                        totalwatchedTimeForsolar += int.Parse(jsonTextReader.Value?.ToString());
                    }

                    if (jsonTextReader.TokenType == JsonToken.EndObject)
                    {
                        currentCourse = string.Empty;
                    }

                }
            }

            watch.Stop();

            Console.WriteLine($"Total Time Watched for solar : {totalwatchedTimeForsolar}  , Elapsed Time {watch.ElapsedMilliseconds} ms");

        }

        private static void JsonDefaultDeserialisation(string jsoncourseView)
        {
            //find total watched course for 'Solar'
            var watch = new Stopwatch();
            watch.Start();

            var couseViews = JsonConvert.DeserializeObject<List<CourseView>>(jsoncourseView);
            var totalwatchedTimeForsolar = 0;
            foreach (var courseview in couseViews)
            {
                if (courseview.course == "Solar")
                {
                    totalwatchedTimeForsolar += courseview.secondsWatched;
                }

            }

            watch.Stop();

            Console.WriteLine($"Total Watched time Is {totalwatchedTimeForsolar} , Elapsed Time {watch.ElapsedMilliseconds} ms ");
        }

        public static string JsonDefaultSerilisation(List<CourseView> courseViews)
        {

            Stopwatch watch = new Stopwatch();
            watch.Start();
            var jsonCourseViews = JsonConvert.SerializeObject(courseViews, Formatting.Indented);
            watch.Stop();
            Console.WriteLine($"Elapsed time is {watch.ElapsedMilliseconds} ms");
            return jsonCourseViews;
        }

        public static string JsonManaulSerilisation(List<CourseView> courseViews)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var jsonString = new StringBuilder();
            var jsonStringWriter = new StringWriter(jsonString);

            using (var jsonTextwriter = new JsonTextWriter(jsonStringWriter))
            {
                jsonTextwriter.Formatting = Formatting.Indented;
                jsonTextwriter.WriteStartObject();

                jsonTextwriter.WritePropertyName("courseViews");

                jsonTextwriter.WriteStartArray();

                foreach (var courseview in courseViews)
                {
                    jsonTextwriter.WriteStartObject();

                    jsonTextwriter.WritePropertyName("userId");
                    jsonTextwriter.WriteValue(courseview.userId);

                    jsonTextwriter.WritePropertyName("user");
                    jsonTextwriter.WriteValue("user_" + courseview.userId);

                    jsonTextwriter.WritePropertyName("course");
                    jsonTextwriter.WriteValue(courseview.course);

                    jsonTextwriter.WritePropertyName("watchedDate");
                    jsonTextwriter.WriteValue(courseview.watchedDate);

                    jsonTextwriter.WritePropertyName("secondsWatched");
                    jsonTextwriter.WriteValue(courseview.secondsWatched);

                    jsonTextwriter.WriteEndObject();
                }

                jsonTextwriter.WriteEndArray();
                jsonTextwriter.WriteEndObject();

            }

            watch.Stop();
            Console.WriteLine($"Elapsed time is {watch.ElapsedMilliseconds} ms");

            return jsonString.ToString();

        }
    }
}

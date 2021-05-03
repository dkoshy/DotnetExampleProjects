using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleJsonGenerator
{
  public static class Generator
    {

        static List<string> courses = new List<string>() { "Solar", "DotTrace", "Jira" };
        static Random randomGenerator  =  new Random();
        /// <summary>
        /// Single Json Example
        /// </summary>
        /// <returns></returns>
        public static string SingleJson()
        {
            return File.ReadAllText("AuthorSingle.json");
        }

        public static string SingleJosnError()
        {
            return File.ReadAllText("AuthorSingleError.json");
        }

        public static string ExtendedSingleJson()
        {
            return File.ReadAllText("AuthorExtended.json");
        }

        public static string SmallJson()
        {
            return File.ReadAllText("AuthorSmall.json");

        }

        public static string DateJson()
        {
            return File.ReadAllText("AuthorDateJson");
        }

       public static List<CourseView> GetCourseViews(int numberOfViews)
        {
            var courseViews = new List<CourseView>();
            for(int i = 0; i<numberOfViews; i++)
            {
                int generatedId = randomGenerator.Next(9999, 1000000);

                CourseView courseView = new CourseView
                {
                    userId = generatedId,
                    user = "user_" + generatedId,
                    course = courses[randomGenerator.Next(courses.Count())],
                    watchedDate = new DateTime(2015, 07, randomGenerator.Next(1, 28)),
                    secondsWatched = randomGenerator.Next(0, 1000)
                };
                courseViews.Add(courseView);
            }
            return courseViews;
        }

        public static string GetCourseViewString(int numberOfViews)
        {
            List<CourseView> courseViews = GetCourseViews(numberOfViews);

            dynamic jsonResult = new ExpandoObject();
            jsonResult.views = courseViews;
            return JsonConvert.SerializeObject(jsonResult, Formatting.Indented);

            //return JsonConvert.SerializeObject(courseViews, Formatting.Indented);
        }

        public static List<UserInteraction> GetUserInteractions(int numberOfInteractions)
        {
            List<UserInteraction> userLogs = new List<UserInteraction>();
            for (int i = 0; i < numberOfInteractions; i++)
            {
                UserInteraction uI = new UserInteraction();
                uI.GenerateFakeLog();
                userLogs.Add(uI);
            }
            return userLogs;
        }


    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonExampleSenarios
{
    public class CeateJsonWithLINQDeo
    {

        public static void ShowImperative()
        {
            Console.WriteLine("Imperative Way of Creating Json");

            var jObjectauthor = new JObject();

            jObjectauthor.Add("author", "Deepak Koshy");

            var jCourses = new JArray();
            jCourses.Add(new JValue("Solar"));
            jCourses.Add(new JValue("Python"));

            var dotTrace = new JValue("DoTrace");
            var jira = new JValue("Jira");

            jCourses.Add(jira);

            jira.AddBeforeSelf(dotTrace);

            jObjectauthor.Add("courses", jCourses);

            var dateSince = new JValue(new DateTime(2020, 12, 02));
            jObjectauthor.Add("since", dateSince);

            Console.WriteLine(jObjectauthor);

            //Converting JObject to a Dynamic Object

            dynamic dynamicAuthor = jObjectauthor;

            dynamicAuthor.happy = true;
            dynamicAuthor.issue = null;

            Console.WriteLine(dynamicAuthor);

        }

        public static void ShowDeclarative()
        {

            Console.WriteLine("Declarative Way of Craeting Json");

            var jauthor = new JObject()
            {
                new JProperty("name","Deepak Koshy"),
                new JProperty("courses", new JArray()
                {
                    new JValue("Solar"),
                    new JValue("Python"),
                    new JValue("Hadoop")
                }),
                new JProperty("since", DateTimeOffset.UtcNow ),
                new JProperty("car", new JObject()
                {
                    new JProperty("model" , "1990"),
                    new JProperty("")
                }),
               new JProperty("happy", true),
               new JProperty("issues", null)
   
            };

            jauthor.Remove("happy");

            Console.WriteLine(jauthor);

        }

        public static void ShowFromObject()
        {
            Console.WriteLine("Creating json from Objects");

            var courses = new List<string> { "Solar", "C#", "Javascript" };

            dynamic dauthor = new ExpandoObject();

            dauthor.name = "Deepak Koshy";
            dauthor.courses = from c in courses
                              orderby c
                              select c;

            dauthor.since = DateTimeOffset.UtcNow;
            dauthor.happy = true;
            dauthor.issues = null;
            Console.WriteLine(JObject.FromObject(dauthor));

        }

    }
}

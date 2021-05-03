using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SampleJsonGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonExampleSenarios
{
    public class ParseJsonDemo
    {
        public static void ShowStringParse()
        {
            Console.WriteLine("Parse json from String");
            //JObject(), Jarray() and JToken()


            var sigleAuthor = Generator.SingleJson();

            var stringArray = @"[{model : 'ALTO 800'  , year : 2020}
                                ,{model : 'Celerio'  , year : 2018}
                                ,{model : 'Siwt'  , year : 2019}]";
            var stringValue = @"{name : 'Deepak'}";

            var jAuthor = JObject.Parse(sigleAuthor);
            var jarray = JArray.Parse(stringArray);
            var jtoken = JToken.Parse(stringValue);

            Console.WriteLine(jAuthor.First);
            Console.WriteLine(jAuthor.Last);
            var car = jarray[0].ToObject<Car>();
            Console.WriteLine($"{car.model} , {car.year}");
            Console.WriteLine(jtoken["name"]);
        }

        public static void ShowStreamParse()
        {
            Console.WriteLine("---Parse From stream -----");

            JObject authorOject;
            
            using(StreamReader stReader = new StreamReader("AuthorSingle.json"))
            {
                authorOject = (JObject)JToken.ReadFrom(new JsonTextReader(stReader));
            }

            Console.WriteLine(authorOject.First);
            Console.WriteLine(authorOject.Last);
            Console.WriteLine(authorOject.Children().ElementAt(3));
            Console.WriteLine(authorOject["car"].ToObject<Car>());
            Console.WriteLine(authorOject.SelectToken("name"));
        }
    }
}

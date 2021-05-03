using Newtonsoft.Json;
using SampleJsonGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonExampleSenarios
{
    public class JsonBasicConceptsDemo
    {
        public static void ShowJsonTextReader()
        {
            Console.WriteLine("--Json Text Reader--");

            var strJson = Generator.SingleJson();

            using(var strReader = new StringReader(strJson))
            {
                var jsonTextReader = new JsonTextReader(strReader);
                while (jsonTextReader.Read())
                {
                   if(jsonTextReader.TokenType == JsonToken.PropertyName)
                    {
                        var tokenName = jsonTextReader.Value;
                        jsonTextReader.Read();

                        Console.WriteLine($"{tokenName} : {jsonTextReader.Value}");
                    }
                }
            }
        }

        public static void ShwoJsonWriter()
        {
            Console.WriteLine("--Json Text Writer --");

            var jsonstring = new StringBuilder();

            var strWriter = new StringWriter(jsonstring);

          using(var writer = new JsonTextWriter(strWriter))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartObject();
                writer.WritePropertyName("name");
                writer.WriteValue("Deepak Koshy");
                writer.WritePropertyName("courses");
                writer.WriteStartArray();
                writer.WriteValue("Solar");
                writer.WriteValue("C#");
                writer.WriteValue("Java");
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            Console.WriteLine(strWriter.ToString());
        }

        public static void ShowJsonSerlizer()
        {
            Console.WriteLine("-Json Serlizer Demo-");

            Author Deepak = new Author()
            {
                name = "Deepak",
                courses = new string[] { "Solr", "Spark", "Jira" },
                happy = true,
                since = new DateTime(2014, 01, 15)
            };


            using(var streamWriter = new StreamWriter("1_1_authorData.json"))
            {
                var serlializer = new JsonSerializer();
                serlializer.Serialize(streamWriter, Deepak);
            }


            using (var streamWriter = new StreamWriter("1_2_authorData.json"))
            {
                var serlializer = new JsonSerializer();
                serlializer.Formatting = Formatting.Indented;
                serlializer.Serialize(streamWriter, Deepak);
            }


            Console.WriteLine(File.ReadAllText("1_1_authorData.json"));

            Console.WriteLine(File.ReadAllText("1_2_authorData.json"));
        }
    }
}

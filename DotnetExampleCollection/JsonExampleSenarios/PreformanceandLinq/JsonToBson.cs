using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SampleJsonGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonExampleSenarios
{
    public class JsonToBson
    {
        public static void showBisonSerilisation()
        {
            Console.WriteLine("--Read json Data--");
            var jsostr = File.ReadAllText("AuthorSingle.json");

            var authorOject = JsonConvert.DeserializeObject<Author>(jsostr);

            var bsonMemorySteam = new MemoryStream();

            using(var bisonDataWriter = new BsonWriter(bsonMemorySteam))
            {
                var jsonSerilizer = new JsonSerializer();
                jsonSerilizer.Serialize(bisonDataWriter, authorOject);
            }

            var bsonstring = Convert.ToBase64String(bsonMemorySteam.ToArray());
            Console.WriteLine("- Bison String -");
            Console.WriteLine(bsonstring);

            //Deserilise bison string
            var bsonByteArray = Convert.FromBase64String(bsonstring);
            var anothermemeoryStream = new MemoryStream(bsonByteArray);

            Author AnothetAuthor;
            using(var bsonstreamReader = new BsonReader(anothermemeoryStream))
            {
                var jsonDataSerilizer = new JsonSerializer();
                AnothetAuthor = jsonDataSerilizer.Deserialize<Author>(bsonstreamReader);
            }
            Console.WriteLine(AnothetAuthor.name);
        }
    }
}

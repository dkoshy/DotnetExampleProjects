using Movies.Client.Models;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Movies.Client
{
    public static class ClientExtentions 
    {
        public static T ReadFromStream<T>(this Stream stream)
        {
            T streamResult;
            using (var streamreader = new StreamReader(stream))
            {
                using (var jsontext = new JsonTextReader(streamreader))
                {
                    var jsonserlizer = new JsonSerializer();
                    streamResult = jsonserlizer.Deserialize<T>(jsontext);
                }
            }

            return streamResult;
        }

        public static void WriteToStream<T>(this Stream memoryStreamcontent , T ObjectToWrite)
        {
            using (var streameWriter = new StreamWriter(memoryStreamcontent, new UTF8Encoding(), 1024, true))
            {
                using (var jsonWriter = new JsonTextWriter(streameWriter))
                {
                    var jsonserliser = new JsonSerializer();
                    jsonserliser.Serialize(jsonWriter, ObjectToWrite);
                    jsonWriter.Flush();
                }
            }
        }
    }
}

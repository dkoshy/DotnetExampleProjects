using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using JFormatting = Newtonsoft.Json.Formatting;

namespace JsonExampleSenarios
{
    public class JsonToXmlDemo
    {
        public static void ShowToandFrom()
        {
            Console.WriteLine("converting from XML to JSON");

            var xmlstingDoc = File.ReadAllText("SampleAuthors.xml");
            Console.WriteLine("-Xml string Document-");
            Console.WriteLine(xmlstingDoc);

            var xmlAuthorDoc = new XmlDocument();
            xmlAuthorDoc.LoadXml(xmlstingDoc);

            var jsonAuthorDoc = JsonConvert.SerializeXmlNode(xmlAuthorDoc, JFormatting.Indented);
            Console.WriteLine(jsonAuthorDoc);


            //convert json to xml

            xmlAuthorDoc = JsonConvert.DeserializeXmlNode(jsonAuthorDoc);
            Console.WriteLine("-converted XML doc - ");
            Console.WriteLine(xmlAuthorDoc.InnerXml);

            //convert usong XNode
            var xAuthorDoc = JsonConvert.DeserializeXNode(jsonAuthorDoc);
            Console.WriteLine("\n-Converte using XNode -");
            Console.WriteLine(xAuthorDoc);
        }

        public static void ShowForceArray()
        {
            Console.Clear();
            Console.WriteLine("---Force Array---");

            var strXDoc = File.ReadAllText("SampleForceArray.xml");
            Console.WriteLine("xml string data");
            Console.WriteLine(strXDoc);

            var parsedXdoc = XDocument.Parse(strXDoc);
            
            var jsonxmldoc = JsonConvert.SerializeXNode(parsedXdoc,JFormatting.Indented);
            Console.WriteLine("Json Parsed string Data");
            Console.WriteLine(jsonxmldoc);

            var toxmlDoc = JsonConvert.DeserializeXNode(jsonxmldoc,"TestXMLDoc");
            Console.WriteLine("-Json XML Data Converted");
            Console.WriteLine(toxmlDoc);

        }

        public static void Shoncompatibilities()
        {
            Console.WriteLine("--Show Data Incompatiblities--");

            var xmlDocStr = File.ReadAllText("SampleComprehensive.xml");

            var parsedXml = XDocument.Parse(xmlDocStr);
            Console.WriteLine("--original XML Document--");
            Console.WriteLine(parsedXml.ToString());

            var jsonXml = JsonConvert.SerializeObject(parsedXml, JFormatting.Indented);
            Console.WriteLine("--json XML document ---");
            Console.WriteLine(jsonXml);



        }
    }
}

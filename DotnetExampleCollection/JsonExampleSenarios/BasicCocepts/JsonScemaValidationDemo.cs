using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SampleJsonGenerator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonExampleSenarios
{
    public class JsonScemaValidationDemo
    {
        public static void ShowManualValidation()
        {
            Console.WriteLine("--Manual Validation using JsonSchema--");

            var jsonstr = Generator.SingleJson();
            var jauthorObject = JObject.Parse(jsonstr);

            var manualSchema = ManauallyGenerateSchema();
            IList<string> why;
            bool validateManual = jauthorObject.IsValid(manualSchema, out why);

            Console.WriteLine($"Validation Success : {validateManual} ");
            why.ToList().ForEach(r =>
            {
                Console.WriteLine($"Error : {r}");
            });
        }

        public static void ShowGenerateValidation()
        {
            Console.WriteLine("--Generate Data Validation withAuto Generated");

            var jSchemaGenerator = new JsonSchemaGenerator();
            var generateSchema = jSchemaGenerator.Generate(typeof(AnotherAuthor));
            Console.WriteLine(generateSchema.ToString());

            var jsostr = Generator.SingleJson();
            var jObjectauthor = JObject.Parse(jsostr);

            Console.WriteLine($"Is a Valid Schema : {jObjectauthor.IsValid(generateSchema)}");
        }


        private static JSchema ManauallyGenerateSchema()
        {
            var schema = new JSchema
            {
                Type = JSchemaType.Object,

                Properties =
                {
                    {"name" , new JSchema{ Type = JSchemaType.String  } },

                    { "courses", new JSchema{
                         Type = JSchemaType.Array,
                         Items = { new JSchema { Type = JSchemaType.String } }
                    } },
                    { "since", new JSchema { Type = JSchemaType.String } },
                    { "happy", new JSchema { Type = JSchemaType.Boolean } },
                    { "issues", new JSchema { Type = JSchemaType.Object } },
                    { "car", new JSchema { Type = JSchemaType.Object } },
                    { "authorRelationship", new JSchema { Type = JSchemaType.Integer } },

                },
               
            };

            return schema;
        }
    }
}

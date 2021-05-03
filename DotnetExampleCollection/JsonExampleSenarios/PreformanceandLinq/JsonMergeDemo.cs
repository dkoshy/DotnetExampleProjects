using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonExampleSenarios
{
    public class JsonMergeDemo
    {
        public static void Show()
        {
            Console.WriteLine("Merging Two Json and  Array merging options");

            // Two JSON objects
            string authorComments = @"{

                'name': 'Xavier',
                'comments': ['Helpful Solr course(1)', 'Please provide better Jira samples(2),', 'Python(4)'],
                'upVotes': 5
            }";

            string newComment = @"{

                'name': 'Xavier',
                'comments': ['Quick dotTrace tutorials!(3)', 'Helpful Solr course(1)', 'python(4)'],
                'upVotes': 1,
                'reviewer' : 'Deepak Koshy'
            }";


            var jData = JObject.Parse(newComment);
            var comments = jData["comments"].ToObject<List<string>>();
            jData["comments"] = new JArray(comments.Select(c => c.ToLower()));

            Console.WriteLine(jData.ToString());

            //merging with concat option

            var jAuthorConcat = JObject.Parse(authorComments);
            var jautherNewConcat = JObject.Parse(newComment);

            jAuthorConcat.Merge(jautherNewConcat, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Concat
            });

            Console.WriteLine(jAuthorConcat.ToString());

            //merging with union 

            var jAuthorUnion = JObject.Parse(authorComments);
            var jautherNewUnion = JObject.Parse(newComment);

            jAuthorUnion.Merge(jautherNewUnion, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union,
                
            });

            Console.WriteLine(jAuthorUnion.ToString());

            // merging with replace

            var jAuthorReplace = JObject.Parse(authorComments);
            var jautherNewReplace = JObject.Parse(newComment);

            jAuthorReplace.Merge(jautherNewReplace, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Replace

            });

            Console.WriteLine(jAuthorReplace.ToString());

            //merging with merge

            var jAuthorMerge = JObject.Parse(authorComments);
            var jauthorNewMerge = JObject.Parse(newComment);

            jAuthorMerge.Merge(jauthorNewMerge, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Merge
            });

            Console.WriteLine(jAuthorMerge.ToString());
        }


    }
}

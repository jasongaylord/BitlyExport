using BitlyAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BitlyExport
{
    class Program
    {
        private const string token = "REPLACE_WITH_ACCESS_TOKEN";
        private const string group_id = "REPLACE_WITH_GROUP_ID";
        private const int page_size = 100;
        private const int sleep_seconds = 1;
        private const string output_file = "C:\\bitlinks.json";
        public static int total_links { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Preparing to process...");
            SaveLinks();
            Console.ReadLine();
        }

        static async void SaveLinks()
        {
            var bitly = new Bitly(token);
            var linksPage1 = await bitly.GetBitlinksByGroup(group_id, page_size);
            total_links = linksPage1.Pagination.Total;

            var links = new List<BitlyLink>();
            links.AddRange(linksPage1.Links);

            var page_count = (total_links / page_size);
            if ((total_links % page_size) > 0) { page_count++; }

            Console.WriteLine("Found " + total_links + " bitlink(s).");
            Console.WriteLine("Processing page 1 of " + page_count + "; Adding " + linksPage1.Links.Count + " links.");

            for (var x = 2; x <= page_count; x++)
            {
                var additionalLinks = await bitly.GetBitlinksByGroup(group_id, page_size, x);
                Console.WriteLine("Processing page " + x + " of " + page_count + "; Adding " + additionalLinks.Links.Count + " links.");
                links.AddRange(additionalLinks.Links);
                Thread.Sleep(sleep_seconds);
            }

            var list = JsonConvert.SerializeObject(links);
            File.WriteAllText(output_file, list);

            Console.WriteLine("Press any key to close.");
            Console.ReadLine();
        }
    }
}

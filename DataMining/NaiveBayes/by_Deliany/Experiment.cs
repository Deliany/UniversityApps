using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace by_Deliany
{
    class Experiment
    {
        public ArrayList FilmNames { get; set; }


        public Experiment()
        {
            string html = getUrlData(@"C:\Users\SiLenT\Desktop\1.html");
            parsePage(html);
        }

        //Parse page data
        private void parsePage(string html)
        {
            FilmNames = new ArrayList();
            FilmNames = matchAll(@"<span class=""post-b"">(.*?)</span>", html);
            //for (int i = 0; i < FilmNames.Count; ++i)
            //{
            //    FilmNames[i] = Regex.Replace((string)FilmNames[i], @"</a>", "");
            //}
            
        }

        //Match single instance
        private string match(string regex, string html, int i = 1)
        {
            return new Regex(regex, RegexOptions.Multiline).Match(html).Groups[i].Value.Trim();
        }

        //Match all instances and return as ArrayList
        private ArrayList matchAll(string regex, string html, int i = 1)
        {
            ArrayList list = new ArrayList();
            foreach (Match m in new Regex(regex, RegexOptions.Multiline).Matches(html))
                list.Add(m.Groups[i].Value.Trim());
            return list;
        }

        //Get URL Data
        private string getUrlData(string url)
        {
            WebClient client = new WebClient();
            Random r = new Random();
            //Random IP Address
            client.Headers["X-Forwarded-For"] = r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255);
            //Random User-Agent
            client.Headers["User-Agent"] = "Mozilla/" + r.Next(3, 5) + ".0 (Windows NT " + r.Next(3, 5) + "." + r.Next(0, 2) + "; rv:2.0.1) Gecko/20100101 Firefox/" + r.Next(3, 5) + "." + r.Next(0, 5) + "." + r.Next(0, 5);
            Stream datastream = client.OpenRead(url);
            StreamReader reader = new StreamReader(datastream);
            StringBuilder sb = new StringBuilder();
            while (!reader.EndOfStream)
                sb.Append(WebUtility.HtmlDecode(reader.ReadLine()));
            return sb.ToString();
        }
    }
}

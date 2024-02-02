using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = @"https://mulan.fandom.com/wiki/Mulan/Transcript";
            var web = new HtmlAgilityPack.HtmlWeb();
            var doc = web.Load(url);

            var node = doc.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']");
            var node1 = node.SelectSingleNode(".//div[@class=\"mw-parser-output\"]");
            var nodes = node1.SelectNodes("//p");

            foreach (var pnode in nodes) {
                System.Console.WriteLine(pnode.InnerText);
            }
        }
    }
}

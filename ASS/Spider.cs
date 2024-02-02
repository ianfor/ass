using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASS
{
    class Spider
    {
        private string _url = null;
        public Spider(string url)
        {
            _url = url;
        }

        public bool Get(ref List<KeyValuePair<string, string>> result, ref List<string> unknows)
        {
            var web = new HtmlAgilityPack.HtmlWeb();
            var doc = web.Load(_url);

            if (doc == null)
                return false;

            var node = doc.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']");
            if (node == null)
                return false;

            var node1 = node.SelectSingleNode(".//div[@class=\"mw-parser-output\"]");
            if (node1 == null)
                return false;

            var nodes = node1.SelectNodes("//p");
            if (nodes == null)
                return false;

            string lastname = null;
            foreach (var pnode in nodes)
            {
                var res = this.ParseNode(pnode, lastname);
                if (res != null)
                {
                    result.Add(res.Value);
                    lastname = res.Value.Key;
                }                    
                else
                    unknows.Add(pnode.InnerText);
            }
            
            return true;
        }

        private KeyValuePair<string, string>? ParseNode(HtmlAgilityPack.HtmlNode node, string lastname)
        {
            var bnode = node.SelectNodes(".//b");
            if (bnode == null && lastname == null)
            {
                return null;
            }

            var text = node.InnerText.Trim();
            var index = text.IndexOf(":");
            if (index != -1)
            {
                var name = text.Substring(0, index);
                var value = text.Substring(index).ToLower();
                return KeyValuePair.Create(name, value);
            }
            else
            {
                if (lastname != null)
                {
                    return KeyValuePair.Create(lastname, node.InnerText.ToLower());
                }
            }

            return null;
        }
    }
}

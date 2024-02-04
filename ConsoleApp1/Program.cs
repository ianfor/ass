using System;
using System.Text.RegularExpressions;
namespace ConsoleApp1
{
    class Program
    {
        static string LongestCommonSubsequence(string X, string Y)
        {
            int m = X.Length;
            int n = Y.Length;
            int[,] dp = new int[m + 1, n + 1];

            for (int i = 0; i <= m; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    if (i == 0 || j == 0)
                        dp[i, j] = 0;
                    else if (X[i - 1] == Y[j - 1])
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    else
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }


            int index = dp[m, n];

            char[] lcs = new char[index + 1];
            {
                lcs[index] = '\u0000';
                int i = m, j = n;
                while (i > 0 && j > 0)
                {

                    if (X[i - 1] == Y[j - 1])
                    {

                        lcs[index - 1] = X[i - 1];


                        i--;
                        j--;
                        index--;
                    }

                    else if (dp[i - 1, j] > dp[i, j - 1])
                        i--;
                    else
                        j--;
                }

            }

            return new string(lcs, 0, lcs.Length - 1);
        }

        public static string LCS(string str1, string str2)
        {
            if (str1 == str2)
            {
                return str1;
            }
            else if (String.IsNullOrEmpty(str1) || String.IsNullOrEmpty(str2))
            {
                return null;
            }
            var d = new int[str1.Length, str2.Length];
            var index = 0;
            var length = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                for (int j = 0; j < str2.Length; j++)
                {
                    //左上角
                    var n = i - 1 >= 0 && j - 1 >= 0 ? d[i - 1, j - 1] : 0;
                    //当前节点值 = “1 + 左上角的值”：“0”
                    d[i, j] = str1[i] == str2[j] ? 1 + n : 0;
                    //如果是最大值，则记录该值和行号
                    if (d[i, j] > length)
                    {
                        length = d[i, j];
                        index = i;
                    }

                }
            }
            return str1.Substring(index - length + 1, length);

        }
        static void Main(string[] args)
        {
            //var url = @"https://mulan.fandom.com/wiki/Mulan/Transcript";
            //var web = new HtmlAgilityPack.HtmlWeb();
            //var doc = web.Load(url);

            //var node = doc.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']");
            //var node1 = node.SelectSingleNode(".//div[@class=\"mw-parser-output\"]");
            //var nodes = node1.SelectNodes("//p");

            //foreach (var pnode in nodes) {
            //    System.Console.WriteLine(pnode.InnerText);
            //}

            //string s1 = " how are, you";
            //s1 = Regex.Replace(s1, @"[^a-zA-Z0-9 ]", "");
            //System.Console.WriteLine(s1);

            string s1 = "this is a hell world test failed";
            string s2 = "that is a hell return test failed";

            string res = LCS(s1, s2);
            System.Console.WriteLine(res);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
namespace ASS
{
    class AssParser
    {
        private string _path;
        private List<string> _headers;
        private List<String> _contents;
        private string _lastname;
        public AssParser(string path)
        {
            _path = path;
            _headers = new List<String>();
            _contents = new List<String>();
        }

        public bool Parse()
        {
            try 
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    var container = this._headers;
                    int headline = 0;
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        container.Add(line);
                        if (line == "[Events]")
                        {
                            headline = 1;
                        }
                        else if (headline == 1)
                        {
                            headline++;
                        }

                        if (headline == 2)
                        {
                            container = this._contents;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        private string TrimNonEnglishChars(string input)
        {
            int start = 0;
            int end = input.Length - 1;
          
            while (start < input.Length && !char.IsLetterOrDigit(input[start]))
            {
                start++;
            }
                       
            while (end >= 0 && !char.IsLetterOrDigit(input[end]))
            {
                end--;
            }
                       
            if (start > end)
            {
                return string.Empty;
            }
           
            return input.Substring(start, end - start + 1);
        }
       
        private string MatchLine(string sentence, ref int startline, List<KeyValuePair<string, string>> storys)
        {
            if (string.IsNullOrWhiteSpace(sentence))
                return null;

            for(int index = 0; index < storys.Count; ++index)
            {
                int eindex = (index + startline) % storys.Count;
                var item = storys.ElementAt(eindex);
                if (item.Value.Contains(sentence))
                {
                    startline = eindex;
                    return item.Key;
                }

                string s1 = item.Value.Replace("'m", " am");
                s1 = s1.Replace("'re", " are");
                s1 = s1.Replace("'s", " is");
                s1 = s1.Replace("n't", " not");
                s1 = s1.Replace("'ve", " have");
                s1 = s1.Replace("'ll", " well");

                string s2 = sentence.Replace("'m", " am");
                s2 = s2.Replace("'re", " are");
                s2 = s2.Replace("'s", " is");
                s2 = s2.Replace("n't", " not");
                s2 = s2.Replace("'ve", " have");
                s2 = s2.Replace("'ll", " well");

                if (s1.Contains(s2))
                {
                    startline = eindex;
                    return item.Key;
                }

                s1 = Regex.Replace(s1, @"[^a-zA-Z0-9]", "");
                s2 = Regex.Replace(s2, @"[^a-zA-Z0-9]", "");
                if (s1.Contains(s2))
                {
                    startline = eindex;
                    return item.Key;
                }
            }

            return null;
        }

        private string LongestCommonSubsequence(string X, string Y)
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

        private string LCS(string str1, string str2)
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
        private string SimilarMatch(string line, List<KeyValuePair<string, string>> storys)
        {
            string s2 = line.Replace("'m", " am");
            s2 = s2.Replace("'re", " are");
            s2 = s2.Replace("'s", " is");
            s2 = s2.Replace("n't", " not");
            s2 = s2.Replace("'ve", " have");
            s2 = s2.Replace("'ll", " well");
            s2 = Regex.Replace(s2, @"[^a-zA-Z0-9 ]", "");

            string[] splits = s2.Split(' ').Select(s => s.Trim()).ToArray();
            int min = splits.Length >= 2 ? 2 : splits.Length;
     
            List< KeyValuePair<string, int>> matchs = new List<KeyValuePair<string, int>>();
            for (int index = 0; index < storys.Count; ++index)
            {
                var item = storys.ElementAt(index);
                string s1 = item.Value.Replace("'m", " am");
                s1 = s1.Replace("'re", " are");
                s1 = s1.Replace("'s", " is");
                s1 = s1.Replace("n't", " not");
                s1 = s1.Replace("'ve", " have");
                s1 = s1.Replace("'ll", " well");
                s1 = Regex.Replace(s1, @"[^a-zA-Z0-9 ]", "");
                var result = LCS(s1, s2);

                int words = 0;
                foreach( var word in result.Trim().Split(' '))
                {
                    if (splits.Contains(word.Trim()))
                        words++;
                }

                if (words >= min)
                {
                    matchs.Add(KeyValuePair.Create(result, index));
                }
            }

            if (matchs.Count < 1) return null;

            var resultes = matchs.OrderBy(new Func<KeyValuePair<string, int>, int>(
                (KeyValuePair<string, int> source)=>
                {
                    return source.Key.Length;
                })).Reverse();

            int pos = resultes.ElementAt(0).Value;
            return storys[pos].Key;
        }
        private string GetEnglish(string line)
        {
            string findstr = "0,0,0,,";
            int index = line.LastIndexOf("\\N{\\fs60}");
            int index2 = line.IndexOf(findstr);
            if (index2 == -1)
                return null;

            if (index == -1)
                return line.Substring(index2 + findstr.Length);

            return line.Substring(index2 + findstr.Length, index - index2 - findstr.Length);           
        }

        private void ReplaceLineName(int index, string name)
        {
            if (this._lastname != name)
            {
                this._lastname = name;
                string line = this._contents.ElementAt(index);
                string newline = line.Replace(",Default,,0,0,0,", ",Default," + name + ",0,0,0,");
                this._contents[index] = newline;
            }
            
        }

        private bool HandleLine(int index, ref int startline, ref List<KeyValuePair<string, int>> fails, List<KeyValuePair<string, string>> storys, Action<string> log)
        {
            string line = this._contents.ElementAt(index);
            char[] sepatators = { ',', '.', '!', '?', '。', '！', '，', '？' };

            if (string.IsNullOrWhiteSpace(line))
                return true;

            int fileline = index + _headers.Count + 1;
            string english = GetEnglish(line);
            if (english == null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    log("在 " + fileline + "获取英文失败: " + line);
                return true;
            }

            var back_english = english;
            english = english.ToLower();
            english = Regex.Replace(english, @"\s+", " ");
            english = english.Replace('-', ' ');
            english = TrimNonEnglishChars(english);

            var key = MatchLine(english, ref startline, storys);
            if (key != null)
            {
                ReplaceLineName(index, key);
                return true;
            }

            string[] splits = english.Split(sepatators);
            List<string> names = new List<string>();
            foreach(var ss in splits.OrderBy(new Func<string, int>((string soruce) => { return soruce.Length; })).Reverse())
            {
                var s = ss.Trim();
                if (!string.IsNullOrWhiteSpace(s) && s.Length > 4)
                {
                    string result = MatchLine(s, ref startline, storys);
                    if (result != null)
                    {
                        if (!names.Contains(result))
                            names.Add(result);
                    }
                }                
            }

            if (names.Count > 0)
            {
                string name = string.Join("&", names);
                ReplaceLineName(index, name);
                return true;
            }
            log("在" + fileline + "初次匹配失败: " + back_english + " 即将使用相似度匹配算法进行匹配");
            string sname = SimilarMatch(english, storys);
            if (sname != null)
            {
                log("在" + fileline + "相似度匹配成功: " + back_english);
                ReplaceLineName(index, sname);
                return true;
            }
            else
            {
                log("在" + fileline + "相似度匹配失败: " + back_english);
                fails.Add(KeyValuePair.Create(back_english, fileline));
            }

            this._lastname = string.Empty;
            return false;
        }

        //https://mulan.fandom.com/wiki/Mulan/Transcript
        public void Merge(List<KeyValuePair<string, string>> storys, ref List<KeyValuePair<string, int>> fails, Action<int, int> callback, Action<string> log)
        {
            int total = this._contents.Count();
            int startline = 0;
            _lastname = string.Empty;
            for (int index = 0; index < total; ++index)
            {
                callback(index + 1, total);
                HandleLine(index, ref startline, ref fails, storys, log);
            }
        }

        public string ResultFileName()
        {
            int index = this._path.LastIndexOf(".");
            string filepath = this._path.Substring(0, index) + "_res" + this._path.Substring(index);
            return filepath;
        }

        public void GenerateResult()
        {
            string filepath = ResultFileName();

            using (StreamWriter sw = new StreamWriter(filepath))
            {
                foreach(var line in this._headers)
                {
                    sw.WriteLine(line);
                }

                foreach(var line in this._contents)
                {
                    sw.WriteLine(line);
                }
            }
        }
    }
}

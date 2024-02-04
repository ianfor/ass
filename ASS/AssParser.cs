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
        private string MatchSentence(string sentence, List<KeyValuePair<string, string>> storys)
        {
            sentence = sentence.ToLower();
            sentence = Regex.Replace(sentence, @"\s+", " ");
            sentence = sentence.Replace('-', ' ');
            sentence = TrimNonEnglishChars(sentence);
            if (string.IsNullOrWhiteSpace(sentence))
                return null;

            foreach (var item in storys)
            {
                if (item.Value == sentence)
                {
                    return item.Key;
                }
            }

            foreach (var item in storys)
            {
                if (item.Value.Contains(sentence))
                {
                    return item.Key;
                }
            }

            foreach(var item in storys)
            {
                string s1 = Regex.Replace(item.Value, @"[^a-zA-Z0-9]", "");
                string s2 = Regex.Replace(sentence, @"[^a-zA-Z0-9]", "");
                if (s1.Contains(s2))
                {
                    return item.Key;
                }
            }

            return null;
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
        private void HandleLine(int index, List<KeyValuePair<string, string>> storys, Action<string> log)
        {
            string line = this._contents.ElementAt(index);
            char[] sepatators = { ',', '.', '!', '?', '。', '！', '，', '？' };

            if (string.IsNullOrWhiteSpace(line))
                return;

            string english = GetEnglish(line);
            if (english == null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    log("在 " + (index + _headers.Count) + "获取英文失败: " + line);
                return;
            }

            string[] splits = english.Split(sepatators);
            foreach(var ss in splits.OrderBy(new Func<string, int>((string soruce) => { return soruce.Length; })).Reverse())
            {
                var s = ss.Trim();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    string result = MatchSentence(s, storys);
                    if (result != null)
                    {
                        string newline = line.Replace(",Default,,0,0,0,", ",Default," + result + ",0,0,0,");
                        this._contents[index] = newline;
                        return;
                    }
                }                
            }

            log("在" + (index + _headers.Count) + "匹配失败: " + english + " 即将使用相似度匹配算法进行匹配");
        }

        //https://mulan.fandom.com/wiki/Mulan/Transcript
        public void Merge(List<KeyValuePair<string, string>> storys, Action<int, int> callback, Action<string> log)
        {
            int total = this._contents.Count();
            for(int index = 0; index < total; ++index)
            {
                callback(index + 1, total);
                HandleLine(index, storys, log);
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

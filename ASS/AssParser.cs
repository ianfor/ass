using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

        public void Merge(List<KeyValuePair<string, string>> storys, Action<int, int> callback)
        {

        }


        public void GenerateResult()
        {
            int index = this._path.LastIndexOf(".");
            string filepath = this._path.Substring(0, index) + "_res" + this._path.Substring(index);
            
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

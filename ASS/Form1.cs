using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASS
{
    public partial class Form1 : Form
    {
        private List<string> _logs = new List<string>();
        public Form1()
        {
            InitializeComponent();

            this.start.Enabled = true;
            this.progressBar1.Value = 0;

#if DEBUG
            this.textBox1.Text = "https://mulan.fandom.com/wiki/Mulan/Transcript";
            this.textBox2.Text = @"C:\Users\Administrator\Downloads\[xiepp.com]Mulan.1998.BluRay.720p.x264.AC3.4Audios-CMCT.EN.ass";
#endif
        }

        private void start_Click(object sender, EventArgs e)
        {
            this.progressBar1.Value = 0;

            var th = new System.Threading.Thread(new System.Threading.ThreadStart(Work));            
            th.Start();
         }

        private void Log(string str, bool mainthread)
        {
            _logs.Add(str);
            string log = String.Join("\r\n", _logs);
            if (mainthread)
            {
                this.textBox3.Text = log;
            }
            else
            {
                this.Invoke(new Action(() => 
                {
                    this.textBox3.Text = log;
                }));
            }
        }
        private void Work()
        {
            this.Invoke(new Action(()=> {
                this.progressBar1.Value = 0;
                this.start.Enabled = false;
                this._logs.Clear();
                Log("start ...", true);
            }));

            Log("开始分析网页: " + this.textBox1.Text, false);
            var spider = new Spider(this.textBox1.Text);
            var result = new List<KeyValuePair<string, string>>();
            var unknows = new List<string>();
            if (spider.Get(ref result, ref unknows))
            {
                this.Invoke((new Action(() => 
                {
                    this.progressBar1.Value = 10;
                })));
                Log("网页分析成功, 获取了 " + result.Count().ToString() + " 行数据. unknows: ", false);
                Log(String.Join("\r\n", unknows), false);

                var parser = new AssParser(this.textBox2.Text);
                List<KeyValuePair<string, int>> fails = new List<KeyValuePair<string, int>>();
                if (parser.Parse())
                {
                    Log("加载:" + this.textBox2.Text + "文件成功.", false);
                    parser.Merge(result, ref fails, new Action<int, int>((int curcent, int total)=> 
                    {
                        int val = (int)((double)curcent / (double)total * 70);
                        this.Invoke(new Action(() =>
                        {
                            this.progressBar1.Value = 20+val;
                        }));
                        
                    }),
                    new Action<string>((string log)=> 
                    {
                        Log(log, false);
                    }));
                    parser.GenerateResult();
                    Log("合并完成, ass 文件: \r\n" + parser.ResultFileName(), false);
                    if (fails.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("以下行数匹配失败:");
                        foreach (var item in fails)
                        {
                            sb.AppendLine("第" + item.Value.ToString() + "行: " + item.Key);
                        }

                        Log(sb.ToString(), false);
                    }
                    
                }
                else
                {
                    Log("加载ass文件失败.", false);
                }
            }
            else
            {
                Log("网页分析失败.", false);
            }

            this.Invoke(new Action(() =>
            {
                this.progressBar1.Value = 100;
                this.start.Enabled = true;
            }));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ChangeStartBtn();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ChangeStartBtn();
        }

        private void ChangeStartBtn()
        {
            if (this.textBox1.Text.StartsWith("http") && System.IO.File.Exists(this.textBox2.Text))
            {
                this.start.Enabled = true;
            }
            else
            {
                this.start.Enabled = false;
            }
        }

        private void filebro_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "ASS|*.ass|All File|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = dialog.FileName;
            }
        }
    }
}

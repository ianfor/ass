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

            this.start.Enabled = false;
            this.progressBar1.Value = 0;
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

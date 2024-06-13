using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leaf.xNet;
using System.Threading;

namespace HttpX___GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public List<string> domains = new List<string>();
        int total_domains = 0;
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] text = Clipboard.GetText().Split('\n');
            foreach(var line in text)
            {
                domains.Add(line);

            }
            total_domains = domains.Count;
            label2.Text = total_domains.ToString();
            MessageBox.Show($"Total Domains Saved (From clipboard): '{total_domains.ToString()}' .", "Done", MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach(var domain in domains)
            {
                new Thread(() =>
                {
                    try
                    {
                        var d = domain;
                        if (!domain.Contains("://"))
                        {
                            d = "http://" + domain;
                        }
                        int status_code = (int)new HttpRequest().Get(d).StatusCode;
                        ListViewItem item = new ListViewItem(domain);
                        item.SubItems.Add(status_code.ToString());
                        listView1.Invoke(new MethodInvoker(delegate
                        {
                            listView1.Items.Add(item);
                        }));
                    }
                    catch (HttpException ex)
                    {
                        int stats = 0;
                        if(ex.HttpStatusCode.ToString() == "Conflict")
                        {
                            stats = 409;
                        }else if (ex.HttpStatusCode.ToString() == "InternalServerError")
                        {
                            stats = 500;
                        }
                        else if(ex.HttpStatusCode.ToString().ToLower() == "notfound")
                        {
                            stats = 404;
                        }
                        ListViewItem item = new ListViewItem(domain);
                        if (stats != 0) item.SubItems.Add(stats.ToString());
                        else item.SubItems.Add(ex.HttpStatusCode.ToString());
                        listView1.Invoke(new MethodInvoker(delegate
                        {
                            listView1.Items.Add(item);
                        }));

                    }
                    catch
                    {

                    }
                })
                { IsBackground = true }.Start();

            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string t = "";
            foreach(ListViewItem item in listView1.SelectedItems)
            {
                t += item.SubItems[0].Text + '\n';
            }
            Clipboard.SetText(t);
            MessageBox.Show("Copied to Clipboard !", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

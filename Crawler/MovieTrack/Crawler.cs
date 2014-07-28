using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Collections;
using System.Text.RegularExpressions;
using Ionic.Zip;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.XtraBars;

namespace MovieTrack
{
    public partial class Crawler : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        MovieTracker mt = new MovieTracker();
        string folderPath;

        public Crawler()
        {
            InitializeComponent();
            simpleButton1.Enabled = false;
            simpleButton3.Enabled = false;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            simpleButton2.Enabled = false;
            richTextBox1.Text += textBox1.Text;
            richTextBox1.Text += "\n";
            string url = textBox1.Text;
            textBox1.Text = "";
            if (url.StartsWith("http://"))
            {
                url = url.Replace("http://", "");
            }
            if (url.EndsWith("/"))
            {
                url = url.TrimEnd('/');
            }
            if (url.Length > 5)
            {
                Thread t = new Thread(() => doStuff(url));
                t.Start();
                //doStuff(url);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            var result = new StringBuilder();
            var thread = new Thread(obj =>
            {
                var builder = (StringBuilder)obj;
                using (var dialog = new FolderBrowserDialog())
                {
                    DialogResult folderDialog = this.folderBrowserDialog1.ShowDialog();
                    if (folderDialog == DialogResult.OK)
                    {
                        folderPath = this.folderBrowserDialog1.SelectedPath;
                        mt.path = folderPath;
                    }
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(result);

            simpleButton1.Enabled = true;
            simpleButton2.Enabled = false;
            simpleButton3.Enabled = true;
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() => mt.getMoreInfoAboutMovies());
            t.Start();
        }

        public void doStuff(string subtitlesUrl)
        {
            //do stuff here
            Queue<Url> urls = new Queue<Url>();
            List<Url> urlList = new List<Url>();
            string baseUrl = subtitlesUrl;
            Url url = new Url();
            url.url = baseUrl;
            url.state = false;
            List<string> allUrl = mt.getAllLinksInUrl(url, baseUrl);
            url.state = true;
            url.sourceUrl = "";
            urlList.Add(url);
            try
            {
                foreach (var item in allUrl)
                {
                    try
                    {
                        if (item.Equals("/"))
                        {
                            continue;
                        }
                        Url subUrl = new Url();
                        subUrl.url = item;
                        subUrl.sourceUrl = baseUrl;
                        subUrl.state = false;
                        urls.Enqueue(subUrl);
                        while (urls.Count > 0)
                        {
                            Url tempSubUrl = new Url();
                            tempSubUrl = urls.Dequeue();
                            if (mt.ifNotVisited(urlList, tempSubUrl))
                            {
                                List<string> tempAllUrl = mt.getAllLinksInUrl(tempSubUrl, baseUrl);
                                tempSubUrl.state = true;
                                urlList.Add(tempSubUrl);
                                if (tempAllUrl != null)
                                {
                                    foreach (var item2 in tempAllUrl)
                                    {
                                        if (item2.Equals("/"))
                                        {
                                            continue;
                                        }
                                        Url subUrl2 = new Url();
                                        subUrl2.url = item2;
                                        subUrl2.sourceUrl = tempSubUrl.url;
                                        subUrl2.state = false;
                                        urls.Enqueue(subUrl2);
                                        this.BeginInvoke(new MethodInvoker(delegate()
                                        {
                                            richTextBox2.Text += item2;
                                            richTextBox2.Text += "\n";
                                        }));
                                        if (richTextBox2.TextLength > 4000)
                                            richTextBox2.Clear();
                                    }
                                }
                            }
                        }
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.Text += item;
                            richTextBox2.Text += "\n";
                        }));
                        if (richTextBox2.TextLength > 4000)
                            richTextBox2.Clear();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
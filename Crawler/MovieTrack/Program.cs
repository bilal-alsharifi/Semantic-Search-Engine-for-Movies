using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using Ionic.Zip;
using System.Windows.Forms;

namespace MovieTrack
{

    class Program
    {
        static void Main(string[] args)
        {

        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Crawler());
       
            //Task task1 = Task.Factory.StartNew(() => doStuff("subscene.com"));
            //Task task2 = Task.Factory.StartNew(() => doStuff("www.subtitlesource.org"));
            //Task task3 = Task.Factory.StartNew(() => doStuff("www.opensubtitles.org/en"));
            //Task.WaitAll(task1, task2, task3);
            //Task.WaitAll(task1);
        }

    }
}

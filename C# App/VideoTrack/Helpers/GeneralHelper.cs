using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace IRHomework
{
    class GeneralHelper
    {
        public static String executeCommand(String fileName, String arguments)
        {
            StringBuilder result = new StringBuilder();
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            StreamReader streamReader = process.StandardOutput;
            while (!streamReader.EndOfStream)
            {
                result.AppendLine(streamReader.ReadLine());
            }
            process.WaitForExit();
            process.Close();
            return result.ToString();
        }
        
		public static Boolean isInternetActive()
        {
            Boolean result = false;
            try
            {
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient("www.google.com", 80);
                client.Close();
                result = true;
            }
            catch (System.Net.Sockets.SocketException)
            {
                result = false;
            }
            return result;
        }
        public static string getStopWords(string fileName)
        {
            string stopWords = "";
            StreamReader reader = new StreamReader(fileName);
            while (!reader.EndOfStream)
            {
                stopWords += reader.ReadLine()+" ";
            }
            return stopWords;
        }

        public static List<String> stem(String text, String javaTool, String javaApp, String stemmerApp)
        {
            String output = GeneralHelper.executeCommand("\"" + javaTool + "\"", " -jar " + "\"" + stemmerApp + "\"" + " " + "\"" + text + "\"");
            output = Regex.Replace(output, "[\\s\t\n\r]", "");
            List<String> result = output.Split(';').ToList();
            result = result.Where(w => w.Length > 0).ToList();
            return result;
        }
    }
}

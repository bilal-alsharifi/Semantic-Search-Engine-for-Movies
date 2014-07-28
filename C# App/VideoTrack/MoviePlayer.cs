using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WMPLib;
using HtmlAgilityPack;
using System.Net;

namespace VideoTrack
{
    public partial class MoviePlayer : Form
    {
        string movieTrack;
        string sceneTime;

        public MoviePlayer(string track, string sceneTime)
        {
            InitializeComponent();
            this.movieTrack = track;
            this.sceneTime = sceneTime;
        }

        private void MoviePlayer_Load(object sender, EventArgs e)
        {
            //DisplayVideo(@"C:\Users\Anna Adjemian\dwhelper\anna.mp4", "00:00:55");
            //axShockwaveFlash1.Movie = YoutubeUrlForDisplayFromBegining("http://www.youtube.com/watch?v=mHEVp08LHJA");

            if (movieTrack.Contains("youtube"))
            {
                axShockwaveFlash1.Movie = YoutubeUrlForDisplayFromBegining(movieTrack);
                axWindowsMediaPlayer1.SendToBack();
            }
            else
            {
                DisplayVideo(movieTrack, sceneTime);
                axShockwaveFlash1.SendToBack();
            }
            
        }

        public void DisplayVideo(string pathOfVideoFile, string sceneTime)
        {
            axWindowsMediaPlayer1.URL = pathOfVideoFile;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = Time2Seconds(sceneTime);
        }

        public double Time2Seconds(string sceneTime)
        {
            string time = sceneTime;
            if (time != null && time.Length == 8)
            {
                return TimeSpan.Parse(time).TotalSeconds;
            }
            else
            {
                return 0;
            }
        }

        public string YoutubeUrlForDisplayFromBegining(string url)
        {
            string youtubeUrl = url;
            youtubeUrl = youtubeUrl.Replace("watch?", "");
            youtubeUrl = youtubeUrl.Replace("=", "/");
            return youtubeUrl;
        }

        public string YoutubeUrlForDisplayFromPeriod(string url)
        {
            //http://www.youtube.com/watch?v=Oy8qFfIv12Y
            //http://www.youtube.com/watch?feature=player_detailpage&v=Oy8qFfIv12Y#t=477s
            string youtubeUrl = url;
            youtubeUrl = youtubeUrl.Replace("watch?", "");
            youtubeUrl = youtubeUrl.Replace("=", "/");
            return youtubeUrl;
        }
    }
}

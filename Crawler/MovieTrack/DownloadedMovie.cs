using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieTrack
{
    class DownloadedMovie
    {
        public string movieName;
        public string movieImdbID;
        public string movieYear;
        public string movieThumbnail;

        public DownloadedMovie()
        {
            this.movieName = "";
            this.movieImdbID = "";
            this.movieYear = "";
            this.movieThumbnail = "";
        }

        public DownloadedMovie(string movieName, string movieImdbID, string movieYear, string movieThumbnail)
        {
            this.movieName = movieName;
            this.movieImdbID = movieImdbID;
            this.movieYear = movieYear;
            this.movieThumbnail = movieThumbnail;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRHomework.Entities
{
    class SearchResult
    {
        private String movieTitle;
        private String startTime;
        private String endTime;
        private int score = 1;

        public SearchResult(String movieTitle, String startTime, String endTime)
        {
            this.movieTitle = movieTitle;
            this.startTime = startTime;
            this.endTime = endTime;
        }
        public String getStartTime()
        {
            return this.startTime;
        }
        public String getEndTime()
        {
            return this.endTime;
        }
        public int getScore()
        {
            return this.score;
        }
        public string getMovieTitle()
        {
            return this.movieTitle;
        }
        public void increaseScore()
        {
            this.score++;
        }
        public override String ToString()
        {
            String sep = ";";
            return this.movieTitle + sep + this.startTime + sep + this.endTime + sep + score;
        }
        public override bool Equals(object obj)
        {
            bool result = true;
            SearchResult sr = (SearchResult)obj;
            if (!this.movieTitle.Equals(sr.movieTitle) || !this.startTime.Equals(sr.startTime) || !this.endTime.Equals(sr.endTime))
            {
                result = false;
            }
            return result;
        }
        public override int GetHashCode()
        {
            return (this.movieTitle + this.startTime + this.endTime).GetHashCode();
        }
    }
}

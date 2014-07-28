using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IRHomework
{
    public class Triple
    {
        private String movieTitle = null;
        private String sceneTime;
        private String startTime;
        private String endTime;
        private String su;
        private String pr;
        private String ob;
        private String domain = "";
        public Triple(String movieTitle, String str)
        {
            this.movieTitle = movieTitle;
            str = Regex.Replace(str, "[\\s\t\n\r]", "");
            var list = str.Split(';');
            this.sceneTime = list[0];
            this.su = list[1];
            this.pr = list[2];
            this.ob = list[3];
        }
        public Triple(String su, String pr, String ob, String sceneTime, String domain, String movieTitle)
        {
            this.su = su;
            this.pr = pr;
            this.ob = ob;
            this.sceneTime = sceneTime;
            this.domain = domain;
            this.movieTitle = movieTitle;
        }
        public Triple(String su, String pr, String obAndSceneTimeAndDomain, String movieTitle)
        {
            this.su = su;
            this.pr = pr;
            this.ob = obAndSceneTimeAndDomain;
            this.movieTitle = movieTitle;
            int indexOfAt = this.ob.IndexOf("@");
            int indexOfSharp = this.ob.IndexOf("#");
            if (indexOfAt != -1 && indexOfSharp != -1)
            {
                this.ob = obAndSceneTimeAndDomain.Substring(0, indexOfAt);
                this.sceneTime = obAndSceneTimeAndDomain.Substring(indexOfAt + 1, indexOfSharp - indexOfAt - 1);
                this.domain = obAndSceneTimeAndDomain.Substring(indexOfSharp + 1);
            }
        }
        public Triple clone()
        {
            return new Triple(this.su, this.pr, this.ob, this.sceneTime, this.domain, this.movieTitle);
        }
        public void setSubject(String subject)
        {
            this.su = subject;
        }
        public void setPredicate(String predicate)
        {
            this.pr = predicate;
        }
        public void setObject(String obj)
        {
            this.ob = obj;
        }
        public void setDomain(String domain)
        {
            this.domain = domain;
        }
        public String getMovieTitle()
        {
            return this.movieTitle;
        }
        public String getSceneTime()
        {
            return this.sceneTime;
        }
        public String getStartTime()
        {
            return this.sceneTime.Substring(0, 8);
        }
        public String getEndTime()
        {
            int i = this.sceneTime.Count() - 4;
            return this.sceneTime.Substring(i - 8, 8);
        }
        public String getSubject()
        {
            return this.su;
        }
        public String getPredicate()
        {
            return this.pr;
        }
        public String getObject()
        {
            return this.ob;
        }
        public String getDomain()
        {
            return this.domain;
        }
        public String getSentence()
        {
            return this.su + " " + this.pr + " " + this.ob;
        }
        public Boolean subjectIsValid()
        {
            Boolean isUri = Uri.IsWellFormedUriString(this.su, UriKind.Absolute);
            Boolean isSubject = System.Text.RegularExpressions.Regex.IsMatch(this.su, @"^.*/([\w_\-\.])*$");
            return isUri && isSubject;
        }
        public Boolean predicateIsValid()
        {
            Boolean isUri = Uri.IsWellFormedUriString(this.pr, UriKind.Absolute);
            Boolean isPredicate = System.Text.RegularExpressions.Regex.IsMatch(this.pr, @"^.*/([A-Za-z])([\w_\-\.])*$");
            return isUri && isPredicate;
        }
        public Boolean objectIsValid()
        {
            Boolean isUri = Uri.IsWellFormedUriString(this.ob, UriKind.Absolute);
            Boolean isObject = System.Text.RegularExpressions.Regex.IsMatch(this.ob, @"^([\w _\-\.@#:,&;>])*$");
            return !isUri && isObject;
        }
        public Boolean isValid()
        {
            Boolean result = false;
            if (this.subjectIsValid() && this.predicateIsValid() && this.objectIsValid())
            {
                result = true;
            }
            return result;
        }
        public override String ToString()
        {
            String sep = ";";
            return this.movieTitle + sep + this.sceneTime + sep + this.su + sep + this.pr + sep + this.ob;       
        }
        public override bool Equals(object obj)
        {
            bool result = true;
            Triple tr = (Triple)obj;
            if (!this.ToString().Equals(tr.ToString()))
            {
                result = false;
            }
            return result;
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTrack.Helpers
{
    
    class MovieModel
    {
        static VideoTrackDataContext db = new VideoTrackDataContext();

        public List<String> getActorsNameOfMovie(int movieID)
        {
            var actors = (from x in db.Movie_Actors
                          where x.movieID == movieID
                          select x.Actor.name).ToList();
            return actors;
        }

        public List<String> getLangsOfMovie(int movieID)
        {
            var langs = (from x in db.Movie_Langs
                          where x.movieID == movieID
                          select x.Lang.name).ToList();
            return langs;
        }

        public List<String> getGenresOfMovie(int movieID)
        {
            var genres = (from x in db.Movie_Genres
                         where x.movieID == movieID
                         select x.Genre.name).ToList();
            return genres;
        }

        public String getThumbnailOfMovie(int movieID)
        {
            var thumbnail = from x in db.Movies
                          where x.movieID == movieID
                          select x.thumbnail;
            return thumbnail.First();
        }

        public String getReleaseYearOfMovie(int movieID)
        {
            var year = from x in db.Movies
                            where x.movieID == movieID
                            select x.year;
            return year.First();
        }

        public String getTrackOfMovie(int movieID)
        {
            var video = from x in db.Movies
                       where x.movieID == movieID
                       select x.video;
            if (video != null)
                return video.First();
            else
                return "";
        }
    }
}

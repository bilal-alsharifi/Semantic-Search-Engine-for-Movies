using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieTrack
{

    public class Trigram
    {
        // term with its frequency
        public string t = null;
        public int score = 0;
        public Trigram(string t, int s)
        {
            this.t = t;
            score = s;
        }
    }


    public class TrigramModel
    {
        Trigram[] trigrams;
        public Trigram[] Trigrams
        {
            get { return trigrams; }
        }

        public TrigramModel(Hashtable trigramsAndCounts)
        {
            //convert hashtable to trigam model
            List<string> keys2 = new List<string>();
            List<int> scores2 = new List<int>();
            //convert hashtable to arrays
            foreach (string key in trigramsAndCounts.Keys)
            {
                keys2.Add(key);
                scores2.Add((int)trigramsAndCounts[key]);
            }

            string[] keys = keys2.ToArray();
            int[] scores = scores2.ToArray();

            // sort array results
            Array.Sort(scores, keys);
            Array.Reverse(keys);
            Array.Reverse(scores);

            //build final array
            List<Trigram> result = new List<Trigram>();
            for (int x = 0; x < keys.Length; x++)
            {
                result.Add(new Trigram(keys[x], scores[x]));
            }

            trigrams = result.ToArray();

        }


        public TrigramModel(string[] tgrams)
        {
            // convert array of string to trigram model
            List<string> keys2 = new List<string>();
            List<int> scores2 = new List<int>();
            //convert hashtable to arrays
            int score = 0;
            foreach (string key in tgrams)
            {
                keys2.Add(key);
                scores2.Add(score++);
            }

            string[] keys = keys2.ToArray();
            int[] scores = scores2.ToArray();

            // sort array results
            Array.Sort(scores, keys);
            Array.Reverse(keys);
            Array.Reverse(scores);

            //build final array
            List<Trigram> result = new List<Trigram>();
            for (int x = 0; x < keys.Length; x++)
            {
                result.Add(new Trigram(keys[x], scores[x]));
            }

            trigrams = result.ToArray();

        }

        public bool HasTrigram(string trigram)
        {
            foreach (Trigram t in trigrams)
            {
                if (t.t == trigram) return true;
            }
            return false;
        }

        public int GetScore(string trigram)
        {
            foreach (Trigram t in trigrams)
            {
                if (t.t == trigram) return t.score;
            }
            throw new Exception("No score found for '" + trigram + "'");
        }
    }

    class DBHelper
    {
        public MovieTrackDataContext mvdb = new MovieTrackDataContext();

        public void insertLanguage(string lang)
        {
            var languages = from language in mvdb.Langs
                            where language.name == lang
                            select language.name;

            if (languages.Count() > 0)
            {
                // no thing to do
            }

            else
            {
                Lang sl = new Lang()
                {
                    name = lang
                };

                mvdb.Langs.InsertOnSubmit(sl);
                mvdb.SubmitChanges();
            }
        }

        public void insertMovie(DownloadedMovie downloadedMovie)
        {
            var movies = from movie in mvdb.Movies
                         where movie.name == downloadedMovie.movieName
                         select movie.name;

            if (movies.Count() > 0)
            {
                //no thing to do
            }
            else
            {
                Movie m = new Movie()
                {
                    name = downloadedMovie.movieName,
                    imdb_id = downloadedMovie.movieImdbID,
                    year = downloadedMovie.movieYear,
                    thumbnail = downloadedMovie.movieThumbnail,
                };
                mvdb.Movies.InsertOnSubmit(m);
                mvdb.SubmitChanges();
            }
        }

        public void insertSubtitle(int index, DownloadedMovie downloadedMovie, string lang)
        {
            int movieID = 0;
            int langID = 0;
            var langid = from language in mvdb.Langs
                         where language.name == lang
                         select language.langID;

            if (langid.Count() == 0)
            {
                insertLanguage(lang);
                langID = (from language in mvdb.Langs
                          where language.name == lang
                          select language.langID).First();
            }

            else
            {
                langID = langid.First();
            }

            var movieid = from movie in mvdb.Movies
                          where movie.name == downloadedMovie.movieName
                          select movie.movieID;

            if (movieid.Count() == 0)
            {
                insertMovie(downloadedMovie);
                movieID = (from movie in mvdb.Movies
                           where movie.name == downloadedMovie.movieName
                           select movie.movieID).First();
            }

            else
            {
                movieID = movieid.First();
            }

            //var sub = from subtitle in mvdb.Subtitles
            //          where subtitle.movieID == movieID
            //          && subtitle.langID == langID
            //          select subtitle.subtitleID;

            //if (sub.Count() > 0)
            //{
            //    Subtitle st = new Subtitle()
            //    {
            //        subtitleID = index,
            //        langID = langID,
            //        movieID = movieID
            //    };

            //    mvdb.Subtitles.InsertOnSubmit(st);
            //    mvdb.SubmitChanges();
            //}

            Subtitle st = new Subtitle()
            {
                subtitleID = index,
                langID = langID,
                movieID = movieID
            };

            mvdb.Subtitles.InsertOnSubmit(st);
            mvdb.SubmitChanges();
        }

        public void insertMovie_Lang(string langName, string imdb_id)
        {
            var language_ID = from lang in mvdb.Langs
                              where lang.name == langName
                              select lang.langID;

            if (language_ID.Count() > 0)
            {
                var movie_ID = from movie in mvdb.Movies
                               where movie.imdb_id == imdb_id
                               select movie.movieID;

                var movie_languageID = from movie_language in mvdb.Movie_Langs
                                  where (movie_language.langID == language_ID.First()) 
                                        && (movie_language.movieID == movie_ID.First())
                                  select movie_language.id;
                if (movie_languageID.Count() > 0)
                {
                    //donothing
                }
                else
                {
                    Movie_Lang ml = new Movie_Lang()
                    {
                        movieID = movie_ID.First(),
                        langID = language_ID.First(),
                    };
                    mvdb.Movie_Langs.InsertOnSubmit(ml);
                    mvdb.SubmitChanges();
                }
            }
            else
            {
                insertLanguage(langName);
                var languageID = from lang in mvdb.Langs
                                 where lang.name == langName
                                 select lang.langID;
                var movie_ID = from movie in mvdb.Movies
                               where movie.imdb_id == imdb_id
                               select movie.movieID;
                Movie_Lang ml = new Movie_Lang()
                {
                    movieID = movie_ID.First(),
                    langID = languageID.First(),
                };
                mvdb.Movie_Langs.InsertOnSubmit(ml);
                mvdb.SubmitChanges();
            }
        }

        public int insertActor(string actorName)
        {
            var actorIDs = from actor in mvdb.Actors
                           where actor.name == actorName
                           select actor.actorID;

            if (actorIDs.Count() > 0)
            {
                // no thing to do
            }

            else
            {
                Actor actor = new Actor()
                {
                    name = actorName
                };
                mvdb.Actors.InsertOnSubmit(actor);
                mvdb.SubmitChanges();
            }

            var actorID = from actor in mvdb.Actors
                          where actor.name == actorName
                          select actor.actorID;
            return actorID.First();
        }

        public void insertMovie_Actor(string actorName, string imdb_id)
        {
            var actorID = from actor in mvdb.Actors
                          where actor.name == actorName
                          select actor.actorID;

            if (actorID.Count() > 0)
            {
                var movieID = from movie in mvdb.Movies
                              where movie.imdb_id == imdb_id
                              select movie.movieID;

                var movie_actorID = from movie_actor in mvdb.Movie_Actors
                                  where (movie_actor.actorID == actorID.First()) 
                                        && (movie_actor.movieID == movieID.First())
                                  select movie_actor.id;
                if (movie_actorID.Count() > 0)
                {
                    //donothing
                }
                else
                {
                    Movie_Actor ma = new Movie_Actor()
                    {
                        movieID = movieID.First(),
                        actorID = actorID.First(),
                    };
                    mvdb.Movie_Actors.InsertOnSubmit(ma);
                    mvdb.SubmitChanges();
                }
            }
            else
            {
                var movieID = from movie in mvdb.Movies
                              where movie.imdb_id == imdb_id
                              select movie.movieID;
                Movie_Actor ma = new Movie_Actor()
                {
                    movieID = movieID.First(),
                    actorID = insertActor(actorName),
                };
                mvdb.Movie_Actors.InsertOnSubmit(ma);
                mvdb.SubmitChanges();
            }
        }

        public int insertGenre(string genreName)
        {
            var genreIDs = (from genre in mvdb.Genres
                            where genre.name == genreName
                            select genre.genreID);

            if (genreIDs.Count() > 0)
            {
                // no thing to do
            }

            else
            {
                Genre genre = new Genre()
                {
                    name = genreName
                };

                mvdb.Genres.InsertOnSubmit(genre);
                mvdb.SubmitChanges();
            }

            var genreID = from genre in mvdb.Genres
                          where genre.name == genreName
                          select genre.genreID;
            return genreID.First();
        }

        public void insertMovie_Genre(string genreName, string imdb_id)
        {
            var genreID = from genre in mvdb.Genres
                          where genre.name == genreName
                          select genre.genreID;

            if (genreID.Count() > 0)
            {
                var movieID = from movie in mvdb.Movies
                              where movie.imdb_id == imdb_id
                              select movie.movieID;

                var movie_genreID = from movie_genre in mvdb.Movie_Genres
                                  where (movie_genre.genreID == genreID.First()) 
                                        && (movie_genre.movieID == movieID.First())
                                  select movie_genre.id;
                if (movie_genreID.Count() > 0)
                {
                    //donothing
                }
                else
                {
                    Movie_Genre mg = new Movie_Genre()
                    {
                        movieID = movieID.First(),
                        genreID = genreID.First(),
                    };
                    mvdb.Movie_Genres.InsertOnSubmit(mg);
                    mvdb.SubmitChanges();
                }
            }
            else
            {
                var movieID = from movie in mvdb.Movies
                              where movie.imdb_id == imdb_id
                              select movie.movieID;
                Movie_Genre mg = new Movie_Genre()
                {
                    movieID = movieID.First(),
                    genreID = insertGenre(genreName),
                };
                mvdb.Movie_Genres.InsertOnSubmit(mg);
                mvdb.SubmitChanges();
            }
        }

        public List<string> getAllMovies()
        {
            var movies = from movie in mvdb.Movies
                         select movie.imdb_id;

            if (movies.Count() > 0)
            {
                //no thing to do
                return movies.ToList<string>();
            }
            else
            {
                return null;
            }
        }
    }
}

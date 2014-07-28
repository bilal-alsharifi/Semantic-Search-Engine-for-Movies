using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IRHomework.Entities;

namespace IRHomework.Helpers
{
    class UserQueryHelper
    {
        public static List<Triple> getUserQueryTriples(String inputText, String javaTool, String javaApp, int triplesExtractionMethod, bool withCoreference, bool withLemmatization)
        {
            List<Triple> userQueryTriples = new List<Triple>();
            String output = GeneralHelper.executeCommand(javaTool, " -jar " + javaApp + " " + triplesExtractionMethod + " " + withCoreference + " " + withLemmatization + " " + "\"" + inputText + "\"" + " " + "true");
            String[] listOfTripleStrings = output.Split('\r');
            foreach (String tripleString in listOfTripleStrings)
            {
                if (tripleString.Length > 1 && !tripleString.Contains("error n000002"))
                {
                    Triple tr = new Triple(null, tripleString);
                    userQueryTriples.Add(tr);
                }
            }
            return userQueryTriples;
        }
        public static List<Triple> getTriplesThatMatchSearch(List<Triple> userQueryTriples, List<String> actors, List<String> languages, List<String> genres, String year, String rdfXmlFilesPath)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            List<Triple> triplesThatMatchSearch = new List<Triple>();
            foreach (var movie in db.Movies)
            {
                // search filters
                var movieActors = from t1 in movie.Movie_Actors
                                  join t2 in db.Actors
                                  on t1.actorID equals t2.actorID
                                  select t2.name;
                if (movieActors.Intersect(actors).Count() != actors.Count)
                {
                    continue;
                }
                var movieLanguages = from t1 in movie.Movie_Langs
                                     join t2 in db.Langs
                                     on t1.langID equals t2.langID
                                     select t2.name;
                if (movieLanguages.Intersect(languages).Count() != languages.Count)
                {
                    continue;
                }
                var movieGenres = from t1 in movie.Movie_Genres
                                  join t2 in db.Genres
                                  on t1.genreID equals t2.genreID
                                  select t2.name;
                if (movieGenres.Intersect(genres).Count() != genres.Count)
                {
                    continue;
                }
                if (!movie.year.Contains(year))
                {
                    continue;
                }
                // end

                var subtitles = movie.Subtitles.Where(s => s.status == 1 && s.Lang.name.ToLower() == "english");
                if (subtitles.Count() >= 1)
                {
                    var subtitle = subtitles.First();
                    String rdfXmlFile = rdfXmlFilesPath + subtitle.subtitleID + ".rdf";
                    var iGraph = DotNetRDFHelper.getIGraphFromRdfXmlFile(rdfXmlFile);
                    foreach (Triple tr in userQueryTriples)
                    {
                        List<Triple> triples = DotNetRDFHelper.queryIGraph(tr.getSubject(), tr.getPredicate(), tr.getObject(), tr.getDomain(), iGraph, subtitle.Movie.name);
                        triplesThatMatchSearch.AddRange(triples);
                    }
                }
            }
            return triplesThatMatchSearch;
        }
        public static List<SearchResult> getSearchResutls(List<Triple> triplesThatMatchSearch)
        {
            List<SearchResult> searchResults = new List<SearchResult>();
            foreach (var tr in triplesThatMatchSearch)
            {
                SearchResult sr = new SearchResult(tr.getMovieTitle(), tr.getStartTime(), tr.getEndTime());
                int i = searchResults.IndexOf(sr);
                if (i == -1)
                {
                    searchResults.Add(sr);
                }
                else
                {
                    searchResults.ElementAt(i).increaseScore();
                }
            }
            searchResults = searchResults.OrderByDescending(sr => sr.getScore()).ToList();
            return searchResults;
        }
    }
}

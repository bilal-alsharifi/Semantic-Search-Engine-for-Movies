using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace MovieTrack
{
    class ImdpApi
    {
        private static String getResponse(String movieTitle)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create("http://imdbapi.org/?type=xml&title=" + movieTitle);
            // Set the Method property of the request to POST.
            request.Method = "GET";
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Clean up the streams and the response.
            reader.Close();
            response.Close();
            return responseFromServer;
        }

        public static List<DownloadedMovie> findMovie(String movieTitle)
        {
            String responseFromServer = getResponse(movieTitle);
            XDocument doc = XDocument.Parse(responseFromServer);
            var moviesName = doc.Descendants("title").Select(t => t.Value);
            var imdbIDs = doc.Descendants("imdb_id").Select(t => t.Value);
            var moviesYear = doc.Descendants("year").Select(t => t.Value);
            var moviesThumbnails = doc.Descendants("poster").Select(t => t.Value);
            List<DownloadedMovie> allResult = new List<DownloadedMovie>();
            foreach (var item in moviesName)
            {
                DownloadedMovie movie = new DownloadedMovie();
                movie.movieName = item;
                allResult.Add(movie);
            }
            int index = 0;
            foreach (var item in imdbIDs)
            {
                allResult[index].movieImdbID = item;
                index++;
            }
            index = 0;
            foreach (var item in moviesYear)
            {
                allResult[index].movieYear = item;
                index++;
            }
            index = 0;
            foreach (var item in moviesThumbnails)
            {
                allResult[index].movieThumbnail = item;
                index++;
            }
            List<DownloadedMovie> movieList = new List<DownloadedMovie>();
            foreach (var item in allResult)
            {
                if (!item.movieImdbID.Equals(""))
                {
                    movieList.Add(item);
                }
            }
            return movieList;
        }

        private static String getResponseForIMDB_ID(String imdbID)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create("http://imdbapi.org/?type=xml&id=" + imdbID);
            // Set the Method property of the request to POST.
            request.Method = "GET";
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Clean up the streams and the response.
            reader.Close();
            response.Close();
            return responseFromServer;
        }

        public static List<String> getActors(String imdbID)
        {
            String responseFromServer = getResponseForIMDB_ID(imdbID);
            XDocument doc = XDocument.Parse(responseFromServer);
            var movieActors = doc.Descendants("actors").Elements("item").Select(t => t.Value);
            List<String> allResult = new List<String>();
            foreach (var item in movieActors)
            {
                allResult.Add(item);
            }
            return allResult;
        }

        public static List<String> getGenres(String imdbID)
        {
            String responseFromServer = getResponseForIMDB_ID(imdbID);
            XDocument doc = XDocument.Parse(responseFromServer);
            var movieGenres = doc.Descendants("genres").Elements("item").Select(t => t.Value);
            List<String> allResult = new List<String>();
            foreach (var item in movieGenres)
            {
                allResult.Add(item);
            }
            return allResult;
        }

        public static List<String> getLanguages(String imdbID)
        {
            String responseFromServer = getResponseForIMDB_ID(imdbID);
            XDocument doc = XDocument.Parse(responseFromServer);
            var movieLanguage = doc.Descendants("language").Elements("item").Select(t => t.Value);
            List<String> allResult = new List<String>();
            foreach (var item in movieLanguage)
            {
                allResult.Add(item);
            }
            return allResult;
        }
    }
}

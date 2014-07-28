using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace MovieTrack
{
    class TmdpApi
    {
        private static String getResponse(String movieTitle)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create("http://api.themoviedb.org/2.1/Movie.search/en/xml/c175091044a3420747f2255ec6acaef2/" + movieTitle);
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
            var moviesName = doc.Descendants("original_name").Select(t => t.Value);
            var imdbIDs = doc.Descendants("imdb_id").Select(t => t.Value);
            var moviesYear = doc.Descendants("released").Select(t => t.Value);
            var moviesThumbnails = doc.Descendants("image").Where(t => (t.Attribute("size").Value == "thumb" && t.Attribute("width").Value == "92")).Select(t => t.Attribute("url").Value);
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
                allResult[index].movieYear = item.Split('-')[0];
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
    }
}

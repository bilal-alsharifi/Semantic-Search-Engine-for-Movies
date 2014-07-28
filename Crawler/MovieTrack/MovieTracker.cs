using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Ionic.Zip;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Drawing;

namespace MovieTrack
{
    class MovieTracker
    {
	
        static int index = 1;
        public string path;

        public bool ifNotVisited(List<Url> urlList, Url url)
        {
            foreach (var item in urlList)
            {
                if (item.url.Equals(url.url))
                    return false;
            }
            return true;
        }

        public void downloadSubtitle(Url subtitleDownloadLink, string baseUrl)
        {
            //download subtitle
            byte[] file;
            string fileName = path + @"\" + "SRT";
            DBHelper db = new DBHelper();
            WebClient wc = new WebClient();
            try
            {
                file = wc.DownloadData(subtitleDownloadLink.url);
                MemoryStream ms = new MemoryStream(file);
                ZipFile zip = new ZipFile();
                bool test = false;
                if (ZipFile.IsZipFile(ms, test) && !file.Equals("null"))
                {
                    ms.Position = 0;
                    zip = ZipFile.Read(ms);
                }
                else
                {
                    //download srt files
                    //wc.DownloadFile(url, fileName + ".srt");
                }
                foreach (ZipEntry zipFile in zip)
                {
                    try
                    {
                        if (!File.Exists(fileName))
                        {

                            string extractFileName = zipFile.FileName;
                            if (extractFileName.Contains(".srt") || extractFileName.Contains(".SRT"))
                            {
                                DownloadedMovie movie = getMovieName(subtitleDownloadLink.sourceUrl, baseUrl);
                                if (movie != null)
                                {
                                    bool ifImageDownloaded = downloadMovieImage(movie.movieThumbnail, index, path);
                                    if (ifImageDownloaded)
                                    {
                                        movie.movieThumbnail = index + ".jpg";
                                    }
                                    db.insertMovie(movie);
                                    zipFile.FileName = index + ".srt";
                                    zipFile.Extract(fileName + @"\", ExtractExistingFileAction.DoNotOverwrite);
                                    string language = getMovieLanguage(fileName + @"\" + index + ".srt");//insert language
                                    if (language != null)
                                    {
                                        db.insertLanguage(language);
                                        db.insertSubtitle(index, movie, language);
                                    }
                                    index++;//subtitle id + movie name
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception)
            {
                //Console.WriteLine(we.Message);
            }
        }

        public string getMovieLanguage(string pathOfSrtFile)
        {
            LanguageDetector LD = new LanguageDetector();
            string language;

            string msg = "";
            string text = "";

            Encoding encoding = Encoding.Default;
            String original = String.Empty;

            using (StreamReader sr = new StreamReader(pathOfSrtFile, Encoding.Default))
            {
                for (int i = 0; i < 100; i++)
                {
                    text += sr.ReadLine();
                }
                encoding = sr.CurrentEncoding;
                sr.Close();
            }

            if (encoding == Encoding.UTF8)
                msg = text;

            else
            {
                byte[] encBytes = encoding.GetBytes(text);
                byte[] utf8Bytes = Encoding.Convert(encoding, Encoding.UTF8, encBytes);
                msg = Encoding.UTF8.GetString(utf8Bytes);
            }

            language = LD.Detect(msg);
            string lang = LD.GetLanguageNameByCode(language);
            return lang;
        }

        public DownloadedMovie getExactMovieName(List<string> allSentencesEdited)
        {
            foreach (var item in allSentencesEdited)
            {
                if (item.Equals("") || item.Equals(" ") || item.Equals("  "))
                {
                    continue;
                }
                List<DownloadedMovie> movies = new List<DownloadedMovie>();
                movies.AddRange(ImdpApi.findMovie(item)); //get name from IMDB http://imdbapi.org
                movies.AddRange(TmdpApi.findMovie(item)); //get name from TMDB http://themoviedb.org
                foreach (var item2 in movies)
                {
                    string tempItem2 = item2.movieName.ToLower();
                    tempItem2 = Regex.Replace(tempItem2, "[^0-9a-zA-Z ]+", " ");
                    foreach (var item3 in allSentencesEdited)
                    {
                        if (item3.Contains(tempItem2))
                        {
                            return item2;
                        }
                    }
                }
            }

            return null;
        }

        public DownloadedMovie getMovieName(string url, string baseUrl)
        {
            if (!url.Contains(baseUrl) && url.StartsWith("/"))
            {
                url = "http://" + baseUrl + url;
            }
            if (!url.Contains(baseUrl) && !url.StartsWith("/") && !url.StartsWith("http://"))
            {
                url = "http://" + baseUrl + "/" + url;
            }
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument newdoc = web.Load(url);
            List<string> allLinks = new List<string>();
            try
            {
                allLinks.AddRange(newdoc.DocumentNode.SelectNodes("//title")
                        .Select(x => x.InnerText)
                        .ToList<string>());
                allLinks.AddRange(newdoc.DocumentNode.SelectNodes("//h1")
                        .Select(x => x.InnerText)
                        .ToList<string>());
                List<string> tirmUrl = new List<string>();
                List<string> sentencesFromUrl = new List<string>();
                tirmUrl = url.Split('/').ToList();
                foreach (var item in tirmUrl)
                {
                    sentencesFromUrl.Add(item.Replace('-', ' '));
                }
                allLinks.AddRange(sentencesFromUrl);
                List<string> allSentencesEdited = new List<string>();
                List<string> stopWordForMovies = new List<string>();
                stopWordForMovies.Add("arabic");
                stopWordForMovies.Add("english");
                stopWordForMovies.Add("french");
                stopWordForMovies.Add("dutch");
                stopWordForMovies.Add("spanish");
                stopWordForMovies.Add("subtitles");
                stopWordForMovies.Add("subtitle");
                stopWordForMovies.Add("http");
                stopWordForMovies.Add("movie");
                stopWordForMovies.Add("download");
                stopWordForMovies.Add("title");
                foreach (var item in allLinks)
                {
                    string tempString = Regex.Replace(item, "[^0-9a-zA-Z ]+", "");
                    tempString = tempString.ToLower();
                    foreach (var item2 in stopWordForMovies)
                    {
                        tempString = tempString.Replace(item2, "");
                    }
                    allSentencesEdited.Add(tempString);
                }

                return getExactMovieName(allSentencesEdited);
            }
            catch (Exception)
            {
                //Console.WriteLine(e.Message);
                return null;
            }
        }

        public string getSizeOfURL(string url)
        {
            System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);
            req.Method = "HEAD";
            try
            {
                using (System.Net.WebResponse resp = req.GetResponse())
                {
                    long ContentLength = 0;
                    long result;
                    if (long.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                    {
                        string File_Size;
                        result = ContentLength / 1024;

                        File_Size = result.ToString("0.00");
                        File_Size = File_Size + " KB";
                        if (result > 200)
                        {
                            return "null";
                        }
                        else
                        {
                            return File_Size;
                        }
                    }
                    else
                    {
                        return "null";
                    }
                }
            }
            catch (Exception)
            {
                //Console.WriteLine(we.Message);
                return "null";
            }
        }

        public List<string> getAllLinksInUrl(Url inProgressUrl, string baseUrl)
        {
            HtmlWeb web = new HtmlWeb();
            if (inProgressUrl.url.Equals(baseUrl))
            {
                inProgressUrl.url = "http://" + baseUrl;
            }
            if (!inProgressUrl.url.Contains(baseUrl) && inProgressUrl.url.StartsWith("/"))
            {
                inProgressUrl.url = "http://" + baseUrl + inProgressUrl.url;
            }
            if (!inProgressUrl.url.Contains(baseUrl) && !inProgressUrl.url.StartsWith("/") && !inProgressUrl.url.StartsWith("http://"))
            {
                inProgressUrl.url = "http://" + baseUrl + "/" + inProgressUrl.url;
            }
            WebClient wcf = new WebClient();
            string sizeOfFile = getSizeOfURL(inProgressUrl.url);
            if (sizeOfFile.Equals("null"))
            {
                //the file not accepted
            }
            else
            {
                //the file accepted download it
                downloadSubtitle(inProgressUrl, baseUrl);
            }
            try
            {
                HtmlAgilityPack.HtmlDocument newdoc = web.Load(inProgressUrl.url);
                List<string> allLinks = new List<string>();
                try
                {
                    allLinks.AddRange(newdoc.DocumentNode.SelectNodes("//a[@href]")
                             .Where(y => y.Attributes["href"].Value.StartsWith("http://" + baseUrl) || y.Attributes["href"].Value.StartsWith("/") || !y.Attributes["href"].Value.StartsWith("http"))
                             .Select(x => x.Attributes["href"].Value)
                             .ToList<string>());
                }
                catch (Exception)
                {
                    //Console.WriteLine(e.Message);
                    return null;
                }
                return allLinks;
            }
            catch (Exception)
            {
                //Console.WriteLine(we.Message);
                return null;
            }
        }

        public bool downloadMovieImage(string imageDownloadURL, int index, string path)
        {
            //download subtitle
            string fileName = path + @"\" + "Thumbnails";
            bool ifImageDownLoaded = false;
            WebClient wc = new WebClient();
            try
            {
                Stream ms = wc.OpenRead(imageDownloadURL);
                Bitmap bitmap;
                if (ms != null)
                {
                    bool IsExists = System.IO.Directory.Exists(fileName);
                    if (!IsExists)
                    {
                        System.IO.Directory.CreateDirectory(fileName);
                    }
                    bitmap = new Bitmap(ms);
                    ms.Flush();
                    ms.Close();
                    bitmap.Save(fileName + @"\" + index + ".jpg");
                    ifImageDownLoaded = true;
                    return ifImageDownLoaded;
                }
                return ifImageDownLoaded;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void getMoreInfoAboutMovies()
        {
            List<string> movies = new List<string>();
            DBHelper db = new DBHelper();
            movies = db.getAllMovies();
            if (movies.Count > 0)
            {
                foreach (var imdbIDForMovie in movies)
                {
                    if (!imdbIDForMovie.Equals(""))
                    {
                        List<String> actors = ImdpApi.getActors(imdbIDForMovie);
                        foreach (var item in actors)
                        {
                            db.insertMovie_Actor(item, imdbIDForMovie);
                        }
                        List<String> genres = ImdpApi.getGenres(imdbIDForMovie);
                        foreach (var item in genres)
                        {
                            db.insertMovie_Genre(item, imdbIDForMovie);
                        }
                        List<String> lang = ImdpApi.getLanguages(imdbIDForMovie);
                        foreach (var item in lang)
                        {
                            db.insertMovie_Lang(item, imdbIDForMovie);
                        }
                    }
                }
            }
        }
    }

    public class Url
    {
        public string url;
        public string sourceUrl;
        public bool state;
        public Url()
        {
            this.url = "";
            this.sourceUrl = "";
            this.state = false;
        }
    }

}

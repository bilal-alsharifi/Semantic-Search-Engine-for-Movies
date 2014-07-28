using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IRHomework.Helpers;
using IRHomework.Entities;
using VDS.RDF;


namespace IRHomework
{
    class Program
    {
        static void Main(string[] args)
        {
            #region varialbes
            // editable variables
            String filesPath = @"..\..\..\Files\";
            String javaTool = @"C:\Program Files\Java\jdk1.7.0_09\bin\java.exe";  // do not forget to change this
            int triplesExtractionMethod = 4;
            bool withCoreference = false;
            bool withLemmatization = true;
            // end

            String srtFilesPath = filesPath + @"SRT\";
            String rdfXmlFilesPath = filesPath + @"RDFXML\";
            String toolsPath = filesPath + @"Tools\";
            String javaApp = toolsPath + "javaApp.jar";
            String domainsFinderApp = toolsPath + "DomainsFinder.jar";
            String domainsFinderFilesPath = toolsPath + @"Domains Finder Files";
            Boolean ExDomainsMemoryModeEnabled = true;
            #endregion


            #region convert all SRT files to rdf

            //DataClassesDataContext db = new DataClassesDataContext();
            //String output = null;
            //foreach (var subtitle in db.Subtitles)
            //{
            //    if (subtitle.status == 0 && subtitle.Lang.name.ToLower().Equals("english")) // only process english subtiltes which sa not been processed previoulsy 
            //    {
            //        String srtFile = srtFilesPath + subtitle.subtitleID + ".srt";
            //        String rdfXmlFile = rdfXmlFilesPath + subtitle.subtitleID + ".rdf";

            //        Console.WriteLine("processing: " + srtFile);

            //        output = GeneralHelper.executeCommand("\"" + javaTool + "\"", " -jar " + "\"" + javaApp + "\"" + " " + triplesExtractionMethod + " " + withCoreference + " " + withLemmatization + " " + "\"" + srtFile + "\"");

            //        List<Triple> triples = new List<Triple>();

            //        if (!output.Contains("error n000001")) // if there is no errors in parsing the SRT file
            //        {
            //            String[] listOfTripleStrings = output.Split('\r');
            //            foreach (String tripleString in listOfTripleStrings)
            //            {
            //                if (tripleString.Length > 1) // to prevent processing empty lines
            //                {
            //                    if (!tripleString.Contains("error n000002")) // if there is no errors in extracting triples from one scene
            //                    {
            //                        triples.Add(new Triple(subtitle.Movie.name, tripleString));
            //                    }
            //                    else
            //                    {
            //                        Console.WriteLine("can not extract triples from one scene.");
            //                    }
            //                }
            //            }


            //            Console.WriteLine("finding domains...");
            //            DomainFinderHelper.findDomains(triples, javaTool, domainsFinderApp, domainsFinderFilesPath, ExDomainsMemoryModeEnabled);

            //            Console.WriteLine("counting domains frequency...");
            //            foreach (Triple tr in triples)
            //            {
            //                Domain selectedDomain = db.Domains.SingleOrDefault(d => d.name == tr.getDomain());
            //                Domain domain = null;
            //                if (selectedDomain == null)
            //                {
            //                    domain = new Domain();
            //                    domain.name = tr.getDomain();
            //                    domain.frequency = 1;
            //                    db.Domains.InsertOnSubmit(domain);
            //                }
            //                else
            //                {
            //                    domain = selectedDomain;
            //                    domain.frequency++;
            //                }
            //            }

            //            Console.WriteLine("annotating...");
            //            foreach (Triple tr in triples)
            //            {
            //                SpotLightHelper.annotateTriple(tr);
            //            }

            //            Console.WriteLine("finding abstract triples...");
            //            List<Triple> newTriples = new List<Triple>();
            //            foreach (Triple tr in triples)
            //            {
            //                List<Triple> abstractTriples = DotNetRDFHelper.getAbstractTriples(tr);
            //                newTriples.AddRange(abstractTriples);
            //            }


            //            Console.WriteLine("saving...");
            //            IGraph iGraph = DotNetRDFHelper.getIGraphFromTriples(newTriples);
            //            iGraph.SaveToFile(rdfXmlFile);


            //            // flag subtitles as processed to prevent processing them later
            //            subtitle.status = 1;
            //            db.SubmitChanges();
            //            //

            //            Console.WriteLine("subtitle " + subtitle.subtitleID + " has been converted and saved successfully");

            //            // validation
            //            String rdfXmlString = DotNetRDFHelper.getRdfXmlString(rdfXmlFile);
            //            String errors = DotNetRDFHelper.validateRdfXmlString(rdfXmlString);
            //            if (errors == null)
            //            {
            //                Console.WriteLine("the file has been checked, and it is valid.");
            //            }
            //            else
            //            {
            //                Console.WriteLine("the file has been checked, and it contains the flollwing errors.");
            //                Console.WriteLine(errors);
            //            }
            //            Console.WriteLine("-------------------------------------------------------");
            //            // end
            //        }
            //        else
            //        {
            //            Console.WriteLine("can not parse the SRT file.");
            //        }
            //    }
            //}

            #endregion


            #region user query

            //String inputText = "I will be fine.";
            //List<String> actors = new List<String>();
            //actors.Add("Uriah Shelton");
            //List<String> languages = new List<String>();
            //languages.Add("English");
            //List<String> genres = new List<String>();
            //genres.Add("Family");
            //String year = "";

            //List<Triple> userQueryTriples = UserQueryHelper.getUserQueryTriples(inputText, javaTool, javaApp, triplesExtractionMethod, withCoreference, withLemmatization);

            //List<Triple> triplesThatMatchSearch = UserQueryHelper.getTriplesThatMatchSearch(userQueryTriples, actors, languages, genres, year, rdfXmlFilesPath);

            //List<SearchResult> searchResults = UserQueryHelper.getSearchResutls(triplesThatMatchSearch);
            //foreach (var sr in searchResults)
            //{
            //    Console.WriteLine(sr);
            //}

            #endregion

        }
    }
}


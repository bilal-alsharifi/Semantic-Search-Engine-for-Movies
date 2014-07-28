using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Validation;
using IRHomework.Helpers;
using VDS.RDF.Query;
using HtmlAgilityPack;
using System.Net;

namespace IRHomework
{
    class DotNetRDFHelper
    {
        public static String getRdfXmlString(String rdfXmlFile)
        {
            String rdfXmlString = null;
            using (StreamReader sr = new StreamReader(rdfXmlFile))
            {
                rdfXmlString = sr.ReadToEnd();
            }
            return rdfXmlString;
        }

        public static String validateRdfXmlString(String rdfXmlString)
        {
            String result = null;
            IRdfReader parse = new RdfXmlParser(RdfXmlParserMode.Streaming);
            RdfStrictSyntaxValidator validator = new RdfStrictSyntaxValidator(parse);
            ISyntaxValidationResults validate = validator.Validate(rdfXmlString);
            if (validate.Error != null)
            {
                result = validate.Error.GetBaseException().Message;
            }
            return result;
        }
		
		public static IGraph getIGraphFromTriples(List<Triple> triples)
        {
            IGraph iGraph = new Graph();
            foreach (Triple tr in triples)
            {
                IUriNode su = iGraph.CreateUriNode(UriFactory.Create(tr.getSubject()));
                IUriNode pr = iGraph.CreateUriNode(UriFactory.Create(tr.getPredicate()));
                ILiteralNode ob = iGraph.CreateLiteralNode(tr.getObject() + "@" + tr.getSceneTime() + "#" + tr.getDomain());
                iGraph.Assert(new VDS.RDF.Triple(su, pr, ob));     
            }
            return iGraph;
        }

        //Added By Anna
        public static List<Triple> getTriplesFromIGraph(IGraph iGraph)
        {
            List<Triple> triples = new List<Triple>();
            foreach (VDS.RDF.Triple tr in iGraph.Triples)
            {
                triples.Add(new Triple(tr.Subject.ToString(), tr.Predicate.ToString(), tr.Object.ToString(), ""));
            } 
            return triples;
        }

        public static IGraph getIGraphFromRdfXmlFile(String rdfXmlFile)
        {
            IGraph iGraph = new Graph();
            iGraph.LoadFromFile(rdfXmlFile);
            return iGraph;
        }

        public static IGraph getIGraphFromRdfXmlString(String rdfXmlString)
        {
            IGraph iGraph = new Graph();
            iGraph.LoadFromString(rdfXmlString);
            return iGraph;
        }
        public static SparqlResultSet getTypesFromDBPedia()
        {
            SparqlResultSet resultSet = null;
            String query = "select ?type {"
                         + "?type a owl:Class ."
                         + "}"
                         + "ORDER BY ASC(?type)";
            resultSet = queryDBPedia(query);
            return resultSet;
        }
		
		public static SparqlResultSet getTypesOfResource(String uri)
        {
            SparqlResultSet resultSet = null;
            String query = "SELECT * WHERE { <" + uri + "> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?type }";
            resultSet = queryDBPedia(query);
            return resultSet;
        }
		
		public static List<Triple> getAbstractTriples(Triple triple)
        {
            List<Triple> result = new List<Triple>();
            result.Add(triple);
            SparqlResultSet resultSet = getTypesOfResource(triple.getSubject());
            foreach (var r in resultSet)
            {
                String type = r.Value("type").ToString();
                Triple newTriple = triple.clone();
                newTriple.setSubject(type);
                result.Add(newTriple);
            }
            return result;
        }
		
        public static SparqlResultSet queryDBPedia(String query)
        {
            SparqlResultSet resultSet = null;
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");
            resultSet = endpoint.QueryWithResultSet(query);
            return resultSet;
        }
		
		public static SparqlResultSet queryIGraph(String query, IGraph iGraph)
        {
            SparqlResultSet resultSet = null;
            Object results = iGraph.ExecuteQuery(query);
            resultSet = (SparqlResultSet)results;
            return resultSet;
        }
        public static List<Triple> queryIGraph(String query, IGraph iGraph, String movieTitle)
        {
            List<Triple> result = new List<Triple>();
            SparqlResultSet resultSet = queryIGraph(query, iGraph);
            foreach (var r in resultSet)
            {
                Triple tr = new Triple(r.Value("su").ToString(), r.Value("pr").ToString(), r.Value("ob").ToString(), movieTitle);
                result.Add(tr);
            }
            return result;
        }
        public static List<Triple> queryIGraph(String su, String pr, String ob, String domain, IGraph iGraph, String movieTitle)
        {
            su = su.ToLower();
            pr = pr.ToLower();
            ob = ob.ToLower();
            String query = "SELECT * WHERE {?su ?pr ?ob . FILTER regex(lcase(str(?su)), '^.*" + su + ".*') . FILTER regex(lcase(str(?pr)), '^.*" + pr + ".*') . FILTER regex(lcase(str(?ob)), '^.*" + ob + ".*" + "#" + ".*" + domain + ".*')}";
            return queryIGraph(query, iGraph, movieTitle);
        }
		
        //Edited By Anna
        public static void drawIGraph(VDS.RDF.IGraph iGraph, Microsoft.Glee.GraphViewerGdi.GViewer gViewer)
        {
            Microsoft.Glee.Drawing.Graph gleeGraph = new Microsoft.Glee.Drawing.Graph("label");
            foreach (var tr in iGraph.Triples)
            {
                String su = tr.Subject.ToString();
                String pr = tr.Predicate.ToString();
                String ob = tr.Object.ToString().Split('@')[0];
                Microsoft.Glee.Drawing.IEdge edge = gleeGraph.AddEdge(su, pr, ob);
                edge.EdgeAttr.Id = su+pr+ob;
                Microsoft.Glee.Drawing.INode suNode = gleeGraph.FindNode(su);
                Microsoft.Glee.Drawing.INode obNode = gleeGraph.FindNode(ob);
                edge.EdgeAttr.Color = Microsoft.Glee.Drawing.Color.Green;
                edge.EdgeAttr.Fontcolor = Microsoft.Glee.Drawing.Color.Red;
                suNode.NodeAttribute.Fontcolor = Microsoft.Glee.Drawing.Color.Blue;
                obNode.NodeAttribute.Fontcolor = Microsoft.Glee.Drawing.Color.Blue;
                obNode.NodeAttribute.Shape = Microsoft.Glee.Drawing.Shape.Box;
            }
            gViewer.Graph = gleeGraph;
            gViewer.OutsideAreaBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);     
        }

        public static bool IfUrlNotExist(string url)
        {
            bool ifExist = false;
            HtmlWeb web = new HtmlWeb();
            try
            {
                HtmlAgilityPack.HtmlDocument doc = web.Load(url);
                List<string> all_P_Tags = new List<string>();
                try
                {
                    all_P_Tags.AddRange(doc.DocumentNode.Descendants("p").Select(t => t.InnerText).ToList<string>());
                    foreach (var item in all_P_Tags)
                    {
                        if (item.Equals("No further information is available. (The requested entity is unknown)"))
                        {
                            ifExist = true;
                        }
                    }
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
                return ifExist;
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}

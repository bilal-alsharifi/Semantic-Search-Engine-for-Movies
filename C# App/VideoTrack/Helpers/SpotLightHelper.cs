using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace IRHomework.Helpers
{
    public class SpotLightHelper
    {
        private static String getResponse(String text, String subject, int offsetOfSubject, String predicate, int offsetOfPredicate)
        {
            String serviceEndpoint = "http://spotlight.dbpedia.org/rest/annotate";
            String address = serviceEndpoint + "?spotter=SpotXmlParser" + "&text=" +  "<annotation text=\"" + text + ".\"> <surfaceForm name=\"" + subject + "\" offset=\"" + offsetOfSubject + "\"/> <surfaceForm name=\"" + predicate + "\" offset=\"" + offsetOfPredicate + "\"/> </annotation>";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "text/xml";
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            String responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            return responseFromServer;
        }
        public static void annotateTriple(Triple triple)
        {
            String dummyURI = "http://dummy.org/";
            String sentence = triple.getSentence();
            String subject = triple.getSubject();
            String predicate = triple.getPredicate();
            int offsetOfSubject = sentence.IndexOf(subject);
            int offsetOfPredicate = sentence.IndexOf(predicate);
            String responseFromServer = null;
            try
            {
                responseFromServer = getResponse(sentence, subject, offsetOfSubject, predicate, offsetOfPredicate);
            }
            catch (Exception)
            {
                
            }
            String annotatedSubject = null;
            String annotatedPredicate = null;
            if (responseFromServer != null)
            {
                XDocument doc = XDocument.Parse(responseFromServer);
                var nodes = doc.Descendants("Resources").Descendants("Resource").ToList();
                String uri1 = null;
                String uri2 = null;
                int offset1 = 0;
                int offset2 = 0;
                if (nodes.Count >= 1)
                {
                    uri1 = nodes[0].Attribute("URI").Value;
                    offset1 = int.Parse(nodes[0].Attribute("offset").Value);
                }
                if (nodes.Count >= 2)
                {
                    uri2 = nodes[1].Attribute("URI").Value;
                    offset2 = int.Parse(nodes[1].Attribute("offset").Value);
                }
                if (uri1 != null)
                {
                    if (offset1 == offsetOfSubject)
                    {
                        annotatedSubject = uri1;
                    }
                    else if (offset1 == offsetOfPredicate)
                    {
                        annotatedPredicate = uri1;
                    }
                }
                if (uri2 != null)
                {
                    if (offset2 == offsetOfSubject)
                    {
                        annotatedSubject = uri2;
                    }
                    else if (offset2 == offsetOfPredicate)
                    {
                        annotatedPredicate = uri2;
                    }
                }
            }
            if (annotatedSubject == null)
            {
                annotatedSubject = dummyURI + subject;
            }
            if (annotatedPredicate == null)
            {
                annotatedPredicate = dummyURI + predicate;
            }
            triple.setSubject(Uri.UnescapeDataString(annotatedSubject));
            triple.setPredicate(Uri.UnescapeDataString(annotatedPredicate));
            if (!triple.subjectIsValid())
            {
                annotatedSubject = dummyURI + subject;
                triple.setSubject(annotatedSubject);
            }
            if (!triple.predicateIsValid())
            {
                annotatedPredicate = dummyURI + predicate;
                triple.setPredicate(annotatedPredicate);
            }
        }
    }
}

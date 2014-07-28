using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRHomework.Helpers
{
    class DomainFinderHelper
    {
        public static void findDomains(List<Triple> triples, String javaTool, String domainsFinderApp, String domainsFinderFilesPath, Boolean ExDomainsMemoryModeEnabled)
        {
            String domainsFinderInput = "";
            foreach (var tr in triples)
            {
                domainsFinderInput += tr.getSentence() + ";";
            }
            String domainsFinderOutput = GeneralHelper.executeCommand("\"" + javaTool + "\"", " -jar " + "\"" + domainsFinderApp + "\"" + " " + "\"" + domainsFinderFilesPath + "\"" + " " + "\"" + domainsFinderInput + "\"" + " " + ExDomainsMemoryModeEnabled);
            String[] domains = domainsFinderOutput.Split(';');
            for (int t = 0; t < triples.Count; t++)
            {
                triples.ElementAt(t).setDomain(domains[t]);
            }
        }
    }
}

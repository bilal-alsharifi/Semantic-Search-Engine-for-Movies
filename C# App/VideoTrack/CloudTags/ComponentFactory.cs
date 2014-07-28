using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using VideoTrack.TextAnalyses;

using VideoTrack.TextAnalyses.Extractors;


namespace VideoTrack
{
    internal static class ComponentFactory
    {
     

        public static IEnumerable<string> CreateExtractor(VideoTrackDataContext db)
        {
            
             return new StringExtractor(db);
            
        }

    }
}
using System.Collections.Generic;
using System.Linq;
using System;

namespace VideoTrack.TextAnalyses.Extractors
{
    public class StringExtractor : BaseExtractor
    {
        private readonly string m_Text;

        public StringExtractor(VideoTrackDataContext db)
        {

            var res = from w in db.Domains
                      select w;

            foreach (var term in res)
            {
                int normalize = (int)(term.frequency / 5);
                for (int i = 0; i < normalize; i++)
                {
                    m_Text = m_Text + term.name + " ";
                }
            }
        }

        public override IEnumerable<string> GetWords()
        {
            return GetWords(m_Text);
        }

    }
}

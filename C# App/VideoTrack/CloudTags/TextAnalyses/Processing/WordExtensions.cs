using System;
using System.Collections.Generic;
using System.Linq;

namespace VideoTrack.TextAnalyses.Processing
{
    public static class WordExtensions
    {

        public static IOrderedEnumerable<T> SortByOccurences<T>(this IEnumerable<T> words) where T : IWord
        {
            return 
                words.OrderByDescending(
                    word => word.Occurrences);
        }

        public static IEnumerable<IWord> CountOccurences(this IEnumerable<string> terms)
        {
            return 
                terms.GroupBy(
                    term => term,
                    (term, equivalentTerms) => new Word(term, equivalentTerms.Count()), 
                    StringComparer.InvariantCultureIgnoreCase)
                    .Cast<IWord>();
        }
    }
}
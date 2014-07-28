using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VideoTrack.TextAnalyses.Processing;

namespace VideoTrack.Geometry
{
    public interface ILayout
    {
        void Arrange(IEnumerable<IWord> words, IGraphicEngine graphicEngine);
        IEnumerable<LayoutItem> GetWordsInArea(RectangleF area);
    }
}
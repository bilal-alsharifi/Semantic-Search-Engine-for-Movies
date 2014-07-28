using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using VideoTrack.TextAnalyses.Processing;
using VideoTrack.Geometry;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace VideoTrack
{
    public partial class TagsCloud : DevExpress.XtraEditors.XtraForm
    {
        private VideoTrackDataContext db = new VideoTrackDataContext();
        public string selectedTag = "";
        public TagsCloud()
        {
            InitializeComponent();
            ProcessText();
        }

        private void ProcessText()
        {
            IEnumerable<string> terms = ComponentFactory.CreateExtractor(db);

            IEnumerable<IWord> words = terms.CountOccurences();

            cloudControl.WeightedWords = words.SortByOccurences().Cast<IWord>();
        }

        private void cloudControl_Click(object sender, EventArgs e)
        {
            LayoutItem itemUderMouse;
            Point mousePositionRelativeToControl = cloudControl.PointToClient(new Point(MousePosition.X, MousePosition.Y));
            if (!cloudControl.TryGetItemAtLocation(mousePositionRelativeToControl, out itemUderMouse))
            {
                return;
            }
            MessageBox.Show(itemUderMouse.Word.GetCaption(), string.Format("Statistics for word [{0}]", itemUderMouse.Word.Text));
            selectedTag = itemUderMouse.Word.Text;
        }

    }
}



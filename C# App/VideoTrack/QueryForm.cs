using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using IRHomework;
using IRHomework.Helpers;
using LAIR.ResourceAPIs.WordNet;
using LAIR.Collections.Generic;
using IRHomework.Entities;
using System.Linq;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using System.Speech;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using DevExpress.XtraLayout.Dragging;
using DevExpress.XtraLayout.Customization;
using System.Net.Mail;
using System.Net.Mime;


namespace VideoTrack
{
    public partial class QueryForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        VideoTrackDataContext db = new VideoTrackDataContext();
        String javaTool = @"C:\Program Files\Java\jdk1.7.0_09\bin\java.exe";
        string filesPath = @"..\..\..\Files\";
        String toolsPath;
        String javaApp;
        String stemmerTool;
        int triplesExtractionMethod = 4;
        bool withCoreference = false;
        bool withLemmatization = true;
        String dictPath;
        WordNetEngine wordNetEngine;
        String rdfXmlFilesPath;
        List<String> emailInfo = new List<string>();
        SpeechSynthesizer speechSyntheizer = new SpeechSynthesizer();
        SpeechRecognitionEngine speechRecognizer = new SpeechRecognitionEngine();
        SpeechRecognitionEngine dictationRecognizer = new SpeechRecognitionEngine();
        MailMessage mail = new MailMessage();

        public QueryForm()
        {
            InitializeComponent();
            Grammar speechGrammar = new Grammar(new GrammarBuilder(new Choices("Open", "Close", "copy", "paste", "cut")));
            this.speechRecognizer.SetInputToDefaultAudioDevice();
            this.speechRecognizer.LoadGrammar(speechGrammar);
            this.speechRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechEventHandler);
            this.speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);

            this.dictationRecognizer.SetInputToDefaultAudioDevice();
            this.dictationRecognizer.LoadGrammar(new DictationGrammar());
            this.dictationRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(DictationSpeechRecognized);

            toolsPath = filesPath + @"Tools\";
            javaApp = toolsPath + "javaApp.jar";
            stemmerTool = toolsPath + "Stemmer.jar";
            dictPath = toolsPath + @"Domains Finder Files\wordnet\dict30";
            wordNetEngine = new WordNetEngine(dictPath, false);
            rdfXmlFilesPath = filesPath + @"RDFXML\";

            foreach (Actor actor in db.Actors)
            {
                comboBoxEdit1.Properties.Items.Add(actor.name);
            }

            foreach (Genre genre in db.Genres)
            {
                comboBoxEdit2.Properties.Items.Add(genre.name);
            }

            foreach (Lang language in db.Langs)
            {
                comboBoxEdit3.Properties.Items.Add(language.name);
            }
            List<string> res = (from x in db.Movies
                                orderby x.year ascending
                                select x.year).Distinct().ToList();
            foreach (string year in res)
            {
                comboBoxEdit4.Properties.Items.Add(year);
            }
            foreach (Domain domain in db.Domains)
                repositoryItemComboBox1.Items.Add(domain.name);
            
        }
        
        private void DictationSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            textEdit1.Text += e.Result.Text;
        }

        private void SpeechEventHandler(object sender, SpeechRecognizedEventArgs e)
        {
            //if (e.Result.Confidence > 0.9)
            //    MessageBox.Show(e.Result.Text, "And you said!", MessageBoxButtons.OK);
        }

        public void removeElementsFromListbox(DevExpress.XtraEditors.ListBoxControl listBoxControl)
        {
            List<String> toRemoveItems = new List<string>();
            foreach (String item in listBoxControl.SelectedItems)
            {
                toRemoveItems.Add(item);
            }
            foreach (String item in toRemoveItems)
            {
                listBoxControl.Items.Remove(item);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            removeElementsFromListbox(listBoxControl1);
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            removeElementsFromListbox(listBoxControl2);
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            removeElementsFromListbox(listBoxControl3);
        }

        //search I'm Feeling Lucky
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            listView1.Clear();

            string inputText = textEdit1.Text;
            //get filters selected by user
            //actors
            List<String> actors = new List<String>();
            //genres
            List<String> genres = new List<String>();
            //languages
            List<String> languages = new List<String>();
            //release year
            string year = "";
            
            //extract triples from the free text
            List<Triple> userQueryTriples = UserQueryHelper.getUserQueryTriples(inputText, javaTool, javaApp, triplesExtractionMethod, withCoreference, withLemmatization);

            repositoryItemPictureEdit1.SizeMode = PictureSizeMode.Zoom;
            repositoryItemPictureEdit1.NullText = " ";

            gridControl2.DataSource = GetData(userQueryTriples);
            
            List<Triple> triplesThatMatchSearch = UserQueryHelper.getTriplesThatMatchSearch(userQueryTriples, actors, languages, genres, year, rdfXmlFilesPath);
            List<SearchResult> searchResults = UserQueryHelper.getSearchResutls(triplesThatMatchSearch);
            showResults(searchResults);
        }
        //create the grid columns
        public DataTable createColumns()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Subject", typeof(string)));
            table.Columns.Add(new DataColumn("Predicate", typeof(string)));
            table.Columns.Add(new DataColumn("Object", typeof(string)));
            table.Columns.Add(new DataColumn("Domain", typeof(string)));
            return table;
        }
        //fill the grid 
        public DataTable GetData(List<Triple> triples)
        {
            DataTable table = createColumns();
            foreach (Triple tr in triples)
            {
                table.Rows.Add(new object[] { tr.getSubject(), tr.getPredicate(), tr.getObject() , tr.getDomain() });
            }
            
            return table;
        }

        public List<Triple> createCombinations (List<String> subjectSynonyms, List<String> predicateSynonyms, List<String> objectSynonyms, String domain)
	    {
            List<List<String>> combs = new List<List<String>>();
            combs = findAllCombinations(combs, subjectSynonyms);
            combs = findAllCombinations(combs, predicateSynonyms);
            combs = findAllCombinations(combs, objectSynonyms);
            List<Triple> triples = new List<Triple>();
            foreach (List<String> tr in combs)
            {
                triples.Add(new Triple(tr[0], tr[1], tr[2], "", domain, ""));
            }
            return triples;
	    }

        public List<List<String>> findAllCombinations (List<List<String>> combs, List<String> synonyms)
	    {
		    List<String> combination = new List<String>();
            List<List<String>> temp = new List<List<string>>();
		    if (combs.Count == 0)
		    {
			    for (int i = 0; i< synonyms.Count; i++)
			    {
				    combination = new List<String>();
				    combination.Add(synonyms[i]);
				    combs.Add(combination);
			    }
		    }
		    else
		    {
			    for (int i = 0; i< combs.Count; i++)
			    {
				    for (int j = 0; j< synonyms.Count; j++)
				    {
					    combination = new List<String>();
					    combination.AddRange(combs[i]);
					    combination.Add(synonyms[j]);
                        temp.Add(combination);
				    }				
			    }
                combs = temp;
            }
            return combs;
	    }
   
        public List<String> check_allQueryWordsInTriples(string triples, string queryText)
        {
            List<string> words = GeneralHelper.stem(queryText, javaTool, javaApp, stemmerTool);
            string stopWords = GeneralHelper.getStopWords(filesPath + "StopWords.txt");
            List<String> notContainedWords = new List<string>();
            
            for (int i = 0; i < words.Count; i++)
            {
                if (!triples.Contains(words[i]))
                    if (!stopWords.Contains(words[i]))
                        notContainedWords.Add(words[i]);
            }
            return notContainedWords;
        }

        private void showResults(List<SearchResult> results)
        {
            listView1.FullRowSelect = true;
            listView1.TileSize = new System.Drawing.Size(400, 150);
            ImageList imageListLarge = new ImageList();
            imageListLarge.ImageSize = new System.Drawing.Size(82, 100);
            int i = 0;
            foreach (SearchResult result in results)
            {
                ListViewItem item1 = new ListViewItem(result.getMovieTitle());
                ListViewItem.ListViewSubItem subitem = new ListViewItem.ListViewSubItem(item1, result.getStartTime() + "->" + result.getEndTime());
                //item1.SubItems.Add(result.getStartTime() + "->" + result.getEndTime());
                item1.SubItems.Add(subitem);
                item1.SubItems.Add(result.getStartTime() + "->" + result.getEndTime());
                
                //from thumbnail
                var res = from x in db.Movies
                          where x.name == result.getMovieTitle()
                          select x.thumbnail;
                imageListLarge.Images.Add(Bitmap.FromFile(filesPath + "Thumbnails\\" + res.First()));
                listView1.LargeImageList = imageListLarge;
                item1.ImageIndex = i;
                listView1.Items.Add(item1);
                i++;
            }

        }
        //add rows
        private void simpleButton2_Click_1(object sender, EventArgs e)
        {
            if (gridView2.RowCount == 0)
            {
                gridControl2.DataSource = createColumns();
                gridView2.AddNewRow();
                repositoryItemPictureEdit1.SizeMode = PictureSizeMode.Zoom;
                repositoryItemPictureEdit1.NullText = " ";
            }
            else
                gridView2.AddNewRow();
        }

        private void gridView2_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column == gridColumn8 && e.IsGetData)
            {
                e.Value = imageList1.Images[0];
            }
        }

        private void comboBoxEdit2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (!listBoxControl2.Items.Contains(comboBoxEdit2.SelectedItem))
                listBoxControl2.Items.Add(comboBoxEdit2.SelectedItem);
        }

        private void comboBoxEdit3_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (!listBoxControl3.Items.Contains(comboBoxEdit3.SelectedItem))
                listBoxControl3.Items.Add(comboBoxEdit3.SelectedItem);
        }

        private void comboBoxEdit1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (!listBoxControl1.Items.Contains(comboBoxEdit1.SelectedItem))
                listBoxControl1.Items.Add(comboBoxEdit1.SelectedItem);
        }
        //Search all
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            listView1.Clear();

            string inputText = textEdit1.Text;
            //get filters selected by user
            //actors
            List<String> actors = new List<String>();
            for (int i = 0; i < listBoxControl1.Items.Count; i++)
            {
                actors.Add(listBoxControl1.Items[i].ToString());
            }
            //genres
            List<String> genres = new List<String>();
            for (int i = 0; i < listBoxControl2.Items.Count; i++)
            {
                genres.Add(listBoxControl2.Items[i].ToString());
            }
            //languages
            List<String> languages = new List<String>();
            for (int i = 0; i < listBoxControl3.Items.Count; i++)
            {
                languages.Add(listBoxControl3.Items[i].ToString());
            }
            //release year
            string year = "";
            if (comboBoxEdit4.SelectedItem != null)
                year = comboBoxEdit4.SelectedItem.ToString();
            //extract triples from the free text
            List<Triple> userQueryTriples = UserQueryHelper.getUserQueryTriples(inputText, javaTool, javaApp, triplesExtractionMethod, withCoreference, withLemmatization);
            //find words that couldn't extract triples from
            string triplesString = "";
            foreach (Triple tr in userQueryTriples)
            {
                triplesString += tr.getSentence();
            }
            List<String> notContaintedWords = check_allQueryWordsInTriples(triplesString, inputText);
            string s = "This words couldn't result a triple, use manual adding triples to suggest their locations more accurately: ";
            for (int i = 0; i < notContaintedWords.Count; i++)
                s += notContaintedWords[i] + " , ";
            if (notContaintedWords.Count != 0)
                MessageBox.Show(s);
            //get triples from the grid
            for (int i = 0; i < gridView2.DataRowCount; i++)
            {
                userQueryTriples.Add(new Triple((String)gridView2.GetRowCellValue(i, "Subject"), (String)gridView2.GetRowCellValue(i, "Predicate"), (String)gridView2.GetRowCellValue(i, "Object"), "", (String)gridView2.GetRowCellValue(i, "Domain"), ""));
            }

            List<Triple> allTriples = new List<Triple>();
            foreach (Triple tr in userQueryTriples)
            {
                List<String> subjectSynonyms = new List<String>();
                List<String> predicateSynonyms = new List<String>();
                List<String> objectSynonyms = new List<String>();
                SynSet synSetsToShow = wordNetEngine.GetMostCommonSynSet(tr.getSubject(), WordNetEngine.POS.Noun);
                if (synSetsToShow != null)
                    subjectSynonyms.AddRange(synSetsToShow.Words);
                else
                    subjectSynonyms.Add(tr.getSubject());
                synSetsToShow = wordNetEngine.GetMostCommonSynSet(tr.getPredicate(), WordNetEngine.POS.Verb);
                if (synSetsToShow != null)
                    predicateSynonyms.AddRange(synSetsToShow.Words);
                else
                    predicateSynonyms.Add(tr.getPredicate());
                synSetsToShow = wordNetEngine.GetMostCommonSynSet(tr.getObject(), WordNetEngine.POS.Noun);
                if (synSetsToShow != null)
                    objectSynonyms.AddRange(synSetsToShow.Words);
                else
                    objectSynonyms.Add(tr.getObject());

                allTriples.AddRange(createCombinations(subjectSynonyms, predicateSynonyms, objectSynonyms, tr.getDomain()));
            }

            repositoryItemPictureEdit1.SizeMode = PictureSizeMode.Zoom;
            repositoryItemPictureEdit1.NullText = " ";

            gridControl2.DataSource = GetData(userQueryTriples);

            List<Triple> triplesThatMatchSearch = UserQueryHelper.getTriplesThatMatchSearch(allTriples, actors, languages, genres, year, rdfXmlFilesPath);
            List<SearchResult> searchResults = UserQueryHelper.getSearchResutls(triplesThatMatchSearch);
            showResults(searchResults);
         
        }
        //delete triple in grid
        private void gridView2_Click_1(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            Point clickPoint = view.GridControl.PointToClient(Control.MousePosition);
            var hitInfo = gridView2.CalcHitInfo(clickPoint);
            if (hitInfo.InRowCell)
            {
                int rowHandle = hitInfo.RowHandle;
                GridColumn column = hitInfo.Column;
                if (column == gridColumn8)
                {
                    gridView2.DeleteRow(rowHandle);
                }
            }
        }
        //start voice
        private void label1_Click(object sender, EventArgs e)
        {
            this.speechRecognizer.RecognizeAsyncCancel();
            this.dictationRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            label1.Visible = false;
            label2.Visible = true;
        }
        //stop voice
        private void label2_Click(object sender, EventArgs e)
        {
            this.speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            this.dictationRecognizer.RecognizeAsyncCancel();
            label2.Visible = false;
            label1.Visible = true;
        }

        
        private void simpleButton4_Click_1(object sender, EventArgs e)
        {
            BuyScenes form = new BuyScenes(listView1.SelectedItems);
            form.Show();
                
        }

        private void listView1_DoubleClick_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                var res = from x in db.Movies
                          where x.name == listView1.SelectedItems[0].Text
                          select x.video;
                if (res != null)
                {
                    MoviePlayer form1 = new MoviePlayer(res.First(), listView1.SelectedItems[0].SubItems[0].Text);
                    form1.Show();
                }
                else
                    MessageBox.Show("There is no attached video with that movie!!");
            }
        }
        //search by domain
        private void simpleButton3_Click_1(object sender, EventArgs e)
        {
            TagsCloud form = new TagsCloud();
            form.ShowDialog();
            textEdit1.Text = form.selectedTag;
            List<Triple> allTriples = new List<Triple>();
            allTriples.Add(new Triple("","","","",textEdit1.Text,""));
            List<Triple> triplesThatMatchSearch = UserQueryHelper.getTriplesThatMatchSearch(allTriples, new List<String>(), new List<String>(), new List<String>(), "", rdfXmlFilesPath);
            List<SearchResult> searchResults = UserQueryHelper.getSearchResutls(triplesThatMatchSearch);
            showResults(searchResults);
        }


    }
}
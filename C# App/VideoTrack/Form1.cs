using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.EntityModel;
using IRHomework;
using System.Diagnostics;
using VDS.RDF;
using System.Net;
using System.IO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using System.Threading;
using DevExpress.XtraEditors.Controls;
using IRHomework.Helpers;

namespace VideoTrack
{
    public partial class Form1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        VideoTrackDataContext db1 = new VideoTrackDataContext();
        Helpers.MovieModel movieModel = new Helpers.MovieModel();
        string  filesPath = @"..\..\..\Files\";
        string  javaTool = "";
        string srtFilesPath = "";
        string processedLang = "english";

        public Form1()
        {
            InitializeComponent();
            
            var res = from x in db1.Subtitles
                      where x.status == 0
                      select new
                      {
                          MovieThumbnail = System.Drawing.Image.FromFile(filesPath + "Thumbnails\\" + x.Movie.thumbnail),
                          MovieName = x.Movie.name,
                          ReleaseYear = x.Movie.year,
                          SubtitleLanguage = x.Lang.name,
                          ToConvert = true,
                          x.subtitleID,
                          x.status
                      };

            gridControl1.DataSource = res.ToList();
        }
        //change tabs
        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPageIndex != 0)
            {
                dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                if (xtraTabControl1.SelectedTabPageIndex == 1)
                {
                    var res = from x in db1.Subtitles
                              where x.status == 1
                              select new
                              {
                                  MovieThumbnail = System.Drawing.Image.FromFile(filesPath + "Thumbnails\\" + x.Movie.thumbnail),
                                  MovieName = x.Movie.name,
                                  ReleaseYear = x.Movie.year,
                                  SubtitleLanguage = x.Lang.name,
                                  x.subtitleID,
                              };

                    gridControl2.DataSource = res.ToList();
                }
                else
                {
                    var res = (from x in db1.Movies
                              select new { MovieID = x.movieID, MovieName = x.name }).ToList();

                    lookUpEdit1.Properties.DataSource = res.ToList();
                    lookUpEdit1.SelectedText = "Select Movie";
                    lookUpEdit1.Properties.DisplayMember = "MovieName";
                    lookUpEdit1.Properties.ValueMember = "MovieID";
                    lookUpEdit1.Properties.NullText = "Select Movie";
                    lookUpEdit1.Properties.Columns.Add(new LookUpColumnInfo("MovieID"));
                    lookUpEdit1.Properties.Columns["MovieID"].Visible = false;
                    lookUpEdit1.Properties.Columns.Add(new LookUpColumnInfo("MovieName"));
                }
            }
            else
            {
                dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                dockPanel2.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            }
        }
        //press Edit Movie tab
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            xtraTabControl1.SelectedTabPageIndex = 2;
            dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            dockPanel2.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            
        }
        //press Edit and Draw RDF tab
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            xtraTabControl1.SelectedTabPageIndex = 1;
            dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            dockPanel2.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
        }
        //open Generate RDF tab
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            xtraTabControl1.SelectedTabPageIndex = 0;
            dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
        }
        //call generate RDF in thread
        private void simpleButton1_Click(object sender, EventArgs e)  
        {
            //inialization
            Thread thread = new Thread(() => generateRDF());
            thread.Start();
        }    
        //generate the RDF file
        public void generateRDF()
        {
            if (srtFilesPath == "")
                srtFilesPath = filesPath + @"SRT\";
            if (javaTool == "")
                javaTool = @"C:\Program Files\Java\jdk1.7.0_09\bin\java.exe";
            String rdfXmlFilesPath = filesPath + @"RDFXML\";
            String toolsPath = filesPath + @"Tools\";
            String javaApp = toolsPath + "javaApp.jar";
            String domainsFinderApp = toolsPath + "DomainsFinder.jar";
            String domainsFinderFilesPath = toolsPath + @"Domains Finder Files";
            Boolean ExDomainsMemoryModeEnabled = true;
            int triplesExtractionMethod = radioGroup1.SelectedIndex + 4;
            bool withCoreference = checkEdit1.Checked;
            bool withLemmatization = checkEdit2.Checked;
            memoEdit1.Text = "";

            String output = null;
            int[] selectedRows = gridView1.GetSelectedRows();
            //for (int i = 0; i < gridView1.DataRowCount; i++)
            for (int i = 0; i < selectedRows.Length; i++)
            {
                //row = gridView1.GetDataRow(i+1);//
                if ((((Boolean)gridView1.GetRowCellValue(selectedRows[i], "ToConvert")) == true))
                {
                    if (((String)gridView1.GetRowCellValue(selectedRows[i], "SubtitleLanguage")).ToLower() == processedLang) // only process english subtiltes which sa not been processed previoulsy 
                    {
                        String srtFile = srtFilesPath + ((int)gridView1.GetRowCellValue(selectedRows[i], "subtitleID")) + ".srt";
                        String rdfXmlFile = rdfXmlFilesPath + ((int)gridView1.GetRowCellValue(selectedRows[i], "subtitleID")) + ".rdf";

                        memoEdit1.Text += "processing: " + srtFile + "\r\n";

                        output = GeneralHelper.executeCommand(javaTool, " -jar " + javaApp + " " + triplesExtractionMethod + " " + withCoreference + " " + withLemmatization + " " + "\"" + srtFile + "\"");

                        List<IRHomework.Triple> triples = new List<IRHomework.Triple>();
                        if (!output.Contains("error n000001")) // if there is no errors in parsing the SRT file
                        {
                            String[] listOfTripleStrings = output.Split('\r');
                            foreach (String tripleString in listOfTripleStrings)
                            {
                                if (tripleString.Length > 1) // to prevent processing empty lines
                                {
                                    if (!tripleString.Contains("error n000002")) // if there is no errors in extracting triples from one scene
                                    {
                                        triples.Add(new IRHomework.Triple(((String)gridView1.GetRowCellValue(selectedRows[i], "MovieName")), tripleString));
                                    }
                                    else
                                    {
                                        memoEdit1.Text += "can not extract triples from one scene.\r\n";
                                    }
                                }
                            }


                         memoEdit1.Text += "finding domains...";
                         DomainFinderHelper.findDomains(triples, javaTool, domainsFinderApp, domainsFinderFilesPath, ExDomainsMemoryModeEnabled);

                         memoEdit1.Text += "counting domains frequency...";
                         foreach (IRHomework.Triple tr in triples)
                         {
                             Domain selectedDomain = db1.Domains.SingleOrDefault(d => d.name == tr.getDomain());
                             Domain domain = null;
                             if (selectedDomain == null)
                             {
                                 domain = new Domain();
                                 domain.name = tr.getDomain();
                                 domain.frequency = 1;
                                 db1.Domains.InsertOnSubmit(domain);
                             }
                             else
                             {
                                 domain = selectedDomain;
                                 domain.frequency++;
                             }
                         }

                         memoEdit1.Text += "annotating...";
                         foreach (IRHomework.Triple tr in triples)
                         {
                             SpotLightHelper.annotateTriple(tr);
                         }

                         memoEdit1.Text += "finding abstract triples...";
                         List<IRHomework.Triple> newTriples = new List<IRHomework.Triple>();
                         foreach (IRHomework.Triple tr in triples)
                         {
                              List<IRHomework.Triple> abstractTriples = DotNetRDFHelper.getAbstractTriples(tr);
                              newTriples.AddRange(abstractTriples);
                         }


                         memoEdit1.Text += "saving...";
                         IGraph iGraph = DotNetRDFHelper.getIGraphFromTriples(newTriples);
                         iGraph.SaveToFile(rdfXmlFile);
                        

                            // flag subtitles as processed to prevent processing them later
                            int subId = ((int)gridView1.GetRowCellValue(selectedRows[i], "subtitleID"));
                            var result = from x in db1.Subtitles
                                         where x.subtitleID == subId
                                         select x;
                            Subtitle sub = result.First();
                            sub.status = 1;
                            db1.SubmitChanges();

                            memoEdit1.Text += "subtitle " + ((int)gridView1.GetRowCellValue(selectedRows[i], "subtitleID")) + " has been converted and saved successfully \r\n";

                            // validation
                            String rdfXmlString = DotNetRDFHelper.getRdfXmlString(rdfXmlFile);
                            String errors = DotNetRDFHelper.validateRdfXmlString(rdfXmlString);
                            if (errors == null)
                            {
                                memoEdit1.Text += "the file has been checked, and it is valid.\r\n";
                            }
                            else
                            {
                                memoEdit1.Text += "the file has been checked, and it contains the flollwing errors.\r\n";
                                memoEdit1.Text += errors;
                            }
                            memoEdit1.Text += "-------------------------------------------------------\r\n";
                            // end
                        }
                        else
                        {
                            memoEdit1.Text += "can not parse the SRT file. \r\n";
                        }

                    }
                }
            }

        }
        //ask for Lemmatiztion after check coRefernce
        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            //if (checkEdit1.Checked == true)
            //{
            //    DialogResult response = MessageBox.Show("With Lemmatization?", "Lemmatization", MessageBoxButtons.YesNo);
            //    if (response == DialogResult.Yes)
            //        withLemmatization = true;
            //}
            //else
            //    withLemmatization = false;
        }  
        //browse srt folder
        private void simpleButton2_Click(object sender, EventArgs e)  
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                srtFilesPath = folderBrowserDialog1.SelectedPath;
        }
        //browse jre folder
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                javaTool = folderBrowserDialog1.SelectedPath;
        }
        //run crawler.exe
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "MovieTrack.exe";
            p.Start();
        }  
        //Call Form2 form to draw graph of the selected RDF
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            int[] rows = gridView2.GetSelectedRows();
            
            if (rows.Length > 1)
                MessageBox.Show("You can not select more than one row! Please Try Again");
            else
            {
               int subID = (int)gridView2.GetRowCellValue(rows[0], "subtitleID");           
               Form2 form = new Form2(filesPath + "RDFXML\\" + subID + ".rdf");
               form.Show();
               
            }

        }  
        //select all rows
        private void simpleButton5_Click(object sender, EventArgs e)   
        {
            //if (!allChecked)
            //{
            //    repositoryItemCheckEdit1.ValueChecked = true;
            //    allChecked = true;
            //}
            //else
            //{
            //    repositoryItemCheckEdit1.ValueChecked = true;
            //    allChecked = false;
            //}
            
        }
        //Call EditRDF form to edit the selected RDF
        private void simpleButton7_Click(object sender, EventArgs e)
        {
            int[] rows = gridView2.GetSelectedRows();

            if (rows.Length > 1)
                MessageBox.Show("You can not select more than one row! Please Try Again");
            else
            {
                int subID = (int)gridView2.GetRowCellValue(rows[0], "subtitleID");
                //IGraph iGraph = DotNetRDFHelper.getIGraphFromRdfXmlFile(filesPath + "RDFXML\\" + subID + ".rdf");
                RibbonForm1 form = new RibbonForm1(filesPath + "RDFXML\\" + subID + ".rdf");
                //((GridView)((DevExpress.XtraGrid.GridControl)form.Controls.Find("gridControl1", true)[0]).MainView).Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { Caption = "Subject", Name = "Subject", FieldName = "Subject", Visible = true });
                //((GridView)((DevExpress.XtraGrid.GridControl)form.Controls.Find("gridControl1", true)[0]).MainView).Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { Caption = "Predicate", Name = "Predicate", FieldName = "Predicate", Visible = true });
                //((GridView)((DevExpress.XtraGrid.GridControl)form.Controls.Find("gridControl1", true)[0]).MainView).Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { Caption = "Object", Name = "Object", FieldName = "Object", Visible = true });
                //((GridView)((DevExpress.XtraGrid.GridControl)form.Controls.Find("gridControl1", true)[0]).MainView).Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { Caption = "Time", Name = "Time", FieldName = "Time", Visible = false });
                //((DevExpress.XtraGrid.GridControl)form.Controls.Find("gridControl1", true)[0]).DataSource = GetData(iGraph);
                //((GridView)((DevExpress.XtraGrid.GridControl)form.Controls.Find("gridControl1", true)[0]).MainView).PopulateColumns();
                 form.Show();
            }
            
        }
        //Fill controls with the selected movie's information
        private void lookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            var row = lookUpEdit1.GetSelectedDataRow();
            int movieID = System.Convert.ToInt32(row.ToString().Split('=')[1].Substring(1,1));
            //Thumbnail
            pictureEdit1.Image = System.Drawing.Image.FromFile(filesPath + "Thumbnails\\" + movieModel.getThumbnailOfMovie(movieID));
            //Year
            labelControl10.Text = movieModel.getReleaseYearOfMovie(movieID);
            //Genres
            labelControl11.Text = "";
            List<String> temp = new List<string>();
            temp = movieModel.getGenresOfMovie(movieID);
            for (int i = 0; i < temp.Count; i++ )
            {
                if (i < temp.Count-1)
                    labelControl11.Text += temp[i] + " , ";
                else
                    labelControl11.Text += temp[i];
            }
            //Actors
            temp = new List<string>();
            temp = temp = movieModel.getActorsNameOfMovie(movieID); 
            labelControl12.Text = "";
            for (int i = 0; i < temp.Count; i++)
            {
                if (i % 3 == 0 && i != 0)
                    labelControl12.Text += "\n";
                if (i < temp.Count - 1)
                    labelControl12.Text += temp[i] + " , ";
                else
                    labelControl12.Text += temp[i];
            }
            //languages
            temp = new List<string>();
            temp = temp = movieModel.getLangsOfMovie(movieID); 
            labelControl13.Text = "";
            for (int i = 0; i < temp.Count; i++)
            {
                if (i < temp.Count - 1)
                    labelControl13.Text += temp[i] + " , ";
                else
                    labelControl13.Text += temp[i];
            }
            //Video
            textEdit1.Text = movieModel.getTrackOfMovie(movieID);
        }
        //save the movie track to database
        private void simpleButton6_Click(object sender, EventArgs e)
        {
            var row = lookUpEdit1.GetSelectedDataRow();
            int movieID = System.Convert.ToInt32(row.ToString().Split('=')[1].Substring(1, 1));
            Movie movie = db1.Movies.First(x => x.movieID == movieID);
            movie.video = textEdit1.Text;
            db1.SubmitChanges();
            textEdit1.Text = "";
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            QueryForm form = new QueryForm();
            form.Show();
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            int[] rows = gridView2.GetSelectedRows();
            if (rows.Length > 1)
                MessageBox.Show("You can not select more than one row! Please Try Again");
            else
            {
                int subID = (int)gridView2.GetRowCellValue(rows[0], "subtitleID");
                DomainsChart form = new DomainsChart(filesPath + "RDFXML\\" + subID + ".rdf");
                form.Show();
            }
        } 
      
    }
}

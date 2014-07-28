using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using IRHomework;
using VDS.RDF;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace VideoTrack
{
    public partial class RibbonForm1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        string selectedCellCol;
        int selectedCellRow;
        String fileName;
        List<IRHomework.Triple> triples = new List<IRHomework.Triple>();
        VDS.RDF.IGraph iGraph;
        bool showDummy = true;
        VideoTrackDataContext db = new VideoTrackDataContext();

        public RibbonForm1(String rdffile)
        {
            InitializeComponent();
            this.fileName = rdffile;
            this.iGraph = DotNetRDFHelper.getIGraphFromRdfXmlFile(fileName);
            triples = DotNetRDFHelper.getTriplesFromIGraph(iGraph);
            gridControl1.DataSource = GetData(triples, false);

            foreach (DbPediaType type in db.DbPediaTypes)
            {
                comboBoxEdit2.Properties.Items.Add(type.name.ToString().Split('/')[4]);
            }
            //try
            //{
            //    foreach (var r in DotNetRDFHelper.getTypesFromDBPedia())
            //    {
            //        comboBoxEdit2.Properties.Items.Add(r.Value("type").ToString().Split('/')[4]);
            //    }
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("can not connect to internet");
            //}
         
        }
        //create the grid columns
        public DataTable createColumns()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Subject", typeof(string)));
            table.Columns.Add(new DataColumn("Predicate", typeof(string)));
            table.Columns.Add(new DataColumn("Object", typeof(string)));
            table.Columns.Add(new DataColumn("Time", typeof(string)));
            table.Columns.Add(new DataColumn("Domain", typeof(string)));
            return table;
        }
        //fill the grid 
        public DataTable GetData(List<IRHomework.Triple> triples, bool onlyDummy)
        {
            DataTable table = createColumns();
            foreach (IRHomework.Triple tr in triples)
            {
                if (!onlyDummy)
                    table.Rows.Add(new object[] { tr.getSubject(), tr.getPredicate(), tr.getObject(), tr.getSceneTime(), tr.getDomain() });
                else
                {
                    if (tr.getSubject().Contains("http://dummy.org"))
                        table.Rows.Add(new object[] { tr.getSubject(), tr.getPredicate(), tr.getObject(), tr.getSceneTime(), tr.getDomain() });
                }
            }
            return table;
        }
        //validate the selected cell
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            if (selectedCellCol.Equals("Subject"))
            {
                string sub = comboBoxEdit2.Text;
                if ((!Uri.IsWellFormedUriString(sub, UriKind.Absolute) && comboBoxEdit2.SelectedIndex == -1) || (comboBoxEdit2.Text == ""))
                {
                    MessageBox.Show("The subject you entered is Invalid!");
                }
                else
                {
                    if (!Uri.IsWellFormedUriString(sub, UriKind.Absolute))
                    {
                        sub = "http://dbpedia.org/resource/" + comboBoxEdit2.SelectedText;
                    }
                    IRHomework.Triple triple = new IRHomework.Triple(sub, "", "", "");
                    if (triple.subjectIsValid() && !DotNetRDFHelper.IfUrlNotExist(sub))
                    {
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "Subject", triple.getSubject());
                    }
                    else
                    {
                        MessageBox.Show("The URL you entered Not Found!");
                    }
                }
            }
            else if (selectedCellCol.Equals("Predicate"))
            {
                string pre = comboBoxEdit2.Text;
                if ((!Uri.IsWellFormedUriString(pre, UriKind.Absolute) && comboBoxEdit2.SelectedIndex == -1) || (comboBoxEdit2.Text == ""))
                {
                    MessageBox.Show("The predicate you entered is Invalid!");
                }
                else
                {
                    if ((!Uri.IsWellFormedUriString(pre, UriKind.Absolute)))
                        pre = "http://dbpedia.org/resource/" + comboBoxEdit2.SelectedText;

                    IRHomework.Triple triple = new IRHomework.Triple("", pre, "","");
                    if (triple.predicateIsValid() && !DotNetRDFHelper.IfUrlNotExist(pre))
                    {
                        gridView1.SetRowCellValue(selectedCellRow, selectedCellCol, pre);
                    }
                    else
                    {
                        MessageBox.Show("The URL you entered Not Found!");
                    }
                }
            }
            else
            {
                string obj = comboBoxEdit2.Text;
                if (!Uri.IsWellFormedUriString(obj, UriKind.Absolute))
                {
                    if (obj.Equals(""))
                        MessageBox.Show("The object you entered is Invalid!");
                    else
                    {
                        IRHomework.Triple triple = new IRHomework.Triple("", "", obj,"");
                        if (triple.objectIsValid())
                        {
                            gridView1.SetRowCellValue(selectedCellRow, selectedCellCol, obj);
                        }
                        else
                        {
                            MessageBox.Show("The object you entered is Invalid!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("We support only literal object!");
                }
            }
        }
        //get the selected cell
        private void gridView1_Click(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            Point clickPoint = view.GridControl.PointToClient(Control.MousePosition);
            var hitInfo = gridView1.CalcHitInfo(clickPoint);
            if (hitInfo.InRowCell)
            {
                int rowHandle = hitInfo.RowHandle;
                GridColumn column = hitInfo.Column;
                selectedCellCol = column.FieldName;
                selectedCellRow = rowHandle;
                string cellValue = (String)gridView1.GetRowCellValue(rowHandle, column);
                labelControl4.Text = cellValue;
            }
        }
        //validate all triples and save back to RDF file
        private void simpleButton2_Click(object sender, EventArgs e)  
        {
            VDS.RDF.IGraph iGraph = new VDS.RDF.Graph();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                IUriNode su = iGraph.CreateUriNode(new Uri((String)gridView1.GetRowCellValue(i, "Subject")));
                IUriNode pr = iGraph.CreateUriNode(new Uri((String)gridView1.GetRowCellValue(i, "Predicate")));
                ILiteralNode ob = iGraph.CreateLiteralNode((String)gridView1.GetRowCellValue(i, "Object") + "@" + (String)gridView1.GetRowCellValue(i, "Time") + "#" + (String)gridView1.GetRowCellValue(i, "Domain"));
                iGraph.Assert(new VDS.RDF.Triple(su, pr, ob));
            }
            iGraph.SaveToFile(fileName);
            String rdfXmlString = DotNetRDFHelper.getRdfXmlString(fileName);
            String errors = DotNetRDFHelper.validateRdfXmlString(rdfXmlString);
            if (errors != null)
            {
                MessageBox.Show("the file has been checked, and it contains the flollwing errors.\r\n" + errors);
            }
        }
        //show only Dummy
        private void simpleButton1_Click(object sender, EventArgs e)   
        {
            if (showDummy)
            {
                gridControl1.DataSource = GetData(triples, true);
                simpleButton1.Text = "Show All Triples";
                showDummy = false;
            }
            else
            {
                gridControl1.DataSource = GetData(triples, false);
                simpleButton1.Text = "Show only Dummy Triples";
                showDummy = true;
            }
        }

    }
}
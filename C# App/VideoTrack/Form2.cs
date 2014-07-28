using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Glee.Drawing;
using IRHomework;
using VDS.RDF;


namespace VideoTrack
{
    public partial class Form2 : Form
    {
        string name = "";
        VideoTrackDataContext db = new VideoTrackDataContext();
        Microsoft.Glee.Drawing.Graph graph = new Microsoft.Glee.Drawing.Graph("label");
        double zoom;
        bool isNode;
        string source;
        string target;
        int change;
        VDS.RDF.IGraph iGraph;
        String fileName;
        List<IRHomework.Triple> triples = new List<IRHomework.Triple>();

        public Form2(String rdffile)
        {
            InitializeComponent();
            this.fileName = rdffile;
            this.iGraph = DotNetRDFHelper.getIGraphFromRdfXmlFile(fileName);
            triples = DotNetRDFHelper.getTriplesFromIGraph(iGraph);
            DotNetRDFHelper.drawIGraph(this.iGraph, gViewer);

            foreach (DbPediaType type in db.DbPediaTypes)
            {
                comboBoxEdit1.Properties.Items.Add(type.name.ToString().Split('/')[4]);
            }
        }
        //get the selected node or edge
        private void gViewer_MouseClick(object sender, MouseEventArgs e)
        {
            graph = gViewer.Graph; 
            if (gViewer.SelectedObject != null)
            {
                if (gViewer.SelectedObject is Node)
                {
                    (gViewer.SelectedObject as Node).Attr = ((Node)gViewer.SelectedObject).Attr as NodeAttr;
                    name = ((Node)gViewer.SelectedObject).Attr.Label;
                    labelControl3.Text = name;
                    isNode = true;
                    

                } 
                else if (gViewer.SelectedObject is Edge)
                {
                    (gViewer.SelectedObject as Edge).Attr = ((Edge)gViewer.SelectedObject).Attr as EdgeAttr;
                    name = ((Edge)gViewer.SelectedObject).Attr.Label;
                    source = ((Edge)gViewer.SelectedObject).Source;
                    target = ((Edge)gViewer.SelectedObject).Target;
                    labelControl3.Text = name;
                    isNode = false;
                    
                }
            }
        }
        //validate all and save back to RDF
        private void simpleButton1_Click(object sender, EventArgs e)      
        {
            iGraph = DotNetRDFHelper.getIGraphFromTriples(triples);         
            iGraph.SaveToFile(fileName);
            String rdfXmlString = DotNetRDFHelper.getRdfXmlString(fileName);
            String errors = DotNetRDFHelper.validateRdfXmlString(rdfXmlString);
            if (errors != null)
            {
                MessageBox.Show("the file has been checked, and it contains the following errors.\r\n" + errors);
            }
            else
            {
                this.iGraph = DotNetRDFHelper.getIGraphFromRdfXmlFile(fileName);
                triples = DotNetRDFHelper.getTriplesFromIGraph(iGraph);
                DotNetRDFHelper.drawIGraph(this.iGraph, gViewer);
            }
        }
        //check current node or edge Validation & apply changes
        private void simpleButton2_Click(object sender, EventArgs e)      
        {
            bool render = true;
            String currentURI = "";
            if (isNode)
            {
                Boolean isUri = Uri.IsWellFormedUriString(name, UriKind.Absolute);
                if (isUri)    //node is subject
                {
                    string sub = comboBoxEdit1.Text;
                    if ((!Uri.IsWellFormedUriString(sub, UriKind.Absolute) && comboBoxEdit1.SelectedIndex == -1) || (comboBoxEdit1.Text == ""))
                    {
                        render = false;
                        MessageBox.Show("The subject you entered is Invalid!");
                    }
                    else
                    {
                       isUri = Uri.IsWellFormedUriString(sub, UriKind.Absolute);
                       if (!isUri)
                           sub = "http://dbpedia.org/resource/" + comboBoxEdit1.SelectedText;
                       
                        IRHomework.Triple triple = new IRHomework.Triple(sub, "", "", "");
                        if (triple.subjectIsValid() && !DotNetRDFHelper.IfUrlNotExist(sub))
                        {
                            graph.FindNode(name).NodeAttribute.Label = sub;
                            var matches = triples.FindAll(x => x.getSubject() == name);
                            foreach (IRHomework.Triple tr in matches)
                            {
                                tr.setSubject(sub);
                            }
                            zoom = gViewer.ZoomF;
                            HScrollBar h = ((HScrollBar)gViewer.Controls[3]);
                            change = h.Value / h.SmallChange;
                        }
                        else
                        {
                            render = false;
                            MessageBox.Show("The URL you entered Not Found!");
                        }
                   }
                   currentURI = sub;
                   
                }
                else     //node is object
                {
                    string obj = comboBoxEdit1.Text;
                    if (!Uri.IsWellFormedUriString(obj, UriKind.Absolute))
                    {
                        if (obj.Equals(""))
                            MessageBox.Show("The object you entered is Invalid!");
                        else
                        {
                            IRHomework.Triple triple = new IRHomework.Triple("", "", obj,"");
                            if (triple.objectIsValid())
                            {
                                graph.FindNode(name).NodeAttribute.Label = obj;
                                var matches = triples.FindAll(x => x.getObject().Split('@')[0] == name);
                                foreach (IRHomework.Triple tr in matches)
                                {
                                    tr.setObject(obj + "@" + tr.getObject().Split('@')[1].Split('#')[0] + "#" + tr.getObject().Split('#')[1]);
                                }
                                zoom = gViewer.ZoomF;
                                HScrollBar h = ((HScrollBar)gViewer.Controls[3]);
                                change = h.Value / h.SmallChange;
                            }
                            else
                            {
                                render = false;
                                MessageBox.Show("The object you entered is Invalid!");
                            }
                        }
                    }
                    else
                    {
                        render = false;
                        MessageBox.Show("We support only literal object!");
                    }
                    currentURI = obj;
                }

            }
            else
            {
                string pre = comboBoxEdit1.Text;
                if ((!Uri.IsWellFormedUriString(pre, UriKind.Absolute) && comboBoxEdit1.SelectedIndex == -1) || (comboBoxEdit1.Text == ""))
                {
                    render = false;
                    MessageBox.Show("The predicate you entered is Invalid!");
                }
                else
                {
                    if ((!Uri.IsWellFormedUriString(pre, UriKind.Absolute)))
                        pre = "http://dbpedia.org/resource/" + comboBoxEdit1.SelectedText;

                    IRHomework.Triple triple = new IRHomework.Triple("", pre, "","");
                    if (triple.predicateIsValid() && !DotNetRDFHelper.IfUrlNotExist(pre))
                    {
                        graph.EdgeById(source + name + target).EdgeAttr.Label = pre;
                        var matches = triples.Find(x => x.getPredicate() == name && x.getSubject() == source);
                        matches.setPredicate(pre);

                        zoom = gViewer.ZoomF;
                        HScrollBar h = ((HScrollBar)gViewer.Controls[3]);
                        change = h.Value / h.SmallChange;
                    }
                    else
                    {
                        render = false;
                        MessageBox.Show("The URL you entered Not Found!");
                    }
                    currentURI = pre;
                }

            }

            //re render graph
            if (render)
            {
                gViewer.Graph = graph;
                gViewer.ZoomF = zoom;
                HScrollBar h = ((HScrollBar)gViewer.Controls[3]);
                h.Value = change* h.SmallChange;
                comboBoxEdit1.Text = "";
                labelControl3.Text = currentURI;
            }
        }

    }
}

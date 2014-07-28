using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using IRHomework;
using DevExpress.XtraCharts;

namespace VideoTrack
{
    public partial class DomainsChart : DevExpress.XtraEditors.XtraForm
    {
        public DomainsChart(string rdfPath)
        {
            InitializeComponent();
            List<String> domains = getDomainsFromRdfXmlFile(rdfPath);
            Series series1 = new Series("Pie Series 1", ViewType.Pie3D);
            chartControl1.Series.Add(series1);
            drawDomainsChart(domains, series1);
            series1.PointOptions.ValueNumericOptions.Format = NumericFormat.Percent;
            series1.PointOptions.ValueNumericOptions.Precision = 0;
            series1.PointOptions.PointView = PointView.ArgumentAndValues;
            //((Pie3DSeriesView)series1.View).ExplodedPoints.Add(series1.Points[0]);
            ((Pie3DSeriesView)series1.View).ExplodedDistancePercentage = 30;
            
        }

        public static List<String> getDomainsFromRdfXmlFile(String fileName)
        {
            List<String> result = new List<String>();
            var iGraph = DotNetRDFHelper.getIGraphFromRdfXmlFile(fileName);
            foreach (var r in DotNetRDFHelper.queryIGraph("", "", "", "", iGraph, ""))
            {
                result.Add(r.getDomain());
            }
            return result;
        }

        public void drawDomainsChart(List<String> domains, Series series)
        {
            
            List<string> domainsNames = new List<string>();
            int[] domainsFrequency = new int[domains.Count];
            foreach (var d in domains)
            {
                int i = domainsNames.IndexOf(d);
                if (i == -1)
                {
                    domainsNames.Add(d);
                    domainsFrequency[domainsNames.Count - 1]++;
                }
                else
                {
                    domainsFrequency[i]++;
                }
            }
            chartControl1.Series[0].Points.Clear();
            for (int i = 0; i < domainsNames.Count; i++)
            {
                String name = domainsNames[i];
                int frequency = domainsFrequency[i];
                if (frequency > 10)
                {
                    DevExpress.XtraCharts.SeriesPoint dp = new DevExpress.XtraCharts.SeriesPoint(name, new object[] { ((object)(frequency)) }, i);
                    series.Points.Add(dp);
                }
            }
            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (chartControl1.Legend.Visible)
            {
                chartControl1.Legend.Visible = false;
                simpleButton1.Text = "Show Legend";
            }
            else
            {
                chartControl1.Legend.Visible = true;
                simpleButton1.Text = "Hide Legend";
            }
        }
    }
}
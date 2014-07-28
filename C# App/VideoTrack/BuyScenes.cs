using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Net.Mail;
using System.Net.Mime;
using System.Linq;

namespace VideoTrack
{
    public partial class BuyScenes : DevExpress.XtraEditors.XtraForm
    {
        

        public BuyScenes( System.Windows.Forms.ListView.SelectedListViewItemCollection items)
        {
            InitializeComponent();
            int i = 0;
            listView1.FullRowSelect = true;
            listView1.TileSize = new System.Drawing.Size(400, 150);
            ImageList imageListLarge = new ImageList();
            imageListLarge.ImageSize = new System.Drawing.Size(82, 100);
            foreach (ListViewItem item in items)
            {
                ListViewItem item1 = new ListViewItem(item.Text);
                item1.SubItems.Add(item.SubItems[1].Text);
                
                var res = from x in db.Movies
                          where x.name == item.Text
                          select x.thumbnail;
                imageListLarge.Images.Add(Bitmap.FromFile(filesPath + "Thumbnails\\" + res.First()));
                listView1.LargeImageList = imageListLarge;
                item1.ImageIndex = i;
                listView1.Items.Add(item1);
                i++;
            }
        }

        VideoTrackDataContext db = new VideoTrackDataContext();
        string filesPath = @"..\..\..\Files\";
        MailMessage mail = new MailMessage();
        ListViewItem dragItem = null;
        private ListViewItem newItem = null;
        List<String> emailInfo = new List<string>();

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            SmtpClient SmtpServer = new SmtpClient();
            SmtpServer.Credentials = new System.Net.NetworkCredential("videotrack.anaj@gmail.com", "asdf7ghlk");
            SmtpServer.Port = 587;
            SmtpServer.Host = "smtp.gmail.com";
            SmtpServer.EnableSsl = true;

            mail = new MailMessage();
            String[] addr = TextBox1.Text.Split(',');
            try
            {
                mail.From = new MailAddress("videotrack.anaj@gmail.com", "Video Track Company", System.Text.Encoding.UTF8);
                Byte i;
                for (i = 0; i < addr.Length; i++)
                    mail.To.Add(addr[i]);
                mail.Subject = "Scenes";
                foreach (string s in emailInfo)
                {
                    mail.Body += "Movie Title: " + s.Split('#')[0] + "\n" + "From " + s.Split('#')[1].Split('-')[0] + ", To " + s.Split('#')[1].Split('>')[1] + "\n\n";
                }
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                mail.ReplyTo = new MailAddress(TextBox1.Text);
                SmtpServer.Send(mail);
                MessageBox.Show("Your message is sent");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SetDefaultCursor()
        {
            Cursor = Cursors.Default;
        }

        private void SetDefaultLabel()
        {
            label1.ImageIndex = 0;
            SetDefaultCursor();
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks >= 2) return;
            Cursor = new Cursor(@"..\..\..\Icons\delete.ico");
            newItem = listView1.GetItemAt(e.X, e.Y);
        }

        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (newItem == null || e.Button != MouseButtons.Left) return;

            dragItem = new ListViewItem();
            emailInfo.Add(newItem.Text + "#" + newItem.SubItems[1].Text);
            
            listView1.Items.Remove(newItem);
            dragItem.Text = newItem.Text;
            listView1.DoDragDrop(dragItem, DragDropEffects.Copy);
        }

        private void listView1_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
        }

        private void label3_DragDrop(object sender, DragEventArgs e)
        {
            listView1.Items.Remove(dragItem);
            dragItem.Text = newItem.Text;
        }

        private void label3_DragEnter(object sender, DragEventArgs e)
        {
            label3.ImageIndex = 1;
            e.Effect = DragDropEffects.Copy;
            Cursor = new Cursor(@"..\..\..\Icons\copy.ico");
        }

        private void label3_DragLeave(object sender, EventArgs e)
        {
            SetDefaultLabel();
        }

        private void label3_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            label5.Text = emailInfo.Count.ToString();
            SetDefaultCursor();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net.Mime;

namespace VideoTrack
{
    public partial class SendEmail : Form
    {
        //String path;
        MailMessage mail = new MailMessage();
        public SendEmail()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            SmtpClient SmtpServer = new SmtpClient();
            SmtpServer.Credentials = new System.Net.NetworkCredential("adjemiananna0@gmail.com", "an-aj_90GG");
            SmtpServer.Port = 587;
            SmtpServer.Host = "smtp.gmail.com";
            SmtpServer.EnableSsl = true;
            mail = new MailMessage();
            String[] addr = TextBox1.Text.Split(',');
            try
            {
                mail.From = new MailAddress("adjemiananna0@gmail.com", "Developers", System.Text.Encoding.UTF8);
                Byte i;
                for (i = 0; i < addr.Length; i++)
                    mail.To.Add(addr[i]);
                mail.Subject = TextBox3.Text;
                mail.Body = TextBox4.Text;
                
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
             
    }
}
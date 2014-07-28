namespace VideoTrack
{
    partial class TagsCloud
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cloudControl = new VideoTrack.CloudControl();
            this.SuspendLayout();
            // 
            // cloudControl
            // 
            this.cloudControl.AutoSize = true;
            this.cloudControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.cloudControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cloudControl.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cloudControl.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cloudControl.LayoutType = VideoTrack.LayoutType.Spiral;
            this.cloudControl.Location = new System.Drawing.Point(0, 0);
            this.cloudControl.MaxFontSize = 50;
            this.cloudControl.MinFontSize = 10;
            this.cloudControl.Name = "cloudControl";
            this.cloudControl.Palette = new System.Drawing.Color[] {
        System.Drawing.Color.DarkRed,
        System.Drawing.Color.DarkBlue,
        System.Drawing.Color.DarkGreen,
        System.Drawing.Color.Navy,
        System.Drawing.Color.DarkCyan,
        System.Drawing.Color.DarkOrange,
        System.Drawing.Color.DarkGoldenrod,
        System.Drawing.Color.DarkKhaki,
        System.Drawing.Color.Blue,
        System.Drawing.Color.Red,
        System.Drawing.Color.Green};
            this.cloudControl.Size = new System.Drawing.Size(1233, 635);
            this.cloudControl.TabIndex = 6;
            this.cloudControl.WeightedWords = null;
            this.cloudControl.Click += new System.EventHandler(this.cloudControl_Click);
            // 
            // TagsCloud
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 635);
            this.Controls.Add(this.cloudControl);
            this.Name = "TagsCloud";
            this.Text = "TagsCloud";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CloudControl cloudControl;
    }
}
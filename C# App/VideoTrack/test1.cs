using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace VideoTrack
{
    public partial class test1 : DevExpress.XtraEditors.XtraForm
    {
        public test1()
        {
            InitializeComponent();
        }
        ListViewItem dragItem = null;

        private void test1_Load(object sender, EventArgs e)
        {

        }

        private void SetDefaultCursor()
        {
            Cursor = Cursors.Default;
        }

        private void SetDragCursor(DragDropEffects e)
        {
            if (e == DragDropEffects.Move)
                Cursor = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraLayout.Demos.Images.move.ico"));
            if (e == DragDropEffects.Copy)
                Cursor = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraLayout.Demos.Images.copy.ico"));
            if (e == DragDropEffects.None)
                Cursor = Cursors.No;
        }

        private ListViewItem newItem = null;

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor = new Cursor(@"D:\Anna\Anna\College\Fifth Year\5th year\2nd Semester\IR\Practical\Homework\VideoTrack\delete.ico");
            newItem = listView1.GetItemAt(e.X, e.Y);
            //DragDropEffects dde1 = DoDragDrop(newItem, DragDropEffects.Copy);
        }

        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (newItem == null || e.Button != MouseButtons.Left) return;
            
             dragItem = new ListViewItem();
            //dragItem.Name = Guid.NewGuid().ToString();
            //dragItem.Control = new TextEdit();
            //dragItem.Control.Name = Guid.NewGuid().ToString();
            listView1.Items.Remove(newItem);
            dragItem.Text = newItem.Text;
            listView1.DoDragDrop(dragItem, DragDropEffects.Copy);
        }

        private void listView1_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
        }

        private void label1_DragDrop(object sender, DragEventArgs e)
        {
            //if (CanRecycleDragItem())
            //{
            //    Control control = dragItem.Control;
            //    dragItem.Parent.Remove(dragItem);
            //    if (control != null)
            //    {
            //        control.Parent = null;
            //        control.Dispose();
            //    }
            //    dragItem = null;
            //}
            
            listView1.Items.Remove(dragItem);
            dragItem.Text = newItem.Text;
            //SetDefaultLabel();
        }

        private void label1_DragEnter(object sender, DragEventArgs e)
        {
            //if (CanRecycleDragItem())
           // {
                label1.ImageIndex = 1;
                e.Effect = DragDropEffects.Copy;
                Cursor = new Cursor(@"D:\Anna\Anna\College\Fifth Year\5th year\2nd Semester\IR\Practical\Homework\VideoTrack\copy.ico");//(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("VideoTrack.Resources.open-32x32.en.png"));
            //}
        }

        private void label1_DragLeave(object sender, EventArgs e)
        {
            SetDefaultLabel();
        }

        private void SetDefaultLabel()
        {
            label1.ImageIndex = 0;
            SetDefaultCursor();
        }

        private void listView1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void label1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            SetDefaultCursor();
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            SetDefaultCursor();
        }

    }
}
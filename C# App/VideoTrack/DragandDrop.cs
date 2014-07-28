using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DragDropLayoutControl;
using DevExpress.XtraLayout.Dragging;
using DevExpress.XtraLayout.Customization;

namespace VideoTrack
{
    public partial class DragandDrop : DevExpress.XtraEditors.XtraForm, IDragManager
    {
        public DragandDrop()
        {
            InitializeComponent();
        }

        LayoutControlItem dragItem = null;
        LayoutControlItem IDragManager.DragItem 
        {
            get { return dragItem; } set { dragItem = value; } 
        }
        void IDragManager.SetDragCursor(DragDropEffects e) 
        { 
            SetDragCursor(e); 
        }
        private void SetDefaultCursor()
        {
            Cursor = Cursors.Default;
        }
        private void SetDragCursor(DragDropEffects e)
        {
            if (e == DragDropEffects.Move)
                Cursor = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(@"D:\Anna\Anna\College\Fifth Year\5th year\2nd Semester\IR\Practical\Homework\VideoTrack\Icons\move.ico"));
            if (e == DragDropEffects.Copy)
                Cursor = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(@"D:\Anna\Anna\College\Fifth Year\5th year\2nd Semester\IR\Practical\Homework\VideoTrack\Icons\copy.ico"));
            if (e == DragDropEffects.None)
                Cursor = Cursors.No;
        }
        private LayoutControlItem GetDragNode(IDataObject data)
        {
            return data.GetData(typeof(LayoutControlItem)) as LayoutControlItem;
        }
        //ListView1
        private ListViewItem newItem = null;
        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            newItem = listView1.GetItemAt(e.X, e.Y);
        }
        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (newItem == null || e.Button != MouseButtons.Left) return;
            dragItem = new LayoutControlItem();
            dragItem.Name = Guid.NewGuid().ToString();
            dragItem.Control = new TextEdit();
            dragItem.Control.Name = Guid.NewGuid().ToString();
            dragItem.Text = newItem.Text;
            listView1.DoDragDrop(dragItem, DragDropEffects.Copy);
        }
        private void listView1_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
        }
        //Label1
        private void label1_DragDrop(object sender, DragEventArgs e)
        {
            if (CanRecycleDragItem())
            {
                Control control = dragItem.Control;
                dragItem.Parent.Remove(dragItem);
                if (control != null)
                {
                    control.Parent = null;
                    control.Dispose();
                }
                dragItem = null;
            }
            SetDefaultLabel();
        }
        private void label1_DragEnter(object sender, DragEventArgs e)
        {
            if (CanRecycleDragItem())
            {
                label1.ImageIndex = 1;
                e.Effect = DragDropEffects.Copy;
                Cursor = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(@"D:\Anna\Anna\College\Fifth Year\5th year\2nd Semester\IR\Practical\Homework\VideoTrack\Icons\delete.ico"));
            }
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
        protected bool CanRecycleDragItem()
        {
            if (dragItem == null) return false;
            if (dragItem.Owner == null) return false;
            return true;
        }

        private void dragDropLayoutControl3_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void DragandDrop_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
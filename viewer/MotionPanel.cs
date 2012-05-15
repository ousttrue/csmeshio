using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;


namespace SlimDXViewer
{
    public class MotionPanel : Panel
    {
        meshio.Vmd.Model _vmd;
        ListView _lv = new ListView();

        public MotionPanel()
        {
            SuspendLayout();

            ColumnHeader name = new ColumnHeader();
            name.Text = "bone";
            name.Width = -1;
            ColumnHeader frame = new ColumnHeader();
            frame.Text = "frame";

            _lv.Parent = this;
            _lv.FullRowSelect = true;
            _lv.GridLines = true;
            _lv.AllowColumnReorder = true;
            _lv.Sorting = SortOrder.Ascending;
            _lv.Columns.AddRange(new ColumnHeader[] {name, frame});
                
            _lv.VirtualMode=true;
            _lv.VirtualListSize=0;
            _lv.RetrieveVirtualItem += 
                new RetrieveVirtualItemEventHandler(OnRetrieveVirtualItem);

            _lv.Dock = DockStyle.Fill;
            _lv.View = System.Windows.Forms.View.Details;

            ResumeLayout();
        }

        void OnRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
            var item=_vmd.BoneMotions[e.ItemIndex];
            e.Item = new ListViewItem(new string[]{
                    item.BoneName, 
                    item.FrameNum.ToString()
                    });
        }

        public void Load(meshio.Vmd.Model vmd)
        {
            _vmd=vmd;
            _lv.VirtualListSize=vmd.BoneMotions.Length;
        }
    }

}


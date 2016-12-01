using System;
using System.Drawing;
using System.Windows.Forms;

namespace MdiTabStrip
{
    internal class MdiMenuItem : ToolStripMenuItem
    {
        private bool m_isMouseOver;

        private MdiTab m_ownerTab;
        public MdiMenuItem(MdiTab tab, EventHandler handler)
        {
            m_ownerTab = tab;
            Click += handler;
        }

        public Form Form
        {
            get { return m_ownerTab.Form; }
        }

        public bool IsMouseOver
        {
            get { return m_isMouseOver; }
        }

        public bool IsTabActive
        {
            get { return m_ownerTab.IsActive; }
        }

        public bool IsTabVisible
        {
            get { return m_ownerTab.Visible; }
        }

        internal Image CheckedImage
        {
            get
            {
                Bitmap bmp = new Bitmap(typeof(MdiTabStrip), "CheckedImage.bmp");
                bmp.MakeTransparent(bmp.GetPixel(1, 1));
                return bmp;
            }
        }
        
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            m_isMouseOver = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            m_isMouseOver = false;
            Invalidate();
        }
    }

}

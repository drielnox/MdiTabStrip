using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MdiTabStrip
{
    [ToolboxItem(false)]
    public partial class MdiTabStripItemBase : Control
    {
        public void InvokeMouseDown(MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        public void InvokeMouseEnter(EventArgs e)
        {
            OnMouseEnter(e);
        }

        public void InvokeMouseHover(EventArgs e)
        {
            OnMouseHover(e);
        }

        public void InvokeMouseLeave(EventArgs e)
        {
            OnMouseLeave(e);
        }

        public void InvokeMouseMove(MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        public void InvokeMouseUp(MouseEventArgs e)
        {
            OnMouseUp(e);
        }
    }
}

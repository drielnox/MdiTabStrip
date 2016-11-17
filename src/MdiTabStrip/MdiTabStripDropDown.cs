using System.Windows.Forms;
using System.ComponentModel;

namespace MdiTabStrip
{
    [ToolboxItem(false)]
    internal class MdiTabStripDropDown : ContextMenuStrip
    {
        internal MdiTabStripDropDown() : base()
        {
            Renderer = new MdiMenuStripRenderer();
        }

        internal void SetItemChecked(MdiMenuItem item)
        {
            foreach (MdiMenuItem mi in Items)
            {
                mi.Checked = false;
            }

            item.Checked = true;
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            base.OnItemAdded(e);

            SetItemChecked((MdiMenuItem)e.Item);
        }
    }
}

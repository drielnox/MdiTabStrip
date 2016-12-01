using System;
using System.Collections.Generic;
using System.Text;

namespace MdiTabStrip
{
    public class MdiTabStripTabClickedEventArgs : EventArgs
    {
        private MdiTab mdiTab;

        public MdiTabStripTabClickedEventArgs(MdiTab mdiTab)
        {
            // TODO: Complete member initialization
            this.mdiTab = mdiTab;
        }
    }
}

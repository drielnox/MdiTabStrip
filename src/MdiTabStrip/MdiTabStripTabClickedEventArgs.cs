using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MdiTabStrip
{
    internal class MdiTabStripTabClickedEventArgs
    {
        private MdiTab mdiTab;

        public MdiTabStripTabClickedEventArgs(MdiTab mdiTab)
        {
            // TODO: Complete member initialization
            this.mdiTab = mdiTab;
        }
    }
}

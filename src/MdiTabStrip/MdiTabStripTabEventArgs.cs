﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MdiTabStrip
{
    public class MdiTabStripTabEventArgs : EventArgs
    {
        private MdiTab tab;

        public MdiTabStripTabEventArgs(MdiTab tab)
        {
            // TODO: Complete member initialization
            this.tab = tab;
        }
    }
}

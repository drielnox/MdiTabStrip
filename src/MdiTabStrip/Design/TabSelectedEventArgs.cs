using System;

namespace MdiTabStrip.Design
{
    internal class TabSelectedEventArgs : EventArgs
    {
        private TabType _tabType;

        public TabType TabType
        {
            get { return _tabType; }
        }

        public TabSelectedEventArgs(TabType tabType)
        {
            _tabType = tabType;
        }
    }
}

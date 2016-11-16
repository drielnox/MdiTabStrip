using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MdiTabStrip
{
    public class MdiTabStrip : ScrollableControl, ISupportInitialize
    {
        public Color ActiveTabBorderColor { get; internal set; }
        public Color ActiveTabColor { get; internal set; }
        public Font ActiveTabFont { get; internal set; }
        public Color ActiveTabForeColor { get; internal set; }
        public bool Animate { get; internal set; }
        public Color CloseButtonBackColor { get; internal set; }
        public Color CloseButtonBorderColor { get; internal set; }
        public Color CloseButtonForeColor { get; internal set; }
        public Color CloseButtonHotForeColor { get; internal set; }
        public bool DisplayFormIcon { get; internal set; }
        public Color InactiveTabBorderColor { get; internal set; }
        public Color InactiveTabColor { get; internal set; }
        public Font InactiveTabFont { get; internal set; }
        public Color InactiveTabForeColor { get; internal set; }
        public int MaxTabWidth { get; internal set; }
        public bool MdiNewTabVisible { get; internal set; }
        public int MinTabWidth { get; internal set; }
        public Color MouseOverTabColor { get; internal set; }
        public Font MouseOverTabFont { get; internal set; }
        public Color MouseOverTabForeColor { get; internal set; }
        internal MdiChildWindowState MdiWindowState { get; set; }
        internal MdiTabPermanence TabPermanence { get; set; }

        public void BeginInit()
        {
            throw new NotImplementedException();
        }

        public void EndInit()
        {
            throw new NotImplementedException();
        }
    }
}

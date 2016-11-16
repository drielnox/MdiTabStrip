using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace MdiTabStrip
{
    internal class TabStripLayoutEngine : LayoutEngine
    {

        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            MdiTabStrip strip = (MdiTabStrip)container;
            int proposedWidth = strip.MaxTabWidth;
            int visibleCount = strip.Tabs.VisibleCount;
            Rectangle stripRectangle = strip.DisplayRectangle;
            int tabAreaWidth = stripRectangle.Width;
            Point nextLocation = stripRectangle.Location;
            int leftOver = 0;
            int visibleIndex = 0;

            //If the MdiTabStrip's DisplayRectangle width is less than 1 or there are no tabs
            //to display then don't try to layout the control.
            if (tabAreaWidth < 1 | visibleCount < 1)
            {
                //If the MdiNewTab is visible then we need to layout it's position.
                LayoutMdiNewTab(strip, nextLocation, stripRectangle.Height + strip.Margin.Bottom);
                return false;
            }

            //For each of the scroll tabs need to determine their location and height (the width
            //is set in the MdiTabStrip constructor and is fixed). The width of the scroll tab
            //also needs to be subtracted from the tabAreaWidth so that the true tab area can be
            //properly calculated.
            if (strip.RightToLeft == RightToLeft.Yes)
            {
                nextLocation.X = stripRectangle.Right;

                if (strip.RightScrollTab.Visible)
                {
                    nextLocation = MirrorScrollTab(strip.RightScrollTab, nextLocation, stripRectangle.Height);
                    tabAreaWidth -= strip.RightScrollTab.Width;
                }

                if (strip.DropDownTab.Visible)
                {
                    nextLocation = MirrorScrollTab(strip.DropDownTab, nextLocation, stripRectangle.Height);
                    tabAreaWidth -= strip.DropDownTab.Width;
                }

                if (strip.LeftScrollTab.Visible)
                {
                    nextLocation = MirrorScrollTab(strip.LeftScrollTab, nextLocation, stripRectangle.Height);
                    tabAreaWidth -= strip.LeftScrollTab.Width;
                }
            }
            else
            {
                if (strip.LeftScrollTab.Visible)
                {
                    nextLocation = SetScrollTab(strip.LeftScrollTab, nextLocation, stripRectangle.Height);
                    tabAreaWidth -= strip.LeftScrollTab.Width;
                }

                if (strip.DropDownTab.Visible)
                {
                    nextLocation = SetScrollTab(strip.DropDownTab, nextLocation, stripRectangle.Height);
                    tabAreaWidth -= strip.DropDownTab.Width;
                }

                if (strip.RightScrollTab.Visible)
                {
                    nextLocation = SetScrollTab(strip.RightScrollTab, nextLocation, stripRectangle.Height);
                    tabAreaWidth -= strip.RightScrollTab.Width;
                }
            }

            if (strip.MdiNewTabVisible)
            {
                tabAreaWidth -= strip.MdiNewTab.Width;
            }

            //If the total width of all visible tabs is greater than the total area available for the
            //tabs then need to set the proposed width of each tab. We also retreive the remainder for use below.
            if (visibleCount * proposedWidth > tabAreaWidth)
            {
                //The \ operator returns an Integer value and disgards the remainder.
                proposedWidth = tabAreaWidth / visibleCount;

                leftOver = tabAreaWidth % visibleCount;
            }

            //Set the tabWidth to the larger of the two variables; proposed width and minimum width.
            proposedWidth = Math.Max(proposedWidth, strip.MinTabWidth);

            //Set each visible tab's width and location and perform layout on each tab.
            foreach (MdiTab tab in strip.Tabs)
            {
                if (tab.Visible)
                {
                    Size tabSize = new Size(proposedWidth, stripRectangle.Height);

                    //Suspend the tab's layout so that we can set it's properties without triggering
                    //extraneous layouts. Once all changes are made then we can PerformLayout.
                    tab.SuspendLayout();

                    //To allow the tabs to completely fill the total available width we adjust the width
                    //of the tabs (starting with the first tab) by one. The number of tabs that need to be
                    //adjusted is determined by the leftOver variable that was calculated above.
                    if (proposedWidth < strip.MaxTabWidth && visibleIndex < (leftOver - 1))
                    {
                        tabSize.Width = proposedWidth + 1;
                    }

                    if (strip.RightToLeft == RightToLeft.Yes)
                    {
                        nextLocation.X -= tabSize.Width;
                        tab.Size = tabSize;
                        tab.Location = nextLocation;
                    }
                    else
                    {
                        tab.Size = tabSize;
                        tab.Location = nextLocation;
                        nextLocation.X += tabSize.Width;
                    }

                    visibleIndex += 1;
                    tab.ResumeLayout();
                    tab.PerformLayout();
                }
            }

            LayoutMdiNewTab(strip, nextLocation, stripRectangle.Height);

            //Return False because we don't want layout to be performed again by the parent of the container
            return false;
        }

        private void LayoutMdiNewTab(MdiTabStrip strip, Point position, int height)
        {
            if (strip.MdiNewTabVisible)
            {
                if (strip.RightToLeft == RightToLeft.Yes)
                {
                    MirrorNewTab(strip.MdiNewTab, position, height);
                }
                else
                {
                    SetNewTab(strip.MdiNewTab, position, height);
                }
            }
        }

        private Point SetScrollTab(MdiScrollTab tab, Point position, int height)
        {
            if (tab.Visible)
            {
                tab.Location = position;
                tab.Height = height;
                tab.PerformLayout();
            }

            return new Point(position.X + tab.Width, position.Y);
        }

        private Point SetNewTab(MdiNewTab tab, Point position, int height)
        {
            tab.Location = position;
            tab.Height = height;
            tab.PerformLayout();

            return new Point(position.X + tab.Width, position.Y);
        }

        private Point MirrorScrollTab(MdiScrollTab tab, Point position, int height)
        {
            if (tab.Visible)
            {
                tab.Location = new Point(position.X - tab.Width, position.Y);
                tab.Height = height;
                tab.PerformLayout();
            }

            return tab.Location;
        }

        private Point MirrorNewTab(MdiNewTab tab, Point position, int height)
        {
            tab.Location = new Point(position.X - tab.Width, position.Y);
            tab.Height = height;
            tab.PerformLayout();

            return tab.Location;
        }

    }
}

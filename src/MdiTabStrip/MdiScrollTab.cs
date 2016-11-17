using System;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MdiTabStrip
{
    [ToolboxItem(false)]
    public class MdiScrollTab : MdiTab
    {
        private bool m_mouseDown = false;
        private ScrollTabType m_scrollTabType = ScrollTabType.ScrollTabLeft;
        private MdiTabStripDropDown withEventsField_m_mdiMenu = new MdiTabStripDropDown();
        internal MdiTabStripDropDown m_mdiMenu
        {
            get { return withEventsField_m_mdiMenu; }
            set
            {
                if (withEventsField_m_mdiMenu != null)
                {
                    withEventsField_m_mdiMenu.Closed -= m_mdiMenu_Closed;
                    withEventsField_m_mdiMenu.Opened -= m_mdiMenu_Opened;
                }
                withEventsField_m_mdiMenu = value;
                if (withEventsField_m_mdiMenu != null)
                {
                    withEventsField_m_mdiMenu.Closed += m_mdiMenu_Closed;
                    withEventsField_m_mdiMenu.Opened += m_mdiMenu_Opened;
                }
            }
        }
        private bool m_isDroppedDown = false;

        private bool m_dropDownByTab = false;


        internal event ScrollTabEventHandler ScrollTab;
        internal delegate void ScrollTabEventHandler(ScrollDirection direction);


        public MdiScrollTab(MdiTabStrip owner, ScrollTabType scrollType) : base(owner)
        {
            m_scrollTabType = scrollType;
            owner.RightToLeftChanged += OnOwnerRightToLeftChanged;
            //We initially hide the scroll tab
            Visible = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_mdiMenu.Dispose();

                if (ParentInternal != null)
                {
                    ParentInternal.RightToLeftChanged -= OnOwnerRightToLeftChanged;
                }
            }

            base.Dispose(disposing);
        }

        internal MdiTabStripDropDown MdiMenu
        {
            get { return m_mdiMenu; }
        }

        internal ScrollTabType ScrollTabType
        {
            get { return m_scrollTabType; }
        }

        private void m_mdiMenu_Closed(object sender, System.Windows.Forms.ToolStripDropDownClosedEventArgs e)
        {
            if (!IsMouseOver)
            {
                m_isDroppedDown = false;
            }
        }

        private void m_mdiMenu_Opened(object sender, System.EventArgs e)
        {
            m_isDroppedDown = true;
        }


        internal override void DrawControl(System.Drawing.Graphics g)
        {
            DrawTab(g);
        }

        private void DrawTab(System.Drawing.Graphics g)
        {
            if (this.m_scrollTabType == ScrollTabType.ScrollTabLeft)
            {
                DrawLeftGlyph(g);
            }
            else if (this.m_scrollTabType == ScrollTabType.ScrollTabRight)
            {
                DrawRightGlyph(g);
            }
            else
            {
                DrawDropDownGlyph(g);
            }
        }

        private void DrawLeftGlyph(Graphics g)
        {
            Rectangle rect = new Rectangle(Left + ((Width / 2) - 6), Top + 13, 11, 5);
            Point[] lines1 =
                {
            new Point(rect.X + 4, rect.Y),
            new Point(rect.X + 5, rect.Y),
            new Point(rect.X + 3, rect.Y + 2),
            new Point(rect.X + 5, rect.Y + 4),
            new Point(rect.X + 4, rect.Y + 4),
            new Point(rect.X + 2, rect.Y + 2),
            new Point(rect.X + 4, rect.Y)
        };
            Point[] lines2 =
                {
            new Point(rect.X + 8, rect.Y),
            new Point(rect.X + 9, rect.Y),
            new Point(rect.X + 7, rect.Y + 2),
            new Point(rect.X + 9, rect.Y + 4),
            new Point(rect.X + 8, rect.Y + 4),
            new Point(rect.X + 6, rect.Y + 2),
            new Point(rect.X + 8, rect.Y)
        };

            DrawChevron(g, lines1, lines2);
        }

        private void DrawRightGlyph(Graphics g)
        {
            Rectangle rect = new Rectangle(Left + ((Width / 2) - 5), Top + 13, 11, 5);
            Point[] lines1 = {
            new Point(rect.X + 1, rect.Y),
            new Point(rect.X + 2, rect.Y),
            new Point(rect.X + 4, rect.Y + 2),
            new Point(rect.X + 2, rect.Y + 4),
            new Point(rect.X + 1, rect.Y + 4),
            new Point(rect.X + 3, rect.Y + 2),
            new Point(rect.X + 1, rect.Y)
        };
            Point[] lines2 = {
            new Point(rect.X + 5, rect.Y),
            new Point(rect.X + 6, rect.Y),
            new Point(rect.X + 8, rect.Y + 2),
            new Point(rect.X + 6, rect.Y + 4),
            new Point(rect.X + 5, rect.Y + 4),
            new Point(rect.X + 7, rect.Y + 2),
            new Point(rect.X + 5, rect.Y)
        };

            DrawChevron(g, lines1, lines2);
        }

        private void DrawChevron(Graphics g, Point[] chevron1, Point[] chevron2)
        {
            g.SmoothingMode = SmoothingMode.None;

            using (Pen glyphPen = new Pen(TabForeColor, 1))
            {
                if (!Enabled)
                {
                    Color c = ParentInternal.InactiveTabForeColor;
                    int luminosity = 0;
                    int num1 = c.R;
                    int num2 = c.G;
                    int num3 = c.B;
                    int num4 = Math.Max(Math.Max(num1, num2), num3);
                    int num5 = Math.Min(Math.Min(num1, num2), num3);
                    int num6 = (num4 + num5);

                    luminosity = (((num6 * 240) + 255) / 510);

                    if (luminosity == 0)
                    {
                        glyphPen.Color = ControlPaint.LightLight(c);

                    }
                    else if (luminosity < 120)
                    {
                        glyphPen.Color = ControlPaint.Light(c, 0.5f);
                    }
                    else
                    {
                        glyphPen.Color = ControlPaint.Light(c, 0.5f);
                    }
                }

                glyphPen.StartCap = LineCap.Square;
                glyphPen.EndCap = LineCap.Square;
                g.DrawLines(glyphPen, chevron1);
                g.DrawLines(glyphPen, chevron2);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
        }

        private void DrawDropDownGlyph(Graphics g)
        {
            Rectangle rect = new Rectangle(Left + ((Width / 2) - 3), Top + 12, 4, 6);
            Point[] dropDown = {
            new Point(rect.X, rect.Y + 1),
            new Point(rect.X + 3, rect.Y + 5),
            new Point(rect.X + 6, rect.Y + 1)
        };

            using (SolidBrush glyphBrush = new SolidBrush(TabForeColor))
            {
                g.FillPolygon(glyphBrush, dropDown);
            }
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);

            m_isDroppedDown = false;
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            m_mouseDown = true;
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }

            if (m_mouseDown)
            {
                m_mouseDown = false;

                if (this.m_scrollTabType == ScrollTabType.ScrollTabLeft)
                {
                    ScrollDirection direction = ScrollDirection.Left;

                    if (this.ParentInternal.RightToLeft == RightToLeft.Yes)
                    {
                        direction = ScrollDirection.Right;
                    }

                    if (ScrollTab != null)
                    {
                        ScrollTab(direction);
                    }
                }
                else if (this.m_scrollTabType == ScrollTabType.ScrollTabRight)
                {
                    ScrollDirection direction = ScrollDirection.Right;

                    if (this.ParentInternal.RightToLeft == RightToLeft.Yes)
                    {
                        direction = ScrollDirection.Left;
                    }

                    if (ScrollTab != null)
                    {
                        ScrollTab(direction);
                    }
                }
                else
                {
                    if (m_isDroppedDown)
                    {
                        m_isDroppedDown = false;
                    }
                    else
                    {
                        Point dropPoint = default(Point);

                        if (this.ParentInternal.RightToLeft == RightToLeft.Yes)
                        {
                            dropPoint = ParentInternal.PointToScreen(new Point(Right - m_mdiMenu.Width, ParentInternal.Height - 5));
                        }
                        else
                        {
                            dropPoint = ParentInternal.PointToScreen(new Point(Left, ParentInternal.Height - 5));
                        }

                        m_mdiMenu.Show(dropPoint, ToolStripDropDownDirection.Default);
                    }
                }
            }
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            //Not implemeted, but overridden to bypass inherited functionality
        }

        private void OnOwnerRightToLeftChanged(object sender, EventArgs e)
        {
            //Need to know when the MdiTabStrip RightToLeft property has changed so that the drop down menu's
            //RightToLeft property is set to match it.
            m_mdiMenu.RightToLeft = ParentInternal.RightToLeft;
        }
    }
}

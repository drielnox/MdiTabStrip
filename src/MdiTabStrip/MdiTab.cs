using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace MdiTabStrip
{
    /// <summary>
    /// Represents a selectable tab that corresponds to exactly one open <see cref="Form"/> 
    /// whose <see cref="Form.MdiParent"/> property has been 
    /// set to an instance of another form in an MDI application.
    /// </summary>
    [ToolboxItem(false)]
    public class MdiTab : MdiTabStripItemBase
    {
        private MdiTabStrip m_owner;
        private Form m_form;
        private bool m_isMouseOver = false;
        private bool m_isMouseOverCloseButton = false;
        private bool m_isSwitching = false;
        private Rectangle m_dragBox = Rectangle.Empty;
        private Point[] m_activeBounds;
        private Point[] m_activeInnerBounds;
        private Point[] m_inactiveBounds;
        private Point[] m_inactiveInnerBounds;
        private Point[] m_closeButtonBounds;
        private Point[] m_closeButtonGlyphBounds;
        private Cursor m_dragCursor = null;
        private bool m_isAnimating = false;
        private AnimationType m_animationType;
        private int m_currentFrame = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MdiTab"/> class.
        /// </summary>
        public MdiTab(MdiTabStrip owner)
        {
            ParentInternal = owner;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MdiTab"/> and optionally releases the managed resources. 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_dragCursor != null)
                {
                    m_dragCursor.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets the instance of a <see cref="Form"/> the <see cref="MdiTab"/> represents.
        /// </summary>
        /// <returns>The <see cref="Form"/> object the tab represents.</returns>
        public Form Form
        {
            get { return m_form; }
            set { m_form = value; }
        }

        internal MdiTabStrip ParentInternal
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        internal bool IsMouseOver
        {
            get { return m_isMouseOver; }
            set { m_isMouseOver = value; }
        }

        internal bool IsActive
        {
            get { return object.ReferenceEquals(ParentInternal.ActiveTab, this); }
        }

        private bool IsAnimating
        {
            get { return m_isAnimating; }
            set
            {
                m_isAnimating = value;

                if (value)
                {
                    ParentInternal.AddAnimatingTab(this);
                }
                else
                {
                    ParentInternal.RemoveAnimatingTab(this);
                    m_animationType = AnimationType.None;
                }
            }
        }

        internal int CurrentFrame
        {
            get { return m_currentFrame; }
            set { m_currentFrame = value; }
        }

        internal AnimationType AnimationType
        {
            get { return m_animationType; }
        }

        /// <summary>
        /// Gets the rectangle that represents the display area of the control.
        /// </summary>
        /// <returns>A <see cref="Rectangle"/> that represents the display area of the control.</returns>
        public override System.Drawing.Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rect = base.DisplayRectangle;
                rect.Offset(Location.X, Location.Y);
                return rect;
            }
        }

        internal bool IsMouseOverCloseButton
        {
            get { return m_isMouseOverCloseButton; }
            set
            {
                if (m_isMouseOverCloseButton != value)
                {
                    string txt = Form.Text;

                    m_isMouseOverCloseButton = value;

                    if (value)
                    {
                        txt = "Close Tab";
                    }

                    if (ParentInternal.ShowTabToolTip)
                    {
                        ParentInternal.UpdateToolTip(txt);
                    }

                    ParentInternal.Invalidate();
                }
            }
        }

        internal bool CanDrag
        {
            get
            {
                if (ParentInternal.Tabs.Count == 1)
                {
                    return false;
                }

                return !(ParentInternal.TabPermanence == MdiTabPermanence.First & (ParentInternal.Tabs.IndexOf(this) == 0));
            }
        }

        internal bool CanClose
        {
            get
            {
                if (ParentInternal.TabPermanence == MdiTabPermanence.First && (ParentInternal.Tabs.IndexOf(this) == 0))
                {
                    return false;
                }
                else if (ParentInternal.TabPermanence == MdiTabPermanence.LastOpen && ParentInternal.Tabs.Count == 1)
                {
                    return false;
                }

                return true;
            }
        }

        private bool CanAnimate
        {
            get { return ParentInternal.Animate; }
        }

        private Color TabBackColor
        {
            get
            {
                Color tabcolor = ParentInternal.InactiveTabColor;

                if (IsActive)
                {
                    tabcolor = ParentInternal.ActiveTabColor;
                }
                else if (!Enabled)
                {
                    tabcolor = ParentInternal.InactiveTabColor;
                }
                else if (IsAnimating)
                {
                    tabcolor = ParentInternal.BackColorFadeSteps[m_currentFrame];
                }
                else if (IsMouseOver)
                {
                    tabcolor = ParentInternal.MouseOverTabColor;
                }

                return tabcolor;
            }
        }

        protected Color TabForeColor
        {
            get
            {
                Color foreColor = ParentInternal.InactiveTabForeColor;

                if (IsActive)
                {
                    foreColor = ParentInternal.ActiveTabForeColor;
                }
                else if (IsAnimating)
                {
                    foreColor = ParentInternal.ForeColorFadeSteps[m_currentFrame];
                }
                else if (IsMouseOver)
                {
                    foreColor = ParentInternal.MouseOverTabForeColor;
                }

                return foreColor;
            }
        }

        private Font TabFont
        {
            get
            {
                //We default to the font for the inactive tab because it is more ofter used. If animating we switch
                //to the font for the moused over tab when the current frame is in the latter half of the animation.
                Font font = ParentInternal.InactiveTabFont;

                if (IsActive)
                {
                    font = ParentInternal.ActiveTabFont;
                }
                else if (IsAnimating)
                {
                    if (CurrentFrame > (ParentInternal.Duration / 2))
                    {
                        font = ParentInternal.MouseOverTabFont;
                    }
                }
                else if (IsMouseOver)
                {
                    font = ParentInternal.MouseOverTabFont;
                }

                return font;
            }
        }



        internal bool HitTest(int x, int y)
        {
            bool hit = false;

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.StartFigure();
                gp.AddLines(m_inactiveBounds);
                gp.CloseFigure();

                using (Pen borderpen = new Pen(Color.Black, 1))
                {
                    if (gp.IsOutlineVisible(x, y, borderpen) || gp.IsVisible(x, y))
                    {
                        hit = true;
                    }
                }
            }

            return hit;
        }

        private bool closeButtonHitTest(int x, int y)
        {
            bool hit = false;

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.StartFigure();
                gp.AddLines(m_closeButtonBounds);
                gp.CloseFigure();

                using (Pen borderpen = new Pen(Color.Black, 1))
                {
                    if (gp.IsOutlineVisible(x, y, borderpen) || gp.IsVisible(x, y))
                    {
                        hit = true;
                    }
                }
            }

            return hit;
        }



        internal void DrawControlBackground(Graphics g)
        {
            if (IsActive)
            {
                DrawActiveTabBackground(g);
            }
            else
            {
                DrawInactiveTabBackground(g);
            }
        }

        private void DrawActiveTabBackground(Graphics g)
        {
            //The shadowRectangle fills the divider that is a part of the active tab and spans the width
            //of the parent MdiTabStrip.
            Rectangle shadowRectangle = new Rectangle(ParentInternal.ClientRectangle.X, DisplayRectangle.Bottom, ParentInternal.ClientRectangle.Width, ParentInternal.Padding.Bottom);
            Blend shadowBlend = new Blend();

            g.SmoothingMode = SmoothingMode.None;
            shadowBlend.Factors = new float[] {
            0f,
            0.1f,
            0.3f,
            0.4f
        };
            shadowBlend.Positions = new float[] {
            0f,
            0.5f,
            0.8f,
            1f
        };

            using (GraphicsPath outerPath = new GraphicsPath())
            {
                outerPath.AddLines(m_activeBounds);

                using (LinearGradientBrush gradientBrush = GetGradientBackBrush())
                {
                    g.FillPath(gradientBrush, outerPath);
                }

                using (LinearGradientBrush shadowBrush = new LinearGradientBrush(shadowRectangle, TabBackColor, Color.Black, LinearGradientMode.Vertical))
                {
                    shadowBrush.Blend = shadowBlend;
                    g.FillRectangle(shadowBrush, shadowRectangle);
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(new Pen(ParentInternal.ActiveTabBorderColor), outerPath);
            }

            //Draw the inner border
            using (GraphicsPath innerPath = new GraphicsPath())
            {
                innerPath.AddLines(m_activeInnerBounds);

                Color lineColor = Color.FromArgb(120, 255, 255, 255);
                g.DrawPath(new Pen(lineColor), innerPath);
            }

            if (CanClose)
            {
                DrawCloseButton(g);
            }
        }

        private void DrawInactiveTabBackground(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.None;

            using (GraphicsPath outerPath = new GraphicsPath())
            {
                outerPath.AddLines(m_inactiveBounds);

                using (LinearGradientBrush gradientBrush = GetGradientBackBrush())
                {
                    g.FillPath(gradientBrush, outerPath);
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(new Pen(ParentInternal.InactiveTabBorderColor), outerPath);
            }

            //Draw the inner border
            using (GraphicsPath innerPath = new GraphicsPath())
            {
                innerPath.AddLines(m_inactiveInnerBounds);

                Color lineColor = Color.FromArgb(120, 255, 255, 255);
                g.DrawPath(new Pen(lineColor), innerPath);
            }
        }

        protected LinearGradientBrush GetGradientBackBrush()
        {
            LinearGradientBrush b = new LinearGradientBrush(DisplayRectangle, Color.White, TabBackColor, LinearGradientMode.Vertical);
            Blend bl = new Blend();

            if (IsActive)
            {
                bl.Factors = new float[] {
                0.3f,
                0.4f,
                0.5f,
                1f,
                1f
            };
                bl.Positions = new float[] {
                0f,
                0.2f,
                0.35f,
                0.35f,
                1f
            };
            }
            else
            {
                bl.Factors = new float[] {
                0.3f,
                0.4f,
                0.5f,
                1f,
                0.8f,
                0.7f
            };
                bl.Positions = new float[] {
                0f,
                0.2f,
                0.4f,
                0.4f,
                0.8f,
                1f
            };
            }

            b.Blend = bl;

            return b;
        }



        internal virtual void DrawControl(Graphics g)
        {
            if (IsActive)
            {
                DrawActiveTab(g);
            }
            else
            {
                DrawInactiveTab(g);
            }
        }

        private void DrawActiveTab(Graphics g)
        {
            //The proposedSize variable determines the size available to draw the text of the tab.
            Size proposedSize = new Size(Width - 5, Height);

            if (CanClose)
            {
                //If the tab can close then subtract the button's width
                proposedSize.Width -= 22;
                DrawCloseButton(g);
            }

            if (ParentInternal.DisplayFormIcon)
            {
                //If the tab will display an icon the subtract the icon's width
                proposedSize.Width -= 22;
                DrawFormIcon(g);
            }

            DrawTabText(g, proposedSize);
        }

        private void DrawFormIcon(Graphics g)
        {
            Rectangle iconRectangle = default(Rectangle);

            if (ParentInternal.RightToLeft == RightToLeft.Yes)
            {
                iconRectangle = new Rectangle(Right - 20, Top + 5, 17, 17);
            }
            else
            {
                iconRectangle = new Rectangle(Left + 5, Top + 5, 17, 17);
            }

            if (!IsActive)
            {
                iconRectangle.Offset(0, 2);
            }

            using (Bitmap bmp = new Bitmap(Form.Icon.Width, Form.Icon.Height, PixelFormat.Format32bppArgb))
            {
                using (Graphics bg = Graphics.FromImage(bmp))
                {
                    bg.DrawIcon(Form.Icon, 0, 0);
                }

                g.DrawImage(bmp, iconRectangle);
            }
        }

        private void DrawTabText(Graphics g, Size proposedSize)
        {
            Size s = default(Size);
            Rectangle textRectangle = default(Rectangle);
            TextFormatFlags textFlags = TextFormatFlags.WordEllipsis | TextFormatFlags.EndEllipsis;
            bool isRightToLeft = ParentInternal.RightToLeft == RightToLeft.Yes;

            if (isRightToLeft)
            {
                textFlags = textFlags | TextFormatFlags.Right;
            }

            s = TextRenderer.MeasureText(g, Form.Text, TabFont, proposedSize, textFlags);
            textRectangle = new Rectangle(Left + 5, Top + 8, proposedSize.Width, s.Height);

            if (isRightToLeft)
            {
                if (IsActive && CanClose)
                {
                    textRectangle.Offset(22, 0);
                }
            }
            else
            {
                if (ParentInternal.DisplayFormIcon)
                {
                    textRectangle.Offset(17, 0);
                }
            }

            if (!IsActive)
            {
                textRectangle.Offset(0, 2);
            }

            TextRenderer.DrawText(g, Form.Text, TabFont, textRectangle, TabForeColor, textFlags);
        }

        private void DrawCloseButton(Graphics g)
        {
            if (IsMouseOverCloseButton)
            {
                DrawActiveCloseButton(g);
            }
            else
            {
                DrawInactiveCloseButton(g);
            }
        }

        private void DrawActiveCloseButton(Graphics g)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(m_closeButtonBounds);

                using (SolidBrush backBrush = new SolidBrush(ParentInternal.CloseButtonBackColor))
                {
                    g.FillPath(backBrush, gp);
                }

                using (Pen borderPen = new Pen(ParentInternal.CloseButtonBorderColor))
                {
                    g.DrawPath(borderPen, gp);
                }
            }

            DrawCloseButtonGlyph(g, ParentInternal.CloseButtonHotForeColor);
        }

        private void DrawInactiveCloseButton(Graphics g)
        {
            DrawCloseButtonGlyph(g, ParentInternal.CloseButtonForeColor);
        }

        private void DrawCloseButtonGlyph(Graphics g, Color glyphColor)
        {
            g.SmoothingMode = SmoothingMode.None;

            using (GraphicsPath shadow = new GraphicsPath())
            {
                Matrix translateMatrix = new Matrix();
                Color shadowColor = Color.FromArgb(30, 0, 0, 0);

                shadow.AddLines(m_closeButtonGlyphBounds);
                translateMatrix.Translate(1, 1);
                shadow.Transform(translateMatrix);

                using (SolidBrush shadowBrush = new SolidBrush(shadowColor))
                {
                    g.FillPath(shadowBrush, shadow);
                }
                using (Pen shadowPen = new Pen(shadowColor))
                {
                    g.DrawPath(shadowPen, shadow);
                }
            }

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(m_closeButtonGlyphBounds);

                using (SolidBrush glyphBrush = new SolidBrush(glyphColor))
                {
                    g.FillPath(glyphBrush, gp);
                }
                using (Pen glyphPen = new Pen(glyphColor))
                {
                    g.DrawPath(glyphPen, gp);
                }
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
        }

        private void DrawInactiveTab(Graphics g)
        {
            //The proposedSize variable determines the size available to draw the text of the tab.
            Size proposedSize = new Size(Width - 5, Height);

            if (ParentInternal.DisplayFormIcon)
            {
                //If the tab will display an icon the subtract the icon's width
                proposedSize.Width -= 22;
                DrawFormIcon(g);
            }

            DrawTabText(g, proposedSize);
        }



        internal void StartAnimation(AnimationType animation)
        {
            //When the cursor is moved over the control very quick it causes some odd behavior with the animation
            //These two checks are done to make sure that the tab isn't needlessly added to the animation arraylist.
            if (animation == AnimationType.FadeIn && CurrentFrame == ParentInternal.Duration - 1)
            {
                return;
            }

            if (animation == AnimationType.FadeOut && CurrentFrame == 0)
            {
                return;
            }

            m_animationType = animation;
            if (((ParentInternal != null)))
            {
                IsAnimating = true;
            }
        }

        internal void OnAnimationTick(int newFrame)
        {
            m_currentFrame = newFrame;
            ParentInternal.Invalidate(DisplayRectangle, false);
        }

        internal void StopAnimation()
        {
            IsAnimating = false;
            ParentInternal.Invalidate(DisplayRectangle, false);
        }
        [DllImport("User32.dll", EntryPoint = "LoadCursorFromFileW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]



        private static extern IntPtr LoadCursorFromFile(string filename);

        private Cursor GetCustomCursor(string fileName)
        {
            IntPtr hCursor = default(IntPtr);
            Cursor result = null;

            try
            {
                hCursor = LoadCursorFromFile(fileName);
                if (!IntPtr.Zero.Equals(hCursor))
                {
                    result = new Cursor(hCursor);
                }
            }
            catch (Exception ex)
            {
                //Catch but don't process the exception. If this method returns nothing then
                //the default Windows drag cursor will be used.
                return null;
            }
            return result;
        }





        protected override void OnLayout(LayoutEventArgs levent)
        {
            m_activeBounds = new Point[] {
            new Point(-2, ParentInternal.Bottom),
            new Point(-2, Bottom),
            new Point(Left - 3, Bottom),
            new Point(Left, Bottom - 1),
            new Point(Left, Top + 3),
            new Point(Left + 2, Top),
            new Point(Right - 2, Top),
            new Point(Right, Top + 3),
            new Point(Right, Bottom - 1),
            new Point(Right + 2, Bottom),
            new Point(ParentInternal.Width, Bottom),
            new Point(ParentInternal.Width, ParentInternal.Bottom)
        };
            m_activeInnerBounds = new Point[] {
            new Point(-1, ParentInternal.Bottom),
            new Point(-1, Bottom + 1),
            new Point(Left - 4, Bottom + 1),
            new Point(Left + 1, Bottom),
            new Point(Left + 1, Top + 4),
            new Point(Left + 3, Top + 1),
            new Point(Right - 3, Top + 1),
            new Point(Right - 1, Top + 4),
            new Point(Right - 1, Bottom),
            new Point(Right + 3, Bottom + 1),
            new Point(ParentInternal.Width - 1, Bottom + 1),
            new Point(ParentInternal.Width - 1, ParentInternal.Bottom)
        };
            m_inactiveBounds = new Point[] {
            new Point(Left, Bottom),
            new Point(Left, Top + 5),
            new Point(Left + 2, Top + 2),
            new Point(Right - 2, Top + 2),
            new Point(Right, Top + 5),
            new Point(Right, Bottom)
        };
            m_inactiveInnerBounds = new Point[] {
            new Point(Left + 1, Bottom),
            new Point(Left + 1, Top + 6),
            new Point(Left + 3, Top + 3),
            new Point(Right - 3, Top + 3),
            new Point(Right - 1, Top + 6),
            new Point(Right - 1, Bottom)
        };

            if (ParentInternal.RightToLeft == RightToLeft.Yes)
            {
                m_closeButtonBounds = new Point[] {
                new Point(Left + 18, Top + 7),
                new Point(Left + 6, Top + 7),
                new Point(Left + 4, Top + 9),
                new Point(Left + 4, Top + 20),
                new Point(Left + 6, Top + 22),
                new Point(Left + 18, Top + 22),
                new Point(Left + 20, Top + 20),
                new Point(Left + 20, Top + 9),
                new Point(Left + 18, Top + 7)
            };
                Point startPoint = new Point(Left + 8, Top + 11);
                m_closeButtonGlyphBounds = new Point[] {
                new Point(startPoint.X, startPoint.Y),
                new Point(startPoint.X + 2, startPoint.Y),
                new Point(startPoint.X + 4, startPoint.Y + 2),
                new Point(startPoint.X + 6, startPoint.Y),
                new Point(startPoint.X + 8, startPoint.Y),
                new Point(startPoint.X + 5, startPoint.Y + 3),
                new Point(startPoint.X + 5, startPoint.Y + 4),
                new Point(startPoint.X + 8, startPoint.Y + 7),
                new Point(startPoint.X + 6, startPoint.Y + 7),
                new Point(startPoint.X + 4, startPoint.Y + 5),
                new Point(startPoint.X + 2, startPoint.Y + 7),
                new Point(startPoint.X, startPoint.Y + 7),
                new Point(startPoint.X + 3, startPoint.Y + 4),
                new Point(startPoint.X + 3, startPoint.Y + 3),
                new Point(startPoint.X, startPoint.Y)
            };
            }
            else
            {
                m_closeButtonBounds = new Point[] {
                new Point(Right - 18, Top + 7),
                new Point(Right - 6, Top + 7),
                new Point(Right - 4, Top + 9),
                new Point(Right - 4, Top + 20),
                new Point(Right - 6, Top + 22),
                new Point(Right - 18, Top + 22),
                new Point(Right - 20, Top + 20),
                new Point(Right - 20, Top + 9),
                new Point(Right - 18, Top + 7)
            };
                Point startPoint = new Point(Right - 16, Top + 11);
                m_closeButtonGlyphBounds = new Point[] {
                new Point(startPoint.X, startPoint.Y),
                new Point(startPoint.X + 2, startPoint.Y),
                new Point(startPoint.X + 4, startPoint.Y + 2),
                new Point(startPoint.X + 6, startPoint.Y),
                new Point(startPoint.X + 8, startPoint.Y),
                new Point(startPoint.X + 5, startPoint.Y + 3),
                new Point(startPoint.X + 5, startPoint.Y + 4),
                new Point(startPoint.X + 8, startPoint.Y + 7),
                new Point(startPoint.X + 6, startPoint.Y + 7),
                new Point(startPoint.X + 4, startPoint.Y + 5),
                new Point(startPoint.X + 2, startPoint.Y + 7),
                new Point(startPoint.X, startPoint.Y + 7),
                new Point(startPoint.X + 3, startPoint.Y + 4),
                new Point(startPoint.X + 3, startPoint.Y + 3),
                new Point(startPoint.X, startPoint.Y)
            };
            }
        }



        protected override void OnGiveFeedback(GiveFeedbackEventArgs gfbevent)
        {
            gfbevent.UseDefaultCursors = m_dragCursor == null;

            if ((gfbevent.Effect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                Cursor.Current = m_dragCursor;
            }
            else
            {
                Cursor.Current = Cursors.No;
            }
        }



        protected override void OnMouseEnter(EventArgs e)
        {
            IsMouseOver = true;

            if (CanAnimate && !IsActive)
            {
                StartAnimation(AnimationType.FadeIn);
            }

            if (Form != null)
            {
                ParentInternal.UpdateToolTip(Form.Text);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            //Reset the mouse over fields to False
            IsMouseOver = false;
            IsMouseOverCloseButton = false;

            if (CanAnimate)
            {
                if (!IsActive)
                {
                    //If not the currently active tab
                    if (IsAnimating)
                    {
                        //If the tab is currently animating then change it's animation type to properly fade
                        //back to the inactive color.
                        m_animationType = AnimationType.FadeOut;
                    }
                    else
                    {
                        //The cursor was still over the tab and animation had finished so we need to fade
                        //from the mouseover color to the inactive color
                        StartAnimation(AnimationType.FadeOut);
                    }
                }
                else
                {
                    //If it is the active tab then reset the current frame to 0 because the tab
                    //might have been selected while animation was in process
                    m_currentFrame = 0;
                }
            }
            else
            {
                //If the tab cannot animate then invalidate the tab to repaint with the inactive color
                ParentInternal.Invalidate(DisplayRectangle, false);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Size dragsize = SystemInformation.DragSize;
            m_owner.OnMdiTabClicked(new MdiTabStripTabClickedEventArgs(this));

            if (CanDrag)
            {
                //If the tab can be dragged, which is determined by the TabPermenance property, then set the
                //drag box and load the custom cursor.
                m_dragBox = new Rectangle(new Point(e.X - (dragsize.Width / 2), e.Y - (dragsize.Height / 2)), dragsize);

                if (m_dragCursor == null)
                {
                    string filePath = System.IO.Path.Combine(Application.StartupPath, "MyDragTab.cur");
                    m_dragCursor = GetCustomCursor(filePath);
                }
            }

            if (!IsActive)
            {
                //Set the isSwitching field. This prevents the tab from being closed in the MouseUp event
                //if the cursor is over the area in which the close button will be displayed.
                m_isSwitching = true;
                Form.Activate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            m_dragBox = Rectangle.Empty;

            //If the tab is closable and the user is not switching tabs and the mouse was clicked over the
            //close button then close the form. The tab is removed via the FormClose event handler in MdiTabStrip class.
            if (CanClose && !m_isSwitching && closeButtonHitTest(e.X, e.Y))
            {
                Form.Close();
            }

            m_isSwitching = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (CanDrag)
                {
                    //If the tab can be dragged, which is determined by the TabPermenace property, then.
                    if (m_dragBox != Rectangle.Empty & !m_dragBox.Contains(e.X, e.Y))
                    {
                        //If the cursor has been moved out of the bounds of the drag box while the left
                        //mouse button is down then initiate dragging by calling the DoDragDrop method.

                        m_isSwitching = false;
                        DragDropEffects dropEffects = DoDragDrop(this, DragDropEffects.Move);
                        m_dragBox = Rectangle.Empty;
                    }
                }
            }
            else
            {
                //if active then test if mouse cursor is over the close button to display the tool tip.
                if (IsActive)
                {
                    IsMouseOverCloseButton = closeButtonHitTest(e.X, e.Y);
                }
            }
        }




    }
}

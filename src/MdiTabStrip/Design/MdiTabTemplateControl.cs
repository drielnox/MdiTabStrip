using MdiTabStrip.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MdiTabStrip.Design
{
    [ToolboxItem(false)]
    public partial class MdiTabTemplateControl : Control
    {
        private ActiveMdiTabProperties _activeTemplate = new ActiveMdiTabProperties();
        private InactiveMdiTabProperties _inactiveTemplate = new InactiveMdiTabProperties();
        private MdiTabProperties _mouseOverTemplate = new MdiTabProperties();

        private Point[] _activeBounds;
        private Point[] _activeInnerBounds;
        private Point[] _inactiveBounds;
        private Point[] _inactiveInnerBounds;
        private Point[] _mouseOverBounds;
        private Point[] _mouseOverInnerBounds;
        private Point[] _closeButtonBounds;
        private Point[] _closeButtonGlyphBounds;
        private bool _isMouseOverCloseButton = false;

        internal event EventHandler<TabSelectedEventArgs> TabSelected;

        public ActiveMdiTabProperties ActiveTabTemplate
        {
            get { return _activeTemplate; }
        }
        public InactiveMdiTabProperties InactiveTabTemplate
        {
            get { return _inactiveTemplate; }
        }
        public MdiTabProperties MouseOverTabTemplate
        {
            get { return _mouseOverTemplate; }
        }
        public bool IsMouseOverCloseButton
        {
            get { return _isMouseOverCloseButton; }
            set
            {
                if (_isMouseOverCloseButton != value)
                {
                    _isMouseOverCloseButton = value;
                    Invalidate();
                }
            }
        }
        protected override Size DefaultSize
        {
            get { return new Size(50, 40); }
        }

        public MdiTabTemplateControl()
        {
            InitializeComponent();

            DoubleBuffered = true;
            GetTabBounds();
            Dock = DockStyle.Top;

            _activeTemplate.OnPropertyChanged += _activeTemplate_OnPropertyChanged;
            _inactiveTemplate.OnPropertyChanged += _inactiveTemplate_OnPropertyChanged;
            _mouseOverTemplate.OnPropertyChanged += _mouseOverTemplate_OnPropertyChanged;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            DrawTab(pe.Graphics);
        }

        protected override void OnPaintBackground(PaintEventArgs pe)
        {
            base.OnPaintBackground(pe);

            pe.Graphics.FillRectangle(Brushes.White, pe.ClipRectangle);
            DrawTabBackground(pe.Graphics);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (CloseButtonHitTest(e.X, e.Y))
            {
                IsMouseOverCloseButton = true;
            }
            else
            {
                IsMouseOverCloseButton = false;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            TabSelectedEventArgs ev = null;

            if (ActiveHitTest(e.X, e.Y))
            {
                ev = new TabSelectedEventArgs(TabType.Active);
            }
            else if (InactiveHitTest(e.X, e.Y))
            {
                ev = new TabSelectedEventArgs(TabType.Inactive);
            }
            else if (MouseOverHitTest(e.X, e.Y))
            {
                ev = new TabSelectedEventArgs(TabType.MouseOver);
            }

            if (ev != null)
            {
                TabSelected(this, ev);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GetTabBounds();
            Invalidate();
        }

        protected virtual void DrawTab(Graphics g)
        {
            DrawActiveTab(g);
            DrawInactiveTab(g);
            DrawMouseOverTab(g);
        }

        private void GetTabBounds()
        {
            Point startPoint;

            if (RightToLeft == RightToLeft.Yes)
            {
                _activeBounds = new Point[]
                {
                    new Point(-2, Height),
                    new Point(-2, Height - 5),
                    new Point(55, Height - 5),
                    new Point(58, Height - 6),
                    new Point(58, 11),
                    new Point(60, 8),
                    new Point(198, 8),
                    new Point(200, 11),
                    new Point(200, Height - 6),
                    new Point(202, Height - 5),
                    new Point(Width, Height - 5),
                    new Point(Width, Height)
                };
                _activeInnerBounds = new Point[]
                {
                    new Point(-1, Height),
                    new Point(-1, Height - 4),
                    new Point(56, Height - 4),
                    new Point(59, Height - 6),
                    new Point(59, 12),
                    new Point(61, 9),
                    new Point(197, 9),
                    new Point(199, 12),
                    new Point(199, Height - 6),
                    new Point(201, Height - 4),
                    new Point(Width - 1, Height - 4),
                    new Point(Width - 1, Height)
                };
                _inactiveBounds = new Point[]
                {
                    new Point(200, Height - 5),
                    new Point(200, 13),
                    new Point(202, 10),
                    new Point(348, 10),
                    new Point(350, 13),
                    new Point(350, Height - 5)
                };
                _inactiveInnerBounds = new Point[]
                {
                    new Point(201, Height - 5),
                    new Point(201, 14),
                    new Point(203, 11),
                    new Point(347, 11),
                    new Point(349, 14),
                    new Point(349, Height - 5)
                };
                _mouseOverBounds = new Point[]
                {
                    new Point(350, Height - 5),
                    new Point(350, 13),
                    new Point(352, 10),
                    new Point(498, 10),
                    new Point(500, 13),
                    new Point(500, Height - 5)
                };
                _mouseOverInnerBounds = new Point[]
                {
                    new Point(351, Height - 5),
                    new Point(351, 14),
                    new Point(353, 11),
                    new Point(497, 11),
                    new Point(499, 14),
                    new Point(499, Height - 5)
                };
                _closeButtonBounds = new Point[]
                {
                    new Point(75, 15),
                    new Point(63, 15),
                    new Point(61, 17),
                    new Point(61, 28),
                    new Point(63, 30),
                    new Point(75, 30),
                    new Point(77, 28),
                    new Point(77, 17),
                    new Point(75, 15)
                };
                startPoint = new Point(65, 19);
            }
            else
            {
                _activeBounds = new Point[]
                {
                    new Point(-2, Height),
                    new Point(-2, Height - 5),
                    new Point(5, Height - 5),
                    new Point(8, Height - 6),
                    new Point(8, 11),
                    new Point(10, 8),
                    new Point(148, 8),
                    new Point(150, 11),
                    new Point(150, Height - 6),
                    new Point(152, Height - 5),
                    new Point(Width, Height - 5),
                    new Point(Width, Height)
                };
                _activeInnerBounds = new Point[]
                {
                    new Point(-1, Height),
                    new Point(-1, Height - 4),
                    new Point(6, Height - 4),
                    new Point(9, Height - 6),
                    new Point(9, 12),
                    new Point(11, 9),
                    new Point(147, 9),
                    new Point(149, 12),
                    new Point(149, Height - 6),
                    new Point(151, Height - 4),
                    new Point(Width - 1, Height - 4),
                    new Point(Width - 1, Height)
                };
                _inactiveBounds = new Point[]
                {
                    new Point(150, Height - 5),
                    new Point(150, 13),
                    new Point(152, 10),
                    new Point(298, 10),
                    new Point(300, 13),
                    new Point(300, Height - 5)
                };
                _inactiveInnerBounds = new Point[]
                {
                    new Point(151, Height - 5),
                    new Point(151, 14),
                    new Point(153, 11),
                    new Point(297, 11),
                    new Point(299, 14),
                    new Point(299, Height - 5)
                };
                _mouseOverBounds = new Point[]
                {
                    new Point(300, Height - 5),
                    new Point(300, 13),
                    new Point(302, 10),
                    new Point(448, 10),
                    new Point(450, 13),
                    new Point(450, Height - 5)
                };
                _mouseOverInnerBounds = new Point[]
                {
                    new Point(301, Height - 5),
                    new Point(301, 14),
                    new Point(303, 11),
                    new Point(447, 11),
                    new Point(449, 14),
                    new Point(449, Height - 5)
                };
                _closeButtonBounds = new Point[]
                {
                    new Point(132, 15),
                    new Point(144, 15),
                    new Point(146, 17),
                    new Point(146, 28),
                    new Point(144, 30),
                    new Point(132, 30),
                    new Point(130, 28),
                    new Point(130, 17),
                    new Point(132, 15)
                };
                startPoint = new Point(134, 19);
            }

            _closeButtonGlyphBounds = new Point[]
            {
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

        private void DrawActiveTab(Graphics g)
        {
            Rectangle iconRectangle;
            Rectangle textRectangle = new Rectangle(30, 16, 98, Height - 21);

            if (RightToLeft == RightToLeft.Yes)
            {
                iconRectangle = new Rectangle(180, 13, 17, 17);
                textRectangle.Offset(52, 0);
            }
            else
            {
                iconRectangle = new Rectangle(13, 13, 17, 17);
            }

            DrawFormIcon(g, iconRectangle);
            DrawTabText(g, textRectangle, "Active Tab", ActiveTabTemplate.ForeColor, ActiveTabTemplate.Font);
            DrawCloseButton(g);
        }

        private void DrawInactiveTab(Graphics g)
        {
            Rectangle iconRectangle;
            Rectangle textRectangle = new Rectangle(172, 18, 123, Height - 23);

            if (RightToLeft == RightToLeft.Yes)
            {
                iconRectangle = new Rectangle(330, 15, 17, 17);
                textRectangle.Offset(37, 0);
            }
            else
            {
                iconRectangle = new Rectangle(155, 15, 17, 17);
            }

            DrawFormIcon(g, iconRectangle);
            DrawTabText(g, textRectangle, "Inactive Tab", InactiveTabTemplate.ForeColor, InactiveTabTemplate.Font);
        }

        private void DrawMouseOverTab(Graphics g)
        {
            Rectangle iconRectangle;
            Rectangle textRectangle = new Rectangle(322, 18, 123, Height - 23);

            if (RightToLeft == RightToLeft.Yes)
            {
                iconRectangle = new Rectangle(480, 15, 17, 17);
                textRectangle.Offset(37, 0);
            }
            else
            {
                iconRectangle = new Rectangle(305, 15, 17, 17);
            }

            DrawFormIcon(g, iconRectangle);
            DrawTabText(g, textRectangle, "MouseOver Tab", MouseOverTabTemplate.ForeColor, MouseOverTabTemplate.Font);
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

        private void DrawTabText(Graphics g, Rectangle rect, string text, Color color, Font font)
        {
            TextFormatFlags textFlags = TextFormatFlags.WordEllipsis | TextFormatFlags.EndEllipsis;

            if (RightToLeft == RightToLeft.Yes)
            {
                textFlags = textFlags | TextFormatFlags.Right;
            }
            else
            {
                textFlags = textFlags | TextFormatFlags.Left;
            }

            TextRenderer.DrawText(g, text, font, rect, color, textFlags);
        }

        private void DrawFormIcon(Graphics g, Rectangle iconRectangle)
        {
            Icon icon = Resources.Document;

            using (Bitmap bmp = new Bitmap(icon.Width, icon.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics bg = Graphics.FromImage(bmp))
                {
                    bg.DrawIcon(icon, 0, 0);
                }

                g.DrawImage(bmp, iconRectangle);
            }
        }

        private void DrawInactiveCloseButton(Graphics g)
        {
            DrawCloseButtonGlyph(g, ActiveTabTemplate.CloseButtonForeColor);
        }

        private void DrawActiveCloseButton(Graphics g)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(_closeButtonBounds);

                using (SolidBrush backBrush = new SolidBrush(ActiveTabTemplate.CloseButtonBackColor))
                {
                    g.FillPath(backBrush, gp);
                }

                using (Pen borderPen = new Pen(ActiveTabTemplate.CloseButtonBorderColor))
                {
                    g.DrawPath(borderPen, gp);
                }
            }

            DrawCloseButtonGlyph(g, ActiveTabTemplate.CloseButtonHotForeColor);
        }

        private void DrawCloseButtonGlyph(Graphics g, Color color)
        {
            g.SmoothingMode = SmoothingMode.None;

            using (GraphicsPath shadow = new GraphicsPath())
            {
                Matrix translateMatrix = new Matrix();
                Color shadowColor = Color.FromArgb(120, 130, 130, 130);

                shadow.AddLines(_closeButtonGlyphBounds);
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
                gp.AddLines(_closeButtonGlyphBounds);

                using (SolidBrush glyphBrush = new SolidBrush(color))
                {
                    g.FillPath(glyphBrush, gp);
                }

                using (Pen glyphPen = new Pen(color))
                {
                    g.DrawPath(glyphPen, gp);
                }
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
        }

        private void DrawTabBackground(Graphics g)
        {
            DrawInactiveTabBackground(g);
            DrawMouseOverTabBackground(g);
            DrawActiveTabBackground(g);
        }

        private void DrawActiveTabBackground(Graphics g)
        {
            Rectangle rect = DisplayRectangle;
            Rectangle shadowRectangle = new Rectangle(0, Height - 5, Width, 5);
            Blend shadowBlend = new Blend();

            rect.Offset(0, 8);
            rect.Height -= 8;
            g.SmoothingMode = SmoothingMode.None;
            shadowBlend.Factors = new float[] { 0.0F, 0.1F, 0.3F, 0.4F };
            shadowBlend.Positions = new float[] { 0.0F, 0.5F, 0.8F, 1.0F };

            using (GraphicsPath outerPath = new GraphicsPath())
            {
                outerPath.AddLines(_activeBounds);

                using (LinearGradientBrush gradientbrush = new LinearGradientBrush(rect, Color.White, ActiveTabTemplate.BackColor, LinearGradientMode.Vertical))
                {
                    Blend bl = new Blend();
                    bl.Factors = new float[] { 0.3F, 0.4F, 0.5F, 1.0F, 1.0F };
                    bl.Positions = new float[] { 0.0F, 0.2F, 0.35F, 0.35F, 1.0F };

                    gradientbrush.Blend = bl;
                    g.FillPath(gradientbrush, outerPath);
                }

                using (LinearGradientBrush shadowBrush = new LinearGradientBrush(shadowRectangle, ActiveTabTemplate.BackColor, Color.Black, LinearGradientMode.Vertical))
                {
                    shadowBrush.Blend = shadowBlend;
                    g.FillRectangle(shadowBrush, shadowRectangle);
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(new Pen(ActiveTabTemplate.BorderColor), outerPath);
            }

            using (GraphicsPath innerPath = new GraphicsPath())
            {
                innerPath.AddLines(_activeInnerBounds);

                Color lineColor = Color.FromArgb(120, 255, 255, 255);
                g.DrawPath(new Pen(lineColor), innerPath);
            }
        }

        private void DrawMouseOverTabBackground(Graphics g)
        {
            Rectangle rect = DisplayRectangle;

            rect.Offset(0, 8);
            rect.Height -= 8;
            g.SmoothingMode = SmoothingMode.None;

            using (GraphicsPath outerPath = new GraphicsPath())
            {
                outerPath.AddLines(_mouseOverBounds);

                using (LinearGradientBrush gradientbrush = new LinearGradientBrush(rect, Color.White, MouseOverTabTemplate.BackColor, LinearGradientMode.Vertical))
                {
                    Blend bl = new Blend();
                    bl.Factors = new float[] { 0.3F, 0.4F, 0.5F, 1.0F, 0.8F, 0.7F };
                    bl.Positions = new float[] { 0.0F, 0.2F, 0.4F, 0.4F, 0.8F, 1.0F };

                    gradientbrush.Blend = bl;
                    g.FillPath(gradientbrush, outerPath);
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(new Pen(InactiveTabTemplate.BorderColor), outerPath);
            }

            using (GraphicsPath innerPath = new GraphicsPath())
            {
                innerPath.AddLines(_mouseOverInnerBounds);

                Color lineColor = Color.FromArgb(120, 255, 255, 255);
                g.DrawPath(new Pen(lineColor), innerPath);
            }
        }

        private void DrawInactiveTabBackground(Graphics g)
        {
            Rectangle rect = DisplayRectangle;

            rect.Offset(0, 8);
            rect.Height -= 8;
            g.SmoothingMode = SmoothingMode.None;

            using (GraphicsPath outerPath = new GraphicsPath())
            {
                outerPath.AddLines(_inactiveBounds);

                using (LinearGradientBrush gradientbrush = new LinearGradientBrush(rect, Color.White, InactiveTabTemplate.BackColor, LinearGradientMode.Vertical))
                {
                    Blend bl = new Blend();
                    bl.Factors = new float[] { 0.3F, 0.4F, 0.5F, 1.0F, 0.8F, 0.7F };
                    bl.Positions = new float[] { 0.0F, 0.2F, 0.4F, 0.4F, 0.8F, 1.0F };

                    gradientbrush.Blend = bl;
                    g.FillPath(gradientbrush, outerPath);
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(new Pen(InactiveTabTemplate.BorderColor), outerPath);
            }

            using (GraphicsPath innerPath = new GraphicsPath())
            {
                innerPath.AddLines(_inactiveInnerBounds);

                Color lineColor = Color.FromArgb(120, 255, 255, 255);
                g.DrawPath(new Pen(lineColor), innerPath);
            }
        }

        private bool ActiveHitTest(int x, int y)
        {
            bool hit = false;

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.StartFigure();
                gp.AddLines(_activeBounds);
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

        private bool InactiveHitTest(int x, int y)
        {
            bool hit = false;

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.StartFigure();
                gp.AddLines(_inactiveBounds);
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

        private bool MouseOverHitTest(int x, int y)
        {
            bool hit = false;

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.StartFigure();
                gp.AddLines(_mouseOverBounds);
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

        private bool CloseButtonHitTest(int x, int y)
        {
            bool hit = false;

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.StartFigure();
                gp.AddLines(_closeButtonBounds);
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

        private void _activeTemplate_OnPropertyChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void _inactiveTemplate_OnPropertyChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void _mouseOverTemplate_OnPropertyChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
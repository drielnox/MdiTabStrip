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
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            DrawTab(pe.Graphics);
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
    }
}

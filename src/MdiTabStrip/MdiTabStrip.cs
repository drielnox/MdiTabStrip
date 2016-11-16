using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using MdiTabStrip.Design;
using System.Drawing.Imaging;
using System.Windows.Forms.Layout;

namespace MdiTabStrip
{
    /// <summary>
    /// Provides a container for tabs representing forms opened in an MDI application.
    /// </summary>
    [Designer(typeof(MdiTabStripDesigner))]
    [ToolboxBitmap(typeof(MdiTabStrip), "MdiTabStrip.bmp")]
    public class MdiTabStrip : ScrollableControl, ISupportInitialize
    {
        #region "Fields"
        private TabStripLayoutEngine _layout;
        private MdiTab _activeTab;
        private MdiTabCollection _tabs = new MdiTabCollection();
        private int _indexOfTabForDrop;
        private bool _isDragging = false;
        private int _maxTabWidth = 200;
        private int _minTabWidth = 80;
        private MdiTabStripItemBase _mouseOverControl = null;
        private ScrollDirection _dragDirection = ScrollDirection.Left;
        private MdiScrollTab withEventsField__leftScrollTab = new MdiScrollTab(this, ScrollTabType.ScrollTabLeft);
        private MdiScrollTab _leftScrollTab
        {
            get { return withEventsField__leftScrollTab; }
            set
            {
                if (withEventsField__leftScrollTab != null)
                {
                    withEventsField__leftScrollTab.ScrollTab -= leftTabScroll_ScrollTab;
                }
                withEventsField__leftScrollTab = value;
                if (withEventsField__leftScrollTab != null)
                {
                    withEventsField__leftScrollTab.ScrollTab += leftTabScroll_ScrollTab;
                }
            }
        }
        private MdiScrollTab _dropDownScrollTab = new MdiScrollTab(this, ScrollTabType.ScrollTabDropDown);
        private MdiScrollTab withEventsField__rightScrollTab = new MdiScrollTab(this, ScrollTabType.ScrollTabRight);
        private MdiScrollTab _rightScrollTab
        {
            get { return withEventsField__rightScrollTab; }
            set
            {
                if (withEventsField__rightScrollTab != null)
                {
                    withEventsField__rightScrollTab.ScrollTab -= rightTabScroll_ScrollTab;
                }
                withEventsField__rightScrollTab = value;
                if (withEventsField__rightScrollTab != null)
                {
                    withEventsField__rightScrollTab.ScrollTab += rightTabScroll_ScrollTab;
                }
            }
        }
        private MdiNewTab _newTab = new MdiNewTab(this);
        private bool _mdiNewTabVisible = false;
        private int _mdiNewTabWidth = 25;
        private Image _mdiNewTabImage = null;
        private MdiTabPermanence _tabPermanence = MdiTabPermanence.None;
        private bool _displayFormIcon = true;
        private MdiChildWindowState _mdiWindowState = MdiChildWindowState.Normal;
        private ToolTip withEventsField__toolTip = new ToolTip();
        private ToolTip _toolTip
        {
            get { return withEventsField__toolTip; }
            set
            {
                if (withEventsField__toolTip != null)
                {
                    withEventsField__toolTip.Popup -= _toolTip_Popup;
                    withEventsField__toolTip.Draw -= _toolTip_Draw;
                }
                withEventsField__toolTip = value;
                if (withEventsField__toolTip != null)
                {
                    withEventsField__toolTip.Popup += _toolTip_Popup;
                    withEventsField__toolTip.Draw += _toolTip_Draw;
                }
            }
        }
        private bool _showTabToolTip = true;

        private string _newTabToolTipText = "New Tab";
        //Active tab fields
        private Color _activeTabColor = Color.LightSteelBlue;
        private Color _activeTabBorderColor = Color.Gray;
        private Color _activeTabForeColor = SystemColors.ControlText;
        private Font _activeTabFont = SystemFonts.DefaultFont;
        private Color _closeButtonBackColor = Color.Gainsboro;
        private Color _closeButtonBorderColor = Color.Gray;
        private Color _closeButtonForeColor = SystemColors.ControlText;

        private Color _closeButtonHotForeColor = Color.Firebrick;
        //Inactive tab fields
        private Color _inactiveTabColor = Color.Gainsboro;
        private Color _inactiveTabBorderColor = Color.Silver;
        private Color _inactiveTabForeColor = SystemColors.ControlText;

        private Font _inactiveTabFont = SystemFonts.DefaultFont;
        //Moused over tab fields
        private Color _mouseOverTabColor = Color.LightSteelBlue;
        private Color _mouseOverTabForeColor = SystemColors.ControlText;

        private Font _mouseOverTabFont = SystemFonts.DefaultFont;
        //Fade animation fields
        private bool _animate = true;
        private int _duration = 20;
        private Timer withEventsField__timer = new Timer();
        private Timer _timer
        {
            get { return withEventsField__timer; }
            set
            {
                if (withEventsField__timer != null)
                {
                    withEventsField__timer.Tick -= _timer_Tick;
                }
                withEventsField__timer = value;
                if (withEventsField__timer != null)
                {
                    withEventsField__timer.Tick += _timer_Tick;
                }
            }
        }
        private List<Color> _backColorFadeArray = new List<Color>();
        private List<Color> _foreColorFadeArray;
        #endregion
        private ArrayList _animatingTabs = new ArrayList();

        #region "Events"
        /// <summary>
        /// Occurs when the <see cref="MdiTab"/> has been made active.
        /// </summary>
        public event EventHandler CurrentMdiTabChanged;

        /// <summary>
        /// Occurs when a new <see cref="MdiTab"/> is added to the <see cref="MdiTabCollection"/>.
        /// </summary>
        public event EventHandler<MdiTabStripTabEventArgs> MdiTabAdded;

        /// <summary>
        /// Occurs when the <see cref="MdiTab"/> is clicked.
        /// </summary>
        public event EventHandler<MdiTabStripTabClickedEventArgs> MdiTabClicked;

        /// <summary>
        /// Occurs when the <see cref="MdiTab"/> has been moved to a new position.
        /// </summary>
        public event EventHandler MdiTabIndexChanged;

        /// <summary>
        /// Occurs when an <see cref="MdiTab"/> is removed from the <see cref="MdiTabCollection"/>.
        /// </summary>
        public event EventHandler<MdiTabStripTabEventArgs> MdiTabRemoved;

        /// <summary>
        /// Occurs when the <see cref="MdiNewTab"/> is clicked.
        /// </summary>
        public event EventHandler MdiNewTabClicked;
        #endregion

        #region "Constructor/Destructor"
        /// <summary>
        /// Initializes a new instance of the <see cref="MdiTabStrip"/> class.
        /// </summary>
        public MdiTabStrip()
            : base()
        {
            ResizeRedraw = true;
            DoubleBuffered = true;
            MinimumSize = new Size(50, 33);
            Dock = DockStyle.Top;
            AllowDrop = true;
            //Padding values directly affect the tab's placement, change these in the designer to see
            //how the tab's size and placement change.
            Padding = new Padding(5, 3, 20, 5);
            _timer.Interval = 2;
            _backColorFadeArray = GetFadeSteps(_inactiveTabColor, _mouseOverTabColor);
            _foreColorFadeArray = GetFadeSteps(_inactiveTabForeColor, _mouseOverTabForeColor);
            _toolTip.AutoPopDelay = 2000;
            _toolTip.OwnerDraw = true;

            //Setup scrolltab sizes
            _leftScrollTab.Size = new Size(20, DisplayRectangle.Height);
            _dropDownScrollTab.Size = new Size(14, DisplayRectangle.Height);
            _rightScrollTab.Size = new Size(20, DisplayRectangle.Height);
            _newTab.Size = new Size(25, DisplayRectangle.Height);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MdiTabStrip"/> and optionally releases the managed resources. 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LeftScrollTab.Dispose();
                DropDownTab.Dispose();
                RightScrollTab.Dispose();
                MdiNewTab.Dispose();
                _toolTip.Dispose();

                _activeTabFont.Dispose();
                _inactiveTabFont.Dispose();
                _mouseOverTabFont.Dispose();

                _timer.Dispose();

                Form parent = FindForm();

                if (parent != null)
                {
                    //Unhook event handler registered with the top form.
                    parent.MdiChildActivate -= MdiChildActivated;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region "ISupportInitialize implementation"
        //Initialization is used so that the top form can be found. This is needed in case the control
        //is added to a container control such as a panel.

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
            SuspendLayout();
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            ResumeLayout();
            Form parent = FindForm();

            if (parent != null)
            {
                //Register event handler with top form.
                parent.MdiChildActivate += MdiChildActivated;
            }
        }
        #endregion

        #region "Properties"

        #region "Active Tab properties"
        /// <summary>
        /// Gets or sets the background color of the active tab.
        /// </summary>
        /// <returns>The background <see cref="Color"/> of the active <see cref="MdiTab"/>. The default value is LightSteelBlue.</returns>
        [DefaultValue(typeof(Color), "LightSteelBlue"), Category("Active Tab"), Description("The background color of the currently active tab.")]
        public Color ActiveTabColor
        {
            get { return _activeTabColor; }
            set
            {
                _activeTabColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the active tab.
        /// </summary>
        /// <returns>The foreground <see cref="Color"/> of the active <see cref="MdiTab"/>. The default value is ControlText.</returns>
        [DefaultValue(typeof(Color), "ControlText"), Category("Active Tab"), Description("The foreground color of the currently active tab, which is used to display text.")]
        public Color ActiveTabForeColor
        {
            get { return _activeTabForeColor; }
            set
            {
                _activeTabForeColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the border color of the active tab.
        /// </summary>
        /// <returns>The border <see cref="Color"/> of the active <see cref="MdiTab"/>. The default value is Gray.</returns>
        [DefaultValue(typeof(Color), "Gray"), Category("Active Tab"), Description("The border color of the currently active tab.")]
        public Color ActiveTabBorderColor
        {
            get { return _activeTabBorderColor; }
            set
            {
                _activeTabBorderColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the font of the active tab.
        /// </summary>
        /// <returns>The <see cref="Font"/> to apply to the text displayed by the active <see cref="MdiTab"/>. The value returned will vary depending on the user's operating system the local culture setting of their system.</returns>
        [DefaultValue(typeof(Font), "SystemFonts.DefaultFont"), Category("Active Tab"), Description("The font used to display text in the currently active tab.")]
        public Font ActiveTabFont
        {
            get { return _activeTabFont; }
            set
            {
                _activeTabFont = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the background color of the close button.
        /// </summary>
        /// <returns>The background <see cref="Color"/> of the close button. The default value is Gainsboro.</returns>
        [DefaultValue(typeof(Color), "Gainsboro"), Category("Active Tab"), Description("The background color of the close button when moused over.")]
        public Color CloseButtonBackColor
        {
            get { return _closeButtonBackColor; }
            set { _closeButtonBackColor = value; }
        }

        /// <summary>
        /// Gets or sets the foreground color of the close button.
        /// </summary>
        /// <returns>The foreground <see cref="Color"/> of the close button. The default value is ControlText.</returns>
        [DefaultValue(typeof(Color), "ControlText"), Category("Active Tab"), Description("The foreground color of the close button, used to display the glyph.")]
        public Color CloseButtonForeColor
        {
            get { return _closeButtonForeColor; }
            set
            {
                _closeButtonForeColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the close button when the mouse cursor is hovered over it.
        /// </summary>
        /// <returns>The foreground <see cref="Color"/> of the close button when hovered over. The default value is Firebrick.</returns>
        [DefaultValue(typeof(Color), "Firebrick"), Category("Active Tab"), Description("The foreground color of the close button when moused over, used to display the glyph.")]
        public Color CloseButtonHotForeColor
        {
            get { return _closeButtonHotForeColor; }
            set { _closeButtonHotForeColor = value; }
        }

        /// <summary>
        /// Gets or sets the border color of the close button.
        /// </summary>
        /// <returns>The border <see cref="Color"/> of the close button. The default value is Gray.</returns>
        [DefaultValue(typeof(Color), "Gray"), Category("Active Tab"), Description("The border color of the close button when moused over.")]
        public Color CloseButtonBorderColor
        {
            get { return _closeButtonBorderColor; }
            set { _closeButtonBorderColor = value; }
        }
        #endregion

        #region "Inactive Tab properties"
        /// <summary>
        /// Gets or sets the background color of the inactive tab.
        /// </summary>
        /// <returns>The background <see cref="Color"/> of the inactive <see cref="MdiTab"/>. The default value is Gainsboro.</returns>
        [DefaultValue(typeof(Color), "Gainsboro"), Category("Inactive Tab"), Description("The background color of all inactive tabs.")]
        public Color InactiveTabColor
        {
            get { return _inactiveTabColor; }
            set
            {
                _inactiveTabColor = value;
                _backColorFadeArray = GetFadeSteps(InactiveTabColor, MouseOverTabColor);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the inactive tab.
        /// </summary>
        /// <returns>The foreground <see cref="Color"/> of the inactive <see cref="MdiTab"/>. The default value is ControlText.</returns>
        [DefaultValue(typeof(Color), "ControlText"), Category("Inactive Tab"), Description("The foreground color of all inactive tabs, which is used to display text.")]
        public Color InactiveTabForeColor
        {
            get { return _inactiveTabForeColor; }
            set
            {
                _inactiveTabForeColor = value;
                _foreColorFadeArray = GetFadeSteps(InactiveTabForeColor, MouseOverTabForeColor);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the border color of the inactive tab.
        /// </summary>
        /// <returns>The border <see cref="Color"/> of the inactive <see cref="MdiTab"/>. The default value is Silver.</returns>
        [DefaultValue(typeof(Color), "Silver"), Category("Inactive Tab"), Description("The border color of all inactive tabs.")]
        public Color InactiveTabBorderColor
        {
            get { return _inactiveTabBorderColor; }
            set
            {
                _inactiveTabBorderColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the font of the inactive tab.
        /// </summary>
        /// <returns>The <see cref="Font"/> to apply to the text displayed by the inactive <see cref="MdiTab"/>. The value returned will vary depending on the user's operating system the local culture setting of their system.</returns>
        [DefaultValue(typeof(Font), "SytemFonts.DefaultFont"), Category("Inactive Tab"), Description("The font used to display text in all inactive tabs.")]
        public Font InactiveTabFont
        {
            get { return _inactiveTabFont; }
            set
            {
                _inactiveTabFont = value;
                Invalidate();
            }
        }
        #endregion

        #region "Mouseover Tab properties"
        /// <summary>
        /// Gets or sets the background color of the moused over tab.
        /// </summary>
        /// <returns>The background <see cref="Color"/> of the moused over <see cref="MdiTab"/>. The default value is LightSteelBlue.</returns>
        [DefaultValue(typeof(Color), "LightSteelBlue"), Category("Mouse Over Tab"), Description("The background color for the tab the mouse cursor is currently over.")]
        public Color MouseOverTabColor
        {
            get { return _mouseOverTabColor; }
            set
            {
                _mouseOverTabColor = value;
                _backColorFadeArray = GetFadeSteps(InactiveTabColor, MouseOverTabColor);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the moused over tab.
        /// </summary>
        /// <returns>The foreground <see cref="Color"/> of the moused over <see cref="MdiTab"/>. The default value is ControlText.</returns>
        [DefaultValue(typeof(Color), "ControlText"), Category("Mouse Over Tab"), Description("The foreground color of the tab the mouse cursor is currently over, which is used to display text and glyphs.")]
        public Color MouseOverTabForeColor
        {
            get { return _mouseOverTabForeColor; }
            set
            {
                _mouseOverTabForeColor = value;
                _foreColorFadeArray = GetFadeSteps(InactiveTabForeColor, MouseOverTabForeColor);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the font of the moused over tab.
        /// </summary>
        /// <returns>The <see cref="Font"/> to apply to the text displayed by the moused over <see cref="MdiTab"/>. The value returned will vary depending on the user's operating system the local culture setting of their system.</returns>
        [DefaultValue(typeof(Font), "SystemFonts.DefaultFont"), Category("Mouse Over Tab"), Description("The font used to display text in the tab the mouse cursor is currently over.")]
        public Font MouseOverTabFont
        {
            get { return _mouseOverTabFont; }
            set
            {
                _mouseOverTabFont = value;
                Invalidate();
            }
        }
        #endregion

        #region "ScrollTab properties"
        [Browsable(false)]
        public MdiScrollTab LeftScrollTab
        {
            get { return _leftScrollTab; }
        }

        [Browsable(false)]
        public MdiScrollTab RightScrollTab
        {
            get { return _rightScrollTab; }
        }

        [Browsable(false)]
        public MdiScrollTab DropDownTab
        {
            get { return _dropDownScrollTab; }
        }
        #endregion

        [Browsable(false)]
        public MdiNewTab MdiNewTab
        {
            get { return _newTab; }
        }

        internal int Duration
        {
            get { return _duration; }
        }

        /// <summary>
        /// Gets the rectangle that represents the display area of the control.
        /// </summary>
        /// <returns>A <see cref="Rectangle"/> that represents the display area of the control.</returns>
        public override System.Drawing.Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rect = new Rectangle(0, 0, Width, Height);

                rect.X += Padding.Left;
                rect.Y += Padding.Top;
                rect.Width -= Padding.Left + Padding.Right;
                rect.Height -= Padding.Top + Padding.Bottom;

                return rect;
            }
        }

        /// <summary>
        /// Gets the default size of the control.
        /// </summary>
        /// <returns>The default <see cref="Size"/> of the control.</returns>
        protected override System.Drawing.Size DefaultSize
        {
            get { return new Size(50, 35); }
        }

        /// <summary>
        /// Gets or sets if the control should animate between the inactive and moused over background colors.
        /// </summary>
        /// <returns>true if the should animate; otherwise, false. The default is true.</returns>
        [DefaultValue(true), Category("Behavior")]
        public bool Animate
        {
            get { return _animate; }
            set { _animate = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether an icon is displayed on the tab for the form.
        /// </summary>
        /// <returns>true if the control displays an icon in the tab; otherwise, false. The default is true.</returns>
        [DefaultValue(true), Category("Appearance")]
        public bool DisplayFormIcon
        {
            get { return _displayFormIcon; }
            set
            {
                _displayFormIcon = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="ToolStripRenderer"/> used to customize the look and feel of the <see cref="MdiTabStrip"/>'s drop down.
        /// </summary>
        /// <returns>A <see cref="ToolStripRenderer"/> used to customize the look and feel of a <see cref="MdiTabStrip"/>'s drop down.</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ToolStripRenderer DropDownRenderer
        {
            get { return _dropDownScrollTab.MdiMenu.Renderer; }
            set { _dropDownScrollTab.MdiMenu.Renderer = value; }
        }

        internal bool IsFirstTabActive
        {
            get { return Tabs.IndexOf(ActiveTab) == 0; }
        }

        internal bool IsLastTabActive
        {
            get { return Tabs.IndexOf(ActiveTab) == Tabs.Count - 1; }
        }

        /// <summary>
        /// Gets or sets the desired window state of all child <see cref="Form"/>s.
        /// </summary>
        /// <returns>normal if each form's WindowState and ControlBox property settings should be obeyed; otherwise, 
        /// maximized, to force all forms to be maximized in the MDI window. The default is normal.</returns>
        [DefaultValue(typeof(MdiChildWindowState), "Normal"), Category("Layout"), Description("Gets or sets the desired window state of all child forms")]
        public MdiChildWindowState MdiWindowState
        {
            get { return _mdiWindowState; }
            set { _mdiWindowState = value; }
        }

        /// <summary>
        /// Gets or sets the permanance of the tab.
        /// </summary>
        /// <returns>first if the first tab open should not be closeable, last open if the last remaining tab should not be closeable; otherwise none. The default is all tabs are closeable.</returns>
        [DefaultValue(typeof(MdiTabPermanence), "None"), Category("Behavior"), Description("Defines how the control will handle the closing of tabs. The first tab or the last remaining tab can be restricted from closing or a setting of 'None' will allow all tabs to be closed.")]
        public MdiTabPermanence TabPermanence
        {
            get { return _tabPermanence; }
            set
            {
                _tabPermanence = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the <see cref="MdiNewTab"/>.
        /// </summary>
        [DefaultValue(false), Category("Appearance"), Description("Gets or sets whether or not the control will display the MdiNewTab.")]
        public bool MdiNewTabVisible
        {
            get { return _mdiNewTabVisible; }
            set
            {
                if (_mdiNewTabVisible != value)
                {
                    _mdiNewTabVisible = value;
                    PerformLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the <see cref="MdiNewTab"/>.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DefaultValue(25), Category("Layout"), Description("Gets or sets the width of the MdiNewTab.")]
        public int MdiNewTabWidth
        {
            //Return Me._mdiNewTabWidth
            get { return MdiNewTab.Width; }
            set
            {
                //If Me._mdiNewTabWidth <> value Then
                //Me._mdiNewTabWidth = value
                MdiNewTab.Width = value;
                PerformLayout();
                Invalidate();
                //End If
            }
        }

        /// <summary>
        /// Gets or sets the image for the <see cref="MdiNewTab"/>.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Category("Appearance"), Description("Gets or sets the image for the MdiNewTab.")]
        public Image MdiNewTabImage
        {
            get { return _mdiNewTabImage; }
            set { _mdiNewTabImage = value; }
        }

        /// <summary>
        /// Gets all the tabs that belong to the <see cref="MdiTabStrip"/>.
        /// </summary>
        /// <returns>An object of type <see cref="MdiTabCollection"/>, representing all the tabs contained by an <see cref="MdiTabStrip"/>.</returns>
        [Browsable(false)]
        public MdiTabCollection Tabs
        {
            get { return _tabs; }
        }

        /// <summary>
        /// Passes a reference to the cached <see cref="LayoutEngine"/> returned by the layout engine interface.
        /// </summary>
        /// <returns>A <see cref="LayoutEngine"/> that represents the cached layout engine returned by the layout engine interface.</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override LayoutEngine LayoutEngine
        {
            get
            {
                if (_layout == null)
                {
                    _layout = new TabStripLayoutEngine();
                }

                return (LayoutEngine)_layout;
            }
        }

        /// <summary>
        /// Gets or sets the maximum width for the tab.
        /// </summary>
        /// <returns>The maximum width a tab can be sized to. The default value is 200.</returns>
        /// <remarks>This property affects the tab's size when resizing the control and is used in conjunction with the <seealso cref="MinTabWidth"/> property.</remarks>
        [DefaultValue(200), Category("Layout"), Description("The maximum width for each tab.")]
        public int MaxTabWidth
        {
            get { return _maxTabWidth; }
            set { _maxTabWidth = value; }
        }

        /// <summary>
        /// Gets or sets the minimum width for the tab.
        /// </summary>
        /// <returns>The minimum width a tab can be sized to. The default is 80.</returns>
        /// <remarks>This property affects the tab's size when resizing the control and is used in conjunction with the <seealso cref="MaxTabWidth"/> property.</remarks>
        [DefaultValue(80), Category("Layout"), Description("The minimum width for each tab.")]
        public int MinTabWidth
        {
            get { return _minTabWidth; }
            set { _minTabWidth = value; }
        }

        /// <summary>
        /// Gets or sets the active tab.
        /// </summary>
        /// <returns>The <see cref="MdiTab"/> that is currenly active.</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public MdiTab ActiveTab
        {
            get { return _activeTab; }
            set
            {
                if (!object.ReferenceEquals(_activeTab, value))
                {
                    _activeTab = value;
                    OnCurrentMdiTabChanged(new EventArgs());
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        internal bool IsDragging
        {
            get { return _isDragging; }
            set { _isDragging = value; }
        }

        internal List<Color> BackColorFadeSteps
        {
            get { return _backColorFadeArray; }
        }

        internal List<Color> ForeColorFadeSteps
        {
            get { return _foreColorFadeArray; }
        }

        private MdiTabStripItemBase MouseOverControl
        {
            get { return _mouseOverControl; }
            set
            {
                _mouseOverControl = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Specifies whether to display ToolTips on tabs.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Specifies whether to display ToolTips on tabs.")]
        public bool ShowTabToolTip
        {
            get { return _showTabToolTip; }
            set { _showTabToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the ToolTip text of the <see cref="MdiNewTab"/>.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets the ToolTip text of the MdiNewTab.")]
        public string NewTabToolTipText
        {
            get { return _newTabToolTipText; }
            set { _newTabToolTipText = value; }
        }
        #endregion

        #region "Methods"

        #region "Form Event Handlers"
        protected void MdiChildActivated(object sender, EventArgs e)
        {
            Form f = ((Form)sender).ActiveMdiChild;

            //If the ActiveMDIChild is nothing then exit routine.
            if (f == null)
            {
                return;
            }

            //If a tab has already been created for the form then activate it,
            //otherwise create a new one.
            if (TabExists(f))
            {
                ActivateTab(f);
            }
            else
            {
                CreateTab(f);
            }

            //If the first tab has been made active then disable the left scroll tab
            //If the last tab has been made active then disable the right scroll tab
            LeftScrollTab.Enabled = (RightToLeft == RightToLeft.Yes ? !IsLastTabActive : !IsFirstTabActive);
            RightScrollTab.Enabled = (RightToLeft == RightToLeft.Yes ? !IsFirstTabActive : !IsLastTabActive);

            Invalidate();
        }

        protected void OnFormTextChanged(object sender, EventArgs e)
        {
            Form f = (Form)sender;

            //Find the menu item that cooresponds to this form and update it's Text property.
            //Can't override the menuitem's Text property to return the Form.Text property because when
            //the form's text property is changed the drop down menu does not resize itself accordingly.
            foreach (MdiMenuItem mi in _dropDownScrollTab._mdiMenu.Items)
            {
                if (object.ReferenceEquals(mi.Form, f))
                {
                    mi.Text = f.Text;
                }
            }

            Invalidate();
        }

        private bool TabExists(Form mdiForm)
        {
            foreach (MdiTab tab in Tabs)
            {
                if (object.ReferenceEquals(tab.Form, mdiForm))
                {
                    return true;
                }
            }

            return false;
        }

        private void ActivateTab(Form mdiForm)
        {
            foreach (MdiTab t in Tabs)
            {
                if (object.ReferenceEquals(t.Form, mdiForm))
                {
                    ActiveTab = t;

                    //Find the menu item of the drop down menu and set it's Checked property
                    foreach (MdiMenuItem mi in _dropDownScrollTab._mdiMenu.Items)
                    {
                        if (object.ReferenceEquals(mi.Form, mdiForm))
                        {
                            _dropDownScrollTab._mdiMenu.SetItemChecked(mi);
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }

                    return;
                }
            }
        }

        private void CreateTab(Form mdiForm)
        {
            MdiTab tab = new MdiTab(this);

            //Set up the tab
            if (_mdiWindowState == MdiChildWindowState.Maximized)
            {
                mdiForm.SuspendLayout();
                mdiForm.FormBorderStyle = FormBorderStyle.None;
                mdiForm.ControlBox = false;
                mdiForm.HelpButton = false;
                mdiForm.MaximizeBox = false;
                mdiForm.MinimizeBox = false;
                mdiForm.SizeGripStyle = SizeGripStyle.Hide;
                mdiForm.ShowIcon = false;
                mdiForm.Dock = DockStyle.Fill;
                mdiForm.ResumeLayout(true);
            }

            tab.Form = mdiForm;

            //Register event handler with the MdiChild form's FormClosed event.
            mdiForm.FormClosed += OnFormClosed;
            mdiForm.TextChanged += OnFormTextChanged;

            //Add the new tab to the Tabs collection and set it as the active tab
            Tabs.Add(tab);
            OnMdiTabAdded(new MdiTabStripTabEventArgs(tab));
            ActiveTab = tab;

            //Create a cooresponding menu item in the drop down menu
            AddMdiItem(mdiForm, tab);
            UpdateTabVisibility(ScrollDirection.Right);
        }

        private void RemoveTab(Form mdiForm)
        {
            foreach (MdiTab tab in Tabs)
            {
                if (object.ReferenceEquals(tab.Form, mdiForm))
                {
                    //This algorithm will get the index of the tab that is higher than the tab
                    //that is to be removed. This has the affect of making the tab occuring after
                    //the tab just closed the active tab.
                    int tabIndex = Tabs.IndexOf(tab);

                    //Remove tab from the Tabs collection
                    Tabs.Remove(tab);
                    OnMdiTabRemoved(new MdiTabStripTabEventArgs(tab));

                    //Remove the cooresponding menu item from the drop down menu.
                    foreach (MdiMenuItem mi in _dropDownScrollTab._mdiMenu.Items)
                    {
                        if (object.ReferenceEquals(mi.Form, tab.Form))
                        {
                            _dropDownScrollTab._mdiMenu.Items.Remove(mi);
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }

                    //If the tab removed was the last tab in the collection then
                    //set the index to the last tab.
                    if (tabIndex > Tabs.Count - 1)
                    {
                        tabIndex = Tabs.Count - 1;
                    }

                    if (tabIndex > -1)
                    {
                        //Call the Form's Activate method to allow the event handlers
                        //to perform their neccessary calculations.
                        Tabs[tabIndex].Form.Activate();
                    }
                    else
                    {
                        ActiveTab = null;
                    }

                    UpdateTabVisibility(ScrollDirection.Right);
                    Invalidate();
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
        }

        protected void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            //Only remove the tab when the form was closed by the user. All other close reason look like they
            //will also be closing the Mdi parent and so will dispose the MdiTabStrip.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                RemoveTab((Form)sender);
            }
        }
        #endregion

        #region "Paint Methods"

        #region "ToolTip painting"
        private void _toolTip_Popup(object sender, PopupEventArgs e)
        {
            Size s = TextRenderer.MeasureText(_toolTip.GetToolTip(e.AssociatedControl), SystemFonts.SmallCaptionFont);

            s.Width += 4;
            s.Height += 6;
            e.ToolTipSize = s;
        }

        private void _toolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            Rectangle rect = e.Bounds;

            using (LinearGradientBrush lgb = new LinearGradientBrush(e.Bounds, Color.WhiteSmoke, Color.Silver, LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(lgb, e.Bounds);
            }

            rect.Width -= 1;
            rect.Height -= 1;
            e.Graphics.DrawRectangle(SystemPens.ControlDarkDark, rect);
            e.DrawText();
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            foreach (MdiTab tab in Tabs)
            {
                if (tab.Visible)
                {
                    tab.DrawControl(e.Graphics);
                }
            }

            if (RightScrollTab.Visible)
            {
                RightScrollTab.DrawControl(e.Graphics);
            }

            if (LeftScrollTab.Visible)
            {
                LeftScrollTab.DrawControl(e.Graphics);
            }

            if (DropDownTab.Visible)
            {
                DropDownTab.DrawControl(e.Graphics);
            }

            if (MdiNewTabVisible)
            {
                _newTab.DrawControl(e.Graphics);
            }

            //Draw DragDrop glyphs
            if (IsDragging)
            {
                MdiTab mditab = Tabs[_indexOfTabForDrop];
                Point[] topTriangle = null;
                Point[] bottomTriangle = null;

                if (_dragDirection == ScrollDirection.Left)
                {
                    //Glyphs need to be located on the left side of the tab
                    topTriangle = new Point[] {
					new Point(mditab.Left - 3, 0),
					new Point(mditab.Left + 3, 0),
					new Point(mditab.Left, 5)
				};
                    bottomTriangle = new Point[] {
					new Point(mditab.Left - 3, Height - 1),
					new Point(mditab.Left + 3, Height - 1),
					new Point(mditab.Left, Height - 6)
				};
                }
                else
                {
                    //Glyphs need to be located on the right side of the tab
                    topTriangle = new Point[] {
					new Point(mditab.Right - 3, 0),
					new Point(mditab.Right + 3, 0),
					new Point(mditab.Right, 5)
				};
                    bottomTriangle = new Point[] {
					new Point(mditab.Right - 3, Height - 1),
					new Point(mditab.Right + 3, Height - 1),
					new Point(mditab.Right, Height - 6)
				};
                }

                e.Graphics.FillPolygon(Brushes.Black, topTriangle);
                e.Graphics.FillPolygon(Brushes.Black, bottomTriangle);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            foreach (MdiTab tab in Tabs)
            {
                //Draw the active tab last to make sure nothing paints over it.
                if (!tab.IsActive && tab.Visible)
                {
                    tab.DrawControlBackground(e.Graphics);
                }
            }

            if (RightScrollTab != null && _rightScrollTab.Visible)
            {
                RightScrollTab.DrawControlBackground(e.Graphics);
            }

            if (LeftScrollTab != null && _leftScrollTab.Visible)
            {
                LeftScrollTab.DrawControlBackground(e.Graphics);
            }

            if (DropDownTab != null && _dropDownScrollTab.Visible)
            {
                DropDownTab.DrawControlBackground(e.Graphics);
            }

            if (_newTab != null && MdiNewTabVisible)
            {
                _newTab.DrawControlBackground(e.Graphics);
            }

            if (ActiveTab != null)
            {
                ActiveTab.DrawControlBackground(e.Graphics);
            }
        }
        #endregion

        #region "Fade Animation"
        /// <summary>
        /// This method creates a Bitmap using the duration field as the width and creates a LinearGradientBrush
        /// using the colors passed in as parameters. It then fills the bitmap using
        /// the brush and reads the color values of each pixel along the width into a List for use in the
        /// fade animations. This method is called in the constructor and the Set procedures of the
        /// InactiveTabColor, InactiveTabForeColor, MouseOverTabColor and MouseOverTabForeColor properties.
        /// </summary>
        /// <remarks></remarks>
        private List<Color> GetFadeSteps(Color color1, Color color2)
        {
            List<Color> colorArray = new List<Color>();

            using (Bitmap bmp = new Bitmap(_duration, 1))
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    using (LinearGradientBrush lgb = new LinearGradientBrush(rect, color1, color2, LinearGradientMode.Horizontal))
                    {
                        g.FillRectangle(lgb, rect);
                    }
                }

                for (int x = 0; x <= bmp.Width - 1; x++)
                {
                    colorArray.Add(bmp.GetPixel(x, 0));
                }
            }

            return colorArray;
        }

        /// <summary>
        /// For each tick of the Timer this event handler will iterate through the ArrayList of tabs that
        /// are currently needing to animate. For each tab it's current frame is incremented and sent as a
        /// parameter in the OnAnimationTick method. Depending on the animation type if the tab's current
        /// frame is 0 or equal to the Duration - 1 then the tab's animation will be stopped.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        /// <remarks></remarks>
        private void _timer_Tick(object sender, System.EventArgs e)
        {
            int index = (_animatingTabs.Count - 1);
            while ((index >= 0))
            {
                MdiTab tab = (MdiTab)_animatingTabs[index];
                int frame = tab.CurrentFrame;
                if (tab.AnimationType == AnimationType.FadeIn)
                {
                    if (frame == _duration - 1)
                    {
                        tab.StopAnimation();
                        return;
                    }
                    frame += 1;
                }
                else if (tab.AnimationType == AnimationType.FadeOut)
                {
                    if (frame == 0)
                    {
                        tab.StopAnimation();
                        return;
                    }
                    frame -= 1;
                }

                tab.OnAnimationTick(frame);
                index -= 1;
            }
        }

        internal void AddAnimatingTab(MdiTab tab)
        {
            if (_animatingTabs.IndexOf(tab) < 0)
            {
                //Add the tab to the arraylist only if it is not already in here.
                _animatingTabs.Add(tab);
                if (_animatingTabs.Count == 1)
                {
                    _timer.Enabled = true;
                }
            }
        }

        internal void RemoveAnimatingTab(MdiTab tab)
        {
            _animatingTabs.Remove(tab);
            if (_animatingTabs.Count == 0)
            {
                _timer.Enabled = false;
            }
        }
        #endregion

        #region "Mouse Events"
        /// <summary>
        /// Determines which tab the cursor is over, sends the appropriate MouseEvent to it and caches the tab.
        /// When the cached tab doesn't match the one the cursor is over then MouseLeave is invoked for this tab
        /// and MouseEnter is invoked for the new tab. If the tab has not changed then the MouseMove event is invoked.
        /// </summary>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void CheckMousePosition(MouseEventArgs e)
        {
            //This test is done to handle a user attempting to drag a tab to a new location.
            //Without this test in place DragDrop would not be initiated when a user clicks and starts
            //to drag at a point close to a tabs edge.
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && _mouseOverControl != null)
            {
                _mouseOverControl.InvokeMouseMove(e);
                return;
            }

            foreach (MdiTab tab in Tabs)
            {
                if (tab.Visible && tab.HitTest(e.X, e.Y))
                {
                    if (!object.ReferenceEquals(tab, _mouseOverControl))
                    {
                        if (_mouseOverControl != null)
                        {
                            _mouseOverControl.InvokeMouseLeave(new EventArgs());
                        }

                        MouseOverControl = tab;
                        tab.InvokeMouseEnter(new EventArgs());
                    }
                    else
                    {
                        tab.InvokeMouseMove(e);
                    }
                    return;
                }
            }

            if (LeftScrollTab.Visible && LeftScrollTab.HitTest(e.X, e.Y))
            {
                if (!object.ReferenceEquals(LeftScrollTab, _mouseOverControl))
                {
                    if (_mouseOverControl != null)
                    {
                        _mouseOverControl.InvokeMouseLeave(new EventArgs());
                    }

                    MouseOverControl = LeftScrollTab;
                    LeftScrollTab.InvokeMouseEnter(new EventArgs());
                }
                else
                {
                    LeftScrollTab.InvokeMouseMove(e);
                }
                return;
            }
            else if (DropDownTab.Visible && DropDownTab.HitTest(e.X, e.Y))
            {
                if (!object.ReferenceEquals(DropDownTab, _mouseOverControl))
                {
                    if (_mouseOverControl != null)
                    {
                        _mouseOverControl.InvokeMouseLeave(new EventArgs());
                    }

                    MouseOverControl = DropDownTab;
                    DropDownTab.InvokeMouseEnter(new EventArgs());

                    if (ShowTabToolTip)
                    {
                        UpdateToolTip("Tab List");
                    }
                }
                else
                {
                    DropDownTab.InvokeMouseMove(e);
                }
                return;
            }
            else if (RightScrollTab.Visible && RightScrollTab.HitTest(e.X, e.Y))
            {
                if (!object.ReferenceEquals(RightScrollTab, _mouseOverControl))
                {
                    if (_mouseOverControl != null)
                    {
                        _mouseOverControl.InvokeMouseLeave(new EventArgs());
                    }

                    MouseOverControl = RightScrollTab;
                    RightScrollTab.InvokeMouseEnter(new EventArgs());
                }
                else
                {
                    RightScrollTab.InvokeMouseMove(e);
                }
                return;
            }
            else if (MdiNewTabVisible && MdiNewTab.HitTest(e.X, e.Y))
            {
                if (!object.ReferenceEquals(MdiNewTab, _mouseOverControl))
                {
                    if (_mouseOverControl != null)
                    {
                        _mouseOverControl.InvokeMouseLeave(new EventArgs());
                    }

                    MouseOverControl = MdiNewTab;
                    MdiNewTab.InvokeMouseEnter(new EventArgs());

                    if (ShowTabToolTip)
                    {
                        UpdateToolTip(NewTabToolTipText);
                    }
                }
                else
                {
                    MdiNewTab.InvokeMouseMove(e);
                }
                return;
            }

            if (_mouseOverControl != null)
            {
                _mouseOverControl.InvokeMouseLeave(new EventArgs());
            }

            _mouseOverControl = null;
            _toolTip.Hide(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            CheckMousePosition(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_mouseOverControl != null)
            {
                _mouseOverControl.InvokeMouseDown(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (MouseOverControl != null)
            {
                MouseOverControl.InvokeMouseUp(e);
            }
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);

            //Call each tab's MouseLeave method so that it can properly animate a fade out and
            //reset it's current frame to zero.
            foreach (MdiTab tab in Tabs)
            {
                tab.InvokeMouseLeave(e);
            }

            LeftScrollTab.InvokeMouseLeave(e);
            DropDownTab.InvokeMouseLeave(e);
            RightScrollTab.InvokeMouseLeave(e);
            MdiNewTab.InvokeMouseLeave(e);
            MouseOverControl = null;

            Invalidate();
        }

        internal void UpdateToolTip(string tipText)
        {
            _toolTip.Hide(this);
            _toolTip.Active = false;
            _toolTip.Active = true;
            Point location = Cursor.Position;
            location.Y = (location.Y + (Cursor.Size.Height - Cursor.Current.HotSpot.Y));
            _toolTip.Show(tipText, this, base.PointToClient(location), _toolTip.AutoPopDelay);
        }

        #endregion

        #region "Drag Drop Methods"
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (!drgevent.Data.GetDataPresent(typeof(MdiTab)))
            {
                drgevent.Effect = DragDropEffects.None;
                return;
            }

            IsDragging = true;
            drgevent.Effect = DragDropEffects.Move;

            Point pt = PointToClient(new Point(drgevent.X, drgevent.Y));
            DragDropHitTest(pt.X, pt.Y);
            Invalidate();
        }

        private void DragDropHitTest(int mouseX, int mouseY)
        {
            //MdiTab tab = null;

            foreach (MdiTab tab in Tabs)
            {
                if (tab.CanDrag && tab.Visible)
                {
                    //Only test mouse position if the tab is visible and can be dragged (which signifies
                    //whether or not the tab can be reordered)
                    if (tab.HitTest(mouseX, mouseY))
                    {
                        int activeIndex = Tabs.IndexOf(ActiveTab);
                        if (tab == null)
                        {
                            //This should never happen but check just in case.
                            _indexOfTabForDrop = activeIndex;
                        }
                        else if (object.ReferenceEquals(tab, ActiveTab))
                        {
                            //When starting a drag operation this should be the first test hit. We set the index
                            //to the active tab and setup the direction so that the indicator is displayed one
                            //the correct side of the tab.
                            _indexOfTabForDrop = activeIndex;

                            if (RightToLeft == RightToLeft.Yes)
                            {
                                _dragDirection = ScrollDirection.Right;
                            }
                            else
                            {
                                _dragDirection = ScrollDirection.Left;
                            }
                        }
                        else
                        {
                            //The code below determines the index at which the tab being currently dragged
                            //should be dropped at based on the direction the tab is being dragged (determined
                            //by the active tab's current index) as well as splitting the tabs 80/20.
                            //It is easier to understand seeing it in action than it is to explain it.
                            int currentIndex = Tabs.IndexOf(tab);

                            if (RightToLeft == RightToLeft.Yes)
                            {
                                if (currentIndex <= activeIndex)
                                {
                                    int a = tab.Location.X + (tab.Width * 0.2);

                                    _dragDirection = ScrollDirection.Right;

                                    if (mouseX < a)
                                    {
                                        if (currentIndex + 1 < Tabs.Count)
                                        {
                                            _indexOfTabForDrop = currentIndex + 1;
                                        }
                                    }
                                    else
                                    {
                                        _indexOfTabForDrop = currentIndex;
                                    }
                                }
                                else
                                {
                                    int b = tab.Location.X + (tab.Width * 0.8);

                                    _dragDirection = ScrollDirection.Left;

                                    if (mouseX < b)
                                    {
                                        if (currentIndex < Tabs.Count)
                                        {
                                            _indexOfTabForDrop = currentIndex;
                                        }
                                    }
                                    else
                                    {
                                        if (activeIndex + 1 != currentIndex)
                                        {
                                            _indexOfTabForDrop = currentIndex - 1;
                                        }
                                        else
                                        {
                                            _indexOfTabForDrop = activeIndex;
                                            _dragDirection = ScrollDirection.Right;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (currentIndex <= activeIndex)
                                {
                                    int a = tab.Location.X + (tab.Width * 0.8);

                                    _dragDirection = ScrollDirection.Left;

                                    if (mouseX < a)
                                    {
                                        _indexOfTabForDrop = currentIndex;
                                    }
                                    else
                                    {
                                        if (currentIndex + 1 < Tabs.Count)
                                        {
                                            _indexOfTabForDrop = currentIndex + 1;
                                        }
                                    }
                                }
                                else
                                {
                                    int b = tab.Location.X + (tab.Width * 0.2);

                                    _dragDirection = ScrollDirection.Right;

                                    if (mouseX < b)
                                    {
                                        if (activeIndex + 1 != currentIndex)
                                        {
                                            _indexOfTabForDrop = currentIndex - 1;
                                        }
                                        else
                                        {
                                            _indexOfTabForDrop = activeIndex;
                                            _dragDirection = ScrollDirection.Left;
                                        }
                                    }
                                    else
                                    {
                                        if (currentIndex < Tabs.Count)
                                        {
                                            _indexOfTabForDrop = currentIndex;
                                        }
                                    }
                                }
                            }
                        }

                        break; // TODO: might not be correct. Was : Exit For
                    }
                    // tab.HitTest
                }
                //tab.Visible
            }
            //tab
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(typeof(MdiTab)))
            {
                MdiTab tab = (MdiTab)drgevent.Data.GetData(typeof(MdiTab));

                if (drgevent.Effect == DragDropEffects.Move)
                {
                    //When the tab is dropped it is removed from the collection and then inserted back in at the
                    //designated index. The cooresponding menu item for the drop down is also moved to the same position
                    //in the menu's item collection.
                    if (_tabs.IndexOf(tab) != _indexOfTabForDrop)
                    {
                        Tabs.Remove(tab);
                        Tabs.Insert(_indexOfTabForDrop, tab);
                        OnMdiTabIndexChanged(new EventArgs());
                        PerformLayout();

                        Form f = tab.Form;
                        foreach (MdiMenuItem mi in DropDownTab._mdiMenu.Items)
                        {
                            if (object.ReferenceEquals(mi.Form, f))
                            {
                                DropDownTab._mdiMenu.Items.Remove(mi);
                                DropDownTab._mdiMenu.Items.Insert(_indexOfTabForDrop, mi);
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }

                        //After this operation need to determine if the left or right scroll tab should be enabled or not.
                        LeftScrollTab.Enabled = !IsFirstTabActive;
                        RightScrollTab.Enabled = !IsLastTabActive;
                    }
                }
            }

            IsDragging = false;
            Invalidate();
        }
        #endregion

        #region "ContextMenu methods"
        private void AddMdiItem(Form f, MdiTab tab)
        {
            MdiMenuItem item = new MdiMenuItem(tab, new EventHandler(MenuItemClick));
            Bitmap bmp = new Bitmap(f.Icon.Width, f.Icon.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawIcon(f.Icon, 0, 0);
            }

            item.Image = bmp;
            item.Text = f.Text;
            _dropDownScrollTab._mdiMenu.Items.Add(item);
        }

        private void RemoveMdiItem(Form f)
        {
            foreach (MdiMenuItem mi in _dropDownScrollTab._mdiMenu.Items)
            {
                if (object.ReferenceEquals(mi.Form, f))
                {
                    _dropDownScrollTab._mdiMenu.Items.Remove(mi);
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
        }

        private void MenuItemClick(object sender, EventArgs e)
        {
            MdiMenuItem mItem = sender as MdiMenuItem;
            if (mItem != null)
            {
                ScrollDirection direction = default(ScrollDirection);
                int activeIndex = Tabs.IndexOf(ActiveTab);
                int clickedTabIndex = 0;

                mItem.Form.Activate();
                clickedTabIndex = Tabs.IndexOf(ActiveTab);

                if (activeIndex > clickedTabIndex)
                {
                    direction = ScrollDirection.Left;
                }
                else
                {
                    direction = ScrollDirection.Right;
                }

                UpdateTabVisibility(direction);
            }
        }
        #endregion

        #region "Navigation button Events"
        private void leftTabScroll_ScrollTab(ScrollDirection direction)
        {
            ScrollTabHandler(direction);
        }

        private void rightTabScroll_ScrollTab(ScrollDirection direction)
        {
            ScrollTabHandler(direction);
        }

        /// <summary>
        /// The scroll handler determines the index of the next tab in the direction the user is
        /// intending to scroll. It then calls that tab's Form's Activate method.
        /// </summary>
        /// <param name="direction"></param>
        /// <remarks></remarks>
        private void ScrollTabHandler(ScrollDirection direction)
        {
            int nextIndex = 0;
            if (direction == ScrollDirection.Left)
            {
                nextIndex = Tabs.FirstVisibleTabIndex;
                nextIndex -= 1;
            }
            else
            {
                nextIndex = Tabs.LastVisibleTabIndex;
                nextIndex += 1;
            }

            if (nextIndex > Tabs.Count - 1)
            {
                nextIndex = Tabs.Count - 1;
            }
            else if (nextIndex < 0)
            {
                nextIndex = 0;
            }

            Tabs[nextIndex].Form.Activate();
            UpdateTabVisibility(direction);
        }

        private void UpdateTabVisibility(ScrollDirection direction)
        {
            int tabsToShow = 0;
            int leftTabIndex = 0;
            int rightTabIndex = 0;
            int activeTabIndex = 0;
            int tabAreaWidth = AdjustAvailableWidth();

            //tabAreaWidth = Me.DisplayRectangle.Width

            //'Must subtract the area occupied by the visible scroll tabs to get the
            //'true area the form tabs can occupy.
            //If Me.LeftScrollTab.Visible Then
            //    tabAreaWidth -= Me.LeftScrollTab.Width
            //End If

            //If Me.DropDownTab.Visible Then
            //    tabAreaWidth -= Me.DropDownTab.Width
            //End If

            //If Me.RightScrollTab.Visible Then
            //    tabAreaWidth -= Me.RightScrollTab.Width
            //End If

            //If Me.MdiNewTabVisible Then
            //    tabAreaWidth -= Me._newTab.Width
            //End If

            //Based on the minimum width each tab can be determine the number of tabs
            //that can be shown in the calculated area.
            tabsToShow = tabAreaWidth / MinTabWidth;
            activeTabIndex = Tabs.IndexOf(ActiveTab);

            if (tabsToShow == 1)
            {
                //If only one can be visible then set this tab's index as the right and left
                leftTabIndex = activeTabIndex;
                rightTabIndex = activeTabIndex;
            }
            else if (tabsToShow >= Tabs.Count)
            {
                //If all of the tabs can be visible then set the left index to 0 and 
                //the right to the last tab's index
                leftTabIndex = 0;
                rightTabIndex = Tabs.Count - 1;
            }
            else if (direction == ScrollDirection.Left)
            {
                //Tries to make the active tab the last tab visible. If this calculation puts the left
                //index past zero (negative) then it resets itself so that it shows the number of tabsToShow
                //starting at index zero.
                leftTabIndex = activeTabIndex - (tabsToShow - 1);

                if (leftTabIndex >= 0)
                {
                    rightTabIndex = activeTabIndex;
                }
                else
                {
                    rightTabIndex = activeTabIndex - leftTabIndex;
                    leftTabIndex = 0;
                }
            }
            else if (direction == ScrollDirection.Right)
            {
                //Tries to make the active tab the first tab visible. If this calculation puts the right
                //index past the number of tabs in the collection then it resets itself so that it shows
                //the number of tabsToShow ending at the last index in the collection.
                rightTabIndex = activeTabIndex + (tabsToShow - 1);

                if (rightTabIndex < Tabs.Count)
                {
                    leftTabIndex = activeTabIndex;
                }
                else
                {
                    rightTabIndex = Tabs.Count - 1;
                    leftTabIndex = rightTabIndex - (tabsToShow - 1);
                }
            }
            else
            {
                //The resize event is handled by this section of code. It tries to evenly distribute the hiding
                //and showing of tabs between each side of the active tab. If you have 5 tabs open and the third
                //one is active and you resize the window smaller and smaller you will notice that the last tab
                //on the right disappears first. Then as you continue to resize the first tab on the left 
                //disappears, then the last one on the right and then the first one on the left. At this point
                //only one tab is left visible. If you now resize the window larger a tab on the left will become
                //visible and then one on the right, then the left and then the right.
                int l = tabsToShow / 2;
                int r = 0;

                if (tabsToShow == Tabs.VisibleCount)
                    return;

                if (tabsToShow < Tabs.VisibleCount)
                {
                    SetScrollTabVisibility();
                    AdjustAvailableWidth();
                }

                if (tabsToShow % 2 == 0)
                {
                    r = l - 1;
                }
                else
                {
                    r = l;
                }

                if (activeTabIndex - Tabs.FirstVisibleTabIndex <= Tabs.LastVisibleTabIndex - activeTabIndex)
                {
                    leftTabIndex = activeTabIndex - l;

                    if (leftTabIndex >= 0)
                    {
                        rightTabIndex = activeTabIndex + r;
                    }
                    else
                    {
                        rightTabIndex = tabsToShow - 1;
                        leftTabIndex = 0;
                    }

                    if (rightTabIndex >= Tabs.Count)
                    {
                        rightTabIndex = Tabs.Count - 1;
                        leftTabIndex = rightTabIndex - (tabsToShow - 1);
                    }
                }
                else
                {
                    rightTabIndex = activeTabIndex + r;

                    if (rightTabIndex < Tabs.Count)
                    {
                        leftTabIndex = activeTabIndex - l;
                    }
                    else
                    {
                        rightTabIndex = Tabs.Count - 1;
                        leftTabIndex = rightTabIndex - (tabsToShow - 1);
                    }

                    if (leftTabIndex < 0)
                    {
                        leftTabIndex = 0;
                        rightTabIndex = tabsToShow - 1;
                    }
                }
            }

            //Using the left and right indeces determined above iterate through the tab collection
            //and hide the tab if is not within the range of indeces and show it if it is.
            foreach (MdiTab tab in Tabs)
            {
                int tabPos = Tabs.IndexOf(tab);

                if (tabPos <= rightTabIndex & tabPos >= leftTabIndex)
                {
                    tab.Visible = true;
                }
                else
                {
                    tab.Visible = false;
                }
            }

            //The active tab needs to always be visible. This code ensures that even when the main form
            //is resized to a very small width that this tab remains visible and the control draws correctly.
            if (ActiveTab != null)
            {
                ActiveTab.Visible = true;
            }

            //Figure each scroll tab's visiblity and perform a layout to set each tab's size and location.
            SetScrollTabVisibility();
            PerformLayout();
        }

        private void SetScrollTabVisibility()
        {
            //If tabs are hidden then the left and right scroll tabs need to be displayed.
            //If there are more than one tab open then the drop down tab needs to be displayed.
            //DesignMode is checked so that these tabs will be visible in the design window.
            if (!DesignMode)
            {
                bool hiddenTabs = Tabs.VisibleCount < Tabs.Count;
                bool multipleTabs = Tabs.Count > 1;

                LeftScrollTab.Visible = hiddenTabs;
                RightScrollTab.Visible = hiddenTabs;
                DropDownTab.Visible = multipleTabs;
            }
        }

        private int AdjustAvailableWidth()
        {
            int w = DisplayRectangle.Width;

            //Must subtract the area occupied by the visible scroll tabs to get the
            //true area the form tabs can occupy.
            if (LeftScrollTab.Visible)
            {
                w -= LeftScrollTab.Width;
            }

            if (DropDownTab.Visible)
            {
                w -= DropDownTab.Width;
            }

            if (RightScrollTab.Visible)
            {
                w -= RightScrollTab.Width;
            }

            if (MdiNewTabVisible)
            {
                w -= _newTab.Width;
            }

            return w;
        }
        #endregion

        #region "Resize"
        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            UpdateTabVisibility(ScrollDirection.None);
        }
        #endregion

        #region "Control Events"
        protected internal virtual void OnMdiTabAdded(MdiTabStripTabEventArgs e)
        {
            if (MdiTabAdded != null)
            {
                MdiTabAdded(this, e);
            }
        }

        protected internal virtual void OnMdiTabRemoved(MdiTabStripTabEventArgs e)
        {
            if (MdiTabRemoved != null)
            {
                MdiTabRemoved(this, e);
            }
        }

        public virtual void OnMdiTabClicked(MdiTabStripTabClickedEventArgs e)
        {
            if (MdiTabClicked != null)
            {
                MdiTabClicked(this, e);
            }
        }

        protected internal virtual void OnMdiTabIndexChanged(EventArgs e)
        {
            if (MdiTabIndexChanged != null)
            {
                MdiTabIndexChanged(this, e);
            }
        }

        protected internal virtual void OnCurrentMdiTabChanged(EventArgs e)
        {
            if (CurrentMdiTabChanged != null)
            {
                CurrentMdiTabChanged(this, e);
            }
        }

        protected internal void OnMdiNewTabClick()
        {
            if (MdiNewTabClicked != null)
            {
                MdiNewTabClicked(_newTab, new EventArgs());
            }
        }
        #endregion

        #endregion
    }
}
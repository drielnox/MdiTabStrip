Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Collections.Generic

''' <summary>
''' Provides a container for tabs representing forms opened in an MDI application.
''' </summary>
<Designer(GetType(Design.MdiTabStripDesigner)), _
ToolboxBitmap(GetType(MdiTabStrip), "MdiTabStrip.bmp")> _
Public Class MdiTabStrip
    Inherits ScrollableControl
    Implements ISupportInitialize

#Region "Fields"
    Private m_layout As TabStripLayoutEngine
    Private m_activeTab As MdiTab
    Private m_tabs As New MDITabCollection
    Private m_indexOfTabForDrop As Integer
    Private m_isDragging As Boolean = False
    Private m_maxTabWidth As Integer = 200
    Private m_minTabWidth As Integer = 80
    Private m_mouseOverControl As MdiTabStripItemBase = Nothing
    Private m_dragDirection As ScrollDirection = ScrollDirection.Left
    Private WithEvents m_leftScrollTab As New MdiScrollTab(Me, ScrollTabType.ScrollTabLeft)
    Private WithEvents m_dropDownScrollTab As New MdiScrollTab(Me, ScrollTabType.ScrollTabDropDown)
    Private WithEvents m_rightScrollTab As New MdiScrollTab(Me, ScrollTabType.ScrollTabRight)
    Private WithEvents m_newTab As New MdiNewTab(Me)
    Private m_mdiNewTabVisible As Boolean = False
    Private m_mdiNewTabWidth As Integer = 25
    Private m_mdiNewTabImage As Image = Nothing
    Private m_tabPermanence As MdiTabPermanence = MdiTabPermanence.None
    Private m_displayFormIcon As Boolean = True
    Private m_mdiWindowState As MdiChildWindowState = MdiChildWindowState.Normal
    Private WithEvents m_toolTip As New ToolTip
    Private _showTabToolTip As Boolean = True
    Private _newTabToolTipText As String = "New Tab"

    'Active tab fields
    Private m_activeTabColor As Color = Color.LightSteelBlue
    Private m_activeTabBorderColor As Color = Color.Gray
    Private m_activeTabForeColor As Color = SystemColors.ControlText
    Private m_activeTabFont As Font = SystemFonts.DefaultFont
    Private m_closeButtonBackColor As Color = Color.Gainsboro
    Private m_closeButtonBorderColor As Color = Color.Gray
    Private m_closeButtonForeColor As Color = SystemColors.ControlText
    Private m_closeButtonHotForeColor As Color = Color.Firebrick

    'Inactive tab fields
    Private m_inactiveTabColor As Color = Color.Gainsboro
    Private m_inactiveTabBorderColor As Color = Color.Silver
    Private m_inactiveTabForeColor As Color = SystemColors.ControlText
    Private m_inactiveTabFont As Font = SystemFonts.DefaultFont

    'Moused over tab fields
    Private m_mouseOverTabColor As Color = Color.LightSteelBlue
    Private m_mouseOverTabForeColor As Color = SystemColors.ControlText
    Private m_mouseOverTabFont As Font = SystemFonts.DefaultFont

    'Fade animation fields
    Private m_animate As Boolean = True
    Private m_duration As Integer = 20
    Private WithEvents m_timer As New Timer
    Private m_backColorFadeArray As New System.Collections.Generic.List(Of Color)
    Private m_foreColorFadeArray As List(Of Color)
    Private m_animatingTabs As New ArrayList
#End Region

#Region "Events"
    ''' <summary>
    ''' Occurs when the <see cref="MdiTab"/> has been made active.
    ''' </summary>
    Public Event CurrentMdiTabChanged(ByVal sender As Object, ByVal e As EventArgs)

    ''' <summary>
    ''' Occurs when a new <see cref="MdiTab"/> is added to the <see cref="MdiTabCollection"/>.
    ''' </summary>
    Public Event MdiTabAdded(ByVal sender As Object, ByVal e As MdiTabStripTabEventArgs)

    ''' <summary>
    ''' Occurs when the <see cref="MdiTab"/> is clicked.
    ''' </summary>
    Public Event MdiTabClicked(ByVal sender As Object, ByVal e As MdiTabStripTabClickedEventArgs)

    ''' <summary>
    ''' Occurs when the <see cref="MdiTab"/> has been moved to a new position.
    ''' </summary>
    Public Event MdiTabIndexChanged(ByVal sender As Object, ByVal e As EventArgs)

    ''' <summary>
    ''' Occurs when an <see cref="MdiTab"/> is removed from the <see cref="MdiTabCollection"/>.
    ''' </summary>
    Public Event MdiTabRemoved(ByVal sender As Object, ByVal e As MdiTabStripTabEventArgs)

    ''' <summary>
    ''' Occurs when the <see cref="MdiNewTab"/> is clicked.
    ''' </summary>
    Public Event MdiNewTabClicked(ByVal sender As Object, ByVal e As EventArgs)
#End Region

#Region "Constructor/Destructor"
    ''' <summary>
    ''' Initializes a new instance of the <see cref="MdiTabStrip"/> class.
    ''' </summary>
    Public Sub New()
        MyBase.New()

        ResizeRedraw = True
        DoubleBuffered = True
        MinimumSize = New Size(50, 33)
        Dock = DockStyle.Top
        AllowDrop = True
        'Padding values directly affect the tab's placement, change these in the designer to see
        'how the tab's size and placement change.
        Padding = New Padding(5, 3, 20, 5)
        m_timer.Interval = 2
        m_backColorFadeArray = GetFadeSteps(m_inactiveTabColor, m_mouseOverTabColor)
        m_foreColorFadeArray = GetFadeSteps(m_inactiveTabForeColor, m_mouseOverTabForeColor)
        m_toolTip.AutoPopDelay = 2000
        m_toolTip.OwnerDraw = True

        'Setup scrolltab sizes
        m_leftScrollTab.Size = New Size(20, DisplayRectangle.Height)
        m_dropDownScrollTab.Size = New Size(14, DisplayRectangle.Height)
        m_rightScrollTab.Size = New Size(20, DisplayRectangle.Height)
        m_newTab.Size = New Size(25, DisplayRectangle.Height)
    End Sub

    ''' <summary>
    ''' Releases the unmanaged resources used by the <see cref="MdiTabStrip"/> and optionally releases the managed resources. 
    ''' </summary>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            LeftScrollTab.Dispose()
            DropDownTab.Dispose()
            RightScrollTab.Dispose()
            MdiNewTab.Dispose()
            m_toolTip.Dispose()

            m_activeTabFont.Dispose()
            m_inactiveTabFont.Dispose()
            m_mouseOverTabFont.Dispose()

            m_timer.Dispose()

            Dim parent As Form = FindForm

            If parent IsNot Nothing Then
                'Unhook event handler registered with the top form.
                RemoveHandler parent.MdiChildActivate, AddressOf MdiChildActivated
            End If
        End If

        MyBase.Dispose(disposing)
    End Sub
#End Region

#Region "ISupportInitialize implementation"
    'Initialization is used so that the top form can be found. This is needed in case the control
    'is added to a container control such as a panel.

    ''' <summary>
    ''' Signals the object that initialization is starting.
    ''' </summary>
    Public Sub BeginInit() Implements System.ComponentModel.ISupportInitialize.BeginInit
        SuspendLayout()
    End Sub

    ''' <summary>
    ''' Signals the object that initialization is complete.
    ''' </summary>
    Public Sub EndInit() Implements System.ComponentModel.ISupportInitialize.EndInit
        ResumeLayout()
        Dim parent As Form = FindForm

        If parent IsNot Nothing Then
            'Register event handler with top form.
            AddHandler parent.MdiChildActivate, AddressOf MdiChildActivated
        End If
    End Sub
#End Region

#Region "Properties"

#Region "Active Tab properties"
    ''' <summary>
    ''' Gets or sets the background color of the active tab.
    ''' </summary>
    ''' <returns>The background <see cref="Color"/> of the active <see cref="MdiTab"/>. The default value is LightSteelBlue.</returns>
    <DefaultValue(GetType(Color), "LightSteelBlue"), _
    Category("Active Tab"), _
    Description("The background color of the currently active tab.")> _
    Public Property ActiveTabColor() As Color
        Get
            Return m_activeTabColor
        End Get
        Set(ByVal value As Color)
            m_activeTabColor = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the foreground color of the active tab.
    ''' </summary>
    ''' <returns>The foreground <see cref="Color"/> of the active <see cref="MdiTab"/>. The default value is ControlText.</returns>
    <DefaultValue(GetType(Color), "ControlText"), _
    Category("Active Tab"), _
    Description("The foreground color of the currently active tab, which is used to display text.")> _
    Public Property ActiveTabForeColor() As Color
        Get
            Return m_activeTabForeColor
        End Get
        Set(ByVal value As Color)
            m_activeTabForeColor = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the border color of the active tab.
    ''' </summary>
    ''' <returns>The border <see cref="Color"/> of the active <see cref="MdiTab"/>. The default value is Gray.</returns>
    <DefaultValue(GetType(Color), "Gray"), _
    Category("Active Tab"), _
    Description("The border color of the currently active tab.")> _
    Public Property ActiveTabBorderColor() As Color
        Get
            Return m_activeTabBorderColor
        End Get
        Set(ByVal value As Color)
            m_activeTabBorderColor = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the font of the active tab.
    ''' </summary>
    ''' <returns>The <see cref="Font"/> to apply to the text displayed by the active <see cref="MdiTab"/>. The value returned will vary depending on the user's operating system the local culture setting of their system.</returns>
    <DefaultValue(GetType(Font), "SystemFonts.DefaultFont"), _
    Category("Active Tab"), _
    Description("The font used to display text in the currently active tab.")> _
    Public Property ActiveTabFont() As Font
        Get
            Return m_activeTabFont
        End Get
        Set(ByVal value As Font)
            m_activeTabFont = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the background color of the close button.
    ''' </summary>
    ''' <returns>The background <see cref="Color"/> of the close button. The default value is Gainsboro.</returns>
    <DefaultValue(GetType(Color), "Gainsboro"), _
    Category("Active Tab"), _
    Description("The background color of the close button when moused over.")> _
    Public Property CloseButtonBackColor() As Color
        Get
            Return m_closeButtonBackColor
        End Get
        Set(ByVal value As Color)
            m_closeButtonBackColor = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the foreground color of the close button.
    ''' </summary>
    ''' <returns>The foreground <see cref="Color"/> of the close button. The default value is ControlText.</returns>
    <DefaultValue(GetType(Color), "ControlText"), _
    Category("Active Tab"), _
    Description("The foreground color of the close button, used to display the glyph.")> _
    Public Property CloseButtonForeColor() As Color
        Get
            Return m_closeButtonForeColor
        End Get
        Set(ByVal value As Color)
            m_closeButtonForeColor = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the foreground color of the close button when the mouse cursor is hovered over it.
    ''' </summary>
    ''' <returns>The foreground <see cref="Color"/> of the close button when hovered over. The default value is Firebrick.</returns>
    <DefaultValue(GetType(Color), "Firebrick"), _
    Category("Active Tab"), _
    Description("The foreground color of the close button when moused over, used to display the glyph.")> _
    Public Property CloseButtonHotForeColor() As Color
        Get
            Return m_closeButtonHotForeColor
        End Get
        Set(ByVal value As Color)
            m_closeButtonHotForeColor = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the border color of the close button.
    ''' </summary>
    ''' <returns>The border <see cref="Color"/> of the close button. The default value is Gray.</returns>
    <DefaultValue(GetType(Color), "Gray"), _
    Category("Active Tab"), _
    Description("The border color of the close button when moused over.")> _
    Public Property CloseButtonBorderColor() As Color
        Get
            Return m_closeButtonBorderColor
        End Get
        Set(ByVal value As Color)
            m_closeButtonBorderColor = value
        End Set
    End Property
#End Region

#Region "Inactive Tab properties"
    ''' <summary>
    ''' Gets or sets the background color of the inactive tab.
    ''' </summary>
    ''' <returns>The background <see cref="Color"/> of the inactive <see cref="MdiTab"/>. The default value is Gainsboro.</returns>
    <DefaultValue(GetType(Color), "Gainsboro"), _
    Category("Inactive Tab"), _
    Description("The background color of all inactive tabs.")> _
    Public Property InactiveTabColor() As Color
        Get
            Return m_inactiveTabColor
        End Get
        Set(ByVal value As Color)
            m_inactiveTabColor = value
            m_backColorFadeArray = GetFadeSteps(InactiveTabColor, MouseOverTabColor)
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the foreground color of the inactive tab.
    ''' </summary>
    ''' <returns>The foreground <see cref="Color"/> of the inactive <see cref="MdiTab"/>. The default value is ControlText.</returns>
    <DefaultValue(GetType(Color), "ControlText"), _
    Category("Inactive Tab"), _
    Description("The foreground color of all inactive tabs, which is used to display text.")> _
    Public Property InactiveTabForeColor() As Color
        Get
            Return m_inactiveTabForeColor
        End Get
        Set(ByVal value As Color)
            m_inactiveTabForeColor = value
            m_foreColorFadeArray = GetFadeSteps(InactiveTabForeColor, MouseOverTabForeColor)
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the border color of the inactive tab.
    ''' </summary>
    ''' <returns>The border <see cref="Color"/> of the inactive <see cref="MdiTab"/>. The default value is Silver.</returns>
    <DefaultValue(GetType(Color), "Silver"), _
    Category("Inactive Tab"), _
    Description("The border color of all inactive tabs.")> _
    Public Property InactiveTabBorderColor() As Color
        Get
            Return m_inactiveTabBorderColor
        End Get
        Set(ByVal value As Color)
            m_inactiveTabBorderColor = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the font of the inactive tab.
    ''' </summary>
    ''' <returns>The <see cref="Font"/> to apply to the text displayed by the inactive <see cref="MdiTab"/>. The value returned will vary depending on the user's operating system the local culture setting of their system.</returns>
    <DefaultValue(GetType(Font), "SytemFonts.DefaultFont"), _
    Category("Inactive Tab"), _
    Description("The font used to display text in all inactive tabs.")> _
    Public Property InactiveTabFont() As Font
        Get
            Return m_inactiveTabFont
        End Get
        Set(ByVal value As Font)
            m_inactiveTabFont = value
            Invalidate()
        End Set
    End Property
#End Region

#Region "Mouseover Tab properties"
    ''' <summary>
    ''' Gets or sets the background color of the moused over tab.
    ''' </summary>
    ''' <returns>The background <see cref="Color"/> of the moused over <see cref="MdiTab"/>. The default value is LightSteelBlue.</returns>
    <DefaultValue(GetType(Color), "LightSteelBlue"), _
    Category("Mouse Over Tab"), _
    Description("The background color for the tab the mouse cursor is currently over.")> _
    Public Property MouseOverTabColor() As Color
        Get
            Return m_mouseOverTabColor
        End Get
        Set(ByVal value As Color)
            m_mouseOverTabColor = value
            m_backColorFadeArray = GetFadeSteps(InactiveTabColor, MouseOverTabColor)
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the foreground color of the moused over tab.
    ''' </summary>
    ''' <returns>The foreground <see cref="Color"/> of the moused over <see cref="MdiTab"/>. The default value is ControlText.</returns>
    <DefaultValue(GetType(Color), "ControlText"), _
    Category("Mouse Over Tab"), _
    Description("The foreground color of the tab the mouse cursor is currently over, which is used to display text and glyphs.")> _
    Public Property MouseOverTabForeColor() As Color
        Get
            Return m_mouseOverTabForeColor
        End Get
        Set(ByVal value As Color)
            m_mouseOverTabForeColor = value
            m_foreColorFadeArray = GetFadeSteps(InactiveTabForeColor, MouseOverTabForeColor)
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the font of the moused over tab.
    ''' </summary>
    ''' <returns>The <see cref="Font"/> to apply to the text displayed by the moused over <see cref="MdiTab"/>. The value returned will vary depending on the user's operating system the local culture setting of their system.</returns>
    <DefaultValue(GetType(Font), "SystemFonts.DefaultFont"), _
    Category("Mouse Over Tab"), _
    Description("The font used to display text in the tab the mouse cursor is currently over.")> _
    Public Property MouseOverTabFont() As Font
        Get
            Return m_mouseOverTabFont
        End Get
        Set(ByVal value As Font)
            m_mouseOverTabFont = value
            Invalidate()
        End Set
    End Property
#End Region

#Region "ScrollTab properties"
    <Browsable(False)> _
    Public ReadOnly Property LeftScrollTab() As MdiScrollTab
        Get
            Return m_leftScrollTab
        End Get
    End Property

    <Browsable(False)> _
    Public ReadOnly Property RightScrollTab() As MdiScrollTab
        Get
            Return m_rightScrollTab
        End Get
    End Property

    <Browsable(False)> _
    Public ReadOnly Property DropDownTab() As MdiScrollTab
        Get
            Return m_dropDownScrollTab
        End Get
    End Property
#End Region

    <Browsable(False)> _
    Public ReadOnly Property MdiNewTab() As MdiNewTab
        Get
            Return m_newTab
        End Get
    End Property

    Friend ReadOnly Property Duration() As Integer
        Get
            Return m_duration
        End Get
    End Property

    ''' <summary>
    ''' Gets the rectangle that represents the display area of the control.
    ''' </summary>
    ''' <returns>A <see cref="Rectangle"/> that represents the display area of the control.</returns>
    Public Overrides ReadOnly Property DisplayRectangle() As System.Drawing.Rectangle
        Get
            Dim rect As New Rectangle(0, 0, Width, Height)

            rect.X += Padding.Left
            rect.Y += Padding.Top
            rect.Width -= Padding.Left + Padding.Right
            rect.Height -= Padding.Top + Padding.Bottom

            Return rect
        End Get
    End Property

    ''' <summary>
    ''' Gets the default size of the control.
    ''' </summary>
    ''' <returns>The default <see cref="Size"/> of the control.</returns>
    Protected Overrides ReadOnly Property DefaultSize() As System.Drawing.Size
        Get
            Return New Size(50, 35)
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets if the control should animate between the inactive and moused over background colors.
    ''' </summary>
    ''' <returns>true if the should animate; otherwise, false. The default is true.</returns>
    <DefaultValue(True), _
    Category("Behavior")> _
    Public Property Animate() As Boolean
        Get
            Return m_animate
        End Get
        Set(ByVal value As Boolean)
            m_animate = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether an icon is displayed on the tab for the form.
    ''' </summary>
    ''' <returns>true if the control displays an icon in the tab; otherwise, false. The default is true.</returns>
    <DefaultValue(True), _
    Category("Appearance")> _
    Public Property DisplayFormIcon() As Boolean
        Get
            Return m_displayFormIcon
        End Get
        Set(ByVal value As Boolean)
            m_displayFormIcon = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a <see cref="ToolStripRenderer"/> used to customize the look and feel of the <see cref="MdiTabStrip"/>'s drop down.
    ''' </summary>
    ''' <returns>A <see cref="ToolStripRenderer"/> used to customize the look and feel of a <see cref="MdiTabStrip"/>'s drop down.</returns>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), _
    Browsable(False)> _
    Public Property DropDownRenderer() As ToolStripRenderer
        Get
            Return m_dropDownScrollTab.MdiMenu.Renderer
        End Get
        Set(ByVal value As ToolStripRenderer)
            m_dropDownScrollTab.MdiMenu.Renderer = value
        End Set
    End Property

    Friend ReadOnly Property IsFirstTabActive() As Boolean
        Get
            Return Tabs.IndexOf(ActiveTab) = 0
        End Get
    End Property

    Friend ReadOnly Property IsLastTabActive() As Boolean
        Get
            Return Tabs.IndexOf(ActiveTab) = Tabs.Count - 1
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the desired window state of all child <see cref="Form"/>s.
    ''' </summary>
    ''' <returns>normal if each form's WindowState and ControlBox property settings should be obeyed; otherwise, 
    ''' maximized, to force all forms to be maximized in the MDI window. The default is normal.</returns>
    <DefaultValue(GetType(MdiChildWindowState), "Normal"), _
    Category("Layout"), _
    Description("Gets or sets the desired window state of all child forms")> _
    Public Property MdiWindowState() As MdiChildWindowState
        Get
            Return m_mdiWindowState
        End Get
        Set(ByVal value As MdiChildWindowState)
            m_mdiWindowState = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the permanance of the tab.
    ''' </summary>
    ''' <returns>first if the first tab open should not be closeable, last open if the last remaining tab should not be closeable; otherwise none. The default is all tabs are closeable.</returns>
    <DefaultValue(GetType(MdiTabPermanence), "None"), _
    Category("Behavior"), _
    Description("Defines how the control will handle the closing of tabs. The first tab or the last remaining tab can be restricted from closing or a setting of 'None' will allow all tabs to be closed.")> _
    Public Property TabPermanence() As MdiTabPermanence
        Get
            Return m_tabPermanence
        End Get
        Set(ByVal value As MdiTabPermanence)
            m_tabPermanence = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the visibility of the <see cref="MdiNewTab"/>.
    ''' </summary>
    <DefaultValue(False), _
    Category("Appearance"), _
    Description("Gets or sets whether or not the control will display the MdiNewTab.")> _
    Public Property MdiNewTabVisible() As Boolean
        Get
            Return m_mdiNewTabVisible
        End Get
        Set(ByVal value As Boolean)
            If m_mdiNewTabVisible <> value Then
                m_mdiNewTabVisible = value
                PerformLayout()
                Invalidate()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the width of the <see cref="MdiNewTab"/>.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DefaultValue(25), _
    Category("Layout"), _
    Description("Gets or sets the width of the MdiNewTab.")> _
    Public Property MdiNewTabWidth() As Integer
        Get
            'Return Me.m_mdiNewTabWidth
            Return MdiNewTab.Width
        End Get
        Set(ByVal value As Integer)
            'If Me.m_mdiNewTabWidth <> value Then
            'Me.m_mdiNewTabWidth = value
            MdiNewTab.Width = value
            PerformLayout()
            Invalidate()
            'End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the image for the <see cref="MdiNewTab"/>.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Category("Appearance"), _
    Description("Gets or sets the image for the MdiNewTab.")> _
    Public Property MdiNewTabImage() As Image
        Get
            Return m_mdiNewTabImage
        End Get
        Set(ByVal value As Image)
            m_mdiNewTabImage = value
        End Set
    End Property

    ''' <summary>
    ''' Gets all the tabs that belong to the <see cref="MdiTabStrip"/>.
    ''' </summary>
    ''' <returns>An object of type <see cref="MdiTabCollection"/>, representing all the tabs contained by an <see cref="MdiTabStrip"/>.</returns>
    <Browsable(False)> _
    Public ReadOnly Property Tabs() As MDITabCollection
        Get
            Return m_tabs
        End Get
    End Property

    ''' <summary>
    ''' Passes a reference to the cached <see cref="LayoutEngine"/> returned by the layout engine interface.
    ''' </summary>
    ''' <returns>A <see cref="LayoutEngine"/> that represents the cached layout engine returned by the layout engine interface.</returns>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides ReadOnly Property LayoutEngine() As System.Windows.Forms.Layout.LayoutEngine
        Get
            If m_layout Is Nothing Then
                m_layout = New TabStripLayoutEngine
            End If

            Return m_layout
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the maximum width for the tab.
    ''' </summary>
    ''' <returns>The maximum width a tab can be sized to. The default value is 200.</returns>
    ''' <remarks>This property affects the tab's size when resizing the control and is used in conjunction with the <seealso cref="MinTabWidth"/> property.</remarks>
    <DefaultValue(200), _
    Category("Layout"), _
    Description("The maximum width for each tab.")> _
    Public Property MaxTabWidth() As Integer
        Get
            Return m_maxTabWidth
        End Get
        Set(ByVal value As Integer)
            m_maxTabWidth = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the minimum width for the tab.
    ''' </summary>
    ''' <returns>The minimum width a tab can be sized to. The default is 80.</returns>
    ''' <remarks>This property affects the tab's size when resizing the control and is used in conjunction with the <seealso cref="MaxTabWidth"/> property.</remarks>
    <DefaultValue(80), _
    Category("Layout"), _
    Description("The minimum width for each tab.")> _
    Public Property MinTabWidth() As Integer
        Get
            Return m_minTabWidth
        End Get
        Set(ByVal value As Integer)
            m_minTabWidth = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the active tab.
    ''' </summary>
    ''' <returns>The <see cref="MdiTab"/> that is currenly active.</returns>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), _
    Browsable(False)> _
    Public Property ActiveTab() As MdiTab
        Get
            Return m_activeTab
        End Get
        Set(ByVal value As MdiTab)
            If m_activeTab IsNot value Then
                m_activeTab = value
                OnCurrentMdiTabChanged(New EventArgs)
            End If
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), _
    Browsable(False)> _
    Friend Property IsDragging() As Boolean
        Get
            Return m_isDragging
        End Get
        Set(ByVal value As Boolean)
            m_isDragging = value
        End Set
    End Property

    Friend ReadOnly Property BackColorFadeSteps() As List(Of Color)
        Get
            Return m_backColorFadeArray
        End Get
    End Property

    Friend ReadOnly Property ForeColorFadeSteps() As List(Of Color)
        Get
            Return m_foreColorFadeArray
        End Get
    End Property

    Private Property MouseOverControl() As MdiTabStripItemBase
        Get
            Return m_mouseOverControl
        End Get
        Set(ByVal value As MdiTabStripItemBase)
            m_mouseOverControl = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Specifies whether to display ToolTips on tabs.
    ''' </summary>
    <DefaultValue(True), _
    Category("Behavior"), _
    Description("Specifies whether to display ToolTips on tabs.")> _
    Public Property ShowTabToolTip() As Boolean
        Get
            Return _showTabToolTip
        End Get
        Set(ByVal value As Boolean)
            _showTabToolTip = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the ToolTip text of the <see cref="MdiNewTab"/>.
    ''' </summary>
    <Category("Behavior"), _
    Description("Gets or sets the ToolTip text of the MdiNewTab.")> _
    Public Property NewTabToolTipText() As String
        Get
            Return _newTabToolTipText
        End Get
        Set(ByVal value As String)
            _newTabToolTipText = value
        End Set
    End Property
#End Region

#Region "Methods"

#Region "Form Event Handlers"
    Protected Sub MdiChildActivated(ByVal sender As Object, ByVal e As EventArgs)
        Dim f As Form = CType(sender, Form).ActiveMdiChild

        'If the ActiveMDIChild is nothing then exit routine.
        If f Is Nothing Then
            Return
        End If

        'If a tab has already been created for the form then activate it,
        'otherwise create a new one.
        If TabExists(f) Then
            ActivateTab(f)
        Else
            CreateTab(f)
        End If

        'If the first tab has been made active then disable the left scroll tab
        'If the last tab has been made active then disable the right scroll tab
        LeftScrollTab.Enabled = IIf(Me.RightToLeft = Windows.Forms.RightToLeft.Yes, Not IsLastTabActive, Not IsFirstTabActive)
        RightScrollTab.Enabled = IIf(Me.RightToLeft = Windows.Forms.RightToLeft.Yes, Not IsFirstTabActive, Not IsLastTabActive)

        Invalidate()
    End Sub

    Protected Sub OnFormTextChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim f As Form = CType(sender, Form)

        'Find the menu item that cooresponds to this form and update it's Text property.
        'Can't override the menuitem's Text property to return the Form.Text property because when
        'the form's text property is changed the drop down menu does not resize itself accordingly.
        For Each mi As MdiMenuItem In m_dropDownScrollTab.m_mdiMenu.Items
            If mi.Form Is f Then
                mi.Text = f.Text
            End If
        Next

        Invalidate()
    End Sub

    Private Function TabExists(ByVal mdiForm As Form) As Boolean
        For Each tab As MdiTab In Tabs
            If tab.Form Is mdiForm Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Sub ActivateTab(ByVal mdiForm As Form)
        For Each t As MdiTab In Tabs
            If t.Form Is mdiForm Then
                ActiveTab = t

                'Find the menu item of the drop down menu and set it's Checked property
                For Each mi As MdiMenuItem In m_dropDownScrollTab.m_mdiMenu.Items
                    If mi.Form Is mdiForm Then
                        m_dropDownScrollTab.m_mdiMenu.SetItemChecked(mi)
                        Exit For
                    End If
                Next

                Return
            End If
        Next
    End Sub

    Private Sub CreateTab(ByVal mdiForm As Form)
        Dim tab As New MdiTab(Me)

        'Set up the tab
        If Me.m_mdiWindowState = MdiChildWindowState.Maximized Then
            mdiForm.SuspendLayout()
            mdiForm.FormBorderStyle = FormBorderStyle.None
            mdiForm.ControlBox = False
            mdiForm.HelpButton = False
            mdiForm.MaximizeBox = False
            mdiForm.MinimizeBox = False
            mdiForm.SizeGripStyle = SizeGripStyle.Hide
            mdiForm.ShowIcon = False
            mdiForm.Dock = DockStyle.Fill
            mdiForm.ResumeLayout(True)
        End If

        tab.Form = mdiForm

        'Register event handler with the MdiChild form's FormClosed event.
        AddHandler mdiForm.FormClosed, AddressOf OnFormClosed
        AddHandler mdiForm.TextChanged, AddressOf OnFormTextChanged

        'Add the new tab to the Tabs collection and set it as the active tab
        Tabs.Add(tab)
        OnMdiTabAdded(New MdiTabStripTabEventArgs(tab))
        ActiveTab = tab

        'Create a cooresponding menu item in the drop down menu
        AddMdiItem(mdiForm, tab)
        UpdateTabVisibility(ScrollDirection.Right)
    End Sub

    Private Sub RemoveTab(ByVal mdiForm As Form)
        For Each tab As MdiTab In Tabs
            If tab.Form Is mdiForm Then
                'This algorithm will get the index of the tab that is higher than the tab
                'that is to be removed. This has the affect of making the tab occuring after
                'the tab just closed the active tab.
                Dim tabIndex As Integer = Tabs.IndexOf(tab)

                'Remove tab from the Tabs collection
                Tabs.Remove(tab)
                OnMdiTabRemoved(New MdiTabStripTabEventArgs(tab))

                'Remove the cooresponding menu item from the drop down menu.
                For Each mi As MdiMenuItem In m_dropDownScrollTab.m_mdiMenu.Items
                    If mi.Form Is tab.Form Then
                        m_dropDownScrollTab.m_mdiMenu.Items.Remove(mi)
                        Exit For
                    End If
                Next

                'If the tab removed was the last tab in the collection then
                'set the index to the last tab.
                If tabIndex > Tabs.Count - 1 Then
                    tabIndex = Tabs.Count - 1
                End If

                If tabIndex > -1 Then
                    'Call the Form's Activate method to allow the event handlers
                    'to perform their neccessary calculations.
                    Tabs(tabIndex).Form.Activate()
                Else
                    ActiveTab = Nothing
                End If

                UpdateTabVisibility(ScrollDirection.Right)
                Invalidate()
                Exit For
            End If
        Next
    End Sub

    Protected Sub OnFormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs)
        'Only remove the tab when the form was closed by the user. All other close reason look like they
        'will also be closing the Mdi parent and so will dispose the MdiTabStrip.
        If e.CloseReason = CloseReason.UserClosing Then
            RemoveTab(CType(sender, Form))
        End If
    End Sub
#End Region

#Region "Paint Methods"

#Region "ToolTip painting"
    Private Sub m_toolTip_Popup(ByVal sender As Object, ByVal e As System.Windows.Forms.PopupEventArgs) Handles m_toolTip.Popup
        Dim s As Size = TextRenderer.MeasureText(m_toolTip.GetToolTip(e.AssociatedControl), SystemFonts.SmallCaptionFont)

        s.Width += 4
        s.Height += 6
        e.ToolTipSize = s
    End Sub

    Private Sub m_toolTip_Draw(ByVal sender As Object, ByVal e As System.Windows.Forms.DrawToolTipEventArgs) Handles m_toolTip.Draw
        Dim rect As Rectangle = e.Bounds

        Using lgb As New LinearGradientBrush(e.Bounds, Color.WhiteSmoke, Color.Silver, LinearGradientMode.Vertical)
            e.Graphics.FillRectangle(lgb, e.Bounds)
        End Using

        rect.Width -= 1
        rect.Height -= 1
        e.Graphics.DrawRectangle(SystemPens.ControlDarkDark, rect)
        e.DrawText()
    End Sub
#End Region

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        For Each tab As MdiTab In Tabs
            If tab.Visible Then
                tab.DrawControl(e.Graphics)
            End If
        Next

        If RightScrollTab.Visible Then
            RightScrollTab.DrawControl(e.Graphics)
        End If

        If LeftScrollTab.Visible Then
            LeftScrollTab.DrawControl(e.Graphics)
        End If

        If DropDownTab.Visible Then
            DropDownTab.DrawControl(e.Graphics)
        End If

        If MdiNewTabVisible Then
            m_newTab.DrawControl(e.Graphics)
        End If

        'Draw DragDrop glyphs
        If IsDragging Then
            Dim mditab As MdiTab = Tabs(m_indexOfTabForDrop)
            Dim topTriangle As Point()
            Dim bottomTriangle As Point()

            If Me.m_dragDirection = ScrollDirection.Left Then
                'Glyphs need to be located on the left side of the tab
                topTriangle = New Point() {New Point(mditab.Left - 3, 0), _
                                           New Point(mditab.Left + 3, 0), _
                                           New Point(mditab.Left, 5)}
                bottomTriangle = New Point() {New Point(mditab.Left - 3, Height - 1), _
                                              New Point(mditab.Left + 3, Height - 1), _
                                              New Point(mditab.Left, Height - 6)}
            Else
                'Glyphs need to be located on the right side of the tab
                topTriangle = New Point() {New Point(mditab.Right - 3, 0), _
                                           New Point(mditab.Right + 3, 0), _
                                           New Point(mditab.Right, 5)}
                bottomTriangle = New Point() {New Point(mditab.Right - 3, Height - 1), _
                                              New Point(mditab.Right + 3, Height - 1), _
                                              New Point(mditab.Right, Height - 6)}
            End If

            e.Graphics.FillPolygon(Brushes.Black, topTriangle)
            e.Graphics.FillPolygon(Brushes.Black, bottomTriangle)
        End If
    End Sub

    Protected Overrides Sub OnPaintBackground(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaintBackground(e)

        For Each tab As MdiTab In Tabs
            'Draw the active tab last to make sure nothing paints over it.
            If Not tab.IsActive AndAlso tab.Visible Then
                tab.DrawControlBackground(e.Graphics)
            End If
        Next

        If RightScrollTab IsNot Nothing AndAlso m_rightScrollTab.Visible Then
            RightScrollTab.DrawControlBackground(e.Graphics)
        End If

        If LeftScrollTab IsNot Nothing AndAlso m_leftScrollTab.Visible Then
            LeftScrollTab.DrawControlBackground(e.Graphics)
        End If

        If DropDownTab IsNot Nothing AndAlso m_dropDownScrollTab.Visible Then
            DropDownTab.DrawControlBackground(e.Graphics)
        End If

        If m_newTab IsNot Nothing AndAlso MdiNewTabVisible Then
            m_newTab.DrawControlBackground(e.Graphics)
        End If

        If ActiveTab IsNot Nothing Then
            ActiveTab.DrawControlBackground(e.Graphics)
        End If
    End Sub
#End Region

#Region "Fade Animation"
    ''' <summary>
    ''' This method creates a Bitmap using the duration field as the width and creates a LinearGradientBrush
    ''' using the colors passed in as parameters. It then fills the bitmap using
    ''' the brush and reads the color values of each pixel along the width into a List for use in the
    ''' fade animations. This method is called in the constructor and the Set procedures of the
    ''' InactiveTabColor, InactiveTabForeColor, MouseOverTabColor and MouseOverTabForeColor properties.
    ''' </summary>
    ''' <remarks></remarks>
    Private Function GetFadeSteps(ByVal color1 As Color, ByVal color2 As Color) As List(Of Color)
        Dim colorArray As New List(Of Color)

        Using bmp As New Bitmap(m_duration, 1)
            Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)

            Using g As Graphics = Graphics.FromImage(bmp)
                Using lgb As New LinearGradientBrush(rect, color1, color2, LinearGradientMode.Horizontal)
                    g.FillRectangle(lgb, rect)
                End Using
            End Using

            For x As Integer = 0 To bmp.Width - 1
                colorArray.Add(bmp.GetPixel(x, 0))
            Next
        End Using

        Return colorArray
    End Function

    ''' <summary>
    ''' For each tick of the Timer this event handler will iterate through the ArrayList of tabs that
    ''' are currently needing to animate. For each tab it's current frame is incremented and sent as a
    ''' parameter in the OnAnimationTick method. Depending on the animation type if the tab's current
    ''' frame is 0 or equal to the Duration - 1 then the tab's animation will be stopped.
    ''' </summary>
    ''' <param name="sender">Not used</param>
    ''' <param name="e">Not used</param>
    ''' <remarks></remarks>
    Private Sub m_timer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_timer.Tick
        Dim index As Integer = (m_animatingTabs.Count - 1)
        Do While (index >= 0)
            Dim tab As MdiTab = DirectCast(m_animatingTabs.Item(index), MdiTab)
            Dim frame As Integer = tab.CurrentFrame
            If tab.AnimationType = AnimationType.FadeIn Then
                If frame = m_duration - 1 Then
                    tab.StopAnimation()
                    Exit Sub
                End If
                frame += 1
            ElseIf tab.AnimationType = AnimationType.FadeOut Then
                If frame = 0 Then
                    tab.StopAnimation()
                    Exit Sub
                End If
                frame -= 1
            End If

            tab.OnAnimationTick(frame)
            index -= 1
        Loop
    End Sub

    Friend Sub AddAnimatingTab(ByVal tab As MdiTab)
        If m_animatingTabs.IndexOf(tab) < 0 Then
            'Add the tab to the arraylist only if it is not already in here.
            m_animatingTabs.Add(tab)
            If m_animatingTabs.Count = 1 Then
                m_timer.Enabled = True
            End If
        End If
    End Sub

    Friend Sub RemoveAnimatingTab(ByVal tab As MdiTab)
        m_animatingTabs.Remove(tab)
        If m_animatingTabs.Count = 0 Then
            m_timer.Enabled = False
        End If
    End Sub
#End Region

#Region "Mouse Events"
    ''' <summary>
    ''' Determines which tab the cursor is over, sends the appropriate MouseEvent to it and caches the tab.
    ''' When the cached tab doesn't match the one the cursor is over then MouseLeave is invoked for this tab
    ''' and MouseEnter is invoked for the new tab. If the tab has not changed then the MouseMove event is invoked.
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CheckMousePosition(ByVal e As MouseEventArgs)
        'This test is done to handle a user attempting to drag a tab to a new location.
        'Without this test in place DragDrop would not be initiated when a user clicks and starts
        'to drag at a point close to a tabs edge.
        If e.Button And Windows.Forms.MouseButtons.Left = Windows.Forms.MouseButtons.Left AndAlso m_mouseOverControl IsNot Nothing Then
            m_mouseOverControl.InvokeMouseMove(e)
            Return
        End If

        For Each tab As MdiTab In Tabs
            If tab.Visible AndAlso tab.HitTest(e.X, e.Y) Then
                If tab IsNot m_mouseOverControl Then
                    If m_mouseOverControl IsNot Nothing Then
                        m_mouseOverControl.InvokeMouseLeave(New EventArgs)
                    End If

                    MouseOverControl = tab
                    tab.InvokeMouseEnter(New EventArgs)
                Else
                    tab.InvokeMouseMove(e)
                End If
                Return
            End If
        Next

        If LeftScrollTab.Visible AndAlso LeftScrollTab.HitTest(e.X, e.Y) Then
            If LeftScrollTab IsNot m_mouseOverControl Then
                If m_mouseOverControl IsNot Nothing Then
                    m_mouseOverControl.InvokeMouseLeave(New EventArgs)
                End If

                MouseOverControl = LeftScrollTab
                LeftScrollTab.InvokeMouseEnter(New EventArgs)
            Else
                LeftScrollTab.InvokeMouseMove(e)
            End If
            Return
        ElseIf DropDownTab.Visible AndAlso DropDownTab.HitTest(e.X, e.Y) Then
            If DropDownTab IsNot m_mouseOverControl Then
                If m_mouseOverControl IsNot Nothing Then
                    m_mouseOverControl.InvokeMouseLeave(New EventArgs)
                End If

                MouseOverControl = DropDownTab
                DropDownTab.InvokeMouseEnter(New EventArgs)

                If ShowTabToolTip Then
                    UpdateToolTip("Tab List")
                End If
            Else
                DropDownTab.InvokeMouseMove(e)
            End If
            Return
        ElseIf RightScrollTab.Visible AndAlso RightScrollTab.HitTest(e.X, e.Y) Then
            If RightScrollTab IsNot m_mouseOverControl Then
                If m_mouseOverControl IsNot Nothing Then
                    m_mouseOverControl.InvokeMouseLeave(New EventArgs)
                End If

                MouseOverControl = RightScrollTab
                RightScrollTab.InvokeMouseEnter(New EventArgs)
            Else
                RightScrollTab.InvokeMouseMove(e)
            End If
            Return
        ElseIf MdiNewTabVisible AndAlso MdiNewTab.HitTest(e.X, e.Y) Then
            If MdiNewTab IsNot m_mouseOverControl Then
                If m_mouseOverControl IsNot Nothing Then
                    m_mouseOverControl.InvokeMouseLeave(New EventArgs)
                End If

                MouseOverControl = MdiNewTab
                MdiNewTab.InvokeMouseEnter(New EventArgs)

                If ShowTabToolTip Then
                    UpdateToolTip(NewTabToolTipText)
                End If
            Else
                MdiNewTab.InvokeMouseMove(e)
            End If
            Return
            End If

        If m_mouseOverControl IsNot Nothing Then
            m_mouseOverControl.InvokeMouseLeave(New EventArgs)
        End If

        m_mouseOverControl = Nothing
        m_toolTip.Hide(Me)
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseMove(e)
        CheckMousePosition(e)
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseDown(e)

        If m_mouseOverControl IsNot Nothing Then
            m_mouseOverControl.InvokeMouseDown(e)
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseUp(e)

        If MouseOverControl IsNot Nothing Then
            MouseOverControl.InvokeMouseUp(e)
        End If
    End Sub

    Protected Overrides Sub OnMouseLeave(ByVal e As System.EventArgs)
        MyBase.OnMouseLeave(e)

        'Call each tab's MouseLeave method so that it can properly animate a fade out and
        'reset it's current frame to zero.
        For Each tab As MdiTab In Tabs
            tab.InvokeMouseLeave(e)
        Next

        LeftScrollTab.InvokeMouseLeave(e)
        DropDownTab.InvokeMouseLeave(e)
        RightScrollTab.InvokeMouseLeave(e)
        MdiNewTab.InvokeMouseLeave(e)
        MouseOverControl = Nothing

        Invalidate()
    End Sub

    Friend Sub UpdateToolTip(ByVal tipText As String)
        m_toolTip.Hide(Me)
        m_toolTip.Active = False
        m_toolTip.Active = True
        Dim location As Point = Cursor.Position
        location.Y = (location.Y + (Cursor.Size.Height - Cursor.Current.HotSpot.Y))
        m_toolTip.Show(tipText, Me, MyBase.PointToClient(location), m_toolTip.AutoPopDelay)
    End Sub

#End Region

#Region "Drag Drop Methods"
    Protected Overrides Sub OnDragOver(ByVal drgevent As System.Windows.Forms.DragEventArgs)
        If Not drgevent.Data.GetDataPresent(GetType(MdiTab)) Then
            drgevent.Effect = DragDropEffects.None
            Return
        End If

        IsDragging = True
        drgevent.Effect = DragDropEffects.Move

        Dim pt As Point = PointToClient(New Point(drgevent.X, drgevent.Y))
        DragDropHitTest(pt.X, pt.Y)
        Invalidate()
    End Sub

    Private Sub DragDropHitTest(ByVal mouseX As Integer, ByVal mouseY As Integer)
        Dim tab As MdiTab = Nothing

        For Each tab In Tabs
            If tab.CanDrag AndAlso tab.Visible Then
                'Only test mouse position if the tab is visible and can be dragged (which signifies
                'whether or not the tab can be reordered)
                If tab.HitTest(mouseX, mouseY) Then
                    Dim activeIndex As Integer = Tabs.IndexOf(ActiveTab)
                    If tab Is Nothing Then
                        'This should never happen but check just in case.
                        m_indexOfTabForDrop = activeIndex
                    ElseIf tab Is ActiveTab Then
                        'When starting a drag operation this should be the first test hit. We set the index
                        'to the active tab and setup the direction so that the indicator is displayed one
                        'the correct side of the tab.
                        m_indexOfTabForDrop = activeIndex

                        If Me.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                            m_dragDirection = ScrollDirection.Right
                        Else
                            m_dragDirection = ScrollDirection.Left
                        End If
                    Else
                        'The code below determines the index at which the tab being currently dragged
                        'should be dropped at based on the direction the tab is being dragged (determined
                        'by the active tab's current index) as well as splitting the tabs 80/20.
                        'It is easier to understand seeing it in action than it is to explain it.
                        Dim currentIndex As Integer = Tabs.IndexOf(tab)

                        If Me.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                            If currentIndex <= activeIndex Then
                                Dim a As Integer = tab.Location.X + (tab.Width * 0.2)

                                m_dragDirection = ScrollDirection.Right

                                If mouseX < a Then
                                    If currentIndex + 1 < Tabs.Count Then
                                        m_indexOfTabForDrop = currentIndex + 1
                                    End If
                                Else
                                    m_indexOfTabForDrop = currentIndex
                                End If
                            Else
                                Dim b As Integer = tab.Location.X + (tab.Width * 0.8)

                                m_dragDirection = ScrollDirection.Left

                                If mouseX < b Then
                                    If currentIndex < Tabs.Count Then
                                        m_indexOfTabForDrop = currentIndex
                                    End If
                                Else
                                    If activeIndex + 1 <> currentIndex Then
                                        m_indexOfTabForDrop = currentIndex - 1
                                    Else
                                        m_indexOfTabForDrop = activeIndex
                                        m_dragDirection = ScrollDirection.Right
                                    End If
                                End If
                            End If
                        Else
                            If currentIndex <= activeIndex Then
                                Dim a As Integer = tab.Location.X + (tab.Width * 0.8)

                                m_dragDirection = ScrollDirection.Left

                                If mouseX < a Then
                                    m_indexOfTabForDrop = currentIndex
                                Else
                                    If currentIndex + 1 < Tabs.Count Then
                                        m_indexOfTabForDrop = currentIndex + 1
                                    End If
                                End If
                            Else
                                Dim b As Integer = tab.Location.X + (tab.Width * 0.2)

                                m_dragDirection = ScrollDirection.Right

                                If mouseX < b Then
                                    If activeIndex + 1 <> currentIndex Then
                                        m_indexOfTabForDrop = currentIndex - 1
                                    Else
                                        m_indexOfTabForDrop = activeIndex
                                        m_dragDirection = ScrollDirection.Left
                                    End If
                                Else
                                    If currentIndex < Tabs.Count Then
                                        m_indexOfTabForDrop = currentIndex
                                    End If
                                End If
                            End If
                        End If
                    End If

                    Exit For
                End If ' tab.HitTest
            End If 'tab.Visible
        Next 'tab
    End Sub

    Protected Overrides Sub OnDragDrop(ByVal drgevent As System.Windows.Forms.DragEventArgs)
        If drgevent.Data.GetDataPresent(GetType(MdiTab)) Then
            Dim tab As MdiTab = CType(drgevent.Data.GetData(GetType(MdiTab)), MdiTab)

            If drgevent.Effect = DragDropEffects.Move Then
                'When the tab is dropped it is removed from the collection and then inserted back in at the
                'designated index. The cooresponding menu item for the drop down is also moved to the same position
                'in the menu's item collection.
                If m_tabs.IndexOf(tab) <> m_indexOfTabForDrop Then
                    Tabs.Remove(tab)
                    Tabs.Insert(m_indexOfTabForDrop, tab)
                    OnMdiTabIndexChanged(New EventArgs)
                    PerformLayout()

                    Dim f As Form = tab.Form
                    For Each mi As MdiMenuItem In DropDownTab.m_mdiMenu.Items
                        If mi.Form Is f Then
                            DropDownTab.m_mdiMenu.Items.Remove(mi)
                            DropDownTab.m_mdiMenu.Items.Insert(m_indexOfTabForDrop, mi)
                            Exit For
                        End If
                    Next

                    'After this operation need to determine if the left or right scroll tab should be enabled or not.
                    LeftScrollTab.Enabled = Not IsFirstTabActive
                    RightScrollTab.Enabled = Not IsLastTabActive
                End If
            End If
        End If

        IsDragging = False
        Invalidate()
    End Sub
#End Region

#Region "ContextMenu methods"
    Private Sub AddMdiItem(ByVal f As Form, ByVal tab As MdiTab)
        Dim item As New MdiMenuItem(tab, New EventHandler(AddressOf MenuItemClick))
        Dim bmp As New Bitmap(f.Icon.Width, f.Icon.Height, Imaging.PixelFormat.Format32bppArgb)

        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.HighQuality
            g.DrawIcon(f.Icon, 0, 0)
        End Using

        item.Image = bmp
        item.Text = f.Text
        m_dropDownScrollTab.m_mdiMenu.Items.Add(item)
    End Sub

    Private Sub RemoveMdiItem(ByVal f As Form)
        For Each mi As MdiMenuItem In m_dropDownScrollTab.m_mdiMenu.Items
            If mi.Form Is f Then
                m_dropDownScrollTab.m_mdiMenu.Items.Remove(mi)
                Exit For
            End If
        Next
    End Sub

    Private Sub MenuItemClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim mItem As MdiMenuItem = TryCast(sender, MdiMenuItem)
        If mItem IsNot Nothing Then
            Dim direction As ScrollDirection
            Dim activeIndex As Integer = Tabs.IndexOf(ActiveTab)
            Dim clickedTabIndex As Integer

            mItem.Form.Activate()
            clickedTabIndex = Tabs.IndexOf(ActiveTab)

            If activeIndex > clickedTabIndex Then
                direction = ScrollDirection.Left
            Else
                direction = ScrollDirection.Right
            End If

            UpdateTabVisibility(direction)
        End If
    End Sub
#End Region

#Region "Navigation button Events"
    Private Sub leftTabScroll_ScrollTab(ByVal direction As ScrollDirection) Handles m_leftScrollTab.ScrollTab
        ScrollTabHandler(direction)
    End Sub

    Private Sub rightTabScroll_ScrollTab(ByVal direction As ScrollDirection) Handles m_rightScrollTab.ScrollTab
        ScrollTabHandler(direction)
    End Sub

    ''' <summary>
    ''' The scroll handler determines the index of the next tab in the direction the user is
    ''' intending to scroll. It then calls that tab's Form's Activate method.
    ''' </summary>
    ''' <param name="direction"></param>
    ''' <remarks></remarks>
    Private Sub ScrollTabHandler(ByVal direction As ScrollDirection)
        Dim nextIndex As Integer = 0
        If direction = ScrollDirection.Left Then
            nextIndex = Tabs.FirstVisibleTabIndex
            nextIndex -= 1
        Else
            nextIndex = Tabs.LastVisibleTabIndex
            nextIndex += 1
        End If

        If nextIndex > Tabs.Count - 1 Then
            nextIndex = Tabs.Count - 1
        ElseIf nextIndex < 0 Then
            nextIndex = 0
        End If

        Tabs(nextIndex).Form.Activate()
        UpdateTabVisibility(direction)
    End Sub

    Private Sub UpdateTabVisibility(ByVal direction As ScrollDirection)
        Dim tabsToShow As Integer
        Dim leftTabIndex As Integer
        Dim rightTabIndex As Integer
        Dim activeTabIndex As Integer
        Dim tabAreaWidth As Integer = AdjustAvailableWidth

        'tabAreaWidth = Me.DisplayRectangle.Width

        ''Must subtract the area occupied by the visible scroll tabs to get the
        ''true area the form tabs can occupy.
        'If Me.LeftScrollTab.Visible Then
        '    tabAreaWidth -= Me.LeftScrollTab.Width
        'End If

        'If Me.DropDownTab.Visible Then
        '    tabAreaWidth -= Me.DropDownTab.Width
        'End If

        'If Me.RightScrollTab.Visible Then
        '    tabAreaWidth -= Me.RightScrollTab.Width
        'End If

        'If Me.MdiNewTabVisible Then
        '    tabAreaWidth -= Me.m_newTab.Width
        'End If

        'Based on the minimum width each tab can be determine the number of tabs
        'that can be shown in the calculated area.
        tabsToShow = tabAreaWidth \ MinTabWidth
        activeTabIndex = Tabs.IndexOf(ActiveTab)

        If tabsToShow = 1 Then
            'If only one can be visible then set this tab's index as the right and left
            leftTabIndex = activeTabIndex
            rightTabIndex = activeTabIndex
        ElseIf tabsToShow >= Tabs.Count Then
            'If all of the tabs can be visible then set the left index to 0 and 
            'the right to the last tab's index
            leftTabIndex = 0
            rightTabIndex = Tabs.Count - 1
        ElseIf direction = ScrollDirection.Left Then
            'Tries to make the active tab the last tab visible. If this calculation puts the left
            'index past zero (negative) then it resets itself so that it shows the number of tabsToShow
            'starting at index zero.
            leftTabIndex = activeTabIndex - (tabsToShow - 1)

            If leftTabIndex >= 0 Then
                rightTabIndex = activeTabIndex
            Else
                rightTabIndex = activeTabIndex - leftTabIndex
                leftTabIndex = 0
            End If
        ElseIf direction = ScrollDirection.Right Then
            'Tries to make the active tab the first tab visible. If this calculation puts the right
            'index past the number of tabs in the collection then it resets itself so that it shows
            'the number of tabsToShow ending at the last index in the collection.
            rightTabIndex = activeTabIndex + (tabsToShow - 1)

            If rightTabIndex < Tabs.Count Then
                leftTabIndex = activeTabIndex
            Else
                rightTabIndex = Tabs.Count - 1
                leftTabIndex = rightTabIndex - (tabsToShow - 1)
            End If
        Else
            'The resize event is handled by this section of code. It tries to evenly distribute the hiding
            'and showing of tabs between each side of the active tab. If you have 5 tabs open and the third
            'one is active and you resize the window smaller and smaller you will notice that the last tab
            'on the right disappears first. Then as you continue to resize the first tab on the left 
            'disappears, then the last one on the right and then the first one on the left. At this point
            'only one tab is left visible. If you now resize the window larger a tab on the left will become
            'visible and then one on the right, then the left and then the right.
            Dim l As Integer = tabsToShow \ 2
            Dim r As Integer

            If tabsToShow = Tabs.VisibleCount Then Exit Sub

            If tabsToShow < Tabs.VisibleCount Then
                SetScrollTabVisibility()
                AdjustAvailableWidth()
            End If

            If tabsToShow Mod 2 = 0 Then
                r = l - 1
            Else
                r = l
            End If

            If activeTabIndex - Tabs.FirstVisibleTabIndex <= Tabs.LastVisibleTabIndex - activeTabIndex Then
                leftTabIndex = activeTabIndex - l

                If leftTabIndex >= 0 Then
                    rightTabIndex = activeTabIndex + r
                Else
                    rightTabIndex = tabsToShow - 1
                    leftTabIndex = 0
                End If

                If rightTabIndex >= Tabs.Count Then
                    rightTabIndex = Tabs.Count - 1
                    leftTabIndex = rightTabIndex - (tabsToShow - 1)
                End If
            Else
                rightTabIndex = activeTabIndex + r

                If rightTabIndex < Tabs.Count Then
                    leftTabIndex = activeTabIndex - l
                Else
                    rightTabIndex = Tabs.Count - 1
                    leftTabIndex = rightTabIndex - (tabsToShow - 1)
                End If

                If leftTabIndex < 0 Then
                    leftTabIndex = 0
                    rightTabIndex = tabsToShow - 1
                End If
            End If
        End If

        'Using the left and right indeces determined above iterate through the tab collection
        'and hide the tab if is not within the range of indeces and show it if it is.
        For Each tab As MdiTab In Tabs
            Dim tabPos As Integer = Tabs.IndexOf(tab)

            If tabPos <= rightTabIndex And tabPos >= leftTabIndex Then
                tab.Visible = True
            Else
                tab.Visible = False
            End If
        Next

        'The active tab needs to always be visible. This code ensures that even when the main form
        'is resized to a very small width that this tab remains visible and the control draws correctly.
        If ActiveTab IsNot Nothing Then
            ActiveTab.Visible = True
        End If

        'Figure each scroll tab's visiblity and perform a layout to set each tab's size and location.
        SetScrollTabVisibility()
        PerformLayout()
    End Sub

    Private Sub SetScrollTabVisibility()
        'If tabs are hidden then the left and right scroll tabs need to be displayed.
        'If there are more than one tab open then the drop down tab needs to be displayed.
        'DesignMode is checked so that these tabs will be visible in the design window.
        If Not DesignMode Then
            Dim hiddenTabs As Boolean = Tabs.VisibleCount < Tabs.Count
            Dim multipleTabs As Boolean = Tabs.Count > 1

            LeftScrollTab.Visible = hiddenTabs
            RightScrollTab.Visible = hiddenTabs
            DropDownTab.Visible = multipleTabs
        End If
    End Sub

    Private Function AdjustAvailableWidth() As Integer
        Dim w As Integer = DisplayRectangle.Width

        'Must subtract the area occupied by the visible scroll tabs to get the
        'true area the form tabs can occupy.
        If LeftScrollTab.Visible Then
            w -= LeftScrollTab.Width
        End If

        If DropDownTab.Visible Then
            w -= DropDownTab.Width
        End If

        If RightScrollTab.Visible Then
            w -= RightScrollTab.Width
        End If

        If MdiNewTabVisible Then
            w -= m_newTab.Width
        End If

        Return w
    End Function
#End Region

#Region "Resize"
    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)
        UpdateTabVisibility(ScrollDirection.None)
    End Sub
#End Region

#Region "Control Events"
    Protected Friend Overridable Sub OnMdiTabAdded(ByVal e As MdiTabStripTabEventArgs)
        RaiseEvent MdiTabAdded(Me, e)
    End Sub

    Protected Friend Overridable Sub OnMdiTabRemoved(ByVal e As MdiTabStripTabEventArgs)
        RaiseEvent MdiTabRemoved(Me, e)
    End Sub

    Protected Friend Overridable Sub OnMdiTabClicked(ByVal e As MdiTabStripTabClickedEventArgs)
        RaiseEvent MdiTabClicked(Me, e)
    End Sub

    Protected Friend Overridable Sub OnMdiTabIndexChanged(ByVal e As EventArgs)
        RaiseEvent MdiTabIndexChanged(Me, e)
    End Sub

    Protected Friend Overridable Sub OnCurrentMdiTabChanged(ByVal e As EventArgs)
        RaiseEvent CurrentMdiTabChanged(Me, e)
    End Sub

    Protected Friend Sub OnMdiNewTabClick()
        RaiseEvent MdiNewTabClicked(m_newTab, New EventArgs)
    End Sub
#End Region

#End Region

#Region "LayoutEngine Class"
    Private Class TabStripLayoutEngine
        Inherits Layout.LayoutEngine

        Public Overrides Function Layout(ByVal container As Object, ByVal layoutEventArgs As System.Windows.Forms.LayoutEventArgs) As Boolean
            Dim strip As MdiTabStrip = CType(container, MdiTabStrip)
            Dim proposedWidth As Integer = strip.MaxTabWidth
            Dim visibleCount As Integer = strip.Tabs.VisibleCount
            Dim stripRectangle As Rectangle = strip.DisplayRectangle
            Dim tabAreaWidth As Integer = stripRectangle.Width
            Dim nextLocation As Point = stripRectangle.Location
            Dim leftOver As Integer = 0
            Dim visibleIndex As Integer = 0

            'If the MdiTabStrip's DisplayRectangle width is less than 1 or there are no tabs
            'to display then don't try to layout the control.
            If tabAreaWidth < 1 Or visibleCount < 1 Then
                'If the MdiNewTab is visible then we need to layout it's position.
                LayoutMdiNewTab(strip, nextLocation, stripRectangle.Height + strip.Margin.Bottom)
                Return False
            End If

            'For each of the scroll tabs need to determine their location and height (the width
            'is set in the MdiTabStrip constructor and is fixed). The width of the scroll tab
            'also needs to be subtracted from the tabAreaWidth so that the true tab area can be
            'properly calculated.
            If strip.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                nextLocation.X = stripRectangle.Right

                If strip.RightScrollTab.Visible Then
                    nextLocation = MirrorScrollTab(strip.RightScrollTab, nextLocation, stripRectangle.Height)
                    tabAreaWidth -= strip.RightScrollTab.Width
                End If

                If strip.DropDownTab.Visible Then
                    nextLocation = MirrorScrollTab(strip.DropDownTab, nextLocation, stripRectangle.Height)
                    tabAreaWidth -= strip.DropDownTab.Width
                End If

                If strip.LeftScrollTab.Visible Then
                    nextLocation = MirrorScrollTab(strip.LeftScrollTab, nextLocation, stripRectangle.Height)
                    tabAreaWidth -= strip.LeftScrollTab.Width
                End If
            Else
                If strip.LeftScrollTab.Visible Then
                    nextLocation = SetScrollTab(strip.LeftScrollTab, nextLocation, stripRectangle.Height)
                    tabAreaWidth -= strip.LeftScrollTab.Width
                End If

                If strip.DropDownTab.Visible Then
                    nextLocation = SetScrollTab(strip.DropDownTab, nextLocation, stripRectangle.Height)
                    tabAreaWidth -= strip.DropDownTab.Width
                End If

                If strip.RightScrollTab.Visible Then
                    nextLocation = SetScrollTab(strip.RightScrollTab, nextLocation, stripRectangle.Height)
                    tabAreaWidth -= strip.RightScrollTab.Width
                End If
            End If

            If strip.MdiNewTabVisible Then
                tabAreaWidth -= strip.MdiNewTab.Width
            End If

            'If the total width of all visible tabs is greater than the total area available for the
            'tabs then need to set the proposed width of each tab. We also retreive the remainder for use below.
            If visibleCount * proposedWidth > tabAreaWidth Then
                'The \ operator returns an Integer value and disgards the remainder.
                proposedWidth = tabAreaWidth \ visibleCount

                leftOver = tabAreaWidth Mod visibleCount
            End If

            'Set the tabWidth to the larger of the two variables; proposed width and minimum width.
            proposedWidth = Math.Max(proposedWidth, strip.MinTabWidth)

            'Set each visible tab's width and location and perform layout on each tab.
            For Each tab As MdiTab In strip.Tabs
                If tab.Visible Then
                    Dim tabSize As New Size(proposedWidth, stripRectangle.Height)

                    'Suspend the tab's layout so that we can set it's properties without triggering
                    'extraneous layouts. Once all changes are made then we can PerformLayout.
                    tab.SuspendLayout()

                    'To allow the tabs to completely fill the total available width we adjust the width
                    'of the tabs (starting with the first tab) by one. The number of tabs that need to be
                    'adjusted is determined by the leftOver variable that was calculated above.
                    If proposedWidth < strip.MaxTabWidth AndAlso visibleIndex < (leftOver - 1) Then
                        tabSize.Width = proposedWidth + 1
                    End If

                    If strip.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                        nextLocation.X -= tabSize.Width
                        tab.Size = tabSize
                        tab.Location = nextLocation
                    Else
                        tab.Size = tabSize
                        tab.Location = nextLocation
                        nextLocation.X += tabSize.Width
                    End If

                    visibleIndex += 1
                    tab.ResumeLayout()
                    tab.PerformLayout()
                End If
            Next tab

            LayoutMdiNewTab(strip, nextLocation, stripRectangle.Height)

            'Return False because we don't want layout to be performed again by the parent of the container
            Return False
        End Function

        Private Sub LayoutMdiNewTab(ByVal strip As MdiTabStrip, ByVal position As Point, ByVal height As Integer)
            If strip.MdiNewTabVisible Then
                If strip.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                    MirrorNewTab(strip.MdiNewTab, position, height)
                Else
                    SetNewTab(strip.MdiNewTab, position, height)
                End If
            End If
        End Sub

        Private Function SetScrollTab(ByVal tab As MdiScrollTab, ByVal position As Point, ByVal height As Integer) As Point
            If tab.Visible Then
                tab.Location = position
                tab.Height = height
                tab.PerformLayout()
            End If

            Return New Point(position.X + tab.Width, position.Y)
        End Function

        Private Function SetNewTab(ByVal tab As MdiNewTab, ByVal position As Point, ByVal height As Integer) As Point
            tab.Location = position
            tab.Height = height
            tab.PerformLayout()

            Return New Point(position.X + tab.Width, position.Y)
        End Function

        Private Function MirrorScrollTab(ByVal tab As MdiScrollTab, ByVal position As Point, ByVal height As Integer) As Point
            If tab.Visible Then
                tab.Location = New Point(position.X - tab.Width, position.Y)
                tab.Height = height
                tab.PerformLayout()
            End If

            Return tab.Location
        End Function

        Private Function MirrorNewTab(ByVal tab As MdiNewTab, ByVal position As Point, ByVal height As Integer) As Point
            tab.Location = New Point(position.X - tab.Width, position.Y)
            tab.Height = height
            tab.PerformLayout()

            Return tab.Location
        End Function

    End Class
#End Region

End Class

''' <summary>
''' Specifies the direction in which a scroll event initiated.
''' </summary>
Public Enum ScrollDirection
    None = 0
    Left = 1
    Right = 2
End Enum

''' <summary>
''' Specifies the type of tab the <see cref="MdiScrollTab"/> object has been initialized as.
''' </summary>
Public Enum ScrollTabType
    ScrollTabLeft = 1
    ScrollTabRight = 2
    ScrollTabDropDown = 3
End Enum

''' <summary>
''' Specifies the desired permanance for the tabs of a <see cref="MdiTabStrip"/>.
''' </summary>
Public Enum MdiTabPermanence
    None = 0
    First = 1
    LastOpen = 2
End Enum

''' <summary>
''' Specifies whether or not to obey each form's individual property settings or force each to form
''' to always be maximized.
''' </summary>
Public Enum MdiChildWindowState
    Normal = 1
    Maximized = 2
End Enum

Friend Enum AnimationType
    None = 0
    FadeIn = 1
    FadeOut = 2
End Enum
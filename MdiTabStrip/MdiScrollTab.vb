Imports System.Drawing.Drawing2D

<System.ComponentModel.ToolboxItem(False)> _
Public Class MdiScrollTab
    Inherits MdiTab

#Region "Fields"
    Private m_mouseDown As Boolean = False
    Private m_scrollTabType As ScrollTabType = ScrollTabType.ScrollTabLeft
    Friend WithEvents m_mdiMenu As New MdiTabStripDropDown
    Private m_isDroppedDown As Boolean = False
    Private m_dropDownByTab As Boolean = False
#End Region

#Region "Events"
    Friend Event ScrollTab(ByVal direction As ScrollDirection)
#End Region

#Region "Constructor/Destructor"
    Sub New(ByVal owner As MdiTabStrip, ByVal scrollType As ScrollTabType)
        MyBase.New(owner)
        Me.m_scrollTabType = scrollType
        AddHandler owner.RightToLeftChanged, AddressOf Me.OnOwnerRightToLeftChanged
        'We initially hide the scroll tab
        Me.Visible = False
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            Me.m_mdiMenu.Dispose()

            If Me.ParentInternal IsNot Nothing Then
                RemoveHandler Me.ParentInternal.RightToLeftChanged, AddressOf Me.OnOwnerRightToLeftChanged
            End If
        End If

        MyBase.Dispose(disposing)
    End Sub
#End Region

#Region "Properties"
    Friend ReadOnly Property MdiMenu() As MdiTabStripDropDown
        Get
            Return Me.m_mdiMenu
        End Get
    End Property

    Friend ReadOnly Property ScrollTabType() As ScrollTabType
        Get
            Return Me.m_scrollTabType
        End Get
    End Property
#End Region

#Region "Methods"

    Private Sub m_mdiMenu_Closed(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripDropDownClosedEventArgs) Handles m_mdiMenu.Closed
        If Not Me.IsMouseOver Then
            Me.m_isDroppedDown = False
        End If
    End Sub

    Private Sub m_mdiMenu_Opened(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_mdiMenu.Opened
        Me.m_isDroppedDown = True
    End Sub

#Region "Paint methods"
    Friend Overrides Sub DrawControl(ByVal g As System.Drawing.Graphics)
        DrawTab(g)
    End Sub

    Private Sub DrawTab(ByVal g As System.Drawing.Graphics)
        If Me.m_scrollTabType = ScrollTabType.ScrollTabLeft Then
            DrawLeftGlyph(g)
        ElseIf Me.m_scrollTabType = ScrollTabType.ScrollTabRight Then
            DrawRightGlyph(g)
        Else
            DrawDropDownGlyph(g)
        End If
    End Sub

    Private Sub DrawLeftGlyph(ByVal g As Graphics)
        Dim rect As New Rectangle(Me.Left + ((Me.Width / 2) - 6), Me.Top + 13, 11, 5)
        Dim lines1() As Point = {New Point(rect.X + 4, rect.Y), _
                                New Point(rect.X + 5, rect.Y), _
                                New Point(rect.X + 3, rect.Y + 2), _
                                New Point(rect.X + 5, rect.Y + 4), _
                                New Point(rect.X + 4, rect.Y + 4), _
                                New Point(rect.X + 2, rect.Y + 2), _
                                New Point(rect.X + 4, rect.Y)}
        Dim lines2() As Point = {New Point(rect.X + 8, rect.Y), _
                                New Point(rect.X + 9, rect.Y), _
                                New Point(rect.X + 7, rect.Y + 2), _
                                New Point(rect.X + 9, rect.Y + 4), _
                                New Point(rect.X + 8, rect.Y + 4), _
                               New Point(rect.X + 6, rect.Y + 2), _
                               New Point(rect.X + 8, rect.Y)}

        DrawChevron(g, lines1, lines2)
    End Sub

    Private Sub DrawRightGlyph(ByVal g As Graphics)
        Dim rect As New Rectangle(Me.Left + ((Me.Width / 2) - 5), Me.Top + 13, 11, 5)
        Dim lines1() As Point = {New Point(rect.X + 1, rect.Y), _
                                New Point(rect.X + 2, rect.Y), _
                                New Point(rect.X + 4, rect.Y + 2), _
                                New Point(rect.X + 2, rect.Y + 4), _
                                New Point(rect.X + 1, rect.Y + 4), _
                                New Point(rect.X + 3, rect.Y + 2), _
                                New Point(rect.X + 1, rect.Y)}
        Dim lines2() As Point = {New Point(rect.X + 5, rect.Y), _
                                New Point(rect.X + 6, rect.Y), _
                               New Point(rect.X + 8, rect.Y + 2), _
                               New Point(rect.X + 6, rect.Y + 4), _
                               New Point(rect.X + 5, rect.Y + 4), _
                               New Point(rect.X + 7, rect.Y + 2), _
                               New Point(rect.X + 5, rect.Y)}

        DrawChevron(g, lines1, lines2)
    End Sub

    Private Sub DrawChevron(ByVal g As Graphics, ByVal chevron1 As Point(), ByVal chevron2 As Point())
        g.SmoothingMode = SmoothingMode.None

        Using glyphPen As New Pen(Me.TabForeColor, 1)
            If Not Me.Enabled Then
                Dim c As Color = Me.ParentInternal.InactiveTabForeColor
                Dim luminosity As Integer
                Dim num1 As Integer = c.R
                Dim num2 As Integer = c.G
                Dim num3 As Integer = c.B
                Dim num4 As Integer = Math.Max(Math.Max(num1, num2), num3)
                Dim num5 As Integer = Math.Min(Math.Min(num1, num2), num3)
                Dim num6 As Integer = (num4 + num5)

                luminosity = (((num6 * 240) + 255) / 510)

                If luminosity = 0 Then
                    glyphPen.Color = ControlPaint.LightLight(c)

                ElseIf luminosity < 120 Then
                    glyphPen.Color = ControlPaint.Light(c, 0.5!)
                Else
                    glyphPen.Color = ControlPaint.Light(c, 0.5!)
                End If
            End If

            glyphPen.StartCap = LineCap.Square
            glyphPen.EndCap = LineCap.Square
            g.DrawLines(glyphPen, chevron1)
            g.DrawLines(glyphPen, chevron2)
        End Using

        g.SmoothingMode = SmoothingMode.AntiAlias
    End Sub

    Private Sub DrawDropDownGlyph(ByVal g As Graphics)
        Dim rect As New Rectangle(Me.Left + ((Me.Width / 2) - 3), Me.Top + 12, 4, 6)
        Dim dropDown() As Point = {New Point(rect.X, rect.Y + 1), New Point(rect.X + 3, rect.Y + 5), New Point(rect.X + 6, rect.Y + 1)}

        Using glyphBrush As New SolidBrush(Me.TabForeColor)
            g.FillPolygon(glyphBrush, dropDown)
        End Using
    End Sub
#End Region

#Region "Mouse methods"
    Protected Overrides Sub OnMouseLeave(ByVal e As System.EventArgs)
        MyBase.OnMouseLeave(e)

        Me.m_isDroppedDown = False
    End Sub
    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        Me.m_mouseDown = True
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        If Not Me.Enabled Then
            Exit Sub
        End If

        If Me.m_mouseDown Then
            Me.m_mouseDown = False

            If Me.m_scrollTabType = ScrollTabType.ScrollTabLeft Then
                Dim direction As ScrollDirection = ScrollDirection.Left

                If Me.ParentInternal.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                    direction = ScrollDirection.Right
                End If

                RaiseEvent ScrollTab(direction)
            ElseIf Me.m_scrollTabType = ScrollTabType.ScrollTabRight Then
                Dim direction As ScrollDirection = ScrollDirection.Right

                If Me.ParentInternal.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                    direction = ScrollDirection.Left
                End If

                RaiseEvent ScrollTab(direction)
            Else
                If Me.m_isDroppedDown Then
                    Me.m_isDroppedDown = False
                Else
                    Dim dropPoint As Point

                    If Me.ParentInternal.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                        dropPoint = Me.ParentInternal.PointToScreen(New Point(Me.Right - Me.m_mdiMenu.Width, Me.ParentInternal.Height - 5))
                    Else
                        dropPoint = Me.ParentInternal.PointToScreen(New Point(Me.Left, Me.ParentInternal.Height - 5))
                    End If

                    Me.m_mdiMenu.Show(dropPoint, ToolStripDropDownDirection.Default)
                End If
            End If
        End If
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        'Not implemeted, but overridden to bypass inherited functionality
    End Sub
#End Region

#End Region

#Region "Event Handlers"
    Private Sub OnOwnerRightToLeftChanged(ByVal sender As Object, ByVal e As EventArgs)
        'Need to know when the MdiTabStrip RightToLeft property has changed so that the drop down menu's
        'RightToLeft property is set to match it.
        Me.m_mdiMenu.RightToLeft = Me.ParentInternal.RightToLeft
    End Sub
#End Region

End Class

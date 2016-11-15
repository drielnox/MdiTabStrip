Imports System.ComponentModel
Imports System.Drawing.Drawing2D

''' <summary>
''' Represents a selectable tab that corresponds to exactly one open <see cref="Form"/> 
''' whose <see cref="Form.MdiParent"/> property has been 
''' set to an instance of another form in an MDI application.
''' </summary>
<ToolboxItem(False)> _
Public Class MdiTab
    Inherits MdiTabStripItemBase

#Region "Fields"
    Private m_owner As MdiTabStrip
    Private m_form As Form
    Private m_isMouseOver As Boolean = False
    Private m_isMouseOverCloseButton As Boolean = False
    Private m_isSwitching As Boolean = False
    Private m_dragBox As Rectangle = Rectangle.Empty
    Private m_activeBounds As Point()
    Private m_activeInnerBounds As Point()
    Private m_inactiveBounds As Point()
    Private m_inactiveInnerBounds As Point()
    Private m_closeButtonBounds As Point()
    Private m_closeButtonGlyphBounds As Point()
    Private m_dragCursor As Cursor = Nothing
    Private m_isAnimating As Boolean = False
    Private m_animationType As AnimationType
    Private m_currentFrame As Integer = 0
#End Region

#Region "Constructor/Destructor"
    ''' <summary>
    ''' Initializes a new instance of the <see cref="MdiTab"/> class.
    ''' </summary>
    Public Sub New(ByVal owner As MdiTabStrip)
        Me.ParentInternal = owner
    End Sub

    ''' <summary>
    ''' Releases the unmanaged resources used by the <see cref="MdiTab"/> and optionally releases the managed resources. 
    ''' </summary>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Me.m_dragCursor IsNot Nothing Then
                Me.m_dragCursor.Dispose()
            End If
        End If

        MyBase.Dispose(disposing)
    End Sub
#End Region

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the instance of a <see cref="Form"/> the <see cref="MdiTab"/> represents.
    ''' </summary>
    ''' <returns>The <see cref="Form"/> object the tab represents.</returns>
    Public Property Form() As Form
        Get
            Return Me.m_form
        End Get
        Set(ByVal value As Form)
            Me.m_form = value
        End Set
    End Property

    Friend Property ParentInternal() As MdiTabStrip
        Get
            Return Me.m_owner
        End Get
        Set(ByVal value As MdiTabStrip)
            Me.m_owner = value
        End Set
    End Property

    Friend Property IsMouseOver() As Boolean
        Get
            Return Me.m_isMouseOver
        End Get
        Set(ByVal value As Boolean)
            Me.m_isMouseOver = value
        End Set
    End Property

    Friend ReadOnly Property IsActive() As Boolean
        Get
            Return Me.ParentInternal.ActiveTab Is Me
        End Get
    End Property

    Private Property IsAnimating() As Boolean
        Get
            Return Me.m_isAnimating
        End Get
        Set(ByVal value As Boolean)
            Me.m_isAnimating = value

            If value Then
                Me.ParentInternal.AddAnimatingTab(Me)
            Else
                Me.ParentInternal.RemoveAnimatingTab(Me)
                Me.m_animationType = AnimationType.None
            End If
        End Set
    End Property

    Friend Property CurrentFrame() As Integer
        Get
            Return Me.m_currentFrame
        End Get
        Set(ByVal value As Integer)
            Me.m_currentFrame = value
        End Set
    End Property

    Friend ReadOnly Property AnimationType() As AnimationType
        Get
            Return Me.m_animationType
        End Get
    End Property

    ''' <summary>
    ''' Gets the rectangle that represents the display area of the control.
    ''' </summary>
    ''' <returns>A <see cref="Rectangle"/> that represents the display area of the control.</returns>
    Public Overrides ReadOnly Property DisplayRectangle() As System.Drawing.Rectangle
        Get
            Dim rect As Rectangle = MyBase.DisplayRectangle
            rect.Offset(Me.Location.X, Me.Location.Y)
            Return rect
        End Get
    End Property

    Friend Property IsMouseOverCloseButton() As Boolean
        Get
            Return Me.m_isMouseOverCloseButton
        End Get
        Set(ByVal value As Boolean)
            If Me.m_isMouseOverCloseButton <> value Then
                Dim txt As String = Me.Form.Text

                Me.m_isMouseOverCloseButton = value

                If value Then
                    txt = "Close Tab"
                End If

                If Me.ParentInternal.ShowTabToolTip Then
                    Me.ParentInternal.UpdateToolTip(txt)
                End If

                Me.ParentInternal.Invalidate()
            End If
        End Set
    End Property

    Friend ReadOnly Property CanDrag() As Boolean
        Get
            If Me.ParentInternal.Tabs.Count = 1 Then
                Return False
            End If

            Return Not (Me.ParentInternal.TabPermanence = MdiTabPermanence.First And (Me.ParentInternal.Tabs.IndexOf(Me) = 0))
        End Get
    End Property

    Friend ReadOnly Property CanClose() As Boolean
        Get
            If Me.ParentInternal.TabPermanence = MdiTabPermanence.First AndAlso (Me.ParentInternal.Tabs.IndexOf(Me) = 0) Then
                Return False
            ElseIf Me.ParentInternal.TabPermanence = MdiTabPermanence.LastOpen AndAlso Me.ParentInternal.Tabs.Count = 1 Then
                Return False
            End If

            Return True
        End Get
    End Property

    Private ReadOnly Property CanAnimate() As Boolean
        Get
            Return Me.ParentInternal.Animate
        End Get
    End Property

    Private ReadOnly Property TabBackColor() As Color
        Get
            Dim tabcolor As Color = Me.ParentInternal.InactiveTabColor

            If Me.IsActive Then
                tabcolor = Me.ParentInternal.ActiveTabColor
            ElseIf Not Me.Enabled Then
                tabcolor = Me.ParentInternal.InactiveTabColor
            ElseIf Me.IsAnimating Then
                tabcolor = Me.ParentInternal.BackColorFadeSteps(Me.m_currentFrame)
            ElseIf Me.IsMouseOver Then
                tabcolor = Me.ParentInternal.MouseOverTabColor
            End If

            Return tabcolor
        End Get
    End Property

    Protected ReadOnly Property TabForeColor() As Color
        Get
            Dim foreColor As Color = Me.ParentInternal.InactiveTabForeColor

            If Me.IsActive Then
                foreColor = Me.ParentInternal.ActiveTabForeColor
            ElseIf Me.IsAnimating Then
                foreColor = Me.ParentInternal.ForeColorFadeSteps(Me.m_currentFrame)
            ElseIf Me.IsMouseOver Then
                foreColor = Me.ParentInternal.MouseOverTabForeColor
            End If

            Return foreColor
        End Get
    End Property

    Private ReadOnly Property TabFont() As Font
        Get
            'We default to the font for the inactive tab because it is more ofter used. If animating we switch
            'to the font for the moused over tab when the current frame is in the latter half of the animation.
            Dim font As Font = Me.ParentInternal.InactiveTabFont

            If Me.IsActive Then
                font = Me.ParentInternal.ActiveTabFont
            ElseIf Me.IsAnimating Then
                If Me.CurrentFrame > (Me.ParentInternal.Duration / 2) Then
                    font = Me.ParentInternal.MouseOverTabFont
                End If
            ElseIf Me.IsMouseOver Then
                font = Me.ParentInternal.MouseOverTabFont
            End If

            Return font
        End Get
    End Property


#End Region

#Region "Methods"

#Region "Hit Testing"
    Friend Function HitTest(ByVal x As Integer, ByVal y As Integer) As Boolean
        Dim hit As Boolean = False

        Using gp As New GraphicsPath
            gp.StartFigure()
            gp.AddLines(m_inactiveBounds)
            gp.CloseFigure()

            Using borderpen As New Pen(Color.Black, 1)
                If gp.IsOutlineVisible(x, y, borderpen) OrElse gp.IsVisible(x, y) Then
                    hit = True
                End If
            End Using
        End Using

        Return hit
    End Function

    Private Function closeButtonHitTest(ByVal x As Integer, ByVal y As Integer) As Boolean
        Dim hit As Boolean = False

        Using gp As New GraphicsPath
            gp.StartFigure()
            gp.AddLines(m_closeButtonBounds)
            gp.CloseFigure()

            Using borderpen As New Pen(Color.Black, 1)
                If gp.IsOutlineVisible(x, y, borderpen) OrElse gp.IsVisible(x, y) Then
                    hit = True
                End If
            End Using
        End Using

        Return hit
    End Function
#End Region

#Region "Paint Background"
    Friend Sub DrawControlBackground(ByVal g As Graphics)
        If Me.IsActive Then
            DrawActiveTabBackground(g)
        Else
            DrawInactiveTabBackground(g)
        End If
    End Sub

    Private Sub DrawActiveTabBackground(ByVal g As Graphics)
        'The shadowRectangle fills the divider that is a part of the active tab and spans the width
        'of the parent MdiTabStrip.
        Dim shadowRectangle As New Rectangle(Me.ParentInternal.ClientRectangle.X, Me.DisplayRectangle.Bottom, Me.ParentInternal.ClientRectangle.Width, Me.ParentInternal.Padding.Bottom)
        Dim shadowBlend As New Blend

        g.SmoothingMode = SmoothingMode.None
        shadowBlend.Factors = New Single() {0.0F, 0.1F, 0.3F, 0.4F}
        shadowBlend.Positions = New Single() {0.0F, 0.5F, 0.8F, 1.0F}

        Using outerPath As New GraphicsPath
            outerPath.AddLines(Me.m_activeBounds)

            Using gradientBrush As LinearGradientBrush = Me.GetGradientBackBrush
                g.FillPath(gradientBrush, outerPath)
            End Using

            Using shadowBrush As New LinearGradientBrush(shadowRectangle, Me.TabBackColor, Color.Black, LinearGradientMode.Vertical)
                shadowBrush.Blend = shadowBlend
                g.FillRectangle(shadowBrush, shadowRectangle)
            End Using

            g.SmoothingMode = SmoothingMode.AntiAlias
            g.DrawPath(New Pen(Me.ParentInternal.ActiveTabBorderColor), outerPath)
        End Using

        'Draw the inner border
        Using innerPath As New GraphicsPath
            innerPath.AddLines(Me.m_activeInnerBounds)

            Dim lineColor As Color = Color.FromArgb(120, 255, 255, 255)
            g.DrawPath(New Pen(lineColor), innerPath)
        End Using

        If Me.CanClose Then
            Me.DrawCloseButton(g)
        End If
    End Sub

    Private Sub DrawInactiveTabBackground(ByVal g As Graphics)
        g.SmoothingMode = SmoothingMode.None

        Using outerPath As New GraphicsPath
            outerPath.AddLines(Me.m_inactiveBounds)

            Using gradientBrush As LinearGradientBrush = Me.GetGradientBackBrush()
                g.FillPath(gradientBrush, outerPath)
            End Using

            g.SmoothingMode = SmoothingMode.AntiAlias
            g.DrawPath(New Pen(Me.ParentInternal.InactiveTabBorderColor), outerPath)
        End Using

        'Draw the inner border
        Using innerPath As New GraphicsPath
            innerPath.AddLines(Me.m_inactiveInnerBounds)

            Dim lineColor As Color = Color.FromArgb(120, 255, 255, 255)
            g.DrawPath(New Pen(lineColor), innerPath)
        End Using
    End Sub

    Protected Function GetGradientBackBrush() As LinearGradientBrush
        Dim b As New LinearGradientBrush(Me.DisplayRectangle, Drawing.Color.White, Me.TabBackColor, LinearGradientMode.Vertical)
        Dim bl As New Blend

        If Me.IsActive Then
            bl.Factors = New Single() {0.3F, 0.4F, 0.5F, 1.0F, 1.0F}
            bl.Positions = New Single() {0.0F, 0.2F, 0.35F, 0.35F, 1.0F}
        Else
            bl.Factors = New Single() {0.3F, 0.4F, 0.5F, 1.0F, 0.8F, 0.7F}
            bl.Positions = New Single() {0.0F, 0.2F, 0.4F, 0.4F, 0.8F, 1.0F}
        End If

        b.Blend = bl

        Return b
    End Function
#End Region

#Region "Paint"
    Friend Overridable Sub DrawControl(ByVal g As Graphics)
        If Me.IsActive Then
            DrawActiveTab(g)
        Else
            DrawInactiveTab(g)
        End If
    End Sub

    Private Sub DrawActiveTab(ByVal g As Graphics)
        'The proposedSize variable determines the size available to draw the text of the tab.
        Dim proposedSize As New Size(Me.Width - 5, Me.Height)

        If Me.CanClose Then
            'If the tab can close then subtract the button's width
            proposedSize.Width -= 22
            Me.DrawCloseButton(g)
        End If

        If Me.ParentInternal.DisplayFormIcon Then
            'If the tab will display an icon the subtract the icon's width
            proposedSize.Width -= 22
            Me.DrawFormIcon(g)
        End If

        Me.DrawTabText(g, proposedSize)
    End Sub

    Private Sub DrawFormIcon(ByVal g As Graphics)
        Dim iconRectangle As Rectangle

        If Me.ParentInternal.RightToLeft = Windows.Forms.RightToLeft.Yes Then
            iconRectangle = New Rectangle(Me.Right - 20, Me.Top + 5, 17, 17)
        Else
            iconRectangle = New Rectangle(Me.Left + 5, Me.Top + 5, 17, 17)
        End If

        If Not Me.IsActive Then
            iconRectangle.Offset(0, 2)
        End If

        Using bmp As New Bitmap(Me.Form.Icon.Width, Me.Form.Icon.Height, Imaging.PixelFormat.Format32bppArgb)
            Using bg As Graphics = Graphics.FromImage(bmp)
                bg.DrawIcon(Me.Form.Icon, 0, 0)
            End Using

            g.DrawImage(bmp, iconRectangle)
        End Using
    End Sub

    Private Sub DrawTabText(ByVal g As Graphics, ByVal proposedSize As Size)
        Dim s As Size
        Dim textRectangle As Rectangle
        Dim textFlags As TextFormatFlags = TextFormatFlags.WordEllipsis Or TextFormatFlags.EndEllipsis
        Dim isRightToLeft As Boolean = Me.ParentInternal.RightToLeft = Windows.Forms.RightToLeft.Yes

        If isRightToLeft Then
            textFlags = textFlags Or TextFormatFlags.Right
        End If

        s = TextRenderer.MeasureText(g, Me.Form.Text, Me.TabFont, proposedSize, textFlags)
        textRectangle = New Rectangle(Me.Left + 5, Me.Top + 8, proposedSize.Width, s.Height)

        If isRightToLeft Then
            If Me.IsActive AndAlso Me.CanClose Then
                textRectangle.Offset(22, 0)
            End If
        Else
            If Me.ParentInternal.DisplayFormIcon Then
                textRectangle.Offset(17, 0)
            End If
        End If

        If Not Me.IsActive Then
            textRectangle.Offset(0, 2)
        End If

        TextRenderer.DrawText(g, Me.Form.Text, Me.TabFont, textRectangle, Me.TabForeColor, textFlags)
    End Sub

    Private Sub DrawCloseButton(ByVal g As Graphics)
        If Me.IsMouseOverCloseButton Then
            Me.DrawActiveCloseButton(g)
        Else
            Me.DrawInactiveCloseButton(g)
        End If
    End Sub

    Private Sub DrawActiveCloseButton(ByVal g As Graphics)
        Using gp As New GraphicsPath
            gp.AddLines(Me.m_closeButtonBounds)

            Using backBrush As New SolidBrush(Me.ParentInternal.CloseButtonBackColor)
                g.FillPath(backBrush, gp)
            End Using

            Using borderPen As New Pen(Me.ParentInternal.CloseButtonBorderColor)
                g.DrawPath(borderPen, gp)
            End Using
        End Using

        Me.DrawCloseButtonGlyph(g, Me.ParentInternal.CloseButtonHotForeColor)
    End Sub

    Private Sub DrawInactiveCloseButton(ByVal g As Graphics)
        Me.DrawCloseButtonGlyph(g, Me.ParentInternal.CloseButtonForeColor)
    End Sub

    Private Sub DrawCloseButtonGlyph(ByVal g As Graphics, ByVal glyphColor As Color)
        g.SmoothingMode = SmoothingMode.None

        Using shadow As New GraphicsPath
            Dim translateMatrix As New Matrix
            Dim shadowColor As Color = Color.FromArgb(30, 0, 0, 0)

            shadow.AddLines(Me.m_closeButtonGlyphBounds)
            translateMatrix.Translate(1, 1)
            shadow.Transform(translateMatrix)

            Using shadowBrush As New SolidBrush(shadowColor)
                g.FillPath(shadowBrush, shadow)
            End Using
            Using shadowPen As New Pen(shadowColor)
                g.DrawPath(shadowPen, shadow)
            End Using
        End Using

        Using gp As New GraphicsPath
            gp.AddLines(Me.m_closeButtonGlyphBounds)

            Using glyphBrush As New SolidBrush(glyphColor)
                g.FillPath(glyphBrush, gp)
            End Using
            Using glyphPen As New Pen(glyphColor)
                g.DrawPath(glyphPen, gp)
            End Using
        End Using

        g.SmoothingMode = SmoothingMode.AntiAlias
    End Sub

    Private Sub DrawInactiveTab(ByVal g As Graphics)
        'The proposedSize variable determines the size available to draw the text of the tab.
        Dim proposedSize As New Size(Me.Width - 5, Me.Height)

        If Me.ParentInternal.DisplayFormIcon Then
            'If the tab will display an icon the subtract the icon's width
            proposedSize.Width -= 22
            Me.DrawFormIcon(g)
        End If

        Me.DrawTabText(g, proposedSize)
    End Sub
#End Region

#Region "Fade Animation"
    Friend Sub StartAnimation(ByVal animation As AnimationType)
        'When the cursor is moved over the control very quick it causes some odd behavior with the animation
        'These two checks are done to make sure that the tab isn't needlessly added to the animation arraylist.
        If animation = AnimationType.FadeIn AndAlso Me.CurrentFrame = Me.ParentInternal.Duration - 1 Then
            Exit Sub
        End If

        If animation = AnimationType.FadeOut AndAlso Me.CurrentFrame = 0 Then
            Exit Sub
        End If

        Me.m_animationType = animation
        If (Not Me.ParentInternal Is Nothing) Then
            Me.IsAnimating = True
        End If
    End Sub

    Friend Sub OnAnimationTick(ByVal newFrame As Integer)
        Me.m_currentFrame = newFrame
        Me.ParentInternal.Invalidate(Me.DisplayRectangle, False)
    End Sub

    Friend Sub StopAnimation()
        Me.IsAnimating = False
        Me.ParentInternal.Invalidate(Me.DisplayRectangle, False)
    End Sub
#End Region

#Region "CustomCursor"
    Private Declare Unicode Function LoadCursorFromFile Lib "User32.dll" Alias "LoadCursorFromFileW" (ByVal filename As String) As IntPtr

    Private Function GetCustomCursor(ByVal fileName As String) As Cursor
        Dim hCursor As IntPtr
        Dim result As Cursor = Nothing

        Try
            hCursor = LoadCursorFromFile(fileName)
            If Not IntPtr.Zero.Equals(hCursor) Then
                result = New Cursor(hCursor)
            End If
        Catch ex As Exception
            'Catch but don't process the exception. If this method returns nothing then
            'the default Windows drag cursor will be used.
            Return Nothing
        End Try
        Return result
    End Function
#End Region

#End Region

#Region "Events"

#Region "Layout"
    Protected Overrides Sub OnLayout(ByVal levent As System.Windows.Forms.LayoutEventArgs)
        Me.m_activeBounds = New Point() {New Point(-2, Me.ParentInternal.Bottom), _
                    New Point(-2, Me.Bottom), _
                    New Point(Me.Left - 3, Me.Bottom), _
                    New Point(Me.Left, Me.Bottom - 1), _
                    New Point(Me.Left, Me.Top + 3), _
                    New Point(Me.Left + 2, Me.Top), _
                    New Point(Me.Right - 2, Me.Top), _
                    New Point(Me.Right, Me.Top + 3), _
                    New Point(Me.Right, Me.Bottom - 1), _
                    New Point(Me.Right + 2, Me.Bottom), _
                    New Point(Me.ParentInternal.Width, Me.Bottom), _
                    New Point(Me.ParentInternal.Width, Me.ParentInternal.Bottom)}
        Me.m_activeInnerBounds = New Point() {New Point(-1, Me.ParentInternal.Bottom), _
                    New Point(-1, Me.Bottom + 1), _
                    New Point(Me.Left - 4, Me.Bottom + 1), _
                    New Point(Me.Left + 1, Me.Bottom), _
                    New Point(Me.Left + 1, Me.Top + 4), _
                    New Point(Me.Left + 3, Me.Top + 1), _
                    New Point(Me.Right - 3, Me.Top + 1), _
                    New Point(Me.Right - 1, Me.Top + 4), _
                    New Point(Me.Right - 1, Me.Bottom), _
                    New Point(Me.Right + 3, Me.Bottom + 1), _
                    New Point(Me.ParentInternal.Width - 1, Me.Bottom + 1), _
                    New Point(Me.ParentInternal.Width - 1, Me.ParentInternal.Bottom)}
        Me.m_inactiveBounds = New Point() {New Point(Me.Left, Me.Bottom), _
                    New Point(Me.Left, Me.Top + 5), _
                    New Point(Me.Left + 2, Me.Top + 2), _
                    New Point(Me.Right - 2, Me.Top + 2), _
                    New Point(Me.Right, Me.Top + 5), _
                    New Point(Me.Right, Me.Bottom)}
        Me.m_inactiveInnerBounds = New Point() {New Point(Me.Left + 1, Me.Bottom), _
                    New Point(Me.Left + 1, Me.Top + 6), _
                    New Point(Me.Left + 3, Me.Top + 3), _
                    New Point(Me.Right - 3, Me.Top + 3), _
                    New Point(Me.Right - 1, Me.Top + 6), _
                    New Point(Me.Right - 1, Me.Bottom)}

        If Me.ParentInternal.RightToLeft = Windows.Forms.RightToLeft.Yes Then
            Me.m_closeButtonBounds = New Point() {New Point(Me.Left + 18, Me.Top + 7), _
                        New Point(Me.Left + 6, Me.Top + 7), _
                        New Point(Me.Left + 4, Me.Top + 9), _
                        New Point(Me.Left + 4, Me.Top + 20), _
                        New Point(Me.Left + 6, Me.Top + 22), _
                        New Point(Me.Left + 18, Me.Top + 22), _
                        New Point(Me.Left + 20, Me.Top + 20), _
                        New Point(Me.Left + 20, Me.Top + 9), _
                        New Point(Me.Left + 18, Me.Top + 7)}
            Dim startPoint As New Point(Me.Left + 8, Me.Top + 11)
            Me.m_closeButtonGlyphBounds = New Point() {New Point(startPoint.X, startPoint.Y), _
                        New Point(startPoint.X + 2, startPoint.Y), _
                        New Point(startPoint.X + 4, startPoint.Y + 2), _
                        New Point(startPoint.X + 6, startPoint.Y), _
                        New Point(startPoint.X + 8, startPoint.Y), _
                        New Point(startPoint.X + 5, startPoint.Y + 3), _
                        New Point(startPoint.X + 5, startPoint.Y + 4), _
                        New Point(startPoint.X + 8, startPoint.Y + 7), _
                        New Point(startPoint.X + 6, startPoint.Y + 7), _
                        New Point(startPoint.X + 4, startPoint.Y + 5), _
                        New Point(startPoint.X + 2, startPoint.Y + 7), _
                        New Point(startPoint.X, startPoint.Y + 7), _
                        New Point(startPoint.X + 3, startPoint.Y + 4), _
                        New Point(startPoint.X + 3, startPoint.Y + 3), _
                        New Point(startPoint.X, startPoint.Y)}
        Else
            Me.m_closeButtonBounds = New Point() {New Point(Me.Right - 18, Me.Top + 7), _
                        New Point(Me.Right - 6, Me.Top + 7), _
                        New Point(Me.Right - 4, Me.Top + 9), _
                        New Point(Me.Right - 4, Me.Top + 20), _
                        New Point(Me.Right - 6, Me.Top + 22), _
                        New Point(Me.Right - 18, Me.Top + 22), _
                        New Point(Me.Right - 20, Me.Top + 20), _
                        New Point(Me.Right - 20, Me.Top + 9), _
                        New Point(Me.Right - 18, Me.Top + 7)}
            Dim startPoint As New Point(Me.Right - 16, Me.Top + 11)
            Me.m_closeButtonGlyphBounds = New Point() {New Point(startPoint.X, startPoint.Y), _
                        New Point(startPoint.X + 2, startPoint.Y), _
                        New Point(startPoint.X + 4, startPoint.Y + 2), _
                        New Point(startPoint.X + 6, startPoint.Y), _
                        New Point(startPoint.X + 8, startPoint.Y), _
                        New Point(startPoint.X + 5, startPoint.Y + 3), _
                        New Point(startPoint.X + 5, startPoint.Y + 4), _
                        New Point(startPoint.X + 8, startPoint.Y + 7), _
                        New Point(startPoint.X + 6, startPoint.Y + 7), _
                        New Point(startPoint.X + 4, startPoint.Y + 5), _
                        New Point(startPoint.X + 2, startPoint.Y + 7), _
                        New Point(startPoint.X, startPoint.Y + 7), _
                        New Point(startPoint.X + 3, startPoint.Y + 4), _
                        New Point(startPoint.X + 3, startPoint.Y + 3), _
                        New Point(startPoint.X, startPoint.Y)}
        End If
    End Sub
#End Region

#Region "DragDrop Events"
    Protected Overrides Sub OnGiveFeedback(ByVal gfbevent As System.Windows.Forms.GiveFeedbackEventArgs)
        gfbevent.UseDefaultCursors = Me.m_dragCursor Is Nothing

        If gfbevent.Effect And DragDropEffects.Move = DragDropEffects.Move Then
            Windows.Forms.Cursor.Current = Me.m_dragCursor
        Else
            Windows.Forms.Cursor.Current = Cursors.No
        End If
    End Sub
#End Region

#Region "Mouse Events"
    Protected Overrides Sub OnMouseEnter(ByVal e As System.EventArgs)
        Me.IsMouseOver = True

        If Me.CanAnimate AndAlso Not Me.IsActive Then
            Me.StartAnimation(AnimationType.FadeIn)
        End If

        If Me.Form IsNot Nothing Then
            Me.ParentInternal.UpdateToolTip(Me.Form.Text)
        End If
    End Sub

    Protected Overrides Sub OnMouseLeave(ByVal e As System.EventArgs)
        'Reset the mouse over fields to False
        Me.IsMouseOver = False
        Me.IsMouseOverCloseButton = False

        If Me.CanAnimate Then
            If Not Me.IsActive Then
                'If not the currently active tab
                If Me.IsAnimating Then
                    'If the tab is currently animating then change it's animation type to properly fade
                    'back to the inactive color.
                    Me.m_animationType = AnimationType.FadeOut
                Else
                    'The cursor was still over the tab and animation had finished so we need to fade
                    'from the mouseover color to the inactive color
                    Me.StartAnimation(AnimationType.FadeOut)
                End If
            Else
                'If it is the active tab then reset the current frame to 0 because the tab
                'might have been selected while animation was in process
                Me.m_currentFrame = 0
            End If
        Else
            'If the tab cannot animate then invalidate the tab to repaint with the inactive color
            Me.ParentInternal.Invalidate(Me.DisplayRectangle, False)
        End If
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim dragsize As Size = SystemInformation.DragSize
        Me.m_owner.OnMdiTabClicked(New MdiTabStripTabClickedEventArgs(Me))

        If Me.CanDrag Then
            'If the tab can be dragged, which is determined by the TabPermenance property, then set the
            'drag box and load the custom cursor.
            Me.m_dragBox = New Rectangle(New Point(e.X - (dragsize.Width / 2), e.Y - (dragsize.Height / 2)), dragsize)

            If Me.m_dragCursor Is Nothing Then
                Dim filePath As String = System.IO.Path.Combine(Application.StartupPath, "MyDragTab.cur")
                Me.m_dragCursor = Me.GetCustomCursor(filePath)
            End If
        End If

        If Not Me.IsActive Then
            'Set the isSwitching field. This prevents the tab from being closed in the MouseUp event
            'if the cursor is over the area in which the close button will be displayed.
            Me.m_isSwitching = True
            Me.Form.Activate()
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        Me.m_dragBox = Rectangle.Empty

        'If the tab is closable and the user is not switching tabs and the mouse was clicked over the
        'close button then close the form. The tab is removed via the FormClose event handler in MdiTabStrip class.
        If Me.CanClose AndAlso Not Me.m_isSwitching AndAlso Me.closeButtonHitTest(e.X, e.Y) Then
            Me.Form.Close()
        End If

        Me.m_isSwitching = False
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        If e.Button And Windows.Forms.MouseButtons.Left = Windows.Forms.MouseButtons.Left Then
            If Me.CanDrag Then
                'If the tab can be dragged, which is determined by the TabPermenace property, then.
                If Rectangle.op_Inequality(Me.m_dragBox, Rectangle.Empty) And Not Me.m_dragBox.Contains(e.X, e.Y) Then
                    'If the cursor has been moved out of the bounds of the drag box while the left
                    'mouse button is down then initiate dragging by calling the DoDragDrop method.

                    Me.m_isSwitching = False
                    Dim dropEffects As DragDropEffects = Me.DoDragDrop(Me, DragDropEffects.Move)
                    Me.m_dragBox = Rectangle.Empty
                End If
            End If
        Else
            'if active then test if mouse cursor is over the close button to display the tool tip.
            If Me.IsActive Then
                Me.IsMouseOverCloseButton = Me.closeButtonHitTest(e.X, e.Y)
            End If
            End If
    End Sub
#End Region

#End Region

End Class


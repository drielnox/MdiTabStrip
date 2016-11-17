Imports System.Drawing.Drawing2D

Namespace Design
    <System.ComponentModel.ToolboxItem(False)> _
    Public Class MdiTabTemplateControl
        Inherits Control

        Private WithEvents _activeTemplate As New ActiveMdiTabProperties
        Private WithEvents _inactiveTemplate As New InactiveMdiTabProperties
        Private WithEvents _mouseOverTemplate As New MdiTabProperties
        Private m_activeBounds As Point()
        Private m_activeInnerBounds As Point()
        Private m_inactiveBounds As Point()
        Private m_inactiveInnerBounds As Point()
        Private m_mouseOverBounds As Point()
        Private m_mouseOverInnerBounds As Point()
        Private m_closeButtonBounds As Point()
        Private m_closeButtonGlyphBounds As Point()
        Private _isMouseOverCloseButton As Boolean = False

        Friend Event TabSelected(ByVal e As TabSelectedEventArgs)

        Public Sub New()
            DoubleBuffered = True
            GetTabBounds()
            Dock = DockStyle.Top
        End Sub

#Region "Properties"
        Public ReadOnly Property ActiveTabTemplate() As ActiveMdiTabProperties
            Get
                Return _activeTemplate
            End Get
        End Property

        Public ReadOnly Property InactiveTabTemplate() As InactiveMdiTabProperties
            Get
                Return _inactiveTemplate
            End Get
        End Property

        Public ReadOnly Property MouseOverTabTemplate() As MdiTabProperties
            Get
                Return _mouseOverTemplate
            End Get
        End Property

        Public Property IsMouseOverCloseButton() As Boolean
            Get
                Return _isMouseOverCloseButton
            End Get
            Set(ByVal value As Boolean)
                If _isMouseOverCloseButton <> value Then
                    _isMouseOverCloseButton = value
                    Invalidate()
                End If
            End Set
        End Property

        Protected Overrides ReadOnly Property DefaultSize() As System.Drawing.Size
            Get
                Return New Size(50, 40)
            End Get
        End Property
#End Region

#Region "Paint"
        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

            DrawTab(e.Graphics)
        End Sub

        Protected Overridable Sub DrawTab(ByVal g As Graphics)
            DrawActiveTab(g)
            DrawInactiveTab(g)
            DrawMouseOverTab(g)
        End Sub

        Private Sub DrawActiveTab(ByVal g As Graphics)
            Dim iconRectangle As Rectangle
            Dim textRectangle As New Rectangle(30, 16, 98, Height - 21)

            If Me.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                iconRectangle = New Rectangle(180, 13, 17, 17)
                textRectangle.Offset(52, 0)
            Else
                iconRectangle = New Rectangle(13, 13, 17, 17)
            End If

            DrawFormIcon(g, iconRectangle)
            DrawTabText(g, textRectangle, "Active Tab", ActiveTabTemplate.ForeColor, ActiveTabTemplate.Font)
            DrawCloseButton(g)
        End Sub

        Private Sub DrawInactiveTab(ByVal g As Graphics)
            Dim iconRectangle As Rectangle
            Dim textRectangle As New Rectangle(172, 18, 123, Height - 23)

            If Me.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                iconRectangle = New Rectangle(330, 15, 17, 17)
                textRectangle.Offset(37, 0)
            Else
                iconRectangle = New Rectangle(155, 15, 17, 17)
            End If

            DrawFormIcon(g, iconRectangle)
            DrawTabText(g, textRectangle, "Inactive Tab", InactiveTabTemplate.ForeColor, InactiveTabTemplate.Font)
        End Sub

        Private Sub DrawMouseOverTab(ByVal g As Graphics)
            Dim iconRectangle As Rectangle
            Dim textRectangle As New Rectangle(322, 18, 123, Height - 23)

            If Me.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                iconRectangle = New Rectangle(480, 15, 17, 17)
                textRectangle.Offset(37, 0)
            Else
                iconRectangle = New Rectangle(305, 15, 17, 17)
            End If
            DrawFormIcon(g, iconRectangle)
            DrawTabText(g, textRectangle, "MouseOver Tab", MouseOverTabTemplate.ForeColor, MouseOverTabTemplate.Font)
        End Sub

        Private Sub DrawFormIcon(ByVal g As Graphics, ByVal rect As Rectangle)
            Dim icon As Icon = My.Resources.document

            Using bmp As New Bitmap(icon.Width, icon.Height, Imaging.PixelFormat.Format32bppArgb)
                Using bg As Graphics = Graphics.FromImage(bmp)
                    bg.DrawIcon(icon, 0, 0)
                End Using

                g.DrawImage(bmp, rect)
            End Using
        End Sub

        Private Sub DrawTabText(ByVal g As Graphics, ByVal rect As Rectangle, ByVal tabText As String, ByVal textColor As Color, ByVal tabFont As Font)
            Dim textFlags As TextFormatFlags = TextFormatFlags.WordEllipsis Or TextFormatFlags.EndEllipsis

            If Me.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                textFlags = textFlags Or TextFormatFlags.Right
            Else
                textFlags = textFlags Or TextFormatFlags.Left
            End If

            TextRenderer.DrawText(g, tabText, tabFont, rect, textColor, textFlags)
        End Sub

        Private Sub DrawCloseButton(ByVal g As Graphics)
            If IsMouseOverCloseButton Then
                DrawActiveCloseButton(g)
            Else
                DrawInactiveCloseButton(g)
            End If
        End Sub

        Private Sub DrawActiveCloseButton(ByVal g As Graphics)
            Using gp As New GraphicsPath
                gp.AddLines(m_closeButtonBounds)

                Using backBrush As New SolidBrush(ActiveTabTemplate.CloseButtonBackColor)
                    g.FillPath(backBrush, gp)
                End Using

                Using borderPen As New Pen(ActiveTabTemplate.CloseButtonBorderColor)
                    g.DrawPath(borderPen, gp)
                End Using
            End Using

            DrawCloseButtonGlyph(g, ActiveTabTemplate.CloseButtonHotForeColor)
        End Sub

        Private Sub DrawInactiveCloseButton(ByVal g As Graphics)
            DrawCloseButtonGlyph(g, ActiveTabTemplate.CloseButtonForeColor)
        End Sub

        Private Sub DrawCloseButtonGlyph(ByVal g As Graphics, ByVal glyphColor As Color)
            g.SmoothingMode = SmoothingMode.None

            Using shadow As New GraphicsPath
                Dim translateMatrix As New Matrix
                Dim shadowColor As Color = Color.FromArgb(120, 130, 130, 130)

                shadow.AddLines(m_closeButtonGlyphBounds)
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
                gp.AddLines(m_closeButtonGlyphBounds)

                Using glyphBrush As New SolidBrush(glyphColor)
                    g.FillPath(glyphBrush, gp)
                End Using
                Using glyphPen As New Pen(glyphColor)
                    g.DrawPath(glyphPen, gp)
                End Using
            End Using

            g.SmoothingMode = SmoothingMode.AntiAlias
        End Sub
#End Region

#Region "Paint Background"
        Protected Overrides Sub OnPaintBackground(ByVal e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaintBackground(e)

            e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle)
            DrawTabBackground(e.Graphics)
        End Sub

        Private Sub DrawTabBackground(ByVal g As Graphics)
            DrawInactiveTabBackground(g)
            DrawMouseOverTabBackground(g)
            DrawActiveTabBackground(g)
        End Sub

        Private Sub DrawActiveTabBackground(ByVal g As Graphics)
            Dim rect As Rectangle = DisplayRectangle
            Dim shadowRectangle As New Rectangle(0, Height - 5, Width, 5)
            Dim shadowBlend As New Blend

            rect.Offset(0, 8)
            rect.Height -= 8
            g.SmoothingMode = SmoothingMode.None
            shadowBlend.Factors = New Single() {0.0F, 0.1F, 0.3F, 0.4F}
            shadowBlend.Positions = New Single() {0.0F, 0.5F, 0.8F, 1.0F}

            Using outerPath As New GraphicsPath
                outerPath.AddLines(m_activeBounds)

                Using gradientbrush As New LinearGradientBrush(rect, Drawing.Color.White, ActiveTabTemplate.BackColor, LinearGradientMode.Vertical)
                    Dim bl As New Blend
                    bl.Factors = New Single() {0.3F, 0.4F, 0.5F, 1.0F, 1.0F}
                    bl.Positions = New Single() {0.0F, 0.2F, 0.35F, 0.35F, 1.0F}

                    gradientbrush.Blend = bl
                    g.FillPath(gradientbrush, outerPath)
                End Using

                Using shadowBrush As New LinearGradientBrush(shadowRectangle, ActiveTabTemplate.BackColor, Color.Black, LinearGradientMode.Vertical)
                    shadowBrush.Blend = shadowBlend
                    g.FillRectangle(shadowBrush, shadowRectangle)
                End Using

                g.SmoothingMode = SmoothingMode.AntiAlias
                g.DrawPath(New Pen(ActiveTabTemplate.BorderColor), outerPath)
            End Using

            Using innerPath As New GraphicsPath
                innerPath.AddLines(m_activeInnerBounds)

                Dim lineColor As Color = Color.FromArgb(120, 255, 255, 255)
                g.DrawPath(New Pen(lineColor), innerPath)
            End Using
        End Sub

        Private Sub DrawInactiveTabBackground(ByVal g As Graphics)
            Dim rect As Rectangle = DisplayRectangle

            rect.Offset(0, 8)
            rect.Height -= 8
            g.SmoothingMode = SmoothingMode.None

            Using outerPath As New GraphicsPath
                outerPath.AddLines(m_inactiveBounds)

                Using gradientbrush As New LinearGradientBrush(rect, Drawing.Color.White, InactiveTabTemplate.BackColor, LinearGradientMode.Vertical)
                    Dim bl As New Blend
                    bl.Factors = New Single() {0.3F, 0.4F, 0.5F, 1.0F, 0.8F, 0.7F}
                    bl.Positions = New Single() {0.0F, 0.2F, 0.4F, 0.4F, 0.8F, 1.0F}

                    gradientbrush.Blend = bl
                    g.FillPath(gradientbrush, outerPath)
                End Using

                g.SmoothingMode = SmoothingMode.AntiAlias
                g.DrawPath(New Pen(InactiveTabTemplate.BorderColor), outerPath)
            End Using

            Using innerPath As New GraphicsPath
                innerPath.AddLines(m_inactiveInnerBounds)

                Dim lineColor As Color = Color.FromArgb(120, 255, 255, 255)
                g.DrawPath(New Pen(lineColor), innerPath)
            End Using
        End Sub

        Private Sub DrawMouseOverTabBackground(ByVal g As Graphics)
            Dim rect As Rectangle = DisplayRectangle

            rect.Offset(0, 8)
            rect.Height -= 8
            g.SmoothingMode = SmoothingMode.None

            Using outerPath As New GraphicsPath
                outerPath.AddLines(m_mouseOverBounds)

                Using gradientbrush As New LinearGradientBrush(rect, Drawing.Color.White, MouseOverTabTemplate.BackColor, LinearGradientMode.Vertical)
                    Dim bl As New Blend
                    bl.Factors = New Single() {0.3F, 0.4F, 0.5F, 1.0F, 0.8F, 0.7F}
                    bl.Positions = New Single() {0.0F, 0.2F, 0.4F, 0.4F, 0.8F, 1.0F}

                    gradientbrush.Blend = bl
                    g.FillPath(gradientbrush, outerPath)
                End Using

                g.SmoothingMode = SmoothingMode.AntiAlias
                g.DrawPath(New Pen(InactiveTabTemplate.BorderColor), outerPath)
            End Using

            Using innerPath As New GraphicsPath
                innerPath.AddLines(m_mouseOverInnerBounds)

                Dim lineColor As Color = Color.FromArgb(120, 255, 255, 255)
                g.DrawPath(New Pen(lineColor), innerPath)
            End Using
        End Sub
#End Region

#Region "Hit Testing"
        Private Function ActiveHitTest(ByVal x As Integer, ByVal y As Integer) As Boolean
            Dim hit As Boolean = False

            Using gp As New GraphicsPath
                gp.StartFigure()
                gp.AddLines(m_activeBounds)
                gp.CloseFigure()

                Using borderpen As New Pen(Color.Black, 1)
                    If gp.IsOutlineVisible(x, y, borderpen) OrElse gp.IsVisible(x, y) Then
                        hit = True
                    End If
                End Using
            End Using

            Return hit
        End Function

        Private Function InactiveHitTest(ByVal x As Integer, ByVal y As Integer) As Boolean
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

        Private Function MouseOverHitTest(ByVal x As Integer, ByVal y As Integer) As Boolean
            Dim hit As Boolean = False

            Using gp As New GraphicsPath
                gp.StartFigure()
                gp.AddLines(m_mouseOverBounds)
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

#Region "Mouse Events"
        Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
            MyBase.OnMouseMove(e)

            If closeButtonHitTest(e.X, e.Y) Then
                IsMouseOverCloseButton = True
            Else
                IsMouseOverCloseButton = False
            End If
        End Sub

        Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
            MyBase.OnMouseDown(e)

            Dim ev As TabSelectedEventArgs = Nothing

            If ActiveHitTest(e.X, e.Y) Then
                ev = New TabSelectedEventArgs(TabType.Active)
            ElseIf InactiveHitTest(e.X, e.Y) Then
                ev = New TabSelectedEventArgs(TabType.Inactive)
            ElseIf MouseOverHitTest(e.X, e.Y) Then
                ev = New TabSelectedEventArgs(TabType.MouseOver)
            End If

            If ev IsNot Nothing Then
                RaiseEvent TabSelected(ev)
            End If
        End Sub
#End Region

#Region "Resizing"
        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            MyBase.OnResize(e)
            GetTabBounds()
            Invalidate()
        End Sub

        Private Sub GetTabBounds()
            Dim startPoint As Point
            If Me.RightToLeft = Windows.Forms.RightToLeft.Yes Then
                m_activeBounds = New Point() {New Point(-2, Height), _
                            New Point(-2, Height - 5), _
                            New Point(55, Height - 5), _
                            New Point(58, Height - 6), _
                            New Point(58, 11), _
                            New Point(60, 8), _
                            New Point(198, 8), _
                            New Point(200, 11), _
                            New Point(200, Height - 6), _
                            New Point(202, Height - 5), _
                            New Point(Width, Height - 5), _
                            New Point(Width, Height)}
                m_activeInnerBounds = New Point() {New Point(-1, Height), _
                            New Point(-1, Height - 4), _
                            New Point(56, Height - 4), _
                            New Point(59, Height - 6), _
                            New Point(59, 12), _
                            New Point(61, 9), _
                            New Point(197, 9), _
                            New Point(199, 12), _
                            New Point(199, Height - 6), _
                            New Point(201, Height - 4), _
                            New Point(Width - 1, Height - 4), _
                            New Point(Width - 1, Height)}
                m_inactiveBounds = New Point() {New Point(200, Height - 5), _
                            New Point(200, 13), _
                            New Point(202, 10), _
                            New Point(348, 10), _
                            New Point(350, 13), _
                            New Point(350, Height - 5)}
                m_inactiveInnerBounds = New Point() {New Point(201, Height - 5), _
                            New Point(201, 14), _
                            New Point(203, 11), _
                            New Point(347, 11), _
                            New Point(349, 14), _
                            New Point(349, Height - 5)}
                m_mouseOverBounds = New Point() {New Point(350, Height - 5), _
                            New Point(350, 13), _
                            New Point(352, 10), _
                            New Point(498, 10), _
                            New Point(500, 13), _
                            New Point(500, Height - 5)}
                m_mouseOverInnerBounds = New Point() {New Point(351, Height - 5), _
                            New Point(351, 14), _
                            New Point(353, 11), _
                            New Point(497, 11), _
                            New Point(499, 14), _
                            New Point(499, Height - 5)}
                m_closeButtonBounds = New Point() {New Point(75, 15), _
                            New Point(63, 15), _
                            New Point(61, 17), _
                            New Point(61, 28), _
                            New Point(63, 30), _
                            New Point(75, 30), _
                            New Point(77, 28), _
                            New Point(77, 17), _
                            New Point(75, 15)}
                startPoint = New Point(65, 19)
            Else
                m_activeBounds = New Point() {New Point(-2, Height), _
                            New Point(-2, Height - 5), _
                            New Point(5, Height - 5), _
                            New Point(8, Height - 6), _
                            New Point(8, 11), _
                            New Point(10, 8), _
                            New Point(148, 8), _
                            New Point(150, 11), _
                            New Point(150, Height - 6), _
                            New Point(152, Height - 5), _
                            New Point(Width, Height - 5), _
                            New Point(Width, Height)}
                m_activeInnerBounds = New Point() {New Point(-1, Height), _
                            New Point(-1, Height - 4), _
                            New Point(6, Height - 4), _
                            New Point(9, Height - 6), _
                            New Point(9, 12), _
                            New Point(11, 9), _
                            New Point(147, 9), _
                            New Point(149, 12), _
                            New Point(149, Height - 6), _
                            New Point(151, Height - 4), _
                            New Point(Width - 1, Height - 4), _
                            New Point(Width - 1, Height)}
                m_inactiveBounds = New Point() {New Point(150, Height - 5), _
                            New Point(150, 13), _
                            New Point(152, 10), _
                            New Point(298, 10), _
                            New Point(300, 13), _
                            New Point(300, Height - 5)}
                m_inactiveInnerBounds = New Point() {New Point(151, Height - 5), _
                            New Point(151, 14), _
                            New Point(153, 11), _
                            New Point(297, 11), _
                            New Point(299, 14), _
                            New Point(299, Height - 5)}
                m_mouseOverBounds = New Point() {New Point(300, Height - 5), _
                            New Point(300, 13), _
                            New Point(302, 10), _
                            New Point(448, 10), _
                            New Point(450, 13), _
                            New Point(450, Height - 5)}
                m_mouseOverInnerBounds = New Point() {New Point(301, Height - 5), _
                            New Point(301, 14), _
                            New Point(303, 11), _
                            New Point(447, 11), _
                            New Point(449, 14), _
                            New Point(449, Height - 5)}
                m_closeButtonBounds = New Point() {New Point(132, 15), _
                            New Point(144, 15), _
                            New Point(146, 17), _
                            New Point(146, 28), _
                            New Point(144, 30), _
                            New Point(132, 30), _
                            New Point(130, 28), _
                            New Point(130, 17), _
                            New Point(132, 15)}
                startPoint = New Point(134, 19)
            End If

            m_closeButtonGlyphBounds = New Point() {New Point(startPoint.X, startPoint.Y), _
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
        End Sub
#End Region

#Region "Property Changed Handlers"
        Private Sub _activeTemplate_PropertyChanged() Handles _activeTemplate.PropertyChanged
            Invalidate()
        End Sub

        Private Sub _inactiveTemplate_PropertyChanged() Handles _inactiveTemplate.PropertyChanged
            Invalidate()
        End Sub

        Private Sub _mouseOverTemplate_PropertyChanged() Handles _mouseOverTemplate.PropertyChanged
            Invalidate()
        End Sub
#End Region

    End Class

    Friend Class TabSelectedEventArgs
        Inherits EventArgs

        Private _tabType As TabType

        Public Sub New(ByVal tabType As TabType)
            _tabType = tabType
        End Sub

        Public ReadOnly Property TabType() As TabType
            Get
                Return _tabType
            End Get
        End Property
    End Class

    Friend Enum TabType
        Active = 1
        Inactive = 2
        MouseOver = 3
    End Enum
End Namespace
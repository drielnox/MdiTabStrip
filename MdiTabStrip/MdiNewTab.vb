Public Class MdiNewTab
    Inherits MdiTab

    Public Sub New(ByVal owner As MdiTabStrip)
        MyBase.New(owner)
    End Sub

    Friend Overrides Sub DrawControl(ByVal g As System.Drawing.Graphics)
        Dim tabImage As Image

        If Me.ParentInternal.MdiNewTabImage Is Nothing Then
            tabImage = My.Resources.NewTab
        Else
            tabImage = Me.ParentInternal.MdiNewTabImage
        End If

        If Me.IsMouseOver Then
            Dim iconRectangle As New Rectangle(Me.Width \ 2 - tabImage.Width \ 2, Me.Height \ 2 - tabImage.Height \ 2, tabImage.Width, tabImage.Height)

            iconRectangle.Offset(Me.Location)
            g.DrawImage(tabImage, iconRectangle)
        End If
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        Me.ParentInternal.OnMdiNewTabClick()
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        'Not implemented but overriden to bypass behavior in inherited class.
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        'Not implemented but overriden to bypass behavior in inherited class.
    End Sub
End Class

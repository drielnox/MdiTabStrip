Public Class MdiNewTab
    Inherits MdiTab

    Public Sub New(ByVal owner As MdiTabStrip)
        MyBase.New(owner)
    End Sub

    Friend Overrides Sub DrawControl(ByVal g As Graphics)
        Dim tabImage As Image

        If ParentInternal.MdiNewTabImage Is Nothing Then
            tabImage = My.Resources.NewTab
        Else
            tabImage = ParentInternal.MdiNewTabImage
        End If

        If IsMouseOver Then
            Dim iconRectangle As New Rectangle(Width \ 2 - tabImage.Width \ 2, Height \ 2 - tabImage.Height \ 2, tabImage.Width, tabImage.Height)

            iconRectangle.Offset(Location)
            g.DrawImage(tabImage, iconRectangle)
        End If
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        ParentInternal.OnMdiNewTabClick()
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        'Not implemented but overriden to bypass behavior in inherited class.
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
        'Not implemented but overriden to bypass behavior in inherited class.
    End Sub
End Class
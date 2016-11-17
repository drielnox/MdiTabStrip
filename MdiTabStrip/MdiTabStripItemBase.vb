<System.ComponentModel.ToolboxItem(False)> _
Public Class MdiTabStripItemBase
    Inherits Control

    Public Sub InvokeMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        OnMouseDown(e)
    End Sub

    Public Sub InvokeMouseEnter(ByVal e As System.EventArgs)
        OnMouseEnter(e)
    End Sub

    Public Sub InvokeMouseHover(ByVal e As EventArgs)
        OnMouseHover(e)
    End Sub

    Public Sub InvokeMouseLeave(ByVal e As System.EventArgs)
        OnMouseLeave(e)
    End Sub

    Public Sub InvokeMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        OnMouseMove(e)
    End Sub

    Public Sub InvokeMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        OnMouseUp(e)
    End Sub
End Class

<System.ComponentModel.ToolboxItem(False)> _
Public Class MdiTabStripItemBase
    Inherits Control

    Public Sub InvokeMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        Me.OnMouseDown(e)
    End Sub

    Public Sub InvokeMouseEnter(ByVal e As System.EventArgs)
        Me.OnMouseEnter(e)
    End Sub

    Public Sub InvokeMouseHover(ByVal e As EventArgs)
        Me.OnMouseHover(e)
    End Sub

    Public Sub InvokeMouseLeave(ByVal e As System.EventArgs)
        Me.OnMouseLeave(e)
    End Sub

    Public Sub InvokeMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        Me.OnMouseMove(e)
    End Sub

    Public Sub InvokeMouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        Me.OnMouseUp(e)
    End Sub
End Class

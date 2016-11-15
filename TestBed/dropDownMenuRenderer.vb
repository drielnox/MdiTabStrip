Public Class dropDownMenuRenderer
    Inherits ToolStripRenderer


    Protected Overrides Sub OnRenderItemBackground(ByVal e As System.Windows.Forms.ToolStripItemRenderEventArgs)
        e.Graphics.FillRectangle(Brushes.Black, e.Item.Bounds)
    End Sub

    Protected Overrides Sub OnRenderToolStripBackground(ByVal e As System.Windows.Forms.ToolStripRenderEventArgs)
        e.Graphics.FillRectangle(New System.Drawing.Drawing2D.LinearGradientBrush(e.AffectedBounds, Color.SlateBlue, Color.White, Drawing2D.LinearGradientMode.Vertical), e.AffectedBounds)
    End Sub
End Class

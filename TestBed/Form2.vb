Public Class Form2

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Text = Me.Text & " - Enhanced"
        Me.OpenToolStripMenuItem.Text = "Open - Enhanced"
    End Sub
End Class
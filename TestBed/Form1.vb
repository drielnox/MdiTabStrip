Public Class Form1

    Private Sub NewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripMenuItem.Click
        Dim aForm As New Form2

        aForm.MdiParent = Me
        aForm.Show()
    End Sub

    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem1.Click
        Dim aForm As New Form3

        aForm.MdiParent = Me
        aForm.Show()
    End Sub

    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        Dim aForm As New Form4

        aForm.MdiParent = Me
        aForm.Show()
    End Sub

    Private Sub ToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem3.Click
        Dim aForm As New Form5

        aForm.MdiParent = Me
        aForm.Show()
    End Sub

    Private Sub ToolStripMenuItem4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem4.Click
        Dim aForm As New Form6

        aForm.MdiParent = Me
        aForm.Show()
    End Sub

    Private Sub MdiTabStrip1_MdiNewTabClicked(ByVal sender As Object, ByVal e As System.EventArgs) Handles MdiTabStrip1.MdiNewTabClicked
        Dim aForm As New Form4

        aForm.MdiParent = Me
        aForm.Show()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub AnimateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnimateToolStripMenuItem.Click
        Me.AnimateToolStripMenuItem.Checked = Not Me.AnimateToolStripMenuItem.Checked
        Me.MdiTabStrip1.Animate = Me.AnimateToolStripMenuItem.Checked
    End Sub

    Private Sub ShowIconToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowIconToolStripMenuItem.Click
        Me.ShowIconToolStripMenuItem.Checked = Not Me.ShowIconToolStripMenuItem.Checked
        Me.MdiTabStrip1.DisplayFormIcon = Me.ShowIconToolStripMenuItem.Checked
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Uncomment the next line to see how the MdiTabStrip drop down menu renderer can be changed.
        'Me.MdiTabStrip1.DropDownRenderer = New dropDownMenuRenderer
    End Sub

    Private Sub NoneToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NoneToolStripMenuItem.Click
        Me.MdiTabStrip1.TabPermanence = MdiTabStrip.MdiTabPermanence.None
        Me.SetTabPermanenceMenuItemCheckedState(Me.NoneToolStripMenuItem)
    End Sub

    Private Sub FirstTabToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FirstTabToolStripMenuItem.Click
        Me.MdiTabStrip1.TabPermanence = MdiTabStrip.MdiTabPermanence.First
        Me.SetTabPermanenceMenuItemCheckedState(Me.FirstTabToolStripMenuItem)
    End Sub

    Private Sub LastTabOpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LastTabOpenToolStripMenuItem.Click
        Me.MdiTabStrip1.TabPermanence = MdiTabStrip.MdiTabPermanence.LastOpen
        Me.SetTabPermanenceMenuItemCheckedState(Me.LastTabOpenToolStripMenuItem)
    End Sub

    Private Sub SetTabPermanenceMenuItemCheckedState(ByVal ti As ToolStripItem)
        For Each item As ToolStripItem In Me.TabPermanenceToolStripMenuItem.DropDownItems
            If item IsNot ti Then
                CType(item, ToolStripMenuItem).Checked = False
            End If
        Next
    End Sub

    Private Sub RightToLeftToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RightToLeftToolStripMenuItem.Click
        Me.RightToLeftToolStripMenuItem.Checked = Not Me.RightToLeftToolStripMenuItem.Checked
        If Me.RightToLeftToolStripMenuItem.Checked Then
            Me.MdiTabStrip1.RightToLeft = Windows.Forms.RightToLeft.Yes
        Else
            Me.MdiTabStrip1.RightToLeft = Windows.Forms.RightToLeft.No
        End If
    End Sub

    Private Sub ShowMdiNewTabToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowMdiNewTabToolStripMenuItem.Click
        Me.ShowMdiNewTabToolStripMenuItem.Checked = Not Me.ShowMdiNewTabToolStripMenuItem.Checked
        Me.MdiTabStrip1.MdiNewTabVisible = Me.ShowMdiNewTabToolStripMenuItem.Checked
    End Sub

    Private Sub ShowToolTipsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowToolTipsToolStripMenuItem.Click
        Me.ShowToolTipsToolStripMenuItem.Checked = Not Me.ShowToolTipsToolStripMenuItem.Checked
        Me.MdiTabStrip1.ShowTabToolTip = Me.ShowToolTipsToolStripMenuItem.Checked
    End Sub

    Private Sub NewToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripButton.Click
        Dim aForm As New Form3

        aForm.MdiParent = Me
        aForm.Show()
    End Sub
End Class

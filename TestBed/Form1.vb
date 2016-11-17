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
        Close()
    End Sub

    Private Sub AnimateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnimateToolStripMenuItem.Click
        AnimateToolStripMenuItem.Checked = Not AnimateToolStripMenuItem.Checked
        MdiTabStrip1.Animate = AnimateToolStripMenuItem.Checked
    End Sub

    Private Sub ShowIconToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowIconToolStripMenuItem.Click
        ShowIconToolStripMenuItem.Checked = Not ShowIconToolStripMenuItem.Checked
        MdiTabStrip1.DisplayFormIcon = ShowIconToolStripMenuItem.Checked
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Uncomment the next line to see how the MdiTabStrip drop down menu renderer can be changed.
        'Me.MdiTabStrip1.DropDownRenderer = New dropDownMenuRenderer
    End Sub

    Private Sub NoneToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NoneToolStripMenuItem.Click
        MdiTabStrip1.TabPermanence = MdiTabStrip2.MdiTabPermanence.None
        SetTabPermanenceMenuItemCheckedState(NoneToolStripMenuItem)
    End Sub

    Private Sub FirstTabToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FirstTabToolStripMenuItem.Click
        MdiTabStrip1.TabPermanence = MdiTabStrip2.MdiTabPermanence.First
        SetTabPermanenceMenuItemCheckedState(FirstTabToolStripMenuItem)
    End Sub

    Private Sub LastTabOpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LastTabOpenToolStripMenuItem.Click
        MdiTabStrip1.TabPermanence = MdiTabStrip2.MdiTabPermanence.LastOpen
        SetTabPermanenceMenuItemCheckedState(LastTabOpenToolStripMenuItem)
    End Sub

    Private Sub SetTabPermanenceMenuItemCheckedState(ByVal ti As ToolStripItem)
        For Each item As ToolStripItem In TabPermanenceToolStripMenuItem.DropDownItems
            If item IsNot ti Then
                CType(item, ToolStripMenuItem).Checked = False
            End If
        Next
    End Sub

    Private Sub RightToLeftToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RightToLeftToolStripMenuItem.Click
        RightToLeftToolStripMenuItem.Checked = Not RightToLeftToolStripMenuItem.Checked
        If RightToLeftToolStripMenuItem.Checked Then
            MdiTabStrip1.RightToLeft = Windows.Forms.RightToLeft.Yes
        Else
            MdiTabStrip1.RightToLeft = Windows.Forms.RightToLeft.No
        End If
    End Sub

    Private Sub ShowMdiNewTabToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowMdiNewTabToolStripMenuItem.Click
        ShowMdiNewTabToolStripMenuItem.Checked = Not ShowMdiNewTabToolStripMenuItem.Checked
        MdiTabStrip1.MdiNewTabVisible = ShowMdiNewTabToolStripMenuItem.Checked
    End Sub

    Private Sub ShowToolTipsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowToolTipsToolStripMenuItem.Click
        ShowToolTipsToolStripMenuItem.Checked = Not ShowToolTipsToolStripMenuItem.Checked
        MdiTabStrip1.ShowTabToolTip = ShowToolTipsToolStripMenuItem.Checked
    End Sub

    Private Sub NewToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripButton.Click
        Dim aForm As New Form3

        aForm.MdiParent = Me
        aForm.Show()
    End Sub
End Class

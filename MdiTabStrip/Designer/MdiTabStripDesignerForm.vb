Namespace Design
    Public Class MdiTabStripDesignerForm
        Private WithEvents _template As MdiTabTemplateControl

        Public Property TabTemplate() As MdiTabTemplateControl
            Get
                Return _template
            End Get
            Set(ByVal value As MdiTabTemplateControl)
                _template = value

                _template.Location = New Point(SplitContainer1.Panel1.Width / 2 - _template.Width / 2, SplitContainer1.Panel1.Height / 2 - _template.Height / 2)
                SplitContainer1.Panel1.Controls.Clear()
                SplitContainer1.Panel1.Controls.Add(value)
                PropertyGrid1.SelectedObject = _template.ActiveTabTemplate
            End Set
        End Property

        Private Sub cboTabType_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboTabType.SelectedValueChanged
            If _template IsNot Nothing Then
                If cboTabType.SelectedItem = "Active Tab" Then
                    PropertyGrid1.SelectedObject = _template.ActiveTabTemplate
                ElseIf cboTabType.SelectedItem = "Inactive Tab" Then
                    PropertyGrid1.SelectedObject = _template.InactiveTabTemplate
                Else
                    PropertyGrid1.SelectedObject = _template.MouseOverTabTemplate
                End If
            End If
        End Sub

        Private Sub _template_TabSelected(ByVal e As TabSelectedEventArgs) Handles _template.TabSelected
            If e.TabType = TabType.Active Then
                cboTabType.SelectedItem = "Active Tab"
            ElseIf e.TabType = TabType.Inactive Then
                cboTabType.SelectedItem = "Inactive Tab"
            Else
                cboTabType.SelectedItem = "Mouseover Tab"
            End If
        End Sub
    End Class
End Namespace
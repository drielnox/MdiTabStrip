Namespace Design

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class MdiTabStripDesignerForm
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MdiTabStripDesignerForm))
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.cboTabType = New System.Windows.Forms.ComboBox
            Me.Label2 = New System.Windows.Forms.Label
            Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid
            Me.btnCancel = New System.Windows.Forms.Button
            Me.btnOK = New System.Windows.Forms.Button
            Me.Label1 = New System.Windows.Forms.Label
            Me.Label3 = New System.Windows.Forms.Label
            Me.PictureBox1 = New System.Windows.Forms.PictureBox
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'SplitContainer1
            '
            Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.SplitContainer1.BackColor = System.Drawing.Color.White
            Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.SplitContainer1.Location = New System.Drawing.Point(0, 112)
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.AutoScroll = True
            Me.SplitContainer1.Panel1.BackColor = System.Drawing.Color.White
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.cboTabType)
            Me.SplitContainer1.Panel2.Controls.Add(Me.Label2)
            Me.SplitContainer1.Panel2.Controls.Add(Me.PropertyGrid1)
            Me.SplitContainer1.Size = New System.Drawing.Size(764, 281)
            Me.SplitContainer1.SplitterDistance = 508
            Me.SplitContainer1.TabIndex = 0
            '
            'cboTabType
            '
            Me.cboTabType.BackColor = System.Drawing.SystemColors.Window
            Me.cboTabType.Dock = System.Windows.Forms.DockStyle.Top
            Me.cboTabType.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.cboTabType.FormattingEnabled = True
            Me.cboTabType.Items.AddRange(New Object() {"Active Tab", "Inactive Tab", "Mouseover Tab"})
            Me.cboTabType.Location = New System.Drawing.Point(0, 20)
            Me.cboTabType.Margin = New System.Windows.Forms.Padding(0)
            Me.cboTabType.Name = "cboTabType"
            Me.cboTabType.Size = New System.Drawing.Size(250, 21)
            Me.cboTabType.TabIndex = 4
            Me.cboTabType.Text = "Active Tab"
            '
            'Label2
            '
            Me.Label2.BackColor = System.Drawing.Color.LightGray
            Me.Label2.Dock = System.Windows.Forms.DockStyle.Top
            Me.Label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.Label2.ForeColor = System.Drawing.Color.Black
            Me.Label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.Label2.Location = New System.Drawing.Point(0, 0)
            Me.Label2.Margin = New System.Windows.Forms.Padding(0)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(250, 20)
            Me.Label2.TabIndex = 5
            Me.Label2.Text = "Tab Properties"
            Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'PropertyGrid1
            '
            Me.PropertyGrid1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.PropertyGrid1.BackColor = System.Drawing.SystemColors.Control
            Me.PropertyGrid1.CommandsVisibleIfAvailable = False
            Me.PropertyGrid1.Location = New System.Drawing.Point(0, 41)
            Me.PropertyGrid1.Margin = New System.Windows.Forms.Padding(0)
            Me.PropertyGrid1.Name = "PropertyGrid1"
            Me.PropertyGrid1.Size = New System.Drawing.Size(250, 238)
            Me.PropertyGrid1.TabIndex = 0
            '
            'btnCancel
            '
            Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Location = New System.Drawing.Point(677, 406)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(75, 23)
            Me.btnCancel.TabIndex = 1
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'btnOK
            '
            Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.btnOK.Location = New System.Drawing.Point(596, 406)
            Me.btnOK.Name = "btnOK"
            Me.btnOK.Size = New System.Drawing.Size(75, 23)
            Me.btnOK.TabIndex = 2
            Me.btnOK.Text = "OK"
            Me.btnOK.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label1.AutoEllipsis = True
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label1.Location = New System.Drawing.Point(22, 72)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(715, 35)
            Me.Label1.TabIndex = 3
            Me.Label1.Text = "To edit the properties of a tab select it from the tab properties drop-down or cl" & _
                "ick on the tab in the preview window. Move the cursor over the 'Close' button to" & _
                " see how it looks when moused over."
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label3.Location = New System.Drawing.Point(163, 14)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(200, 29)
            Me.Label3.TabIndex = 4
            Me.Label3.Text = "MdiTab Designer"
            '
            'PictureBox1
            '
            Me.PictureBox1.Image = Global.MdiTabStrip2.My.Resources.Resources.TabDesigner
            Me.PictureBox1.InitialImage = Nothing
            Me.PictureBox1.Location = New System.Drawing.Point(12, 9)
            Me.PictureBox1.Name = "PictureBox1"
            Me.PictureBox1.Size = New System.Drawing.Size(145, 30)
            Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
            Me.PictureBox1.TabIndex = 5
            Me.PictureBox1.TabStop = False
            '
            'TabTemplateFormEditor
            '
            Me.AcceptButton = Me.btnOK
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.SystemColors.Control
            Me.CancelButton = Me.btnCancel
            Me.ClientSize = New System.Drawing.Size(764, 441)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnOK)
            Me.Controls.Add(Me.PictureBox1)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.SplitContainer1)
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "TabTemplateFormEditor"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "MdiTab Designer"
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents PropertyGrid1 As System.Windows.Forms.PropertyGrid
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents btnOK As System.Windows.Forms.Button
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents cboTabType As System.Windows.Forms.ComboBox
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    End Class
End Namespace
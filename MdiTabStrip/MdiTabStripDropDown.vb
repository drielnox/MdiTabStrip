<System.ComponentModel.ToolboxItem(False)> _
Friend Class MdiTabStripDropDown
    Inherits ContextMenuStrip

    Friend Sub New()
        MyBase.New()

        Me.Renderer = New MdiMenuStripRenderer
    End Sub

    Friend Sub SetItemChecked(ByVal item As MdiMenuItem)
        For Each mi As MdiMenuItem In Me.Items
            mi.Checked = False
        Next

        item.Checked = True
    End Sub

    Protected Overrides Sub OnItemAdded(ByVal e As System.Windows.Forms.ToolStripItemEventArgs)
        MyBase.OnItemAdded(e)

        Me.SetItemChecked(e.Item)
    End Sub
End Class

Friend Class MdiMenuItem
    Inherits ToolStripMenuItem

    Private m_isMouseOver As Boolean = False
    Private m_ownerTab As MdiTab

    Public Sub New(ByVal tab As MdiTab, ByVal handler As EventHandler)
        Me.m_ownerTab = tab
        AddHandler Me.Click, handler
    End Sub

    Public ReadOnly Property Form() As Form
        Get
            Return Me.m_ownerTab.Form
        End Get
    End Property

    Public ReadOnly Property IsMouseOver() As Boolean
        Get
            Return Me.m_isMouseOver
        End Get
    End Property

    Public ReadOnly Property IsTabActive() As Boolean
        Get
            Return Me.m_ownerTab.IsActive
        End Get
    End Property

    Public ReadOnly Property IsTabVisible() As Boolean
        Get
            Return Me.m_ownerTab.Visible
        End Get
    End Property

    Friend Shadows ReadOnly Property CheckedImage() As Image
        Get
            Dim bmp As New Bitmap(GetType(MdiTabStrip), "CheckedImage.bmp")
            bmp.MakeTransparent(bmp.GetPixel(1, 1))
            Return bmp
        End Get
    End Property

    'Public Overrides Property Text() As String
    '    Get
    '        Return Me.Form.Text
    '    End Get
    '    Set(ByVal value As String)
    '        MyBase.Text = value
    '    End Set
    'End Property

    Protected Overrides Sub OnMouseEnter(ByVal e As System.EventArgs)
        MyBase.OnMouseEnter(e)
        Me.m_isMouseOver = True
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnMouseLeave(ByVal e As System.EventArgs)
        MyBase.OnMouseLeave(e)
        Me.m_isMouseOver = False
        Me.Invalidate()
    End Sub
End Class

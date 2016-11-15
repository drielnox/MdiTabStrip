Public Class MdiTabStripTabEventArgs
    Inherits EventArgs

    Private _tab As MdiTab

    Public Sub New(ByVal tabItem As MdiTab)
        Me._tab = tabItem
    End Sub

    Public ReadOnly Property MdiTab() As MdiTab
        Get
            Return _tab
        End Get
    End Property

End Class

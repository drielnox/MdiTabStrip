Public Class MdiTabStripTabClickedEventArgs
    Inherits EventArgs

    Private _tab As MdiTab

    Public Sub New(ByVal tabItem As MdiTab)
        _tab = tabItem
    End Sub

    Public ReadOnly Property ClickedTab() As MdiTab
        Get
            Return _tab
        End Get
    End Property
End Class

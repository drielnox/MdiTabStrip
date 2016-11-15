Imports System.ComponentModel

<EditorBrowsable(False)> _
Public Class MdiTabCollection
    Inherits CollectionBase

    Default Public Property Item(ByVal index As Integer) As MdiTab
        Get
            Return CType(List.Item(index), MdiTab)
        End Get
        Set(ByVal value As MdiTab)
            List.Item(index) = value
        End Set
    End Property

    Public ReadOnly Property VisibleCount() As Integer
        Get
            Dim c As Integer = 0
            For Each tab As MdiTab In List
                If tab.Visible Then
                    c += 1
                End If
            Next

            Return c
        End Get
    End Property

    Public ReadOnly Property FirstVisibleTabIndex() As Integer
        Get
            For Each tab As MdiTab In List
                If tab.Visible Then
                    Return List.IndexOf(tab)
                End If
            Next
        End Get
    End Property

    Public ReadOnly Property LastVisibleTabIndex() As Integer
        Get
            Dim c As Integer = 0
            For Each tab As MdiTab In List
                If tab.Visible Then
                    c = List.IndexOf(tab)
                End If
            Next

            Return c
        End Get
    End Property

    Public Function Add(ByVal tab As MdiTab) As Integer
        Return List.Add(tab)
    End Function

    Public Function Contains(ByVal tab As MdiTab) As Boolean
        Return List.Contains(tab)
    End Function

    Public Sub Insert(ByVal index As Integer, ByVal value As MdiTab)
        List.Insert(index, value)
    End Sub

    Public Function IndexOf(ByVal value As MdiTab) As Integer
        Return List.IndexOf(value)
    End Function

    Public Sub Remove(ByVal value As MdiTab)
        List.Remove(value)
    End Sub

    Protected Overrides Sub OnValidate(ByVal value As Object)
        If Not GetType(MdiTab).IsAssignableFrom(value.GetType) Then
            Throw New Exception("Value must be MdiTab")
        End If
    End Sub
End Class

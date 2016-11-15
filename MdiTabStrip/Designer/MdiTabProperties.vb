Imports System.ComponentModel

Namespace Design
    Public Class MdiTabProperties

        Private _backColor As Color
        Private _foreColor As Color
        Private _font As Font
        Friend Event PropertyChanged()

        <Category("Tab Appearance"), _
        Description("The background color of the tab.")> _
        Public Property BackColor() As Color
            Get
                Return _backColor
            End Get
            Set(ByVal value As Color)
                If _backColor <> value Then
                    _backColor = value
                    RaiseEvent PropertyChanged()
                End If
            End Set
        End Property

        <Category("Tab Appearance"), _
        Description("The text color of the tab.")> _
        Public Property ForeColor() As Color
            Get
                Return _foreColor
            End Get
            Set(ByVal value As Color)
                If _foreColor <> value Then
                    _foreColor = value
                    RaiseEvent PropertyChanged()
                End If
            End Set
        End Property

        <Category("Tab Appearance"), _
        Description("The font used to display the text of the tab.")> _
        Public Property Font() As Font
            Get
                Return _font
            End Get
            Set(ByVal value As Font)
                If Not _font Is value Then
                    _font = value
                    RaiseEvent PropertyChanged()
                End If
            End Set
        End Property

        Protected Sub InvokePropertyChanged()
            RaiseEvent PropertyChanged()
        End Sub
    End Class
End Namespace
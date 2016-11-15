Imports System.ComponentModel

Namespace Design
    Public Class InactiveMdiTabProperties
        Inherits MdiTabProperties

        Private _borderColor As Color

        <Category("Tab Appearance"), _
        Description("The border color of the tab.")> _
        Public Property BorderColor() As Color
            Get
                Return _borderColor
            End Get
            Set(ByVal value As Color)
                If _borderColor <> value Then
                    _borderColor = value
                    InvokePropertyChanged()
                End If
            End Set
        End Property
    End Class
End Namespace
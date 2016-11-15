Imports System.Drawing.Drawing2D
Imports System.ComponentModel

Namespace Design
    Public Class ActiveMdiTabProperties
        Inherits InactiveMdiTabProperties

        Private _closeButtonBackColor As Color
        Private _closeButtonBorderColor As Color
        Private _closeButtonForeColor As Color
        Private _closeButtonHotForeColor As Color

        <Category("Close Button Appearance"), _
        Description("The background color of the tab's close button when moused over.")> _
        Public Property CloseButtonBackColor() As Color
            Get
                Return _closeButtonBackColor
            End Get
            Set(ByVal value As Color)
                If _closeButtonBackColor <> value Then
                    _closeButtonBackColor = value
                    InvokePropertyChanged()
                End If
            End Set
        End Property

        <Category("Close Button Appearance"), _
        Description("The border color of the tab's close button when moused over.")> _
        Public Property CloseButtonBorderColor() As Color
            Get
                Return _closeButtonBorderColor
            End Get
            Set(ByVal value As Color)
                If _closeButtonBorderColor <> value Then
                    _closeButtonBorderColor = value
                    InvokePropertyChanged()
                End If
            End Set
        End Property

        <Category("Close Button Appearance"), _
        Description("The glyph color of the tab's close button.")> _
        Public Property CloseButtonForeColor() As Color
            Get
                Return _closeButtonForeColor
            End Get
            Set(ByVal value As Color)
                If _closeButtonForeColor <> value Then
                    _closeButtonForeColor = value
                    InvokePropertyChanged()
                End If
            End Set
        End Property

        <Category("Close Button Appearance"), _
        Description("The glyph color of the tab's close button when moused over.")> _
        Public Property CloseButtonHotForeColor() As Color
            Get
                Return _closeButtonHotForeColor
            End Get
            Set(ByVal value As Color)
                If _closeButtonHotForeColor <> value Then
                    _closeButtonHotForeColor = value
                    InvokePropertyChanged()
                End If
            End Set
        End Property
    End Class
End Namespace
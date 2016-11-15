Imports System.Windows.Forms.Design
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Reflection

Namespace Design
    Public Class MdiTabStripDesigner
        Inherits ControlDesigner

        Private m_actionLists As DesignerActionListCollection = Nothing

        Public Overrides ReadOnly Property ActionLists() As DesignerActionListCollection
            Get
                If Me.m_actionLists Is Nothing Then
                    Me.m_actionLists = New DesignerActionListCollection
                    Me.m_actionLists.Add(New MdiTabStripDesignerActionList(Me.Component))
                End If

                Return Me.m_actionLists
            End Get
        End Property

        Public Overrides Sub Initialize(ByVal component As System.ComponentModel.IComponent)
            MyBase.Initialize(component)

            Dim tabStrip As MdiTabStrip = CType(Control, MdiTabStrip)
            Dim activeTab As New MdiTab(tabStrip)
            Dim inactiveTab As New MdiTab(tabStrip)
            Dim mouseOverTab As New MdiTab(tabStrip)

            tabStrip.LeftScrollTab.Visible = True
            tabStrip.RightScrollTab.Visible = True
            tabStrip.DropDownTab.Visible = True

            activeTab.Form = New Form1
            tabStrip.ActiveTab = activeTab
            tabStrip.Tabs.Add(activeTab)

            inactiveTab.Form = New Form2
            tabStrip.Tabs.Add(inactiveTab)

            mouseOverTab.Form = New Form3
            mouseOverTab.IsMouseOver = True
            tabStrip.Tabs.Add(mouseOverTab)

            tabStrip.PerformLayout()
        End Sub

        Private Class Form1
            Inherits Form

            Public Sub New()
                Me.Text = "Active Tab"
                Me.Icon = My.Resources.document
            End Sub
        End Class

        Private Class Form2
            Inherits Form

            Public Sub New()
                Me.Text = "Inactive Tab"
                Me.Icon = My.Resources.document
            End Sub
        End Class

        Private Class Form3
            Inherits Form

            Public Sub New()
                Me.Text = "Moused Over Tab"
                Me.Icon = My.Resources.document
            End Sub
        End Class
    End Class

    Public Class MdiTabStripDesignerActionList
        Inherits DesignerActionList

        Private _uiService As DesignerActionUIService = Nothing
        Private _actionItems As DesignerActionItemCollection = Nothing

        Public Sub New(ByVal component As IComponent)
            MyBase.New(component)
            Me._uiService = CType(GetService(GetType(DesignerActionUIService)), DesignerActionUIService)
        End Sub

        Public Overrides Function GetSortedActionItems() As DesignerActionItemCollection
            If _actionItems Is Nothing Then
                _actionItems = New DesignerActionItemCollection

                If TabStrip IsNot Nothing Then
                    _actionItems.Add(New DesignerActionMethodItem(Me, "OpenInactiveTabEditor", "Design Tabs", "Appearance", "Opens the MdiTab Designer window."))

                    _actionItems.Add(New DesignerActionHeaderItem("Behavior"))
                    _actionItems.Add(New DesignerActionPropertyItem("TabPermanence", "Tab permanence", GetCategory(Me.TabStrip, "TabPermanence"), GetDescription(Me.TabStrip, "TabPermanence")))
                    _actionItems.Add(New DesignerActionPropertyItem("Animate", "Perform fade animation on mouse over", GetCategory(Me.TabStrip, "Animate"), GetDescription(Me.TabStrip, "Animate")))
                    _actionItems.Add(New DesignerActionPropertyItem("DisplayFormIcon", "Display the form icon", "Behavior", GetDescription(Me.TabStrip, "DisplayFormIcon")))
                    _actionItems.Add(New DesignerActionPropertyItem("MdiNewTabVisible", "Display the new tab", "Behavior", GetDescription(Me.TabStrip, "MdiNewTabVisible")))

                    _actionItems.Add(New DesignerActionHeaderItem("Layout"))
                    _actionItems.Add(New DesignerActionPropertyItem("MinTabWidth", "Minimum tab width", GetCategory(Me.TabStrip, "MinTabWidth"), GetDescription(Me.TabStrip, "MinTabWidth")))
                    _actionItems.Add(New DesignerActionPropertyItem("MaxTabWidth", "Maximum tab width", GetCategory(Me.TabStrip, "MaxTabWidth"), GetDescription(Me.TabStrip, "MaxTabWidth")))
                    _actionItems.Add(New DesignerActionPropertyItem("MdiWindowState", "Mdi form window state", GetCategory(Me.TabStrip, "MdiWindowState"), GetDescription(Me.TabStrip, "MdiWindowState")))
                End If
            End If

            Return _actionItems
        End Function

        Public ReadOnly Property TabStrip() As MdiTabStrip
            Get
                Return CType(MyBase.Component, MdiTabStrip)
            End Get
        End Property

        Private Sub SetProperty(ByVal propertyName As String, ByVal value As Object)
            Dim prop As PropertyDescriptor = TypeDescriptor.GetProperties(Me.TabStrip)(propertyName)
            prop.SetValue(Me.TabStrip, value)
        End Sub

        Private Function GetCategory(ByVal source As Object, ByVal propertyName As String) As String
            Dim prop As PropertyInfo = source.GetType.GetProperty(propertyName)
            Dim attributes() As Object = prop.GetCustomAttributes(GetType(CategoryAttribute), False)

            If attributes.Length = 0 Then
                Return Nothing
            End If

            Dim attr As CategoryAttribute = CType(attributes(0), CategoryAttribute)

            If attr Is Nothing Then
                Return Nothing
            End If

            Return attr.Category
        End Function

        Private Function GetDescription(ByVal source As Object, ByVal propertyName As String) As String
            Dim prop As PropertyInfo = source.GetType.GetProperty(propertyName)
            Dim attributes() As Object = prop.GetCustomAttributes(GetType(DescriptionAttribute), False)

            If attributes.Length = 0 Then
                Return Nothing
            End If

            Dim attr As DescriptionAttribute = CType(attributes(0), DescriptionAttribute)

            If attr Is Nothing Then
                Return Nothing
            End If

            Return attr.Description
        End Function

#Region "MdiTabStrip Properties"
        Public Property ActiveTabColor() As Color
            Get
                Return Me.TabStrip.ActiveTabColor
            End Get
            Set(ByVal value As Color)
                SetProperty("ActiveTabColor", value)
            End Set
        End Property

        Public Property ActiveTabForeColor() As Color
            Get
                Return Me.TabStrip.ActiveTabForeColor
            End Get
            Set(ByVal value As Color)
                SetProperty("ActiveTabForeColor", value)
            End Set
        End Property

        Public Property ActiveTabFont() As Font
            Get
                Return Me.TabStrip.ActiveTabFont
            End Get
            Set(ByVal value As Font)
                SetProperty("ActiveTabFont", value)
            End Set
        End Property

        Public Property CloseButtonBackColor() As Color
            Get
                Return Me.TabStrip.CloseButtonBackColor
            End Get
            Set(ByVal value As Color)
                SetProperty("CloseButtonBackColor", value)
            End Set
        End Property

        Public Property CloseButtonForeColor() As Color
            Get
                Return Me.TabStrip.CloseButtonForeColor
            End Get
            Set(ByVal value As Color)
                SetProperty("CloseButtonForeColor", value)
            End Set
        End Property

        Public Property CloseButtonHotForeColor() As Color
            Get
                Return Me.TabStrip.CloseButtonHotForeColor
            End Get
            Set(ByVal value As Color)
                SetProperty("CloseButtonHotForeColor", value)
            End Set
        End Property

        Public Property CloseButtonBorderColor() As Color
            Get
                Return Me.TabStrip.CloseButtonBorderColor
            End Get
            Set(ByVal value As Color)
                SetProperty("CloseButtonBorderColor", value)
            End Set
        End Property

        Public Property InactiveTabColor() As Color
            Get
                Return Me.TabStrip.InactiveTabColor
            End Get
            Set(ByVal value As Color)
                SetProperty("InactiveTabColor", value)
            End Set
        End Property

        Public Property InactiveTabForeColor() As Color
            Get
                Return Me.TabStrip.InactiveTabForeColor
            End Get
            Set(ByVal value As Color)
                SetProperty("InactiveTabForeColor", value)
            End Set
        End Property

        Public Property InactiveTabFont() As Font
            Get
                Return Me.TabStrip.InactiveTabFont
            End Get
            Set(ByVal value As Font)
                SetProperty("InactiveTabFont", value)
            End Set
        End Property

        Public Property MouseOverTabColor() As Color
            Get
                Return Me.TabStrip.MouseOverTabColor
            End Get
            Set(ByVal value As Color)
                SetProperty("MouseOverTabColor", value)
            End Set
        End Property

        Public Property MouseOverTabForeColor() As Color
            Get
                Return Me.TabStrip.MouseOverTabForeColor
            End Get
            Set(ByVal value As Color)
                SetProperty("MouseOverTabForeColor", value)
            End Set
        End Property

        Public Property MouseOverTabFont() As Font
            Get
                Return Me.TabStrip.MouseOverTabFont
            End Get
            Set(ByVal value As Font)
                SetProperty("MouseOverTabFont", value)
            End Set
        End Property

        Public Property ActiveTabBorderColor() As Color
            Get
                Return Me.TabStrip.ActiveTabBorderColor
            End Get
            Set(ByVal value As Color)
                SetProperty("ActiveTabBorderColor", value)
            End Set
        End Property

        Public Property InactiveTabBorderColor() As Color
            Get
                Return Me.TabStrip.InactiveTabBorderColor
            End Get
            Set(ByVal value As Color)
                SetProperty("InactiveTabBorderColor", value)
            End Set
        End Property

        Public Property Animate() As Boolean
            Get
                Return Me.TabStrip.Animate
            End Get
            Set(ByVal value As Boolean)
                SetProperty("Animate", value)
            End Set
        End Property

        Public Property TabPermanence() As MdiTabPermanence
            Get
                Return Me.TabStrip.TabPermanence
            End Get
            Set(ByVal value As MdiTabPermanence)
                SetProperty("TabPermanence", value)
            End Set
        End Property

        Public Property MaxTabWidth() As Integer
            Get
                Return Me.TabStrip.MaxTabWidth
            End Get
            Set(ByVal value As Integer)
                SetProperty("MaxTabWidth", value)
            End Set
        End Property

        Public Property MinTabWidth() As Integer
            Get
                Return Me.TabStrip.MinTabWidth
            End Get
            Set(ByVal value As Integer)
                SetProperty("MinTabWidth", value)
            End Set
        End Property

        Public Property DisplayFormIcon() As Boolean
            Get
                Return Me.TabStrip.DisplayFormIcon
            End Get
            Set(ByVal value As Boolean)
                SetProperty("DisplayFormIcon", value)
            End Set
        End Property

        Public Property MdiWindowState() As MdiChildWindowState
            Get
                Return Me.TabStrip.MdiWindowState
            End Get
            Set(ByVal value As MdiChildWindowState)
                SetProperty("MdiWindowState", value)
            End Set
        End Property

        Public ReadOnly Property RightToLeft() As RightToLeft
            Get
                Return Me.TabStrip.RightToLeft
            End Get
        End Property

        Public Property MdiNewTabVisible() As Boolean
            Get
                Return Me.TabStrip.MdiNewTabVisible
            End Get
            Set(ByVal value As Boolean)
                SetProperty("MdiNewTabVisible", value)
            End Set
        End Property
#End Region

        Private Sub OpenInactiveTabEditor()
            Dim editor As New MdiTabStripDesignerForm
            Dim template As New MdiTabTemplateControl()

            template.InactiveTabTemplate.BackColor = Me.InactiveTabColor
            template.InactiveTabTemplate.ForeColor = Me.InactiveTabForeColor
            template.InactiveTabTemplate.Font = Me.InactiveTabFont
            template.InactiveTabTemplate.BorderColor = Me.InactiveTabBorderColor
            template.ActiveTabTemplate.BackColor = Me.ActiveTabColor
            template.ActiveTabTemplate.ForeColor = Me.ActiveTabForeColor
            template.ActiveTabTemplate.Font = Me.ActiveTabFont
            template.ActiveTabTemplate.BorderColor = Me.ActiveTabBorderColor
            template.ActiveTabTemplate.CloseButtonBackColor = Me.CloseButtonBackColor
            template.ActiveTabTemplate.CloseButtonBorderColor = Me.CloseButtonBorderColor
            template.ActiveTabTemplate.CloseButtonForeColor = Me.CloseButtonForeColor
            template.ActiveTabTemplate.CloseButtonHotForeColor = Me.CloseButtonHotForeColor
            template.MouseOverTabTemplate.BackColor = Me.MouseOverTabColor
            template.MouseOverTabTemplate.ForeColor = Me.MouseOverTabForeColor
            template.MouseOverTabTemplate.Font = Me.MouseOverTabFont
            template.RightToLeft = Me.RightToLeft

            editor.TabTemplate = template
            editor.ShowDialog()

            If editor.DialogResult = DialogResult.OK Then
                Me.InactiveTabColor = editor.TabTemplate.InactiveTabTemplate.BackColor
                Me.InactiveTabForeColor = editor.TabTemplate.InactiveTabTemplate.ForeColor
                Me.InactiveTabFont = editor.TabTemplate.InactiveTabTemplate.Font
                Me.InactiveTabBorderColor = editor.TabTemplate.InactiveTabTemplate.BorderColor
                Me.ActiveTabColor = editor.TabTemplate.ActiveTabTemplate.BackColor
                Me.ActiveTabForeColor = editor.TabTemplate.ActiveTabTemplate.ForeColor
                Me.ActiveTabBorderColor = editor.TabTemplate.ActiveTabTemplate.BorderColor
                Me.ActiveTabFont = editor.TabTemplate.ActiveTabTemplate.Font
                Me.CloseButtonBackColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonBackColor
                Me.CloseButtonForeColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonForeColor
                Me.CloseButtonHotForeColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonHotForeColor
                Me.CloseButtonBorderColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonBorderColor
                Me.MouseOverTabColor = editor.TabTemplate.MouseOverTabTemplate.BackColor
                Me.MouseOverTabForeColor = editor.TabTemplate.MouseOverTabTemplate.ForeColor
                Me.MouseOverTabFont = editor.TabTemplate.MouseOverTabTemplate.Font
            End If
        End Sub
    End Class
End Namespace
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
                If m_actionLists Is Nothing Then
                    m_actionLists = New DesignerActionListCollection
                    m_actionLists.Add(New MdiTabStripDesignerActionList(Component))
                End If

                Return m_actionLists
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
                Text = "Active Tab"
                Icon = My.Resources.document
            End Sub
        End Class

        Private Class Form2
            Inherits Form

            Public Sub New()
                Text = "Inactive Tab"
                Icon = My.Resources.document
            End Sub
        End Class

        Private Class Form3
            Inherits Form

            Public Sub New()
                Text = "Moused Over Tab"
                Icon = My.Resources.document
            End Sub
        End Class
    End Class

    Public Class MdiTabStripDesignerActionList
        Inherits DesignerActionList

        Private _uiService As DesignerActionUIService = Nothing
        Private _actionItems As DesignerActionItemCollection = Nothing

        Public Sub New(ByVal component As IComponent)
            MyBase.New(component)
            _uiService = CType(GetService(GetType(DesignerActionUIService)), DesignerActionUIService)
        End Sub

        Public Overrides Function GetSortedActionItems() As DesignerActionItemCollection
            If _actionItems Is Nothing Then
                _actionItems = New DesignerActionItemCollection

                If TabStrip IsNot Nothing Then
                    _actionItems.Add(New DesignerActionMethodItem(Me, "OpenInactiveTabEditor", "Design Tabs", "Appearance", "Opens the MdiTab Designer window."))

                    _actionItems.Add(New DesignerActionHeaderItem("Behavior"))
                    _actionItems.Add(New DesignerActionPropertyItem("TabPermanence", "Tab permanence", GetCategory(TabStrip, "TabPermanence"), GetDescription(TabStrip, "TabPermanence")))
                    _actionItems.Add(New DesignerActionPropertyItem("Animate", "Perform fade animation on mouse over", GetCategory(TabStrip, "Animate"), GetDescription(TabStrip, "Animate")))
                    _actionItems.Add(New DesignerActionPropertyItem("DisplayFormIcon", "Display the form icon", "Behavior", GetDescription(TabStrip, "DisplayFormIcon")))
                    _actionItems.Add(New DesignerActionPropertyItem("MdiNewTabVisible", "Display the new tab", "Behavior", GetDescription(TabStrip, "MdiNewTabVisible")))

                    _actionItems.Add(New DesignerActionHeaderItem("Layout"))
                    _actionItems.Add(New DesignerActionPropertyItem("MinTabWidth", "Minimum tab width", GetCategory(TabStrip, "MinTabWidth"), GetDescription(TabStrip, "MinTabWidth")))
                    _actionItems.Add(New DesignerActionPropertyItem("MaxTabWidth", "Maximum tab width", GetCategory(TabStrip, "MaxTabWidth"), GetDescription(TabStrip, "MaxTabWidth")))
                    _actionItems.Add(New DesignerActionPropertyItem("MdiWindowState", "Mdi form window state", GetCategory(TabStrip, "MdiWindowState"), GetDescription(TabStrip, "MdiWindowState")))
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
            Dim prop As PropertyDescriptor = TypeDescriptor.GetProperties(TabStrip)(propertyName)
            prop.SetValue(TabStrip, value)
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
                Return TabStrip.ActiveTabColor
            End Get
            Set(ByVal value As Color)
                SetProperty("ActiveTabColor", value)
            End Set
        End Property

        Public Property ActiveTabForeColor() As Color
            Get
                Return TabStrip.ActiveTabForeColor
            End Get
            Set(ByVal value As Color)
                SetProperty("ActiveTabForeColor", value)
            End Set
        End Property

        Public Property ActiveTabFont() As Font
            Get
                Return TabStrip.ActiveTabFont
            End Get
            Set(ByVal value As Font)
                SetProperty("ActiveTabFont", value)
            End Set
        End Property

        Public Property CloseButtonBackColor() As Color
            Get
                Return TabStrip.CloseButtonBackColor
            End Get
            Set(ByVal value As Color)
                SetProperty("CloseButtonBackColor", value)
            End Set
        End Property

        Public Property CloseButtonForeColor() As Color
            Get
                Return TabStrip.CloseButtonForeColor
            End Get
            Set(ByVal value As Color)
                SetProperty("CloseButtonForeColor", value)
            End Set
        End Property

        Public Property CloseButtonHotForeColor() As Color
            Get
                Return TabStrip.CloseButtonHotForeColor
            End Get
            Set(ByVal value As Color)
                SetProperty("CloseButtonHotForeColor", value)
            End Set
        End Property

        Public Property CloseButtonBorderColor() As Color
            Get
                Return TabStrip.CloseButtonBorderColor
            End Get
            Set(ByVal value As Color)
                SetProperty("CloseButtonBorderColor", value)
            End Set
        End Property

        Public Property InactiveTabColor() As Color
            Get
                Return TabStrip.InactiveTabColor
            End Get
            Set(ByVal value As Color)
                SetProperty("InactiveTabColor", value)
            End Set
        End Property

        Public Property InactiveTabForeColor() As Color
            Get
                Return TabStrip.InactiveTabForeColor
            End Get
            Set(ByVal value As Color)
                SetProperty("InactiveTabForeColor", value)
            End Set
        End Property

        Public Property InactiveTabFont() As Font
            Get
                Return TabStrip.InactiveTabFont
            End Get
            Set(ByVal value As Font)
                SetProperty("InactiveTabFont", value)
            End Set
        End Property

        Public Property MouseOverTabColor() As Color
            Get
                Return TabStrip.MouseOverTabColor
            End Get
            Set(ByVal value As Color)
                SetProperty("MouseOverTabColor", value)
            End Set
        End Property

        Public Property MouseOverTabForeColor() As Color
            Get
                Return TabStrip.MouseOverTabForeColor
            End Get
            Set(ByVal value As Color)
                SetProperty("MouseOverTabForeColor", value)
            End Set
        End Property

        Public Property MouseOverTabFont() As Font
            Get
                Return TabStrip.MouseOverTabFont
            End Get
            Set(ByVal value As Font)
                SetProperty("MouseOverTabFont", value)
            End Set
        End Property

        Public Property ActiveTabBorderColor() As Color
            Get
                Return TabStrip.ActiveTabBorderColor
            End Get
            Set(ByVal value As Color)
                SetProperty("ActiveTabBorderColor", value)
            End Set
        End Property

        Public Property InactiveTabBorderColor() As Color
            Get
                Return TabStrip.InactiveTabBorderColor
            End Get
            Set(ByVal value As Color)
                SetProperty("InactiveTabBorderColor", value)
            End Set
        End Property

        Public Property Animate() As Boolean
            Get
                Return TabStrip.Animate
            End Get
            Set(ByVal value As Boolean)
                SetProperty("Animate", value)
            End Set
        End Property

        Public Property TabPermanence() As MdiTabPermanence
            Get
                Return TabStrip.TabPermanence
            End Get
            Set(ByVal value As MdiTabPermanence)
                SetProperty("TabPermanence", value)
            End Set
        End Property

        Public Property MaxTabWidth() As Integer
            Get
                Return TabStrip.MaxTabWidth
            End Get
            Set(ByVal value As Integer)
                SetProperty("MaxTabWidth", value)
            End Set
        End Property

        Public Property MinTabWidth() As Integer
            Get
                Return TabStrip.MinTabWidth
            End Get
            Set(ByVal value As Integer)
                SetProperty("MinTabWidth", value)
            End Set
        End Property

        Public Property DisplayFormIcon() As Boolean
            Get
                Return TabStrip.DisplayFormIcon
            End Get
            Set(ByVal value As Boolean)
                SetProperty("DisplayFormIcon", value)
            End Set
        End Property

        Public Property MdiWindowState() As MdiChildWindowState
            Get
                Return TabStrip.MdiWindowState
            End Get
            Set(ByVal value As MdiChildWindowState)
                SetProperty("MdiWindowState", value)
            End Set
        End Property

        Public ReadOnly Property RightToLeft() As RightToLeft
            Get
                Return TabStrip.RightToLeft
            End Get
        End Property

        Public Property MdiNewTabVisible() As Boolean
            Get
                Return TabStrip.MdiNewTabVisible
            End Get
            Set(ByVal value As Boolean)
                SetProperty("MdiNewTabVisible", value)
            End Set
        End Property
#End Region

        Private Sub OpenInactiveTabEditor()
            Dim editor As New MdiTabStripDesignerForm
            Dim template As New MdiTabTemplateControl()

            template.InactiveTabTemplate.BackColor = InactiveTabColor
            template.InactiveTabTemplate.ForeColor = InactiveTabForeColor
            template.InactiveTabTemplate.Font = InactiveTabFont
            template.InactiveTabTemplate.BorderColor = InactiveTabBorderColor
            template.ActiveTabTemplate.BackColor = ActiveTabColor
            template.ActiveTabTemplate.ForeColor = ActiveTabForeColor
            template.ActiveTabTemplate.Font = ActiveTabFont
            template.ActiveTabTemplate.BorderColor = ActiveTabBorderColor
            template.ActiveTabTemplate.CloseButtonBackColor = CloseButtonBackColor
            template.ActiveTabTemplate.CloseButtonBorderColor = CloseButtonBorderColor
            template.ActiveTabTemplate.CloseButtonForeColor = CloseButtonForeColor
            template.ActiveTabTemplate.CloseButtonHotForeColor = CloseButtonHotForeColor
            template.MouseOverTabTemplate.BackColor = MouseOverTabColor
            template.MouseOverTabTemplate.ForeColor = MouseOverTabForeColor
            template.MouseOverTabTemplate.Font = MouseOverTabFont
            template.RightToLeft = RightToLeft

            editor.TabTemplate = template
            editor.ShowDialog()

            If editor.DialogResult = DialogResult.OK Then
                InactiveTabColor = editor.TabTemplate.InactiveTabTemplate.BackColor
                InactiveTabForeColor = editor.TabTemplate.InactiveTabTemplate.ForeColor
                InactiveTabFont = editor.TabTemplate.InactiveTabTemplate.Font
                InactiveTabBorderColor = editor.TabTemplate.InactiveTabTemplate.BorderColor
                ActiveTabColor = editor.TabTemplate.ActiveTabTemplate.BackColor
                ActiveTabForeColor = editor.TabTemplate.ActiveTabTemplate.ForeColor
                ActiveTabBorderColor = editor.TabTemplate.ActiveTabTemplate.BorderColor
                ActiveTabFont = editor.TabTemplate.ActiveTabTemplate.Font
                CloseButtonBackColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonBackColor
                CloseButtonForeColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonForeColor
                CloseButtonHotForeColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonHotForeColor
                CloseButtonBorderColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonBorderColor
                MouseOverTabColor = editor.TabTemplate.MouseOverTabTemplate.BackColor
                MouseOverTabForeColor = editor.TabTemplate.MouseOverTabTemplate.ForeColor
                MouseOverTabFont = editor.TabTemplate.MouseOverTabTemplate.Font
            End If
        End Sub
    End Class
End Namespace
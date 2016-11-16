using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace MdiTabStrip.Design
{
    internal class MdiTabStripDesignerActionList : DesignerActionList
    {
        private DesignerActionItemCollection _actionItems = null;
        private DesignerActionUIService _uiService = null;

        public MdiTabStrip TabStrip
        {
            get { return (MdiTabStrip)Component; }
        }

        public Color ActiveTabColor
        {
            get { return TabStrip.ActiveTabColor; }
            set { SetProperty("ActiveTabColor", value); }
        }
        public Color ActiveTabForeColor
        {
            get { return TabStrip.ActiveTabForeColor; }
            set { SetProperty("ActiveTabForeColor", value); }
        }
        public Font ActiveTabFont
        {
            get { return TabStrip.ActiveTabFont; }
            set { SetProperty("ActiveTabFont", value); }
        }
        public Color CloseButtonBackColor
        {
            get { return TabStrip.CloseButtonBackColor; }
            set { SetProperty("CloseButtonBackColor", value); }
        }
        public Color CloseButtonForeColor
        {
            get { return TabStrip.CloseButtonForeColor; }
            set { SetProperty("CloseButtonForeColor", value); }
        }
        public Color CloseButtonHotForeColor
        {
            get { return TabStrip.CloseButtonHotForeColor; }
            set { SetProperty("CloseButtonHotForeColor", value); }
        }
        public Color CloseButtonBorderColor
        {
            get { return TabStrip.CloseButtonBorderColor; }
            set { SetProperty("CloseButtonBorderColor", value); }
        }
        public Color InactiveTabColor
        {
            get { return TabStrip.InactiveTabColor; }
            set { SetProperty("InactiveTabColor", value); }
        }
        public Color InactiveTabForeColor
        {
            get { return TabStrip.InactiveTabForeColor; }
            set { SetProperty("InactiveTabForeColor", value); }
        }
        public Font InactiveTabFont
        {
            get { return TabStrip.InactiveTabFont; }
            set { SetProperty("InactiveTabFont", value); }
        }
        public Color MouseOverTabColor
        {
            get { return TabStrip.MouseOverTabColor; }
            set { SetProperty("MouseOverTabColor", value); }
        }
        public Color MouseOverTabForeColor
        {
            get { return TabStrip.MouseOverTabForeColor; }
            set { SetProperty("MouseOverTabForeColor", value); }
        }
        public Font MouseOverTabFont
        {
            get { return TabStrip.MouseOverTabFont; }
            set { SetProperty("MouseOverTabFont", value); }
        }
        public Color ActiveTabBorderColor
        {
            get { return TabStrip.ActiveTabBorderColor; }
            set { SetProperty("ActiveTabBorderColor", value); }
        }
        public Color InactiveTabBorderColor
        {
            get { return TabStrip.InactiveTabBorderColor; }
            set { SetProperty("InactiveTabBorderColor", value); }
        }
        public bool Animate
        {
            get { return TabStrip.Animate; }
            set { SetProperty("Animate", value); }
        }
        public MdiTabPermanence TabPermanence
        {
            get { return TabStrip.TabPermanence; }
            set { SetProperty("TabPermanence", value); }
        }
        public int MaxTabWidth
        {
            get { return TabStrip.MaxTabWidth; }
            set { SetProperty("MaxTabWidth", value); }
        }
        public int MinTabWidth
        {
            get { return TabStrip.MinTabWidth; }
            set { SetProperty("MinTabWidth", value); }
        }
        public bool DisplayFormIcon
        {
            get { return TabStrip.DisplayFormIcon; }
            set { SetProperty("DisplayFormIcon", value); }
        }
        public MdiChildWindowState MdiWindowState
        {
            get { return TabStrip.MdiWindowState; }
            set { SetProperty("MdiWindowState", value); }
        }
        public RightToLeft RightToLeft
        {
            get { return TabStrip.RightToLeft; }
        }
        public bool MdiNewTabVisible
        {
            get { return TabStrip.MdiNewTabVisible; }
            set { SetProperty("MdiNewTabVisible", value); }
        }

        public MdiTabStripDesignerActionList(IComponent component)
            : base(component)
        {
            _uiService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            if (_actionItems == null)
            {
                _actionItems = new DesignerActionItemCollection();

                if (TabStrip != null)
                {
                    _actionItems.Add(new DesignerActionMethodItem(this, "OpenInactiveTabEditor", "Design Tabs", "Appearance", "Opens the MdiTab Designer window."));

                    _actionItems.Add(new DesignerActionHeaderItem("Behavior"));
                    _actionItems.Add(new DesignerActionPropertyItem("TabPermanence", "Tab permanence", GetCategory(TabStrip, "TabPermanence"), GetDescription(TabStrip, "TabPermanence")));
                    _actionItems.Add(new DesignerActionPropertyItem("Animate", "Perform fade animation on mouse over", GetCategory(TabStrip, "Animate"), GetDescription(TabStrip, "Animate")));
                    _actionItems.Add(new DesignerActionPropertyItem("DisplayFormIcon", "Display the form icon", "Behavior", GetDescription(TabStrip, "DisplayFormIcon")));
                    _actionItems.Add(new DesignerActionPropertyItem("MdiNewTabVisible", "Display the new tab", "Behavior", GetDescription(TabStrip, "MdiNewTabVisible")));

                    _actionItems.Add(new DesignerActionHeaderItem("Layout"));
                    _actionItems.Add(new DesignerActionPropertyItem("MinTabWidth", "Minimum tab width", GetCategory(TabStrip, "MinTabWidth"), GetDescription(TabStrip, "MinTabWidth")));
                    _actionItems.Add(new DesignerActionPropertyItem("MaxTabWidth", "Maximum tab width", GetCategory(TabStrip, "MaxTabWidth"), GetDescription(TabStrip, "MaxTabWidth")));
                    _actionItems.Add(new DesignerActionPropertyItem("MdiWindowState", "Mdi form window state", GetCategory(TabStrip, "MdiWindowState"), GetDescription(TabStrip, "MdiWindowState")));
                }
            }

            return _actionItems;
        }

        private void SetProperty(string propertyName, object value)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(TabStrip)[propertyName];
            prop.SetValue(TabStrip, value);
        }

        private string GetCategory(object source, string propertyName)
        {
            PropertyInfo prop = source.GetType().GetProperty(propertyName);
            object[] attrs = prop.GetCustomAttributes(typeof(CategoryAttribute), false);

            if (attrs.Length == 0)
            {
                return null;
            }

            CategoryAttribute attr = attrs[0] as CategoryAttribute;
            if (attr == null)
            {
                return null;
            }

            return attr.Category;
        }

        private string GetDescription(object source, string propertyName)
        {
            PropertyInfo prop = source.GetType().GetProperty(propertyName);
            object[] attrs = prop.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs.Length == 0)
            {
                return null;
            }

            DescriptionAttribute attr = attrs[0] as DescriptionAttribute;
            if (attr == null)
            {
                return null;
            }

            return attr.Description;
        }

        private void OpenInactiveTabEditor()
        {
            MdiTabStripDesignerForm editor = new MdiTabStripDesignerForm();
            MdiTabTemplateControl template = new MdiTabTemplateControl();

            template.InactiveTabTemplate.BackColor = InactiveTabColor;
            template.InactiveTabTemplate.ForeColor = InactiveTabForeColor;
            template.InactiveTabTemplate.Font = InactiveTabFont;
            template.InactiveTabTemplate.BorderColor = InactiveTabBorderColor;
            template.ActiveTabTemplate.BackColor = ActiveTabColor;
            template.ActiveTabTemplate.ForeColor = ActiveTabForeColor;
            template.ActiveTabTemplate.Font = ActiveTabFont;
            template.ActiveTabTemplate.BorderColor = ActiveTabBorderColor;
            template.ActiveTabTemplate.CloseButtonBackColor = CloseButtonBackColor;
            template.ActiveTabTemplate.CloseButtonBorderColor = CloseButtonBorderColor;
            template.ActiveTabTemplate.CloseButtonForeColor = CloseButtonForeColor;
            template.ActiveTabTemplate.CloseButtonHotForeColor = CloseButtonHotForeColor;
            template.MouseOverTabTemplate.BackColor = MouseOverTabColor;
            template.MouseOverTabTemplate.ForeColor = MouseOverTabForeColor;
            template.MouseOverTabTemplate.Font = MouseOverTabFont;
            template.RightToLeft = RightToLeft;

            editor.TabTemplate = template;
            editor.ShowDialog();

            if (editor.DialogResult = DialogResult.OK)
            {
                InactiveTabColor = editor.TabTemplate.InactiveTabTemplate.BackColor;
                InactiveTabForeColor = editor.TabTemplate.InactiveTabTemplate.ForeColor;
                InactiveTabFont = editor.TabTemplate.InactiveTabTemplate.Font;
                InactiveTabBorderColor = editor.TabTemplate.InactiveTabTemplate.BorderColor;
                ActiveTabColor = editor.TabTemplate.ActiveTabTemplate.BackColor;
                ActiveTabForeColor = editor.TabTemplate.ActiveTabTemplate.ForeColor;
                ActiveTabBorderColor = editor.TabTemplate.ActiveTabTemplate.BorderColor;
                ActiveTabFont = editor.TabTemplate.ActiveTabTemplate.Font;
                CloseButtonBackColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonBackColor;
                CloseButtonForeColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonForeColor;
                CloseButtonHotForeColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonHotForeColor;
                CloseButtonBorderColor = editor.TabTemplate.ActiveTabTemplate.CloseButtonBorderColor;
                MouseOverTabColor = editor.TabTemplate.MouseOverTabTemplate.BackColor;
                MouseOverTabForeColor = editor.TabTemplate.MouseOverTabTemplate.ForeColor;
                MouseOverTabFont = editor.TabTemplate.MouseOverTabTemplate.Font;
            }
        }
    }
}

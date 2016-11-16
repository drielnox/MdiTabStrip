using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;

namespace MdiTabStrip.Design
{
    public class MdiTabStripDesigner : ControlDesigner
    {
        private DesignerActionListCollection _actionLists = null;

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists==null)
                {
                    _actionLists = new DesignerActionListCollection();
                }

                return base.ActionLists;
            }
        }

        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            MdiTabStrip tabStrip = (MdiTabStrip)Control;
            MdiTab activeTab = new MdiTab(tabStrip);
            MdiTab inactiveTab = new MdiTab(tabStrip);
            MdiTab mouseOverTab = new MdiTab(tabStrip);

            tabStrip.LeftScrollTab.Visible = true;
            tabStrip.RightScrollTab.Visible = true;
            tabStrip.DropDownTab.Visible = true;

            activeTab.Form = new Form1;
            tabStrip.ActiveTab = activeTab;
            tabStrip.Tabs.Add(activeTab);

            inactiveTab.Form = new Form2;
            tabStrip.Tabs.Add(inactiveTab);

            mouseOverTab.Form = new Form3;
            mouseOverTab.IsMouseOver = true;
            tabStrip.Tabs.Add(mouseOverTab);

            tabStrip.PerformLayout()
        }
    }
}

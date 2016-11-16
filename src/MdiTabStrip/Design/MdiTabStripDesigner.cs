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
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MdiTabStrip.Design
{
    public class ActiveMdiTabProperties : InactiveMdiTabProperties
    {
        private Color _closeButtonBackColor;

        [Category("Close Button Appearance")]
        [Description("The background color of the tab's close button when moused over.")]
        public Color CloseButtonBackColor
        {
            get { return _closeButtonBackColor; }
            set
            {
                if (_closeButtonBackColor != value)
                {
                    _closeButtonBackColor = value;
                    InvokePropertyChanged();
                }
            }
        }
    }
}

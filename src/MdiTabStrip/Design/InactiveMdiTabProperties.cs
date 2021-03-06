﻿namespace MdiTabStrip.Design
{
    using System.ComponentModel;
    using System.Drawing;

    public class InactiveMdiTabProperties : MdiTabProperties
    {
        private Color _borderColor;

        [Category("Tab Appearance")]
        [Description("The border color of the tab.")]
        public Color BorderColor
        {
            get { return _borderColor; }

            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    InvokePropertyChanged();
                }
            }
        }
    }
}

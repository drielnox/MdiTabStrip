namespace MdiTabStrip.Design
{
    using System.ComponentModel;
    using System.Drawing;

    public class ActiveMdiTabProperties : InactiveMdiTabProperties
    {
        private Color _closeButtonBackColor;
        private Color _closeButtonBorderColor;
        private Color _closeButtonForeColor;
        private Color _closeButtonHotForeColor;

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

        [Category("Close Button Appearance")]
        [Description("The border color of the tab's close button when moused over.")]
        public Color CloseButtonBorderColor
        {
            get { return _closeButtonBorderColor; }
            set
            {
                if (_closeButtonBorderColor != value)
                {
                    _closeButtonBorderColor = value;
                    InvokePropertyChanged();
                }
            }
        }

        [Category("Close Button Appearance")]
        [Description("The glyph color of the tab's close button.")]
        public Color CloseButtonForeColor
        {
            get { return _closeButtonForeColor; }
            set
            {
                if (_closeButtonForeColor != value)
                {
                    _closeButtonForeColor = value;
                    InvokePropertyChanged();
                }
                _closeButtonForeColor = value;
            }
        }

        [Category("Close Button Appearance")]
        [Description("The glyph color of the tab's close button when moused over.")]
        public Color CloseButtonHotForeColor
        {
            get { return _closeButtonHotForeColor; }
            set
            {
                if (_closeButtonHotForeColor != value)
                {
                    _closeButtonHotForeColor = value;
                    InvokePropertyChanged();
                }
            }
        }
    }
}

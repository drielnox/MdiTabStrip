namespace MdiTabStrip.Design
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    public class MdiTabProperties
    {
        private Color _backColor;
        private Color _foreColor;
        private Font _font;

        internal event EventHandler OnPropertyChanged;

        [Category("Tab Appearance")]
        [Description("The background color of the tab.")]
        public Color BackColor
        {
            get { return _backColor; }

            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    InvokePropertyChanged();
                }
            }
        }

        [Category("Tab Appearance")]
        [Description("The text color of the tab.")]
        public Color ForeColor
        {
            get { return _foreColor; }

            set
            {
                if (_foreColor != value)
                {
                    _foreColor = value;
                    InvokePropertyChanged();
                }
            }
        }

        [Category("Tab Appearance")]
        [Description("The font used to display the text of the tab.")]
        public Font Font
        {
            get { return _font; }

            set
            {
                if (_font != value)
                {
                    _font = value;
                    InvokePropertyChanged();
                }
            }
        }

        protected void InvokePropertyChanged()
        {
            if (OnPropertyChanged != null)
            {
                OnPropertyChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

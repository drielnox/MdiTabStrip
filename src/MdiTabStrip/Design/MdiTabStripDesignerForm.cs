using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MdiTabStrip.Design
{
    public partial class MdiTabStripDesignerForm : Form
    {
        public MdiTabTemplateControl TabTemplate { get; set; }

        public MdiTabStripDesignerForm()
        {
            InitializeComponent();
        }        
    }
}

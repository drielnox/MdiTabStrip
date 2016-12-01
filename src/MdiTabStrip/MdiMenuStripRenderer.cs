using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MdiTabStrip
{
    internal class MdiMenuStripRenderer : ToolStripRenderer
    {
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBorder(e);

            ControlPaint.DrawFocusRectangle(e.Graphics, e.AffectedBounds, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark);
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBackground(e);

            ToolStrip strip = e.ToolStrip;
            double h = strip.Height / strip.Items.Count;

            using (Bitmap scratchImage = new Bitmap(strip.Width, strip.Height))
            {
                using (Graphics g = Graphics.FromImage(scratchImage))
                {
                    RectangleF rect = new RectangleF(0F, 0F, (float)scratchImage.Width, (float)h);

                    g.FillRectangle(Brushes.White, new Rectangle(new Point(0, 0), scratchImage.Size));

                    foreach (MdiMenuItem item in strip.Items)
                    {
                        if (item.IsTabVisible)
                        {
                            g.FillRectangle(Brushes.White, rect);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 225, 225, 225)), rect);
                        }

                        rect.Offset(0F, (float)h);
                    }
                }

                e.Graphics.DrawImage(scratchImage, e.AffectedBounds);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            MdiMenuItem mdiItem = e.Item as MdiMenuItem;

            if (mdiItem.IsMouseOver)
            {
                if (mdiItem.IsTabActive)
                {
                    e.TextColor = Color.Black;
                }
                else
                {
                    e.TextColor = Color.White;
                }
            }

            if (mdiItem.IsTabActive)
            {
                e.TextFont = new Font(e.TextFont, FontStyle.Bold);
            }

            base.OnRenderItemText(e);
        }

        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderItemBackground(e);

            MdiMenuItem mdiItem  = e.Item as MdiMenuItem;

            if (mdiItem.IsMouseOver)
            {
                if (mdiItem.IsTabActive)
                {
                    e.Graphics.DrawRectangle(Pens.Black, e.Item.ContentRectangle);
                }
                else
                {
                    e.Graphics.FillRectangle(Brushes.Black, e.Item.ContentRectangle);
                }
            }
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            MdiMenuItem mdiItem  = e.Item as MdiMenuItem;

            if (!mdiItem.IsTabActive)
            {
                base.OnRenderItemImage(e);
            }
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            MdiMenuItem mdiItem  = e.Item as MdiMenuItem;
            ToolStripItemImageRenderEventArgs tsi = new ToolStripItemImageRenderEventArgs(e.Graphics, e.Item, mdiItem.CheckedImage, e.ImageRectangle);
            base.OnRenderItemCheck(tsi);
        }
    }
}

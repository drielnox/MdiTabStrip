using MdiTabStrip.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace MdiTabStrip
{
    public class MdiNewTab : MdiTab
    {
        public MdiNewTab(MdiTabStrip owner) : base(owner)
        {
        }

        internal override void DrawControl(Graphics g)
        {
            Image tabImage = default(Image);

            if (ParentInternal.MdiNewTabImage == null)
            {
                tabImage = Resources.NewTab;
            }
            else
            {
                tabImage = ParentInternal.MdiNewTabImage;
            }

            if (IsMouseOver)
            {
                Rectangle iconRectangle = new Rectangle(Width / 2 - tabImage.Width / 2, Height / 2 - tabImage.Height / 2, tabImage.Width, tabImage.Height);

                iconRectangle.Offset(Location);
                g.DrawImage(tabImage, iconRectangle);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            ParentInternal.OnMdiNewTabClick();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            //Not implemented but overriden to bypass behavior in inherited class.
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //Not implemented but overriden to bypass behavior in inherited class.
        }
    }
}

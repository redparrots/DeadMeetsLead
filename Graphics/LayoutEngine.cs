using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics
{
    /*
     * The basic idea with the layout engine is that you setup your heirarchy of ILayoutables any way you
     * like, and then run root.LayoutEngine.Layout(root), and no matter how many times you run it the result
     * is the same.
     * 
     * You may also run layoutable.LayoutEngine.Layout(layoutable); on any layoutable in the heirarchy and
     * it will make sure it's children are properly layed out.
     * 
     * The only two out-parameters are: Location and Size
     * 
     * Not all implementations of ILayoutEngine regards all parameters of the ILayoutable, for instance the
     * grid layout doesn't care about Anchor.
     * */

    public interface ILayoutable
    {
        /// <summary>
        /// Size of the layoutable
        /// </summary>
        Vector2 Size { get; set; }

        /// <summary>
        /// Location of the layoutable, in reference to the top left corner of it's parent.
        /// </summary>
        Vector2 Location { set; }

        System.Windows.Forms.DockStyle Dock { get; }
        Orientation Anchor { get; }
        /// <summary>
        /// A more abstract position variable, in reference to where the Anchor is
        /// </summary>
        Vector2 Position { get; }
        /// <summary>
        /// Space outside the border
        /// </summary>
        System.Windows.Forms.Padding Margin { get; }
        /// <summary>
        /// Space inside the border
        /// </summary>
        System.Windows.Forms.Padding Padding { get; }
        bool IsVisible { get; }
        IEnumerable<ILayoutable> LayoutChildren { get; }
    }
    public abstract class ILayoutEngine
    {
        public abstract void Layout(ILayoutable layoutable);
        protected Vector2 MarginSize(ILayoutable layoutable) { return new Vector2(layoutable.Margin.Horizontal, layoutable.Margin.Vertical); }
        protected Vector2 MarginPosition(ILayoutable layoutable) { return new Vector2(layoutable.Margin.Left, layoutable.Margin.Top); }
        protected Vector2 ContentPosition(ILayoutable layoutable) { return new Vector2(layoutable.Padding.Left, layoutable.Padding.Top); }
        protected Vector2 ContentArea(ILayoutable layoutable) { return layoutable.Size - new Vector2(layoutable.Padding.Horizontal, layoutable.Padding.Vertical); }
    }

    public class ForwardLayoutEngine : ILayoutEngine
    {
        public override void Layout(ILayoutable layoutable)
        {
            Vector2 contentPosition = ContentPosition(layoutable);
            Vector2 contentArea = ContentArea(layoutable);
            Vector2 dockPosition = contentPosition;
            Vector2 dockSize = contentArea;
            foreach (var v in layoutable.LayoutChildren)
            {
                if (v.Dock != System.Windows.Forms.DockStyle.None)
                {
                    Vector2 size = Vector2.Zero;
                    Vector2 outerSize = Vector2.Zero;
                    Vector2 pos = Vector2.Zero;
                    if (v.Dock == System.Windows.Forms.DockStyle.Fill)
                    {
                        outerSize = dockSize;
                        size = outerSize - MarginSize(v);
                        pos = dockPosition + MarginPosition(v);
                    }
                    else if (v.Dock == System.Windows.Forms.DockStyle.Bottom)
                    {
                        outerSize = new Vector2(dockSize.X, v.Size.Y + v.Margin.Vertical);
                        size = outerSize - MarginSize(v);
                        pos = dockPosition + dockSize - outerSize + MarginPosition(v);
                        dockSize = new Vector2(dockSize.X, dockSize.Y - outerSize.Y);
                    }
                    else if (v.Dock == System.Windows.Forms.DockStyle.Left)
                    {
                        outerSize = new Vector2(v.Size.X + v.Margin.Horizontal, dockSize.Y);
                        size = outerSize - MarginSize(v);
                        pos = dockPosition + MarginPosition(v);
                        dockPosition = new Vector2(dockPosition.X + outerSize.X, dockPosition.Y);
                        dockSize = new Vector2(dockSize.X - outerSize.X, dockSize.Y);
                    }
                    else if (v.Dock == System.Windows.Forms.DockStyle.Right)
                    {
                        outerSize = new Vector2(v.Size.X + v.Margin.Horizontal, dockSize.Y);
                        size = outerSize - MarginSize(v);
                        pos = dockPosition + dockSize - outerSize + MarginPosition(v);
                        dockSize = new Vector2(dockSize.X - outerSize.X, dockSize.Y);
                    }
                    else if (v.Dock == System.Windows.Forms.DockStyle.Top)
                    {
                        outerSize = new Vector2(dockSize.X, v.Size.Y + v.Margin.Vertical);
                        size = outerSize - MarginSize(v);
                        pos = dockPosition + MarginPosition(v);
                        dockPosition = new Vector2(dockPosition.X, dockPosition.Y + outerSize.Y);
                        dockSize = new Vector2(dockSize.X, dockSize.Y - outerSize.Y);
                    }
                    v.Size = size;
                    v.Location = pos;
                }
                else
                {
                    v.Location = contentPosition +
                        OrientationUtil.Position(v.Anchor, contentArea, v.Position, v.Size);
                }
            }
        }
    }

    public enum FlowOrigin
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
    public class FlowLayoutEngine : ILayoutEngine
    {
        public FlowLayoutEngine()
        {
            HorizontalFill = true;
            NewLine = true;
        }
        public override void Layout(ILayoutable layoutable)
        {
            Vector2 size = PerformLayout(layoutable);
            if (AutoSize)
            {
                if (NewLine)
                {
                    if (HorizontalFill)
                        layoutable.Size = new SlimDX.Vector2(layoutable.Size.X, size.Y + layoutable.Padding.Vertical);
                    else
                        layoutable.Size = new SlimDX.Vector2(size.X + layoutable.Padding.Horizontal, layoutable.Size.Y);
                }
                else
                {
                    layoutable.Size = new Vector2(size.X + layoutable.Padding.Vertical,
                        size.Y + layoutable.Padding.Horizontal);
                }
                if (Origin != FlowOrigin.TopLeft)
                    PerformLayout(layoutable);
            }
        }

        Vector2 PerformLayout(ILayoutable layoutable)
        {
            Vector2 contentPosition = ContentPosition(layoutable);
            Vector2 contentArea = ContentArea(layoutable);
            float x = 0, y = 0, rowMax = 0, sizeX = 0, sizeY = 0;
            foreach (ILayoutable v in layoutable.LayoutChildren)
                if (v.IsVisible)
                {
                    float outerSizeX = v.Size.X + v.Margin.Horizontal;
                    float outerSizeY = v.Size.Y + v.Margin.Vertical;
                    if (NewLine)
                    {
                        if (HorizontalFill)
                        {
                            if (x + outerSizeX > contentArea.X)
                            {
                                x = 0;
                                y += rowMax;
                                rowMax = 0;
                            }
                        }
                        else
                        {
                            if (y + outerSizeY > contentArea.Y)
                            {
                                y = 0;
                                x += rowMax;
                                rowMax = 0;
                            }
                        }
                    }
                    var p = new SlimDX.Vector2(x, y);
                    if (Origin == FlowOrigin.TopRight || Origin == FlowOrigin.BottomRight)
                        p.X = contentArea.X - x - outerSizeX;
                    if (Origin == FlowOrigin.BottomLeft || Origin == FlowOrigin.BottomRight)
                        p.Y = contentArea.Y - y - outerSizeY;
                    v.Location = contentPosition + new Vector2(v.Margin.Left, v.Margin.Top) + p;
                    sizeX = Math.Max(sizeX, x + outerSizeX);
                    sizeY = Math.Max(sizeY, y + outerSizeY);
                    if (HorizontalFill)
                    {
                        x += outerSizeX;
                        rowMax = Math.Max(rowMax, outerSizeY);
                    }
                    else
                    {
                        y += outerSizeY;
                        rowMax = Math.Max(rowMax, outerSizeX);
                    }
                }
            return new Vector2(sizeX, sizeY);
        }

        public FlowOrigin Origin { get; set; }
        public bool AutoSize { get; set; }
        public bool HorizontalFill { get; set; }
        /// <summary>
        /// Move overflowing controls to a new line
        /// </summary>
        public bool NewLine { get; set; }
    }

    public class GridLayoutEngine : ILayoutEngine
    {
        public GridLayoutEngine()
        {
            NWidth = 4;
            NHeight = 4;
            HorizontalFill = true;
            Origin = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
        }
        public override void Layout(ILayoutable layoutable)
        {
            Vector2 contentPosition = ContentPosition(layoutable);
            Vector2 contentArea = ContentArea(layoutable);

            int y = 0, x = 0;
            Action incY = () =>
            {
                if ((Origin & System.Windows.Forms.AnchorStyles.Top) != System.Windows.Forms.AnchorStyles.None)
                    y++;
                if ((Origin & System.Windows.Forms.AnchorStyles.Bottom) != System.Windows.Forms.AnchorStyles.None)
                    y--;
            };
            Action incX = () =>
            {
                if ((Origin & System.Windows.Forms.AnchorStyles.Left) != System.Windows.Forms.AnchorStyles.None)
                    x++;
                if ((Origin & System.Windows.Forms.AnchorStyles.Right) != System.Windows.Forms.AnchorStyles.None)
                    x--;
            };
            if ((Origin & System.Windows.Forms.AnchorStyles.Right) != System.Windows.Forms.AnchorStyles.None)
                x = NWidth - 1;
            if ((Origin & System.Windows.Forms.AnchorStyles.Bottom) != System.Windows.Forms.AnchorStyles.None)
                y = NHeight - 1;

            float dy = contentArea.Y / (float)NHeight, dx = contentArea.X / (float)NWidth;
            foreach (ILayoutable v in layoutable.LayoutChildren)
            {
                v.Location = contentPosition + new SlimDX.Vector2(x * dx, y * dy) + new Vector2(v.Margin.Left, v.Margin.Top);
                v.Size = new SlimDX.Vector2(dx, dy) - new Vector2(v.Margin.Horizontal, v.Margin.Vertical);
                if (HorizontalFill)
                {
                    incX();

                    if (x >= NWidth)
                    {
                        x = 0;
                        incY();
                    }
                    if (x < 0)
                    {
                        x = NWidth - 1;
                        incY();
                    }
                }
                else
                {
                    incY();

                    if (y >= NHeight)
                    {
                        y = 0;
                        incX();
                    }
                    if (y < 0)
                    {
                        y = NHeight - 1;
                        incX();
                    }
                }
            }
        }

        public int NWidth { get; set; }
        public int NHeight { get; set; }
        public System.Windows.Forms.AnchorStyles Origin { get; set; }
        public bool HorizontalFill { get; set; }
    }
}

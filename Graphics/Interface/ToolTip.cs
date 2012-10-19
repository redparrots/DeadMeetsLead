using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics.Interface
{
    public class ToolTip : Control
    {
        public ToolTip()
        {
            Clickable = false;
        }
        public void SetToolTip(Control control, String text)
        {
            if (String.IsNullOrEmpty(text))
            {
                RemoveToolTip(control);
            }
            else
            {
                if(control.ToolTip != null && control.ToolTip is Label)
                {
                    ((Label)control.ToolTip).Text = text;
                }
                else
                    SetToolTip(control, new Label
                    {
                        AutoSize = AutoSizeMode.RestrictedFull,
                        Clickable = false,
                        Text = text,
                        MaxSize = new Vector2(300, float.MaxValue),
                        Padding = new System.Windows.Forms.Padding(3),
                        Background = InterfaceScene.DefaultSlimBorder
                    });
            }
        }
        public void SetToolTip(Control control, Control tooltip)
        {
            if (tooltip == null)
            {
                RemoveToolTip(control);
            }
            else
            {
                if(control.ToolTip != null)
                {
                    bool isDisplaying = current == control;
                    if (isDisplaying)
                        HideTooltip();
                    control.ToolTip = tooltip;
                    if (isDisplaying)
                        ShowTooltip(control);
                }
                else
                    AddToolTip(control, tooltip);
            }
        }
        

        void AddToolTip(Control control, Control tooltip)
        {
            control.ToolTip = tooltip;
            control.MouseEnter += new EventHandler(control_MouseEnter);
            control.MouseLeave += new EventHandler(control_MouseLeave);
            control.MouseMove += new System.Windows.Forms.MouseEventHandler(control_MouseMove);
        }
        void RemoveToolTip(Control control)
        {
            if (current == control)
                HideTooltip();
            control.ToolTip = null;
            control.MouseEnter -= new EventHandler(control_MouseEnter);
            control.MouseLeave -= new EventHandler(control_MouseLeave);
            control.MouseMove -= new System.Windows.Forms.MouseEventHandler(control_MouseMove);
        }

        void ShowTooltip(Control control)
        {
            if (currentTooltip != null)
                currentTooltip.Remove();
            currentTooltip = control.ToolTip;
            AddChild(currentTooltip);
            currentTooltip.PerformLayout();
            current = control;
        }
        void HideTooltip()
        {
            if (current == null) return;

            currentTooltip.Remove();
            currentTooltip = null;
            current = null;
        }

        void control_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            AdjustPosition(e.Location);
        }

        void AdjustPosition(System.Drawing.Point position)
        {
            if (current == null) return;

            Position = Common.Math.ToVector2(position) + new Vector2(10, -currentTooltip.Size.Y - 10);

            if (Position.Y < 0)
                Position = new Vector2(Position.X, 0);

            if (Position.X + currentTooltip.Size.X > ((Control)current.Scene.Root).Size.X)
                Position = new SlimDX.Vector2(Position.X - currentTooltip.Size.X, Position.Y);

            if (Position.Y + currentTooltip.Size.Y > ((Control)current.Scene.Root).Size.Y)
                Position = new SlimDX.Vector2(Position.X, Position.Y - currentTooltip.Size.Y);
        }


        void control_MouseEnter(object sender, EventArgs e)
        {
            var c = (Control)sender;
            ShowTooltip(c);
            AdjustPosition(Scene.View.LocalMousePosition);
        }

        void control_MouseLeave(object sender, EventArgs e)
        {
            HideTooltip();
        }

        Control current, currentTooltip;
    }
}

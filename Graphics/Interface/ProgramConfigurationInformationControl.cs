using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics.Interface
{
    public class ProgramConfigurationInformationControl : Control
    {
        public ProgramConfigurationInformationControl()
        {
            Size = new SlimDX.Vector2(400, 0);
            Common.ProgramConfigurationInformation.WarningsChanged += 
                new Action(() => Invalidate());
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            ClearChildren();
            details.Size = new SlimDX.Vector2(Size.X, details.Size.Y);
            AddChild(new Label
            {
                Text = "Program Configuration Warnings",
                Size = new SlimDX.Vector2(Size.X, 20)
            });
            AddChild(details);
            float y = details.Size.Y + details.Position.Y;
            foreach (var warn in Common.ProgramConfigurationInformation.Warnings)
            {
                var v = warn;
                Button b;
                AddChild(b = new Button
                {
                    Text = "[" + v.Module + "] " + v.Text,
                    Size = new SlimDX.Vector2(Size.X, 20),
                    Position = new SlimDX.Vector2(0, y),
                    TextAnchor = Orientation.Left
                });
                b.MouseEnter += new EventHandler((o, e) => details.Text =
                    "Name: " + v.Text + "\n" +
                    "Module: " + v.Module + "\nImportance: " + v.Importance + "\nType: " + v.Type +
                        "\nDescription: " + v.Description);
                b.MouseLeave += new EventHandler((o, e) => details.Text = "");
                y += 20;
            }
            Size = new SlimDX.Vector2(Size.X, y);
        }
        Label details = new Label
        {
            Size = new SlimDX.Vector2(0, 200),
            Position = new SlimDX.Vector2(0, 20)
        };
    }
}

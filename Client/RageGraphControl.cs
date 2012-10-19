using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class RageGraphControl : UserControl
    {
        public RageGraphControl()
        {
            InitializeComponent();
            Program.Instance.ProgramEvent += new ProgramEventHandler(Instance_ProgramEvent);
        }

        void Instance_ProgramEvent(ProgramEvent e)
        {
            if (e.Type == ProgramEventType.MainCharacterRageLevel)
            {
                rageLevelGained.Add(new PointF(Game.Game.Instance.GameTime - timeOff, ((ProgramEvents.MainCharacterRageLevel)e).RageLevel));
                Invalidate();
            }
            if (e.Type == ProgramEventType.MainCharacterStrike)
            {
                if (timeOff == 0)
                {
                    timeOff = Game.Game.Instance.GameTime;
                    rageLevelGained.Add(new PointF(Game.Game.Instance.GameTime - timeOff, 1));
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            timeOff = 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            //int secWidth = 5 * 60;
            //float xPointsPerSec = Width / (float)secWidth;
            //float yPointsPerRage = Height / 10f;
            //for (int i = 0; i < secWidth; i+=10)
            //    e.Graphics.DrawLine(Pens.Gray, i * xPointsPerSec, 0, i * xPointsPerSec, Height);
            //foreach (var v in rageLevelGained)
            //{
            //    e.Graphics.DrawRectangle(Pens.Red, v.X * xPointsPerSec, Height - v.Y * yPointsPerRage, 1, 1);
            //}

            float xPointsPerRage = Width / 20f;
            float yPointsPerSec = Height / 60f;
            
            for (float x = 0; x < Width; x += xPointsPerRage)
                e.Graphics.DrawLine(Pens.DarkGray, x, 0, x, Height);
            
            for (float y = 0; y < Width; y += yPointsPerSec*10)
                e.Graphics.DrawLine(Pens.DarkGray, 0, y, Width, y);

            for(int i=1; i < rageLevelGained.Count; i++)
            {
                PointF prev = rageLevelGained[i - 1];
                PointF t = rageLevelGained[i];
                float timeDiff = t.X - prev.X;
                e.Graphics.DrawLine(new Pen(Color.FromArgb(100, 255, 0, 0)),
                    xPointsPerRage * (prev.Y), Height - yPointsPerSec * timeDiff,
                    xPointsPerRage * (t.Y), Height - yPointsPerSec * timeDiff);
                //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 255, 0, 0)),
                //    xPointsPerRage * (prev.Y), Height - yPointsPerSec * timeDiff, xPointsPerRage, yPointsPerSec * timeDiff);
            }
        }
        float timeOff;

        List<PointF> rageLevelGained = new List<PointF>();
    }
}

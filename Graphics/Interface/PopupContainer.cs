using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics.Interface
{
    public class PopupContainer : Control
    {
        public PopupContainer()
        {
            Updateable = true;
        }

        ProgressOrientation orientation = ProgressOrientation.BottomToTop;
        public ProgressOrientation Orientation { get { return orientation; } set { orientation = value; Invalidate(); } }

        bool oldControlsAtBottom = true;
        public bool OldControlsAtBottom { get { return oldControlsAtBottom; } set { oldControlsAtBottom = value; Invalidate(); } }

        float padding = 0;
        public float Padding { get { return padding; } set { padding = value; Invalidate(); } }

        float spacing = 10;
        public float Spacing { get { return spacing; } set { spacing = value; Invalidate(); } }

        AnimationTimeType timeType = AnimationTimeType.Speed;
        AnimationTimeType TimeType { get { return timeType; } set { timeType = value; Invalidate(); } }

        float time = 500;
        public float Time { get { return time; } set { time = value; Invalidate(); } }

        protected override void OnChildAdded(Entity e)
        {
            base.OnChildAdded(e);
            popupDatas[(Control)e] = new PopupData
            {
                Control = (Control)e,
            };
            if(Orientation == ProgressOrientation.BottomToTop)
                popupDatas[(Control)e].Value.Value = Size.Y;
            Invalidate();
        }
        protected override void OnChildRemoved(Entity e)
        {
            base.OnChildRemoved(e);
            popupDatas.Remove((Control)e);
            Invalidate();
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            float val = 0;

            var c = new List<Entity>(Children);
            if (!OldControlsAtBottom)
                c.Reverse();
            foreach (Control v in c)
            {
                if (!popupDatas[v].Removing)
                {
                    float goal = 0;
                    if (Orientation == ProgressOrientation.BottomToTop)
                    {
                        goal = Size.Y - Padding - val - v.Size.Y - Spacing;
                    }

                    float time = 1;
                    if (TimeType == AnimationTimeType.Speed)
                        time = Math.Abs((popupDatas[v].Value.Value - goal) / Time);

                    popupDatas[v].Value.ClearKeys();
                    popupDatas[v].Value.AddKey(new Common.InterpolatorKey<float>
                    {
                        Time = time,
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Value = goal
                    });
                    
                    if (Orientation == ProgressOrientation.BottomToTop || Orientation == ProgressOrientation.TopToBottom)
                        val += v.Size.Y + Spacing;
                }
            }
        }
        public void ScrollOut(Control control)
        {
            var p = popupDatas[control];

            float val = 0;
            if (Orientation == ProgressOrientation.BottomToTop)
                val = Size.Y;


            float time = 1;
            if (TimeType == AnimationTimeType.Speed)
                time = Math.Abs((p.Value.Value - val) / Time);

            Common.InterpolatorKey<float> k;
            p.Value.ClearKeys();
            p.Value.AddKey(k = new Common.InterpolatorKey<float>
            {
                Time = time,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = val
            });
            k.Passing += new EventHandler((o, e) => { toRemove.Add(p.Control); });
            p.Removing = true;
            Invalidate();
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

            foreach (var v in popupDatas)
            {
                if (Orientation == ProgressOrientation.BottomToTop)
                    v.Value.Control.Position = new Vector2(
                        v.Value.Control.Position.X,
                        v.Value.Value.Update(e.Dtime));
                else
                    throw new NotImplementedException();
            }
            foreach(var v in toRemove)
                v.Remove();
            toRemove.Clear();
        }
        class PopupData
        {
            public Control Control;
            public bool Removing = false;
            public Common.Interpolator Value = new Common.Interpolator();
        }
        Dictionary<Control, PopupData> popupDatas = new Dictionary<Control, PopupData>();
        List<Control> toRemove = new List<Control>();
    }
}

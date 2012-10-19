using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InterpolatorTest
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            var i = interpolatorVisualization1.Interpolator = new Common.Interpolator();
            i.Value = 0;
            i.AddKey(new Common.InterpolatorKey<float>
            {
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Time = 0,
                Value = 0,
                Type = Common.InterpolatorKeyType.CubicBezier,
                RightControlPoint = 0.5f
            });
            i.AddKey(new Common.InterpolatorKey<float>
            {
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Time = 10,
                Value = 15,
                Type = Common.InterpolatorKeyType.CubicBezier,
                LeftControlPoint = 9.5f,
                RightControlPoint = 10.5f
            });
            i.AddKey(new Common.InterpolatorKey<float>
            {
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Time = 20,
                Value = 20,
                Type = Common.InterpolatorKeyType.CubicBezier,
                LeftControlPoint = -1,
                RightControlPoint = 1
            });
            //i.AddKey(new Common.InterpolatorKey<float> { Period = 2, Repeat = true, Time = 1, Value = 1, TimeType = Common.InterpolatorKeyTimeType.Absolute });
            //i.AddKey(new Common.InterpolatorKey<float> { Period = 5, Repeat = true, Time = 3.5f, Value = 20, TimeType = Common.InterpolatorKeyTimeType.Absolute });
            //i.AddKey(new Common.InterpolatorKey<float> { Period = 2, Repeat = true, Time = 2, Value = 3, TimeType = Common.InterpolatorKeyTimeType.Absolute });
        }
    }
}

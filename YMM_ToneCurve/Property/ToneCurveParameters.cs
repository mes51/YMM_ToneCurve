using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMM_ToneCurve.Property
{
    public class ToneCurveParameters
    {
        public static readonly ToneCurveParameters Empty = new ToneCurveParameters(
            [new ToneCurvePoint(0.0F, 0.0F), new ToneCurvePoint(1.0F, 1.0F)],
            [new ToneCurvePoint(0.0F, 0.0F), new ToneCurvePoint(1.0F, 1.0F)],
            [new ToneCurvePoint(0.0F, 0.0F), new ToneCurvePoint(1.0F, 1.0F)],
            [new ToneCurvePoint(0.0F, 0.0F), new ToneCurvePoint(1.0F, 1.0F)],
            [new ToneCurvePoint(0.0F, 0.0F), new ToneCurvePoint(1.0F, 1.0F)]
        );

        public ToneCurveChannelParameters Rgb { get; set; } = new ToneCurveChannelParameters();

        public ToneCurveChannelParameters R { get; set; } = new ToneCurveChannelParameters();

        public ToneCurveChannelParameters G { get; set; } = new ToneCurveChannelParameters();

        public ToneCurveChannelParameters B { get; set; } = new ToneCurveChannelParameters();

        public ToneCurveChannelParameters A { get; set; } = new ToneCurveChannelParameters();

        public ToneCurveParameters(IEnumerable<ToneCurvePoint> rgb, IEnumerable<ToneCurvePoint> r, IEnumerable<ToneCurvePoint> g, IEnumerable<ToneCurvePoint> b, IEnumerable<ToneCurvePoint> a)
        {
            Rgb = new ToneCurveChannelParameters(rgb);
            R = new ToneCurveChannelParameters(r);
            G = new ToneCurveChannelParameters(g);
            B = new ToneCurveChannelParameters(b);
            A = new ToneCurveChannelParameters(a);
        }
    }

    public class ToneCurveChannelParameters
    {
        public ToneCurvePoint[] Points { get; set; } = [];

        public ToneCurveChannelParameters() { }

        public ToneCurveChannelParameters(IEnumerable<ToneCurvePoint> points)
        {
            Points = points.ToArray();
        }
    }

    public class ToneCurvePoint
    {
        public float InValue { get; set; }

        public float OutValue { get; set; }

        public ToneCurvePoint(float inValue, float outValue)
        {
            InValue = inValue;
            OutValue = outValue;
        }
    }
}

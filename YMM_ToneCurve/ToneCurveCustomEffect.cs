using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using YMM_ToneCurve.Interpolation;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace YMM_ToneCurve
{
    class ToneCurveCustomEffect(IGraphicsDevicesAndContext devices) : D2D1CustomShaderEffectBase(Create<ToneCurveEffectImpl>(devices))
    {
        public int Channel
        {
            get => GetIntValue((int)EffectProperty.Channel);
            set => SetValue((int)EffectProperty.Channel, value);
        }

        public SplinePoint[] Points
        {
            get => GetPoints(EffectProperty.Count, EffectProperty.Points, EffectProperty.Positions);
            set => SetPoints(value, EffectProperty.Count, EffectProperty.Points, EffectProperty.Positions);
        }

        SplinePoint[] GetPoints(EffectProperty countProperty, EffectProperty pointProperty, EffectProperty positionProperty)
        {
            var count = GetIntValue((int)countProperty);
            var result = new SplinePoint[count];
            for (var i = 0; i < count; i++)
            {
                var values = GetVector4Value(i + (int)pointProperty);
                var pos = GetFloatValue(i + (int)positionProperty);
                result[i] = new SplinePoint(pos, values.X, values.Y, values.Z, values.W);
            }

            return result;
        }

        void SetPoints(SplinePoint[] points, EffectProperty countProperty, EffectProperty pointProperty, EffectProperty positionProperty)
        {
            var hashCode = points.GetHashCode();
            if (GetIntValue((int)EffectProperty.UpdateKey) == hashCode)
            {
                return;
            }

            var count = Math.Min(points.Length, ToneCurveEffectImpl.MaxPointCount);
            SetValue((int)countProperty, count);
            for (var i = 0; i < count; i++)
            {
                var point = points[i];
                SetValue(i + (int)pointProperty, new Vector4(point.A, point.B, point.C, point.D));
                SetValue(i + (int)positionProperty, point.X);
            }

            SetValue((int)EffectProperty.UpdateKey, hashCode);
        }
    }

    file static class SplinePointArrayExtension
    {
        public static int CalcHash(this SplinePoint[] points)
        {
            var hashCode = new HashCode();

            foreach (var p in points)
            {
                hashCode.Add(p.GetHashCode());
            }

            return hashCode.ToHashCode();
        }
    }
}

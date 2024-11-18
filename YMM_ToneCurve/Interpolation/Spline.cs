using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMM_ToneCurve.Property;

namespace YMM_ToneCurve.Interpolation
{
    // SEE: https://qiita.com/metaphysical_bard/items/e558b005255d1767fc70
    class Spline
    {
        float[] X { get; }

        float[] A { get; }

        float[] C { get; }

        float[] B { get; }

        float[] D { get; }

        public Spline(IEnumerable<ToneCurvePoint> points)
        {
            var sortedPoints = points.OrderBy(p => p.InValue).ToArray();
            X = [.. sortedPoints.Select(p => p.InValue)];
            A = new float[sortedPoints.Length];
            C = new float[sortedPoints.Length];
            B = new float[sortedPoints.Length];
            D = [.. sortedPoints.Select(p => p.OutValue)];

            var h = X.Zip(X.Skip(1)).Select(t => t.Second - t.First).ToArray();
            var w = new float[sortedPoints.Length];
            w[0] = 1.0F;
            w[^1] = 1.0F;
            for (int i = 1, limit = w.Length - 1; i < limit; i++)
            {
                B[i] = 3.0F * ((D[i + 1] - D[i]) / h[i] - (D[i] - D[i - 1]) / h[i - 1]);
                w[i] = 2.0F * (h[i - 1] + h[i]);
            }
            Thomas(h, w, B);

            for (int i = 0, limit = sortedPoints.Length - 1; i < limit; i++)
            {
                A[i] = (B[i + 1] - B[i]) / (3.0F * h[i]);
                C[i] = (D[i + 1] - D[i]) / h[i] - h[i] * (B[i + 1] + 2.0F * B[i]) / 3.0F;
            }
        }

        public float Interpolate(float x)
        {
            if (X[0] > x)
            {
                return D[0];
            }

            for (int i = 0, limit = X.Length - 1; i < limit; i++)
            {
                if (X[i] <= x && X[i + 1] >= x)
                {
                    var dx = x - X[i];
                    return ((A[i] * dx + B[i]) * dx + C[i]) * dx + D[i];
                }
            }

            return D[^1];
        }

        public SplinePoint[] ToSplinePoints()
        {
            var result = new SplinePoint[X.Length];
            for (var i = 0; i < X.Length; i++)
            {
                result[i] = new SplinePoint(X[i], A[i], B[i], C[i], D[i]);
            }

            return result;
        }

        static void Thomas(float[] ac, float[] b, float[] d)
        {
            for (var i = 1; i < ac.Length; i++)
            {
                var w = ac[i - 1] / b[i - 1];
                b[i] = b[i] - ac[i - 1] * w;
                d[i] = d[i] - d[i - 1] * w;
            }

            d[^1] = d[^1] / b[^1];
            for (var i = ac.Length - 1; i > -1; i--)
            {
                d[i] = (d[i] - d[i + 1] * ac[i]) / b[i];
            }
        }
    }

    readonly record struct SplinePoint(float X, float A, float B, float C, float D);
}

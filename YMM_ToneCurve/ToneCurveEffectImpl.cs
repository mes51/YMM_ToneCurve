using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct2D1;
using YMM_ToneCurve.SourceGenerator.EffectPropertyGenerator;
using YukkuriMovieMaker.Player.Video;

namespace YMM_ToneCurve
{
    [CustomEffect(1)]
    [GenerateEffectProperty("Points", typeof(Vector4), PropertyType.Vector4, (int)EffectProperty.Points, MaxPointCount)]
    [GenerateEffectProperty("Positions", typeof(float), PropertyType.Float, (int)EffectProperty.Positions, MaxPointCount)]
    partial class ToneCurveEffectImpl : D2D1CustomShaderEffectImplBase<ToneCurveEffectImpl>
    {
        public const int MaxPointCount = 256;

        static readonly byte[] ConstantUpdateBuffer = new byte[EffectParameter.Size];

        public ToneCurveEffectImpl() : base(GetShader()) { }

        int channel;
        [CustomEffectProperty(PropertyType.Int32, (int)EffectProperty.Channel)]
        public int Channel
        {
            get => channel;
            set
            {
                if (channel != value)
                {
                    channel = value;
                    UpdateConstants();
                }
            }
        }

        [CustomEffectProperty(PropertyType.Int32, (int)EffectProperty.Count)]
        public int Count { get; set; }

        int updateKey;
        [CustomEffectProperty(PropertyType.Int32, (int)EffectProperty.UpdateKey)]
        public int UpdateKey
        {
            get => updateKey;
            set
            {
                if (updateKey != value)
                {
                    updateKey = value;
                    UpdateConstants();
                }
            }
        }

        protected unsafe override void UpdateConstants()
        {
            ConstantUpdateBuffer.AsSpan().Clear();
            fixed (byte* ptr = ConstantUpdateBuffer)
            {
                Marshal.StructureToPtr(new EffectParameter(Channel, Count, Points, Positions), (nint)ptr, false);
            }
            drawInformation?.SetPixelShaderConstantBuffer(ConstantUpdateBuffer);
        }

        static byte[] GetShader()
        {
            var assembly = typeof(ToneCurveEffectImpl).Assembly;
            using var stream = assembly.GetManifestResourceStream("ToneCurve_Shader.cso");

            if (stream != null)
            {
                using var reader = new BinaryReader(stream);
                return reader.ReadBytes((int)stream.Length);
            }
            else
            {
                return [];
            }
        }
    }

    enum EffectProperty
    {
        Channel = 0,
        Count = Channel + 1,
        Points = Count + 1,
        Positions = Points + ToneCurveEffectImpl.MaxPointCount,

        UpdateKey = Positions + ToneCurveEffectImpl.MaxPointCount
    }

    [StructLayout(LayoutKind.Explicit)]
    file readonly struct AlignedSplinePoint(float X, float A, float B, float C, float D)
    {
        [FieldOffset(0)]
        public readonly float X = X;

        [FieldOffset(sizeof(float))]
        public readonly float A = A;

        [FieldOffset(sizeof(float) * 2)]
        public readonly float B = B;

        [FieldOffset(sizeof(float) * 3)]
        public readonly float C = C;

        [FieldOffset(sizeof(float) * 4)]
        public readonly float D = D;

        // NOTE: 以下定数バッファのアライメント調整用
        [FieldOffset(sizeof(float) * 5)]
        readonly float Filler1;

        [FieldOffset(sizeof(float) * 6)]
        readonly float Filler2;

        [FieldOffset(sizeof(float) * 7)]
        readonly float Filler3;
    }

    [StructLayout(LayoutKind.Explicit)]
    file struct EffectParameter
    {
        public static readonly int Size = Marshal.SizeOf<EffectParameter>();

        [FieldOffset(0)]
        public readonly int Channel;

        [FieldOffset(sizeof(int))]
        public readonly int PointCount;

        [FieldOffset(sizeof(int) * 2)]
        readonly int Filler1;

        [FieldOffset(sizeof(int) * 3)]
        readonly int Filler2;

        [FieldOffset(sizeof(int) * 4)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ToneCurveEffectImpl.MaxPointCount)]
        public AlignedSplinePoint[] SplinePoints;

        public EffectParameter(int channel, int count, Vector4[] points, float[] positions)
        {
            Channel = channel;
            PointCount = Math.Min(Math.Min(Math.Min(count, points.Length), positions.Length), ToneCurveEffectImpl.MaxPointCount);

            SplinePoints = new AlignedSplinePoint[ToneCurveEffectImpl.MaxPointCount];
            for (var i = 0; i < PointCount; i++)
            {
                var point = points[i];
                SplinePoints[i] = new AlignedSplinePoint(positions[i], point.X, point.Y, point.Z, point.W);
            }
        }
    }
}

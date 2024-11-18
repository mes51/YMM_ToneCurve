using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct2D1;
using YMM_ToneCurve.Interpolation;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace YMM_ToneCurve
{
    class ToneCurveProcessor : IVideoEffectProcessor
    {
        public ID2D1Image Output => OutputInternal ?? Input ?? throw new NullReferenceException();

        ID2D1Image? Input { get; set; }

        ID2D1Image? OutputInternal { get; set; }

        ToneCurveCustomEffect RgbREffect { get; }

        ToneCurveCustomEffect RgbGEffect { get; }

        ToneCurveCustomEffect RgbBEffect { get; }

        ToneCurveCustomEffect REffect { get; }

        ToneCurveCustomEffect GEffect { get; }

        ToneCurveCustomEffect BEffect { get; }

        ToneCurveCustomEffect AEffect { get; }

        ToneCurve Item { get; }

        public ToneCurveProcessor(IGraphicsDevicesAndContext devices, ToneCurve toneCurve)
        {
            Item = toneCurve;
            RgbREffect = new ToneCurveCustomEffect(devices);
            RgbGEffect = new ToneCurveCustomEffect(devices);
            RgbBEffect = new ToneCurveCustomEffect(devices);
            REffect = new ToneCurveCustomEffect(devices);
            GEffect = new ToneCurveCustomEffect(devices);
            BEffect = new ToneCurveCustomEffect(devices);
            AEffect = new ToneCurveCustomEffect(devices);
            if (AEffect.IsEnabled)
            {
                OutputInternal = AEffect.Output;
            }
        }

        public void ClearInput()
        {
            if (RgbREffect.IsEnabled)
            {
                RgbREffect.SetInput(0, null, true);
            }
            if (RgbGEffect.IsEnabled)
            {
                RgbGEffect.SetInput(0, null, true);
            }
            if (RgbBEffect.IsEnabled)
            {
                RgbBEffect.SetInput(0, null, true);
            }
            if (REffect.IsEnabled)
            {
                REffect.SetInput(0, null, true);
            }
            if (GEffect.IsEnabled)
            {
                GEffect.SetInput(0, null, true);
            }
            if (BEffect.IsEnabled)
            {
                BEffect.SetInput(0, null, true);
            }
            if (AEffect.IsEnabled)
            {
                AEffect.SetInput(0, null, true);
            }
        }

        public void SetInput(ID2D1Image? input)
        {
            Input = input;
            if (RgbREffect.IsEnabled)
            {
                RgbREffect.Channel = 0;
                RgbREffect.SetInput(0, input, true);
            }
            if (RgbGEffect.IsEnabled)
            {
                RgbGEffect.Channel = 1;
                RgbGEffect.SetInput(0, RgbREffect.Output, true);
            }
            if (RgbBEffect.IsEnabled)
            {
                RgbBEffect.Channel = 2;
                RgbBEffect.SetInput(0, RgbGEffect.Output, true);
            }
            if (REffect.IsEnabled)
            {
                REffect.Channel = 0;
                REffect.SetInput(0, RgbBEffect.Output, true);
            }
            if (GEffect.IsEnabled)
            {
                GEffect.Channel = 1;
                GEffect.SetInput(0, REffect.Output, true);
            }
            if (BEffect.IsEnabled)
            {
                BEffect.Channel = 2;
                BEffect.SetInput(0, GEffect.Output, true);
            }
            if (AEffect.IsEnabled)
            {
                AEffect.Channel = 3;
                AEffect.SetInput(0, BEffect.Output, true);
            }
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            if (!AEffect.IsEnabled || Input == null)
            {
                return effectDescription.DrawDescription;
            }

            var rgbPoints = new Spline(Item.Parameters.Rgb.Points).ToSplinePoints();
            RgbREffect.Points = rgbPoints;
            RgbGEffect.Points = rgbPoints;
            RgbBEffect.Points = rgbPoints;

            REffect.Points = new Spline(Item.Parameters.R.Points).ToSplinePoints();
            GEffect.Points = new Spline(Item.Parameters.G.Points).ToSplinePoints();
            BEffect.Points = new Spline(Item.Parameters.B.Points).ToSplinePoints();
            AEffect.Points = new Spline(Item.Parameters.A.Points).ToSplinePoints();

            return effectDescription.DrawDescription;
        }

        public void Dispose()
        {
            Output?.Dispose();
            if (RgbREffect.IsEnabled)
            {
                RgbREffect.SetInput(0, null, true);
            }
            if (RgbGEffect.IsEnabled)
            {
                RgbGEffect.SetInput(0, null, true);
            }
            if (RgbBEffect.IsEnabled)
            {
                RgbBEffect.SetInput(0, null, true);
            }
            if (REffect.IsEnabled)
            {
                REffect.SetInput(0, null, true);
            }
            if (GEffect.IsEnabled)
            {
                GEffect.SetInput(0, null, true);
            }
            if (BEffect.IsEnabled)
            {
                BEffect.SetInput(0, null, true);
            }
            if (AEffect.IsEnabled)
            {
                AEffect.SetInput(0, null, true);
            }
            RgbREffect.Dispose();
            RgbGEffect.Dispose();
            RgbBEffect.Dispose();
            REffect.Dispose();
            GEffect.Dispose();
            BEffect.Dispose();
            AEffect.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMM_ToneCurve.Property;
using YMM_ToneCurve.Property.Editor;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace YMM_ToneCurve
{
    [VideoEffect("Tone Curve", ["加工"], [], IsAviUtlSupported = false)]
    public class ToneCurve : VideoEffectBase
    {
        public override string Label => "Tone Curve";

        ToneCurveParameters parameters = ToneCurveParameters.Empty;
        [Display(GroupName = "トーンカーブ", Description = "色調補正の量をグラフで調整します。編集するチャンネルはコンボボックスから変更できます")]
        [ToneCurveParametersEditor(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ToneCurveParameters Parameters
        {
            get => parameters;
            set => Set(ref parameters, value);
        }

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new ToneCurveProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables()
        {
            return [];
        }
    }
}

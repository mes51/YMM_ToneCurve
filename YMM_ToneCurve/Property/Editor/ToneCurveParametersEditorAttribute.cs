using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YMM_ToneCurve.View;
using YMM_ToneCurve.ViewModel;
using YukkuriMovieMaker.Commons;

namespace YMM_ToneCurve.Property.Editor
{
    class ToneCurveParametersEditorAttribute : PropertyEditorAttribute2
    {
        public override FrameworkElement Create()
        {
            return new ToneCurveParametersEditControl();
        }

        public override void ClearBindings(FrameworkElement control)
        {
            control.DataContext = null;
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            control.DataContext = new ToneCurveParametersEditViewModel(itemProperties);
        }
    }
}

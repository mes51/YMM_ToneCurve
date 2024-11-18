using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin;

namespace YMM_ToneCurve.View
{
    /// <summary>
    /// ToneCurveParametersEditControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ToneCurveParametersEditControl : UserControl, IPropertyEditorControl
    {
        public static readonly DependencyProperty IsDarkLikeThemeProperty = DependencyProperty.Register(
            nameof(IsDarkLikeTheme),
            typeof(bool),
            typeof(ToneCurveParametersEditControl),
            new FrameworkPropertyMetadata(false)
        );

        public bool IsDarkLikeTheme
        {
            get { return (bool)GetValue(IsDarkLikeThemeProperty); }
            set { SetValue(IsDarkLikeThemeProperty, value); }
        }

        public event EventHandler? BeginEdit;

        public event EventHandler? EndEdit;

        public ToneCurveParametersEditControl()
        {
            IsDarkLikeTheme = IsSelectedDarkLikeTheme();
            InitializeComponent();
        }

        static bool IsSelectedDarkLikeTheme()
        {
            // NOTE: 見つけたので使っているけど、怒られたら別の方法探す
            return ThemeManager.Instance.CurrentThemeName.Contains("Dark") || ThemeManager.Instance.CurrentThemeName.Contains("Black");
        }

        private void ToneCurvePointEditView_BeginEdit(object sender, RoutedEventArgs e)
        {
            BeginEdit?.Invoke(this, EventArgs.Empty);
        }

        private void ToneCurvePointEditView_EndEdit(object sender, RoutedEventArgs e)
        {
            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == null && e.NewValue != null)
            {
                ThemeManager.Instance.ThemeChanged += ThemeManager_ThemeChanged;
            }
            else if (e.OldValue != null && e.NewValue == null)
            {
                ThemeManager.Instance.ThemeChanged -= ThemeManager_ThemeChanged;
            }
        }

        private void ThemeManager_ThemeChanged(object? sender, ThemeManager.ThemeChangedEventArgs e)
        {
            IsDarkLikeTheme = IsSelectedDarkLikeTheme();
        }
    }
}

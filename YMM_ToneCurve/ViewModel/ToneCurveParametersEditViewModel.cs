using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YMM_ToneCurve.Extensions;
using YMM_ToneCurve.Property;
using YMM_ToneCurve.View.Command;
using YukkuriMovieMaker.Commons;

namespace YMM_ToneCurve.ViewModel
{
    class ToneCurveParametersEditViewModel : Bindable
    {
        private ObservableCollection<ToneCurvePoint> rgbPoints = [new ToneCurvePoint(0.0F, 0.0F), new ToneCurvePoint(1.0F, 1.0F)];
        public ObservableCollection<ToneCurvePoint> RgbPoints
        {
            get => rgbPoints;
            set => Set(ref rgbPoints, value);
        }

        private ObservableCollection<ToneCurvePoint> rPoints = [new ToneCurvePoint(0.0F, 0.0F), new ToneCurvePoint(1.0F, 1.0F)];
        public ObservableCollection<ToneCurvePoint> RPoints
        {
            get => rPoints;
            set => Set(ref rPoints, value);
        }

        private ObservableCollection<ToneCurvePoint> gPoints = [new ToneCurvePoint(0.0F, 0.0F), new ToneCurvePoint(1.0F, 1.0F)];
        public ObservableCollection<ToneCurvePoint> GPoints
        {
            get => gPoints;
            set => Set(ref gPoints, value);
        }

        private ObservableCollection<ToneCurvePoint> bPoints = [new ToneCurvePoint(0.0F, 0.0F), new ToneCurvePoint(1.0F, 1.0F)];
        public ObservableCollection<ToneCurvePoint> BPoints
        {
            get => bPoints;
            set => Set(ref bPoints, value);
        }

        private ObservableCollection<ToneCurvePoint> aPoints = [new ToneCurvePoint(0.0F, 0.0F), new ToneCurvePoint(1.0F, 1.0F)];
        public ObservableCollection<ToneCurvePoint> APoints
        {
            get => aPoints;
            set => Set(ref aPoints, value);
        }

        private int editingChannelIndex;
        public int EditingChannelIndex
        {
            get { return editingChannelIndex; }
            set { Set(ref editingChannelIndex, value); }
        }

        ItemProperty TargetProperty { get; }

        bool IsCommiting { get; set; }

        public ICommand CommitPointCommand { get; }

        public ICommand PreviewPointCommand { get; }

        public ToneCurveParametersEditViewModel(ItemProperty[] properties)
        {
            if (properties.Length < 1)
            {
                throw new ArgumentException(null, nameof(properties));
            }
            TargetProperty = properties[0];

            UpdateFromItemProperty();

            ((INotifyPropertyChanged)TargetProperty.PropertyOwner).PropertyChanged += PropertyItem_PropertyChanged;

            CommitPointCommand = new DelegateCommand(() =>
            {
                IsCommiting = true;
                RgbPoints.Sort((a, b) => a.InValue.CompareTo(b.InValue));
                RPoints.Sort((a, b) => a.InValue.CompareTo(b.InValue));
                GPoints.Sort((a, b) => a.InValue.CompareTo(b.InValue));
                BPoints.Sort((a, b) => a.InValue.CompareTo(b.InValue));
                APoints.Sort((a, b) => a.InValue.CompareTo(b.InValue));
                IsCommiting = false;
                UpdateToItemProperty();
            });

            PreviewPointCommand = new DelegateCommand(() => UpdateToItemProperty());
        }

        void UpdateToItemProperty()
        {
            TargetProperty.SetValue(new ToneCurveParameters(RgbPoints, RPoints, GPoints, BPoints, APoints));
        }

        void UpdateFromItemProperty()
        {
            var parameters = TargetProperty.GetValue<ToneCurveParameters>();
            if (parameters == null)
            {
                return;
            }

            RgbPoints = [.. parameters.Rgb.Points];
            RPoints = [.. parameters.R.Points];
            GPoints = [.. parameters.G.Points];
            BPoints = [.. parameters.B.Points];
            APoints = [.. parameters.A.Points];
        }

        private void PropertyItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!IsCommiting && e.PropertyName == TargetProperty.PropertyInfo.Name)
            {
                UpdateFromItemProperty();
            }
        }
    }
}

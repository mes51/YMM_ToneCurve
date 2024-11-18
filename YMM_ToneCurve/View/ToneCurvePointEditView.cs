using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using YMM_ToneCurve.Extensions;
using YMM_ToneCurve.Interpolation;
using YMM_ToneCurve.Property;
using YMM_ToneCurve.ViewModel;

namespace YMM_ToneCurve.View
{
    class ToneCurvePointEditView : FrameworkElement
    {
        const double PointSize = 4.0;

        const double Epsilon = 0.00001;

        const double DefaultDashGap = 10.0;

        public static readonly DependencyProperty PointBrushProperty = DependencyProperty.Register(
            nameof(PointBrush),
            typeof(Brush),
            typeof(ToneCurvePointEditView),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        public static readonly DependencyProperty LineBrushProperty = DependencyProperty.Register(
            nameof(LineBrush),
            typeof(Brush),
            typeof(ToneCurvePointEditView),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, PenParameterChanged)
        );

        public static readonly DependencyProperty LineWidthProperty = DependencyProperty.Register(
            nameof(LineWidth),
            typeof(double),
            typeof(ToneCurvePointEditView),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, PenParameterChanged)
        );

        public static readonly DependencyProperty IsDashedProperty = DependencyProperty.Register(
            nameof(IsDashed),
            typeof(bool),
            typeof(ToneCurvePointEditView),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, PenParameterChanged)
        );

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
            nameof(Points),
            typeof(ObservableCollection<ToneCurvePoint>),
            typeof(ToneCurvePointEditView),
            new FrameworkPropertyMetadata(new ObservableCollection<ToneCurvePoint>(), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            nameof(IsReadOnly),
            typeof(bool),
            typeof(ToneCurvePointEditView),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        public static readonly DependencyProperty IsDarkLikeThemeProperty = DependencyProperty.Register(
            nameof(IsDarkLikeTheme),
            typeof(bool),
            typeof(ToneCurvePointEditView),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, PenParameterChanged)
        );

        public static readonly RoutedEvent BeginEditEvent = EventManager.RegisterRoutedEvent(
            nameof(BeginEdit), RoutingStrategy.Direct, typeof(EventHandler<RoutedEventArgs>), typeof(ToneCurvePointEditView)
        );

        public static readonly RoutedEvent EndEditEvent = EventManager.RegisterRoutedEvent(
            nameof(EndEdit), RoutingStrategy.Direct, typeof(EventHandler<RoutedEventArgs>), typeof(ToneCurvePointEditView)
        );

        public event EventHandler<RoutedEventArgs> EndEdit
        {
            add { AddHandler(EndEditEvent, value); }
            remove { RemoveHandler(EndEditEvent, value); }
        }

        public event EventHandler<RoutedEventArgs> BeginEdit
        {
            add { AddHandler(BeginEditEvent, value); }
            remove { RemoveHandler(BeginEditEvent, value); }
        }

        public bool IsDarkLikeTheme
        {
            get { return (bool)GetValue(IsDarkLikeThemeProperty); }
            set { SetValue(IsDarkLikeThemeProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public ObservableCollection<ToneCurvePoint> Points
        {
            get { return (ObservableCollection<ToneCurvePoint>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public bool IsDashed
        {
            get { return (bool)GetValue(IsDashedProperty); }
            set { SetValue(IsDashedProperty, value); }
        }

        public double LineWidth
        {
            get { return (double)GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }

        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

        public Brush PointBrush
        {
            get { return (Brush)GetValue(PointBrushProperty); }
            set { SetValue(PointBrushProperty, value); }
        }

        Pen LinePen { get; set; } = new Pen(Brushes.Black, 1.0);

        Pen? DashedGapPen { get; set; } = new Pen(Brushes.White, 1.0);

        bool IsClicked { get; set; }

        int BeforeClickPointIndex { get; set; }

        bool AddedPointInClicking { get; set; }

        ToneCurveParametersEditViewModel? ViewModel => DataContext as ToneCurveParametersEditViewModel;

        public ToneCurvePointEditView()
        {
            Points = [new ToneCurvePoint(0.0F, 0.0F), new ToneCurvePoint(1.0F, 1.0F)];

            DataContextChanged += ToneCurvePointEditView_DataContextChanged;
            MouseDown += ToneCurvePointEditView_MouseDown;
            MouseMove += ToneCurvePointEditView_MouseMove;
            MouseUp += ToneCurvePointEditView_MouseUp;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var width = ActualWidth;
            var height = ActualHeight;
            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, width, height));

            if (Points.Count < 2)
            {
                drawingContext.DrawLine(LinePen, new Point(0.0, ActualHeight), new Point(ActualWidth, 0.0));
                return;
            }

            var spline = new Spline([.. Points]);
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(new Point(0.0, Math.Clamp(1.0 - spline.Interpolate(0.0F), 0.0, 1.0) * height), false, false);
                for (var i = 1.0F; i < width; i++)
                {
                    var y = spline.Interpolate(i / (float)width);
                    context.LineTo(new Point(i, Math.Clamp(1.0 - y, 0.0, 1.0) * height), true, false);
                }
            }
            drawingContext.DrawGeometry(null, LinePen, geometry);
            if (IsDashed)
            {
                drawingContext.DrawGeometry(null, DashedGapPen, geometry);
            }

            if (IsReadOnly)
            {
                return;
            }
            var pointBrush = IsDarkLikeTheme ? Brushes.White : Brushes.Black;
            foreach (var p in Points)
            {
                var x = p.InValue * width;
                var y = (1.0 - p.OutValue) * height;
                drawingContext.DrawRectangle(pointBrush, null, new Rect(x - PointSize, y - PointSize, PointSize * 2.0, PointSize * 2.0));
            }
        }

        private void ToneCurvePointEditView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IsClicked = false;
        }

        private void ToneCurvePointEditView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsClicked)
            {
                return;
            }

            IsClicked = false;

            if (AddedPointInClicking)
            {
                Points.RemoveAt(Points.Count - 1);
            }
            var mousePos = e.GetPosition(this);
            var width = ActualWidth;
            var x = mousePos.X / width;
            var isSkip = BeforeClickPointIndex switch
            {
                -1 => Points.Count > 1 && Points[0].InValue <= x,
                int when BeforeClickPointIndex == Points.Count - 1 => Points.Count > 1 && Points[^1].InValue >= x,
                int => Points[BeforeClickPointIndex].InValue >= x || Points[BeforeClickPointIndex + 1].InValue <= x
            };
            if (!isSkip)
            {
                if (Points.Count == 1 && Points[0].InValue == x)
                {
                    x += Epsilon;
                }
                Points.Add(new ToneCurvePoint((float)Math.Clamp(x, 0.0, 1.0), (float)Math.Clamp(1.0 - mousePos.Y / ActualHeight, 0.0, 1.0)));
            }

            ReleaseMouseCapture();
            ViewModel?.CommitPointCommand.Execute(null);
            InvalidateVisual();

            RaiseEvent(new RoutedEventArgs(EndEditEvent));
        }

        private void ToneCurvePointEditView_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsClicked)
            {
                return;
            }

            if (AddedPointInClicking)
            {
                Points.RemoveAt(Points.Count - 1);
            }
            var mousePos = e.GetPosition(this);
            var width = ActualWidth;
            var x = mousePos.X / width;
            var isSkip = BeforeClickPointIndex switch
            {
                -1 => Points.Count > 1 && Points[0].InValue <= x,
                int when BeforeClickPointIndex == Points.Count - 1 => Points.Count > 1 && Points[^1].InValue >= x,
                int => Points[BeforeClickPointIndex].InValue >= x || Points[BeforeClickPointIndex + 1].InValue <= x
            };

            if (!isSkip)
            {
                if (Points.Count == 1 && Points[0].InValue == x)
                {
                    x += Epsilon;
                }
                Points.Add(new ToneCurvePoint((float)Math.Clamp(x, 0.0, 1.0), (float)Math.Clamp(1.0 - mousePos.Y / ActualHeight, 0.0, 1.0)));
            }

            AddedPointInClicking = !isSkip;
            InvalidateVisual();
            ViewModel?.PreviewPointCommand.Execute(null);
        }

        private void ToneCurvePointEditView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly)
            {
                return;
            }

            RaiseEvent(new RoutedEventArgs(BeginEditEvent));

            IsClicked = true;

            var mousePos = e.GetPosition(this);
            var width = ActualWidth;
            var x = mousePos.X / width;

            var registeredPoint = Points.FirstOrDefault(p => Math.Abs(p.InValue - x) * width <= PointSize, new ToneCurvePoint(float.NaN, float.NaN));
            if (!double.IsNaN(registeredPoint.InValue))
            {
                Points.Remove(registeredPoint);
            }

            if (Points.Count >= ToneCurveEffectImpl.MaxPointCount)
            {
                return;
            }

            BeforeClickPointIndex = Points.FindLastIndex(p => p.InValue < x);
            Points.Add(new ToneCurvePoint((float)Math.Clamp(x, 0.0, 1.0), (float)Math.Clamp(1.0 - mousePos.Y / ActualHeight, 0.0, 1.0)));
            AddedPointInClicking = true;

            CaptureMouse();
            InvalidateVisual();
            ViewModel?.PreviewPointCommand.Execute(null);
        }

        private static void PenParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ToneCurvePointEditView view)
            {
                return;
            }

            var lineWidth = view.LineWidth;
            view.LinePen = new Pen(view.LineBrush, lineWidth);
            if (view.IsDashed)
            {
                var dashGap = DefaultDashGap / lineWidth;
                if (double.IsNaN(dashGap))
                {
                    dashGap = 0.0;
                }
                view.DashedGapPen = new Pen(view.IsDarkLikeTheme ? Brushes.Black : Brushes.White, lineWidth);
                view.LinePen.DashStyle = new DashStyle([dashGap, dashGap], 0.0);
                view.DashedGapPen.DashStyle = new DashStyle([dashGap, dashGap], dashGap);
            }
            else
            {
                view.DashedGapPen = null;
            }
            view.InvalidateVisual();
        }
    }
}

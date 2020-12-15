using System;
using Xamarin.Forms;
namespace FingerPaint.Controls.Renderers
{
    public class PDFView : View
    {
        #region Uri
        public static BindableProperty UrlProperty = BindableProperty.Create(nameof(Uri), typeof(string), typeof(PDFView), default(string), BindingMode.OneWay);
        public string Uri
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }
        #endregion

        #region ScrollOffset
        public static BindableProperty ScrollOffsetProperty = BindableProperty.Create(nameof(ScrollOffset), typeof(Point), typeof(PDFView), default(Point), BindingMode.OneWay);
        public Point ScrollOffset
        {
            get { return (Point)GetValue(ScrollOffsetProperty); }
            set { SetValue(ScrollOffsetProperty, value); }
        }
        #endregion

        #region CurrentScale
        public static BindableProperty CurrentScaleProperty = BindableProperty.Create(nameof(CurrentScale), typeof(double), typeof(PDFView), default(double), BindingMode.OneWay);
        public double CurrentScale
        {
            get { return (double)GetValue(CurrentScaleProperty); }
            set { SetValue(CurrentScaleProperty, value); }
        }
        #endregion

        public event EventHandler<double> ScaleChanged;
        public event EventHandler<Point> ScrollChanged;
        public PDFView()
        {
        }

        public void RaiseScaleChanged(double newScale)
        {
            ScaleChanged?.Invoke(this, newScale);
        }


        public void RaiseScrollChanged(Point scrollOffSet)
        {
            ScrollChanged?.Invoke(this, scrollOffSet);
        }

    }
}

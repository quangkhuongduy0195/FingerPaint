using Android.Views;
using FingerPaint.Droid.PdfLibrarys.Utilities;
using static Android.Views.ScaleGestureDetector;
namespace FingerPaint.Droid.PdfLibrarys.Gestures
{
    public class ScaleManager : SimpleOnScaleGestureListener
    {
        private IOnScaleListener _onScaleListener;
        public ScaleManager(IOnScaleListener onScaleListener)
        {
            _onScaleListener = onScaleListener;
        }

        public override bool OnScaleBegin(ScaleGestureDetector detector)
        {
            LogUtils.Log(nameof(ScaleManager), "OnScaleBegin");
            return true;
        }

        public override bool OnScale(ScaleGestureDetector detector)
        {
            LogUtils.Log(nameof(ScaleManager), "OnScale");
            float scaleFactor = detector.ScaleFactor;
            float focusX = detector.FocusX;
            float focusY = detector.FocusY;

            _onScaleListener.OnScale(scaleFactor, focusX, focusY);
            return true;
        }

        public override void OnScaleEnd(ScaleGestureDetector detector)
        {
            LogUtils.Log(nameof(ScaleManager), "OnScaleEnd");
            base.OnScaleEnd(detector);
            _onScaleListener.EndScale();

        }
    }
}

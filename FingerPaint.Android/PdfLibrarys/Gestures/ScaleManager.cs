using Android.Views;
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
            return true;
        }

        public override bool OnScale(ScaleGestureDetector detector)
        {
            float scaleFactor = detector.ScaleFactor;
            float focusX = detector.FocusX;
            float focusY = detector.FocusY;

            _onScaleListener.OnScale(scaleFactor, focusX, focusY);
            return true;
        }

        public override void OnScaleEnd(ScaleGestureDetector detector)
        {
            base.OnScaleEnd(detector);
            _onScaleListener.EndScale();

        }
    }
}

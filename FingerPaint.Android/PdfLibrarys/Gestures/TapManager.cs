using Android.Content;
using Android.Views;
using static Android.Views.GestureDetector;
namespace FingerPaint.Droid.PdfLibrarys.Gestures
{
    public class TapManager : View, IOnDoubleTapListener
    {
        private IOnTapListener _onTapListener;
        public TapManager(Context context, IOnTapListener onTapListener) : base(context)
        {
            _onTapListener = onTapListener;
        }

        public bool OnSingleTapConfirmed(MotionEvent e)
        {
            return true;
        }

        public bool OnDoubleTap(MotionEvent e)
        {
            _onTapListener.OnDoubleTap(e.GetX(), e.GetY());
            return true;
        }

        public bool OnDoubleTapEvent(MotionEvent e)
        {
            return true;
        }
    }
}
